using UnityEngine;
using SuperJoshua.Player;

namespace SuperJoshua.PowerUps
{
    /// <summary>
    /// Estrella de invencibilidad que transforma al jugador en Sonic
    /// Este es el power-up principal del juego Super Joshua
    /// </summary>
    public class StarPowerUp : PowerUpBase
    {
        [Header("Configuración de la Estrella")]
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float pulseDuration = 0.5f;
        [SerializeField] private float pulseScale = 1.2f;

        [Header("Efectos de Transformación")]
        [SerializeField] private GameObject transformationEffect;
        [SerializeField] private AudioClip transformationSound;
        [SerializeField] private Color starGlow = Color.yellow;

        // Referencias para efectos visuales
        private SpriteRenderer spriteRenderer;
        private Vector3 originalScale;

        #region Unity Lifecycle

        protected override void Start()
        {
            base.Start();

            // Configurar tipo de power-up
            powerUpType = PowerUpType.Star;
            pointValue = 1000; // La estrella vale más puntos

            // Obtener componentes
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;

            // Configurar efecto de brillo
            if (spriteRenderer != null)
            {
                spriteRenderer.color = starGlow;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Rotación constante
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Efecto de pulso
            ApplyPulseEffect();
        }

        #endregion

        #region Power-Up Effect Implementation

        /// <summary>
        /// Aplica el efecto de transformación a Sonic
        /// </summary>
        /// <param name="player">Controlador del jugador</param>
        protected override void ApplyEffect(PlayerController player)
        {
            // Obtener la máquina de estados del jugador
            PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();

            if (stateMachine != null)
            {
                // Transformar a Sonic
                stateMachine.ChangeState(PlayerState.Sonic);

                // Efectos especiales de transformación
                PlayTransformationEffects(player.transform.position);

                Debug.Log("¡Estrella recogida! Joshua se transforma en Sonic!");
            }
            else
            {
                Debug.LogWarning("No se encontró PlayerStateMachine en el jugador!");
            }

            // Añadir puntos al marcador
            GameManager.GameManager.Instance?.AddScore(pointValue);
        }

        #endregion

        #region Visual Effects

        /// <summary>
        /// Aplica el efecto de pulso a la estrella
        /// </summary>
        private void ApplyPulseEffect()
        {
            float pulseValue = 1f + Mathf.Sin(Time.time / pulseDuration) * (pulseScale - 1f) * 0.5f;
            transform.localScale = originalScale * pulseValue;
        }

        /// <summary>
        /// Reproduce los efectos especiales de transformación
        /// </summary>
        /// <param name="position">Posición donde reproducir los efectos</param>
        private void PlayTransformationEffects(Vector3 position)
        {
            // Efecto visual de transformación
            if (transformationEffect != null)
            {
                GameObject effect = Instantiate(transformationEffect, position, Quaternion.identity);

                // Destruir el efecto después de un tiempo
                Destroy(effect, 3f);
            }

            // Sonido de transformación
            if (transformationSound != null)
            {
                AudioSource.PlayClipAtPoint(transformationSound, position, 0.8f);
            }

            // TODO: Añadir efectos de cámara (shake, zoom, etc.)
            // TODO: Añadir efectos de partículas más elaborados
        }

        /// <summary>
        /// Efectos adicionales al ser recogida
        /// </summary>
        protected override void PlayPickupEffects()
        {
            base.PlayPickupEffects();

            // Efectos adicionales específicos de la estrella
            // TODO: Flash de pantalla
            // TODO: Slow motion temporal
            // TODO: Efecto de ondas de energía
        }

        #endregion

        #region Editor Debug

        /// <summary>
        /// Dibuja gizmos en el editor para facilitar el diseño de niveles
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Dibujar radio de detección
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);

            // Dibujar dirección de rotación
            Gizmos.color = Color.red;
            Vector3 direction = transform.up;
            Gizmos.DrawRay(transform.position, direction * 0.5f);
        }

        #endregion
    }
}