# Guía: Configuración de Sonidos (Heavy Sound y Light Sound)

Esta guía explica dónde colocar los archivos de audio y cómo asignarlos para que el jugador pueda guiarse al elegir o no una carta.

---

## 🎵 ¿Qué son estos sonidos?

- **Light Sound (Sonido Ligero)**: Se reproduce cuando la carta es **FAVORABLE** (buena para el jugador)
- **Heavy Sound (Sonido Pesado)**: Se reproduce cuando la carta es **NO FAVORABLE** (mala para el jugador)

Estos sonidos se reproducen **ANTES** de que el jugador decida si robar o pasar la carta, solo cuando tiene los **ojos cerrados**.

---

## 📁 Dónde colocar los archivos de audio

### Opción 1: Carpeta de Assets (Recomendado)

1. Crea una carpeta en tu proyecto Unity:
   ```
   Assets/
   └── Audio/
       └── ClosedEyes/
           ├── LightSound.ogg (o .wav, .mp3)
           └── HeavySound.ogg (o .wav, .mp3)
   ```

### Opción 2: Dentro de la carpeta de Scripts relacionada

```
Assets/
└── Scripts/
    └── ClosedEyes/
        └── Audio/
            ├── LightSound.ogg
            └── HeavySound.ogg
```

### Formatos soportados por Unity:
- `.ogg` (recomendado, mejor compresión)
- `.wav` (sin compresión, mejor calidad)
- `.mp3` (comprimido, menor calidad)

---

## 🎮 Cómo asignar los sonidos en Unity

### Paso 1: Importar los archivos de audio

1. Arrastra los archivos de audio desde tu carpeta de Windows/Mac a la carpeta `Assets/Audio/ClosedEyes/` en Unity
2. Unity importará automáticamente los archivos

### Paso 2: Configurar el componente ClosedEyesAudioFeedback

1. **Encuentra el GameObject con el componente:**
   - Busca en la jerarquía el GameObject que tiene el componente `ClosedEyesAudioFeedback`
   - Generalmente está en el GameObject del `Player` o en un GameObject hijo

2. **Selecciona el GameObject** y ve al Inspector

3. **Localiza el componente `ClosedEyesAudioFeedback`**

4. **Asigna los AudioClips:**
   - **Light Sound**: Arrastra el archivo de audio para "carta buena" desde el Project window
   - **Heavy Sound**: Arrastra el archivo de audio para "carta mala" desde el Project window

5. **Ajusta el volumen** (opcional):
   - El campo `Volume` controla el volumen de los sonidos (0.0 a 1.0)

### Paso 3: Verificar el AudioSource

El componente `ClosedEyesAudioFeedback` necesita un `AudioSource`:
- Si no hay uno, el script lo crea automáticamente
- Si quieres configurarlo manualmente:
  1. Agrega un componente `Audio Source` al mismo GameObject
  2. Asigna ese `AudioSource` en el campo `Audio Source` del `ClosedEyesAudioFeedback`

---

## ✅ Verificación

### Checklist:

- [ ] Los archivos de audio están importados en Unity
- [ ] `Light Sound` está asignado en el Inspector
- [ ] `Heavy Sound` está asignado en el Inspector
- [ ] El `AudioSource` está configurado (o se crea automáticamente)
- [ ] El volumen está ajustado correctamente

### Prueba:

1. Ejecuta el juego
2. Cierra los ojos del jugador
3. Presiona "Robar Carta"
4. Deberías escuchar el sonido correspondiente (ligero o pesado)
5. El sonido te indica si la carta es favorable o no

---

## 🔧 Configuración avanzada del AudioSource

Si quieres personalizar el `AudioSource`:

1. Selecciona el GameObject con `ClosedEyesAudioFeedback`
2. En el componente `Audio Source`:
   - **Play On Awake**: Desactivado (se reproduce manualmente)
   - **Loop**: Desactivado (sonido único)
   - **Volume**: 1.0 (o el valor que prefieras)
   - **Spatial Blend**: 2D (sonido no espacial)

---

## 🎯 Flujo del sistema de audio

```
1. Jugador cierra los ojos
   ↓
2. Jugador presiona "Robar Carta"
   ↓
3. Sistema mira la próxima carta (sin robarla)
   ↓
4. Sistema evalúa si la carta es buena o mala
   ↓
5. Se reproduce el sonido:
   - Light Sound → Carta FAVORABLE
   - Heavy Sound → Carta NO FAVORABLE
   ↓
6. Jugador escucha el sonido
   ↓
7. Jugador decide:
   - Confirmar → Roba la carta
   - Cancelar → No roba la carta
```

---

## 🐛 Solución de problemas

### El sonido no se reproduce

1. **Verifica que los ojos estén cerrados:**
   - Los sonidos solo se reproducen con los ojos cerrados
   - Si los ojos están abiertos, no hay sonido

2. **Verifica que los AudioClips estén asignados:**
   - Revisa el Inspector del `ClosedEyesAudioFeedback`
   - Asegúrate de que `Light Sound` y `Heavy Sound` no sean `None`

3. **Verifica el AudioSource:**
   - Asegúrate de que haya un `AudioSource` en el GameObject
   - Verifica que el volumen no esté en 0

4. **Revisa la consola:**
   - Busca mensajes de error o advertencias
   - Deberías ver logs como: `"Playing LIGHT sound for preview..."`

### El sonido se reproduce pero no es claro

1. **Ajusta el volumen:**
   - Cambia el valor de `Volume` en el Inspector
   - Prueba valores entre 0.5 y 1.0

2. **Verifica los archivos de audio:**
   - Asegúrate de que los sonidos sean claramente diferentes
   - Light Sound debe ser más "ligero" o "agradable"
   - Heavy Sound debe ser más "pesado" o "desagradable"

### El sonido se reproduce siempre, incluso con ojos abiertos

- Esto no debería pasar, pero si ocurre:
  1. Verifica que `EyesController` esté asignado en el Inspector
  2. Verifica que `AreEyesOpen` funcione correctamente

---

## 📝 Notas importantes

- Los sonidos se reproducen **antes** de robar la carta, no después
- Solo funcionan cuando los ojos están **cerrados**
- El jugador puede usar estos sonidos para decidir si robar o pasar
- Si los ojos están abiertos, el jugador ve la carta directamente (sin sonido)

---

## 🎨 Recomendaciones de diseño de audio

### Light Sound (Carta Favorable):
- Sonido agradable, ligero, positivo
- Ejemplos: campanita, chime, nota musical ascendente
- Duración: 0.5 - 1 segundo

### Heavy Sound (Carta No Favorable):
- Sonido pesado, negativo, de advertencia
- Ejemplos: golpe sordo, nota grave, sonido de advertencia
- Duración: 0.5 - 1 segundo

### Consejos:
- Los sonidos deben ser claramente distinguibles
- No deben ser demasiado largos (máximo 1-2 segundos)
- El volumen debe ser audible pero no molesto
- Considera usar sonidos que no distraigan demasiado

