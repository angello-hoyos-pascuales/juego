using UnityEngine;
using SuperJoshua.Player;

namespace SuperJoshua.PowerUps
{
    /// <summary>
    /// Moneda coleccionable que otorga puntos al jugador
    /// </summary>
    public class CoinPowerUp : PowerUpBase
    {
        [Header("Configuración de la Moneda")]
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private bool useSpinAnimation = true;

        #region Unity Lifecycle

        protected override void Start()
        {
            base.Start();

            // Configurar tipo de power-up
            powerUpType = PowerUpType.Coin;
            pointValue = 200;
        }

        protected override void Update()
        {
            base.Update();

            // Rotación de la moneda
            if (useSpinAnimation)
            {
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }
        }

        #endregion

        #region Power-Up Effect Implementation

        /// <summary>
        /// Aplica el efecto de la moneda (añadir puntos)
        /// </summary>
        /// <param name="player">Controlador del jugador</param>
        protected override void ApplyEffect(PlayerController player)
        {
            // Añadir puntos al marcador
            GameManager.GameManager.Instance?.AddScore(pointValue);

            // TODO: Implementar sistema de monedas para vidas extra
            // TODO: Añadir contador de monedas en la UI

            Debug.Log($"¡Moneda recogida! +{pointValue} puntos");
        }

        #endregion
    }

    /// <summary>
    /// Anillo coleccionable estilo Sonic (se activa durante la transformación)
    /// </summary>
    public class RingPowerUp : PowerUpBase
    {
        [Header("Configuración del Anillo")]
        [SerializeField] private float glowIntensity = 1.5f;
        [SerializeField] private Color ringColor = Color.yellow;

        // Componentes para el efecto de brillo
        private Light ringLight;

        #region Unity Lifecycle

        protected override void Start()
        {
            base.Start();

            // Configurar tipo de power-up
            powerUpType = PowerUpType.Ring;
            pointValue = 100;

            // Configurar luz de brillo
            SetupGlowEffect();
        }

        #endregion

        #region Power-Up Effect Implementation

        /// <summary>
        /// Aplica el efecto del anillo (puntos y posible extensión de transformación)
        /// </summary>
        /// <param name="player">Controlador del jugador</param>
        protected override void ApplyEffect(PlayerController player)
        {
            // Añadir puntos al marcador
            GameManager.GameManager.Instance?.AddScore(pointValue);

            // Si el jugador está transformado, extender ligeramente la duración
            PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();
            if (stateMachine != null && stateMachine.IsTransformed)
            {
                stateMachine.ExtendTransformation(1f); // Extender 1 segundo
                Debug.Log($"¡Anillo recogido! Transformación extendida +1 segundo");
            }

            Debug.Log($"¡Anillo recogido! +{pointValue} puntos");
        }

        #endregion

        #region Visual Setup

        /// <summary>
        /// Configura el efecto de brillo del anillo
        /// </summary>
        private void SetupGlowEffect()
        {
            // Crear una luz para el efecto de brillo
            GameObject lightObj = new GameObject("RingGlow");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.zero;

            ringLight = lightObj.AddComponent<Light>();
            ringLight.type = LightType.Point;
            ringLight.color = ringColor;
            ringLight.intensity = glowIntensity;
            ringLight.range = 2f;

            // Configurar sprite renderer
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = ringColor;
            }
        }

        #endregion
    }
}