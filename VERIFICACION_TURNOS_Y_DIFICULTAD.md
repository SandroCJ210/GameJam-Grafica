# Verificación: Sistema de Turnos y Dificultad

Este documento explica los cambios realizados para corregir el sistema de turnos y asegurar que los componentes de dificultad funcionen correctamente.

---

## 🔧 Cambios Realizados

### 1. Corrección de la Lógica de "Pasar Consecutivamente"

**Problema anterior:**
- Las elecciones se reseteaban al inicio de cada turno
- No se podía detectar si ambos jugadores pasaron consecutivamente

**Solución implementada:**
- Las elecciones ya NO se resetean al inicio de cada turno
- Solo se resetean cuando alguien roba una carta (Draw)
- Se mantiene Pass si alguien pasó, para detectar cuando ambos pasan consecutivamente
- La ronda termina cuando ambos tienen `gamblerChoice == Pass` al mismo tiempo

**Código en `GameManager.cs`:**
```csharp
// Verificar condición de fin de ronda: ambos deben pasar CONSECUTIVAMENTE
bool endOfRoundCondition = _player.gamblerChoice == GamblerChoice.Pass &&
                           _wizard.gamblerChoice == GamblerChoice.Pass;
if (endOfRoundCondition)
{
    Debug.Log("Ambos pasaron consecutivamente. Terminando ronda...");
    _isRoundOver = true;
    break;
}

// Si alguien robó carta, resetear su elección para el próximo turno
// Pero mantener Pass si pasó, para poder detectar cuando ambos pasan consecutivamente
if (_currentTurn == Turn.PlayerTurn && _player.gamblerChoice == GamblerChoice.Draw)
{
    _player.gamblerChoice = GamblerChoice.None;
}
else if (_currentTurn == Turn.WizardTurn && _wizard.gamblerChoice == GamblerChoice.Draw)
{
    _wizard.gamblerChoice = GamblerChoice.None;
}
```

---

### 2. Inicialización Automática de Componentes de Dificultad

**Problema anterior:**
- Los componentes `probabilityManager` y `difficulty` podían no estar asignados
- No había validación ni inicialización automática

**Solución implementada:**
- Agregado método `InitializeDifficultyComponents()` en `Wizard`
- Busca automáticamente los componentes si no están asignados
- Valida que los componentes críticos estén presentes
- Muestra logs informativos sobre el estado de inicialización

**Componentes que se inicializan:**
1. **Player**: Busca automáticamente si no está asignado
2. **ProbabilityManager**: Busca en el mismo GameObject o en la escena
3. **MageDifficultyConfig**: Debe estar asignado en el Inspector (se muestra advertencia si no está)

---

## ✅ Verificación en Unity

### Checklist de Componentes

#### En el GameObject "Wizard":
- [ ] Componente `Wizard` está presente
- [ ] Campo `Player` está asignado (o se encuentra automáticamente)
- [ ] Campo `Probability Manager` está asignado (opcional, pero recomendado)
- [ ] Campo `Difficulty` (MageDifficultyConfig) está asignado (recomendado)

#### En el GameObject con `ProbabilityManager`:
- [ ] Componente `ProbabilityManager` está presente
- [ ] Campo `Config` (MageDifficultyConfig) está asignado
- [ ] Campo `Copies Per Value` está configurado (por defecto: 4)

#### ScriptableObject `MageDifficultyConfig`:
- [ ] Existe un asset de tipo `MageDifficultyConfig`
- [ ] Tiene valores configurados:
  - `boldness` (0.0 a 1.0)
  - `mistakeChanceEyesClosed` (0.0 a 1.0)
  - `goodCard_Player_EyesOpen` (0.0 a 1.0)
  - `goodCard_Player_EyesClosed` (0.0 a 1.0)
  - `goodCard_Wizard_EyesOpen` (0.0 a 1.0)
  - `goodCard_Wizard_EyesClosed` (0.0 a 1.0)

---

