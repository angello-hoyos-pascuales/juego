using UnityEngine;

namespace SuperJoshua.Player
{
    /// <summary>
    /// Máquina de estados que controla las transformaciones del jugador
    /// entre Joshua (estado normal) y Sonic (estado con poderes)
    /// </summary>
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("Estados del Jugador")]
        [SerializeField] private PlayerStateData joshuaState;
        [SerializeField] private PlayerStateData sonicState;

        [Header("Configuración de Transformación")]
        [SerializeField] private float transformationDuration = 15f;
        [SerializeField] private float warningTime = 3f; // Tiempo de advertencia antes de volver al estado normal

        // Estado actual
        private PlayerState currentState = PlayerState.Joshua;
        private PlayerStateData currentStateData;
        private float transformationTimer = 0f;
        private bool isTransformed = false;

        // Referencias a componentes
        private SpriteRenderer spriteRenderer;
        private PlayerController playerController;

        // Eventos
        public System.Action<PlayerState> OnStateChanged;
        public System.Action OnTransformationWarning;
        public System.Action OnTransformationEnded;

        #region Unity Lifecycle

        private void Awake()
        {
            // Obtener componentes necesarios
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerController = GetComponent<PlayerController>();

            // Configurar estados predeterminados si no están asignados
            SetupDefaultStates();
        }

        private void Start()
        {
            // Inicializar en estado Joshua
            ChangeState(PlayerState.Joshua);
        }

        private void Update()
        {
            // Actualizar temporizador de transformación
            UpdateTransformationTimer();
        }

        #endregion

        #region State Management

        /// <summary>
        /// Cambia el estado del jugador
        /// </summary>
        /// <param name="newState">Nuevo estado del jugador</param>
        public void ChangeState(PlayerState newState)
        {
            if (currentState == newState && !isTransformed) return;

            PlayerState previousState = currentState;
            currentState = newState;

            // Obtener datos del nuevo estado
            currentStateData = GetStateData(newState);

            // Aplicar cambios visuales
            ApplyVisualChanges();

            // Aplicar cambios de estadísticas
            ApplyStatChanges();

            // Manejar transformación temporal
            if (newState == PlayerState.Sonic)
            {
                StartTransformation();
            }
            else if (previousState == PlayerState.Sonic)
            {
                EndTransformation();
            }

            // Notificar cambio de estado
            OnStateChanged?.Invoke(newState);

            Debug.Log($"Estado cambiado de {previousState} a {newState}");
        }

        /// <summary>
        /// Obtiene los datos del estado especificado
        /// </summary>
        /// <param name="state">Estado del cual obtener los datos</param>
        /// <returns>Datos del estado</returns>
        private PlayerStateData GetStateData(PlayerState state)
        {
            return state == PlayerState.Joshua ? joshuaState : sonicState;
        }

        #endregion

        #region Transformation System

        /// <summary>
        /// Inicia la transformación a Sonic
        /// </summary>
        private void StartTransformation()
        {
            isTransformed = true;
            transformationTimer = transformationDuration;

            Debug.Log("¡Transformación a Sonic iniciada!");
        }

        /// <summary>
        /// Termina la transformación y vuelve a Joshua
        /// </summary>
        private void EndTransformation()
        {
            isTransformed = false;
            transformationTimer = 0f;

            OnTransformationEnded?.Invoke();
            Debug.Log("Transformación terminada. Volviendo a Joshua.");
        }

        /// <summary>
        /// Actualiza el temporizador de transformación
        /// </summary>
        private void UpdateTransformationTimer()
        {
            if (!isTransformed) return;

            transformationTimer -= Time.deltaTime;

            // Advertencia antes de que termine la transformación
            if (transformationTimer <= warningTime && transformationTimer > 0f)
            {
                OnTransformationWarning?.Invoke();
            }

            // Terminar transformación cuando se acabe el tiempo
            if (transformationTimer <= 0f)
            {
                ChangeState(PlayerState.Joshua);
            }
        }

        /// <summary>
        /// Extiende el tiempo de transformación (para power-ups adicionales)
        /// </summary>
        /// <param name="extraTime">Tiempo adicional en segundos</param>
        public void ExtendTransformation(float extraTime)
        {
            if (isTransformed)
            {
                transformationTimer += extraTime;
                transformationTimer = Mathf.Min(transformationTimer, transformationDuration * 2f); // Límite máximo
            }
        }

        #endregion

        #region Visual and Stat Changes

        /// <summary>
        /// Aplica los cambios visuales del estado actual
        /// </summary>
        private void ApplyVisualChanges()
        {
            if (currentStateData != null && spriteRenderer != null)
            {
                // Cambiar sprite base
                spriteRenderer.sprite = currentStateData.idleSprite;

                // Cambiar color/tinte
                spriteRenderer.color = currentStateData.playerTint;

                // Activar efectos visuales específicos del estado
                ActivateStateEffects();
            }
        }

        /// <summary>
        /// Aplica los cambios de estadísticas del estado actual
        /// </summary>
        private void ApplyStatChanges()
        {
            if (currentStateData != null && playerController != null)
            {
                // Actualizar estadísticas del controlador
                playerController.UpdateStats(currentStateData);
            }
        }

        /// <summary>
        /// Activa efectos visuales específicos del estado
        /// </summary>
        private void ActivateStateEffects()
        {
            // TODO: Implementar efectos de partículas y otros efectos visuales
        }

        #endregion

        #region Setup and Configuration

        /// <summary>
        /// Configura estados predeterminados si no están asignados
        /// </summary>
        private void SetupDefaultStates()
        {
            // Configurar estado Joshua por defecto
            if (joshuaState == null)
            {
                joshuaState = new PlayerStateData();
                joshuaState.state = PlayerState.Joshua;
                joshuaState.stateName = "Joshua";
                joshuaState.moveSpeed = 5f;
                joshuaState.jumpForce = 12f;
                joshuaState.maxSpeed = 8f;
                joshuaState.canSpinDash = false;
                joshuaState.isInvincible = false;
                joshuaState.playerTint = Color.white;
            }

            // Configurar estado Sonic por defecto
            if (sonicState == null)
            {
                sonicState = new PlayerStateData();
                sonicState.state = PlayerState.Sonic;
                sonicState.stateName = "Sonic";
                sonicState.moveSpeed = 12f;
                sonicState.jumpForce = 15f;
                sonicState.maxSpeed = 20f;
                sonicState.canSpinDash = true;
                sonicState.isInvincible = true;
                sonicState.spinDashForce = 25f;
                sonicState.playerTint = new Color(0.7f, 0.9f, 1f); // Tinte azulado
            }
        }

        #endregion

        #region Public Getters

        public PlayerState CurrentState => currentState;
        public PlayerStateData CurrentStateData => currentStateData;
        public bool IsTransformed => isTransformed;
        public float TransformationTimeLeft => transformationTimer;
        public float TransformationProgress => isTransformed ? (transformationDuration - transformationTimer) / transformationDuration : 0f;

        #endregion
    }
}