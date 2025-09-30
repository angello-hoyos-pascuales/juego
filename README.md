# Super Joshua - Juego 2D de Plataformas

![Super Joshua Logo](https://via.placeholder.com/600x200/4CAF50/FFFFFF?text=SUPER+JOSHUA)

**Super Joshua** es un juego 2D de plataformas inspirado en Mario con mecánicas únicas de transformación. El jugador controla a Joshua, quien puede transformarse en Sonic al recoger estrellas de invencibilidad, obteniendo habilidades especiales como velocidad extrema y el Spin Dash.

## 🎮 Características del Juego

### Personajes
- **Joshua**: Personaje principal basado en pixel art
- **Sonic**: Forma transformada con habilidades especiales

### Mecánicas Principales
- Movimiento estilo Mario (correr, saltar, inercia)
- Sistema de transformación con la Estrella de Invencibilidad
- Aplastamiento de enemigos saltando sobre ellos
- Recolección de monedas y anillos para puntuación
- Sistema de vidas y puntuación progresiva

### Power-ups
- **Estrella de Invencibilidad**: Transforma a Joshua en Sonic
- **Monedas**: Otorgan puntos y vidas extra
- **Anillos**: Puntos adicionales y extensión de transformación

### Habilidades de Sonic
- Velocidad extrema
- Invencibilidad temporal
- Spin Dash (ataque rodante)

## 🛠️ Tecnologías Utilizadas

- **Motor de Juego**: Unity 2022.3 LTS o superior
- **Lenguaje**: C# 9.0+
- **Plataforma**: PC (Windows, macOS, Linux)
- **Arte**: Pixel Art 2D de 16-bit
- **Física**: Unity Physics2D
- **Arquitectura**: Patrón de Máquina de Estados

## 📁 Estructura del Proyecto

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerStates.cs           # Definición de estados del jugador
│   │   ├── PlayerStateMachine.cs     # Máquina de estados y transformaciones
│   │   └── PlayerController.cs       # Controlador de movimiento y físicas
│   ├── PowerUps/
│   │   ├── PowerUpBase.cs           # Clase base para power-ups
│   │   ├── StarPowerUp.cs           # Estrella de invencibilidad
│   │   └── CollectiblePowerUps.cs   # Monedas y anillos
│   └── GameManager/
│       └── GameManager.cs           # Gestor principal del juego
├── Sprites/
│   ├── Player/                      # Sprites de Joshua y Sonic
│   ├── PowerUps/                    # Sprites de power-ups
│   └── Environment/                 # Sprites del entorno
├── Animations/                      # Animaciones de personajes
├── Scenes/                          # Escenas del juego
└── Prefabs/                         # Prefabs reutilizables
```

## 🚀 Configuración del Proyecto

### Requisitos del Sistema
- Unity 2022.3 LTS o superior
- Visual Studio Code o Visual Studio
- Git para control de versiones

### Instalación

1. **Clonar el repositorio**:
   ```bash
   git clone <repository-url>
   cd super-joshua
   ```

2. **Abrir en Unity**:
   - Abrir Unity Hub
   - Hacer clic en "Add" y seleccionar la carpeta del proyecto
   - Asegurarse de usar Unity 2022.3 LTS o superior

3. **Configurar las capas de física**:
   - Ir a Edit > Project Settings > Tags and Layers
   - Configurar las capas: Ground, Player, Enemies, PowerUps

4. **Configurar Input Manager**:
   - Verificar que los controles estén configurados:
     - Horizontal: A/D o flechas izquierda/derecha
     - Jump: Spacebar
     - Fire1: Left Click o Ctrl (para Spin Dash)

## 🎯 Controles del Juego

### Controles de Joshua (Estado Normal)
- **A/D o ←/→**: Mover izquierda/derecha
- **Spacebar**: Saltar
- **Saltar sobre enemigos**: Aplastar enemigos

### Controles de Sonic (Estado Transformado)
- **A/D o ←/→**: Mover izquierda/derecha (velocidad aumentada)
- **Spacebar**: Saltar (altura aumentada)
- **Ctrl (mantener)**: Cargar Spin Dash
- **Ctrl (soltar)**: Ejecutar Spin Dash
- **Invencibilidad**: Automática durante la transformación

### Controles del Sistema
- **Escape o P**: Pausar/Reanudar
- **R**: Reiniciar nivel (en desarrollo)

## 🏗️ Arquitectura del Código

### Sistema de Estados del Jugador

El juego utiliza una **Máquina de Estados** para manejar las transformaciones:

```csharp
public enum PlayerState
{
    Joshua,    // Estado normal
    Sonic      // Estado transformado
}
```

### Flujo de Transformación

1. **Colisión con Estrella**: Detecta contacto con StarPowerUp
2. **Cambio de Estado**: `PlayerStateMachine.ChangeState(PlayerState.Sonic)`
3. **Actualización Visual**: Cambia sprites y efectos visuales
4. **Actualización de Stats**: Modifica velocidad, salto y habilidades
5. **Temporizador**: Inicia cuenta regresiva para volver a Joshua
6. **Reversión**: Automáticamente vuelve al estado normal

### Clases Principales

- **`PlayerController`**: Movimiento, físicas y detección de colisiones
- **`PlayerStateMachine`**: Gestión de estados y transformaciones
- **`PowerUpBase`**: Clase base para todos los power-ups
- **`GameManager`**: Puntuación, vidas y estado global del juego

## 🎨 Assets y Arte

### Sprites Necesarios

Para completar el juego, necesitarás crear los siguientes sprites en pixel art:

#### Joshua (32x32 píxeles recomendado)
- `joshua_idle.png`: Sprite en reposo
- `joshua_walk_01.png` a `joshua_walk_04.png`: Animación de caminar
- `joshua_jump.png`: Sprite de salto
- `joshua_fall.png`: Sprite de caída

#### Sonic (32x32 píxeles recomendado)
- `sonic_idle.png`: Sprite en reposo
- `sonic_run_01.png` a `sonic_run_06.png`: Animación de correr
- `sonic_jump.png`: Sprite de salto
- `sonic_spindash.png`: Sprite de Spin Dash

#### Power-ups (16x16 píxeles recomendado)
- `star.png`: Estrella de invencibilidad
- `coin.png`: Moneda dorada
- `ring.png`: Anillo azul/dorado

### Herramientas Recomendadas para Arte
- **Aseprite**: Editor principal para pixel art
- **GIMP**: Alternativa gratuita
- **Piskel**: Editor online gratuito

## 🏆 Sistema de Puntuación

- **Monedas**: 200 puntos
- **Anillos**: 100 puntos
- **Estrellas**: 1000 puntos
- **Enemigos Derrotados**: 100 puntos
- **Completar Nivel**: 1000 puntos
- **Vida Extra**: Cada 10,000 puntos o 100 monedas

## 🐛 Debugging y Desarrollo

### Logs de Debug
El juego incluye logs detallados para facilitar el desarrollo:

```csharp
Debug.Log("Estado cambiado de Joshua a Sonic");
Debug.Log("¡Estrella recogida! Joshua se transforma en Sonic!");
Debug.Log("Spin Dash ejecutado con fuerza 25!");
```

### Gizmos de Editor
- **Detección de Suelo**: Esfera verde/roja en `groundCheck`
- **Radio de Power-ups**: Círculos amarillos para áreas de detección

### Configuración de Physics2D
- **Gravity Scale**: 3.0 (más pesado que el default)
- **Material de Física**: Crear material con fricción 0.4 para el jugador

## 🔮 Características Futuras

### Próximas Implementaciones
- [ ] Sistema de enemigos con IA básica
- [ ] Más tipos de power-ups
- [ ] Sistema de checkpoints
- [ ] Múltiples niveles con temas diferentes
- [ ] Boss battles
- [ ] Sistema de guardado de progreso
- [ ] Efectos de sonido y música
- [ ] Menú principal y UI mejorada
- [ ] Sistema de logros

### Ideas de Expansión
- [ ] Modo cooperativo local
- [ ] Niveles generados proceduralmente
- [ ] Editor de niveles integrado
- [ ] Diferentes personajes jugables
- [ ] Power-ups temporales adicionales

## 📝 Notas de Desarrollo

### Convenciones de Código
- Usar **PascalCase** para métodos y propiedades públicas
- Usar **camelCase** para variables privadas
- Incluir **XML Documentation** en métodos públicos
- Organizar código en **namespaces** por funcionalidad

### Estructura de Namespaces
```csharp
SuperJoshua.Player          // Todo lo relacionado al jugador
SuperJoshua.PowerUps        // Sistema de power-ups
SuperJoshua.GameManager     // Gestión del estado del juego
SuperJoshua.UI              // Interfaz de usuario (futuro)
SuperJoshua.Audio           // Sistema de audio (futuro)
```

## 🤝 Contribución

Este es un proyecto personal de aprendizaje. Si tienes sugerencias o mejoras:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 👤 Autor

**Tu Nombre**
- GitHub: [@tu-usuario](https://github.com/tu-usuario)
- Email: tu-email@ejemplo.com

## 🙏 Agradecimientos

- Inspirado en los clásicos Super Mario Bros y Sonic the Hedgehog
- Unity Technologies por el excelente motor de juego
- La comunidad de desarrollo indie por la inspiración

---

**¡Disfruta creando y jugando Super Joshua!** 🌟