## 🎮 Flujo de Turnos Corregido

### Flujo Normal:
```
1. Turno del Jugador
   - Jugador elige: Robar o Pasar
   - Si roba: gamblerChoice = Draw → se resetea a None después
   - Si pasa: gamblerChoice = Pass → se mantiene

2. Turno del Mago
   - Mago elige: Robar o Pasar
   - Si roba: gamblerChoice = Draw → se resetea a None después
   - Si pasa: gamblerChoice = Pass → se mantiene

3. Verificación de Fin de Ronda
   - Si ambos tienen Pass → Termina la ronda
   - Si alguien se pasó de 21 → Termina la ronda
   - Si no → Cambia de turno y continúa
```

### Ejemplo de "Ambos Pasan Consecutivamente":
```
Turno 1: Jugador pasa → gamblerChoice = Pass (se mantiene)
Turno 2: Mago pasa → gamblerChoice = Pass (se mantiene)
Verificación: Ambos tienen Pass → ¡Ronda termina!
```

---

## 🐛 Solución de Problemas

### El juego no termina cuando ambos pasan

**Verificar:**
1. Revisa la consola para ver el log: `"Ambos pasaron consecutivamente. Terminando ronda..."`
2. Verifica que `gamblerChoice` se esté estableciendo correctamente:
   - En `Player.Pass()`: `gamblerChoice = GamblerChoice.Pass`
   - En `Wizard.Pass()`: `gamblerChoice = GamblerChoice.Pass`
3. Asegúrate de que `SetEndofTurn()` se llame después de cada acción

### El mago no usa la dificultad correcta

**Verificar:**
1. Revisa la consola al inicio para ver: `"Wizard inicializado - Player: ✓, ProbabilityManager: ✓, Difficulty: ✓"`
2. Si algún componente muestra "✗", asígnalo en el Inspector
3. Verifica que el `MageDifficultyConfig` esté asignado en el campo `Difficulty` del Wizard
4. Verifica que los valores en el `MageDifficultyConfig` sean razonables (0.0 a 1.0)

### Los componentes no se encuentran automáticamente

**Solución:**
- Asigna manualmente los componentes en el Inspector del Wizard
- Asegúrate de que el GameObject con `ProbabilityManager` esté en la escena
- Asegúrate de que el `Player` esté en la escena con el tag "Player"

---

## 📝 Notas Importantes

1. **El `ProbabilityManager` no se usa actualmente en `Wizard.DrawCard()`**
   - El `Wizard` usa `GameManager.DealCard()` que toma cartas del mazo barajado
   - El `ProbabilityManager` está diseñado para influir en qué cartas se dan según la dificultad
   - Si quieres usar `ProbabilityManager`, necesitarías modificar `GameManager.DealCard()` o `Wizard.DrawCard()`

2. **El `MageDifficultyConfig` se usa en `Wizard.DecideAction()`**
   - `boldness`: Afecta la probabilidad de robar cuando el total está entre 12-18
   - `mistakeChanceEyesClosed`: Probabilidad de que el mago tome una decisión incorrecta cuando el jugador tiene los ojos cerrados

3. **Las elecciones se mantienen hasta que alguien roba**
   - Si alguien pasa, su `gamblerChoice` se mantiene como `Pass`
   - Si alguien roba, su `gamblerChoice` se resetea a `None` después del turno
   - Esto permite detectar cuando ambos pasan consecutivamente

---

## 🎯 Próximos Pasos (Opcional)

Si quieres que el `ProbabilityManager` influya en las cartas que se dan:

1. Modificar `GameManager.DealCard()` para usar `ProbabilityManager.GetNextCard()`
2. O modificar `Wizard.DrawCard()` para usar `ProbabilityManager` antes de llamar a `GameManager.DealCard()`
3. Asegurarte de que el `ProbabilityManager` se sincronice con el mazo del `GameManager`

Por ahora, el sistema funciona correctamente con la lógica de turnos y la detección de cuando ambos pasan consecutivamente.

