# 📋 Guía de Configuración de UI

## 🎯 Estructura de la Jerarquía en Unity

```
Hierarchy (Escena Miguel)
│
├── Main Camera
├── Player (Tag: "Player")
│   ├── Component: Player.cs
│   ├── Component: EyesController.cs
│   ├── Component: ClosedEyesAudioFeedback.cs
│   └── Component: CardQualityEvaluator.cs
│
├── Wizard (Tag: "Wizard")
│   └── Component: Wizard.cs
│
├── GameManager
│   └── Component: GameManager.cs
│
├── ProbabilityManager
│   └── Component: ProbabilityManager.cs
│
├── EventSystem (Ya existe en tu escena)
│
└── Canvas (⭐ CREAR ESTO)
    ├── Component: Canvas
    ├── Component: GraphicRaycaster
    │
    ├── PlayerUIManager (GameObject vacío)
    │   └── Component: PlayerUIManager.cs ⭐
    │
    ├── EyesDecisionPanel (Panel/GameObject)
    │   ├── Text: "¿Abrir o cerrar los ojos?"
    │   ├── Button: "Ojos Abiertos 👁️"
    │   └── Button: "Ojos Cerrados 🚫"
    │
    ├── ActionPanel (Panel/GameObject)
    │   ├── Button: "Robar Carta"
    │   └── Button: "Pasar"
    │
    ├── ConfirmationPanel (Panel/GameObject)
    │   ├── Text: "CardPreviewText" (muestra info de la carta)
    │   ├── Button: "Confirmar"
    │   └── Button: "Cancelar"
    │
    └── InfoPanel (Panel/GameObject - Opcional)
        ├── Text: "PlayerTotalText" (muestra total del jugador)
        └── Text: "EyesStatusText" (muestra estado de ojos)
```

## 📝 Pasos para Configurar

### 1. Crear el Canvas
1. En la jerarquía, clic derecho → **UI → Canvas**
2. El Canvas se crea automáticamente con:
   - Canvas (componente)    
   - GraphicRaycaster (componente)
   - EventSystem (si no existe ya)

### 2. Crear el PlayerUIManager
1. Dentro del Canvas, crea un GameObject vacío: **PlayerUIManager**
2. Agrega el componente: **PlayerUIManager.cs**
3. En el Inspector, asigna las referencias:
   - **Player**: Arrastra el GameObject "Player" de la jerarquía
   - **Game Manager**: Arrastra el GameObject "GameManager"
   - **Eyes Controller**: Arrastra el componente EyesController del Player

### 3. Crear los Paneles de UI

#### Panel 1: EyesDecisionPanel
1. Clic derecho en Canvas → **UI → Panel** → Renombrar a "EyesDecisionPanel"
2. Dentro del panel:
   - **Text**: "¿Abrir o cerrar los ojos?"
   - **Button**: "Ojos Abiertos 👁️"
   - **Button**: "Ojos Cerrados 🚫"
3. En PlayerUIManager, arrastra:
   - **Eyes Decision Panel**: El panel que acabas de crear
   - **Open Eyes Button**: El botón de ojos abiertos
   - **Close Eyes Button**: El botón de ojos cerrados

#### Panel 2: ActionPanel
1. Clic derecho en Canvas → **UI → Panel** → Renombrar a "ActionPanel"
2. Dentro del panel:
   - **Button**: "Robar Carta"
   - **Button**: "Pasar"
3. En PlayerUIManager, arrastra:
   - **Action Panel**: El panel que acabas de crear
   - **Draw Card Button**: El botón "Robar Carta"
   - **Pass Button**: El botón "Pasar"

#### Panel 3: ConfirmationPanel
1. Clic derecho en Canvas → **UI → Panel** → Renombrar a "ConfirmationPanel"
2. Dentro del panel:
   - **Text**: "CardPreviewText" (muestra la info de la carta)
   - **Button**: "Confirmar"
   - **Button**: "Cancelar"
3. En PlayerUIManager, arrastra:
   - **Confirmation Panel**: El panel que acabas de crear
   - **Confirm Button**: El botón "Confirmar"
   - **Cancel Button**: El botón "Cancelar"
   - **Card Preview Text**: El Text que muestra la info

#### Panel 4: InfoPanel (Opcional - para mostrar info del jugador)
1. Clic derecho en Canvas → **UI → Panel** → Renombrar a "InfoPanel"
2. Dentro del panel:
   - **Text**: "PlayerTotalText" (muestra "Total: X")
   - **Text**: "EyesStatusText" (muestra "Ojos: ABIERTOS/CERRADOS")
3. En PlayerUIManager, arrastra:
   - **Player Total Text**: El Text del total
   - **Eyes Status Text**: El Text del estado de ojos

## 🔗 Asignación de Referencias en PlayerUIManager

En el Inspector del **PlayerUIManager**, debes asignar:

### Referencias (Header)
- **Player**: Arrastra el GameObject "Player"
- **Game Manager**: Arrastra el GameObject "GameManager"
- **Eyes Controller**: Arrastra el componente EyesController del Player

### UI - Decisión de Ojos
- **Eyes Decision Panel**: El panel EyesDecisionPanel
- **Open Eyes Button**: El botón de ojos abiertos
- **Close Eyes Button**: El botón de ojos cerrados

### UI - Acciones
- **Action Panel**: El panel ActionPanel
- **Draw Card Button**: El botón "Robar Carta"
- **Pass Button**: El botón "Pasar"

### UI - Confirmación
- **Confirmation Panel**: El panel ConfirmationPanel
- **Confirm Button**: El botón "Confirmar"
- **Cancel Button**: El botón "Cancelar"
- **Card Preview Text**: El Text que muestra la info de la carta

### UI - Información
- **Player Total Text**: El Text que muestra el total (opcional)
- **Eyes Status Text**: El Text que muestra el estado de ojos (opcional)

## ✅ Verificación

1. **Canvas**: Debe estar en la raíz de la jerarquía (no dentro de otro objeto)
2. **PlayerUIManager**: Debe estar dentro del Canvas
3. **Todos los paneles**: Deben estar dentro del Canvas
4. **Referencias**: Todas deben estar asignadas en el Inspector del PlayerUIManager

## 🎮 Flujo de la UI

1. **Inicio del turno del jugador**:
   - Se muestra `EyesDecisionPanel`
   - Jugador elige abrir o cerrar ojos

2. **Después de elegir ojos**:
   - Se muestra `ActionPanel`
   - Jugador elige "Robar Carta" o "Pasar"

3. **Si elige "Robar Carta"**:
   - Se muestra `ConfirmationPanel` con el preview
   - Si ojos cerrados: muestra "Sonido: LIGERO/PESADO"
   - Si ojos abiertos: muestra "Carta: X | Nuevo total: Y"
   - Jugador confirma o cancela

4. **Si elige "Pasar"**:
   - El turno termina inmediatamente

## 📌 Notas Importantes

- El Canvas debe estar en la **capa UI** (Layer: UI)
- Los paneles deben estar **desactivados** al inicio (excepto si quieres mostrar info)
- El `PlayerUIManager` se activa automáticamente cuando es el turno del jugador
- Los botones están conectados automáticamente por el script `PlayerUIManager.cs`

