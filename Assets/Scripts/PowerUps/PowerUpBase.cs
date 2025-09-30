using UnityEngine;
using SuperJoshua.Player;

namespace SuperJoshua.PowerUps
{
    /// <summary>
    /// Enumeración de tipos de power-ups disponibles
    /// </summary>
    public enum PowerUpType
    {
        Star,           // Estrella de invencibilidad (transformación a Sonic)
        Coin,           // Moneda para puntuación
        Ring,           // Anillo para puntuación (estilo Sonic)
        ExtraLife,      // Vida extra
        SpeedBoost,     // Aumento temporal de velocidad
        JumpBoost       // Aumento temporal de salto
    }

    /// <summary>
    /// Clase base para todos los power-ups del juego
    /// </summary>
    public abstract class PowerUpBase : MonoBehaviour
    {
        [Header("Configuración del Power-Up")]
        [SerializeField] protected PowerUpType powerUpType;
        [SerializeField] protected int pointValue = 100;
        [SerializeField] protected bool destroyOnPickup = true;
        [SerializeField] protected AudioClip pickupSound;

        [Header("Efectos Visuales")]
        [SerializeField] protected GameObject pickupEffectPrefab;
        [SerializeField] protected float floatAmplitude = 0.1f;
        [SerializeField] protected float floatFrequency = 2f;
        [SerializeField] protected bool enableFloating = true;

        // Variables privadas
        private Vector3 startPosition;
        private float timeOffset;

        #region Unity Lifecycle

        protected virtual void Start()
        {
            // Guardar posición inicial para el efecto de flotación
            startPosition = transform.position;
            timeOffset = Random.Range(0f, 2f * Mathf.PI); // Offset aleatorio para sincronización
        }

        protected virtual void Update()
        {
            // Efecto de flotación
            if (enableFloating)
            {
                ApplyFloatingMotion();
            }
        }

        #endregion

        #region Collision Detection

        /// <summary>
        /// Detecta cuando el jugador toca el power-up
        /// </summary>
        /// <param name="other">Collider que entra en contacto</param>
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    // Aplicar efecto del power-up
                    ApplyEffect(player);

                    // Reproducir efectos
                    PlayPickupEffects();

                    // Destruir si es necesario
                    if (destroyOnPickup)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Aplica el efecto específico del power-up al jugador
        /// </summary>
        /// <param name="player">Referencia al controlador del jugador</param>
        protected abstract void ApplyEffect(PlayerController player);

        #endregion

        #region Visual Effects

        /// <summary>
        /// Aplica el movimiento de flotación
        /// </summary>
        private void ApplyFloatingMotion()
        {
            float newY = startPosition.y + Mathf.Sin((Time.time + timeOffset) * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }

        /// <summary>
        /// Reproduce los efectos visuales y sonoros del power-up
        /// </summary>
        protected virtual void PlayPickupEffects()
        {
            // Efecto visual
            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
            }

            // Efecto sonoro
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Debug.Log($"Power-up {powerUpType} recogido por {pointValue} puntos!");
        }

        #endregion

        #region Public Properties

        public PowerUpType Type => powerUpType;
        public int PointValue => pointValue;

        #endregion
    }
}