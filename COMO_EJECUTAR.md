# 🚀 COMO EJECUTAR SUPER JOSHUA

## Opción 1: Usar el Launcher Automático
1. **Ejecuta `Launch_SuperJoshua.bat`** (doble clic)
2. El script buscará Unity Hub automáticamente
3. Si lo encuentra, lo abrirá
4. Si no, te dará instrucciones detalladas

## Opción 2: Manual
1. **Instalar Unity** (si no lo tienes):
   - Descargar Unity Hub: https://unity.com/download
   - Instalar Unity 2022.3 LTS o superior

2. **Abrir el proyecto**:
   - Abrir Unity Hub
   - Clic en "Add"
   - Seleccionar esta carpeta: `c:\Users\angeh\Desktop\juego`
   - Abrir con Unity 2022.3 LTS+

## 📋 Configuración Inicial en Unity

### 1. Configurar Capas de Física
- **Edit → Project Settings → Tags and Layers**
- Añadir capas:
  - Layer 8: Ground
  - Layer 9: Player
  - Layer 10: PowerUps
  - Layer 11: Enemies

### 2. Crear Sprites Placeholders
Crea sprites temporales de 32x32 pixels para:
- **Joshua**: idle, walk, jump, fall
- **Sonic**: idle, run, jump, spindash  
- **Power-ups**: star (estrella), coin (moneda), ring (anillo)

### 3. Crear GameObjects
- **Player**: 
  - SpriteRenderer + Rigidbody2D + CapsuleCollider2D
  - Asignar scripts: PlayerController + PlayerStateMachine
  - Crear hijo "GroundCheck" (Transform vacío)
  
- **GameManager**: 
  - GameObject vacío con script GameManager
  
- **Power-ups**: 
  - SpriteRenderer + CircleCollider2D (IsTrigger: true)
  - Asignar scripts correspondientes

### 4. Configurar Física
- **Rigidbody2D del Player**:
  - Gravity Scale: 3
  - Freeze Rotation: ✓
- **Crear Physics Material 2D**:
  - Friction: 0.4, Bounciness: 0

### 5. ¡Presionar PLAY!
- Los controles están configurados:
  - **A/D**: Mover
  - **Space**: Saltar  
  - **Ctrl**: Spin Dash (cuando es Sonic)

## 🎮 Estados del Juego

### Joshua (Normal)
- Movimiento estilo Mario
- Salto con inercia
- Puede aplastar enemigos

### Sonic (Transformado)
- Velocidad aumentada
- Invencibilidad temporal
- Spin Dash disponible
- Transformación dura 15 segundos

## 🌟 Power-ups Implementados
- **Estrella**: Transforma a Joshua en Sonic (1000 pts)
- **Moneda**: 200 puntos, cuenta para vida extra
- **Anillo**: 100 puntos, extiende transformación

## 🐛 Debug
- Todos los scripts tienen logs detallados
- Gizmos visuales para GroundCheck
- Sin errores de compilación

---
**¡Super Joshua está listo para jugar!** 🎉

*Todos los sistemas están implementados y funcionando. Solo necesitas sprites y configuración básica en Unity.*