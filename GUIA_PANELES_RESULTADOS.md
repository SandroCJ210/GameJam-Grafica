# Guía: Configuración de Paneles de Resultados y Decisiones del Mago

Esta guía explica cómo configurar los nuevos paneles de UI para mostrar los resultados de la ronda y las decisiones del mago.

## 📋 Paneles Necesarios

### 1. Panel de Resultados (`RoundResultUIManager`)
Muestra al finalizar cada ronda:
- Ganador (Jugador/Mago/Empate)
- Suma del Jugador
- Suma del Mago
- Razón de la victoria

### 2. Panel de Decisiones del Mago (`WizardDecisionsUIManager`)
Muestra en tiempo real las decisiones del mago durante la ronda:
- Cada vez que roba una carta
- Cada vez que pasa
- Total actualizado después de cada acción

---

## 🎨 Configuración en Unity

### Panel de Resultados

1. **Crear el Panel:**
   - En tu Canvas, crea un nuevo `Panel` llamado `ResultPanel`
   - Configura el tamaño y posición (recomendado: centrado, tamaño medio)

2. **Agregar Textos (TextMeshProUGUI):**
   - `WinnerText`: Muestra "Ganador: [Nombre]"
   - `PlayerTotalText`: Muestra "Jugador: [Total]"
   - `WizardTotalText`: Muestra "Mago: [Total]"
   - `ReasonText`: Muestra la razón de la victoria

3. **Agregar Botón:**
   - `ContinueButton`: Botón para continuar a la siguiente ronda

4. **Asignar el Script:**
   - Agrega el componente `RoundResultUIManager` al GameObject del Canvas o a un GameObject hijo
   - Asigna todas las referencias en el Inspector:
     - `Result Panel` → El Panel creado
     - `Winner Text` → El TextMeshProUGUI del ganador
     - `Player Total Text` → El TextMeshProUGUI del total del jugador
     - `Wizard Total Text` → El TextMeshProUGUI del total del mago
     - `Reason Text` → El TextMeshProUGUI de la razón
     - `Continue Button` → El botón de continuar

5. **Estado Inicial:**
   - El panel debe estar **desactivado** al inicio (el script lo oculta automáticamente)

---

### Panel de Decisiones del Mago

1. **Crear el Panel:**
   - En tu Canvas, crea un nuevo `Panel` llamado `WizardDecisionsPanel`
   - Configura el tamaño y posición (recomendado: esquina superior derecha, tamaño pequeño-mediano)

2. **Agregar Texto:**
   - `DecisionsText`: TextMeshProUGUI que mostrará la lista de decisiones
   - Configura el texto para que sea scrollable si es necesario (agrega un `ScrollRect`)

3. **Agregar Botón (Opcional):**
   - `TogglePanelButton`: Botón para mostrar/ocultar el panel
   - Puede ser un botón pequeño en una esquina

4. **Asignar el Script:**
   - Agrega el componente `WizardDecisionsUIManager` al GameObject del Canvas o a un GameObject hijo
   - Asigna todas las referencias en el Inspector:
     - `Decisions Panel` → El Panel creado
     - `Decisions Text` → El TextMeshProUGUI de las decisiones
     - `Toggle Panel Button` → El botón de toggle (opcional)
     - `Panel Visible By Default` → Marca si quieres que el panel esté visible al inicio

5. **Estado Inicial:**
   - El panel puede estar activado o desactivado según `Panel Visible By Default`

---

## 🔧 Estructura Recomendada en la Jerarquía

```
Canvas
├── PlayerUI (Panel principal del jugador)
│   ├── EyesDecisionPanel
│   ├── ActionPanel
│   └── ConfirmationPanel
├── ResultPanel (NUEVO)
│   ├── WinnerText
│   ├── PlayerTotalText
│   ├── WizardTotalText
│   ├── ReasonText
│   └── ContinueButton
└── WizardDecisionsPanel (NUEVO)
    ├── DecisionsText
    └── ToggleButton (opcional)
```

---

## ✅ Verificación

### Panel de Resultados
- [ ] El panel está desactivado al inicio
- [ ] Todas las referencias están asignadas en el Inspector
- [ ] El script `RoundResultUIManager` está en el Canvas o en un GameObject hijo
- [ ] El botón "Continuar" oculta el panel al hacer clic

### Panel de Decisiones del Mago
- [ ] Todas las referencias están asignadas en el Inspector
- [ ] El script `WizardDecisionsUIManager` está en el Canvas o en un GameObject hijo
- [ ] El texto se actualiza cuando el mago toma decisiones
- [ ] El botón de toggle funciona (si está asignado)

---

## 🎮 Funcionamiento

### Panel de Resultados
- Se muestra automáticamente cuando termina una ronda
- Muestra el ganador, las sumas y la razón
- El jugador presiona "Continuar" para ocultarlo y continuar

### Panel de Decisiones del Mago
- Se actualiza automáticamente cada vez que el mago toma una decisión
- Muestra todas las decisiones de la ronda actual
- Se limpia automáticamente cuando comienza una nueva ronda

---

## 🐛 Solución de Problemas

### El panel de resultados no aparece
- Verifica que `RoundResultUIManager` esté en la escena
- Verifica que todas las referencias estén asignadas
- Revisa la consola para ver si hay errores

### Las decisiones del mago no se actualizan
- Verifica que `WizardDecisionsUIManager` esté en la escena
- Verifica que `GameManager.Instance` no sea null
- Revisa la consola para ver si hay errores

### Los textos no se muestran
- Verifica que los TextMeshProUGUI tengan el componente `TextMeshProUGUI` (no `Text`)
- Verifica que los textos estén dentro del panel y sean visibles
- Verifica que el tamaño del texto sea adecuado

---

## 📝 Notas

- Los paneles se crean automáticamente cuando termina una ronda o cuando el mago toma decisiones
- El panel de decisiones se actualiza en tiempo real usando `Update()`
- Puedes optimizar el panel de decisiones usando eventos en lugar de `Update()` si es necesario

