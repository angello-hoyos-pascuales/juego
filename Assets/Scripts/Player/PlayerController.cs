using UnityEngine;

namespace SuperJoshua.Player
{
    /// <summary>
    /// Controlador principal del jugador que maneja el movimiento, salto y mecánicas estilo Mario
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerStateMachine))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Configuración de Movimiento Base")]
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float friction = 8f;
        [SerializeField] private float airControl = 0.5f;

        [Header("Configuración de Salto")]
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        [SerializeField] private LayerMask groundLayer = 1;

        [Header("Configuración de Detección de Suelo")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;

        [Header("Spin Dash (Solo Sonic)")]
        [SerializeField] private float spinDashChargeTime = 1f;
        [SerializeField] private float spinDashMinForce = 15f;

        // Componentes
        private Rigidbody2D rb;
        private PlayerStateMachine stateMachine;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        // Estado del movimiento
        private float horizontalInput;
        private bool jumpInput;
        private bool jumpInputDown;
        private bool isGrounded;
        private bool wasGrounded;

        // Temporizadores
        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private float spinDashChargeCounter;

        // Estadísticas actuales (actualizadas por la máquina de estados)
        private float currentMoveSpeed = 5f;
        private float currentJumpForce = 12f;
        private float currentMaxSpeed = 10f;
        private bool canSpinDash = false;
        private float currentSpinDashForce = 20f;

        // Estados del Spin Dash
        private bool isChargingSpinDash = false;
        private bool isSpinDashing = false;

        #region Unity Lifecycle

        private void Awake()
        {
            // Obtener componentes
            rb = GetComponent<Rigidbody2D>();
            stateMachine = GetComponent<PlayerStateMachine>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // Configurar Rigidbody2D
            rb.freezeRotation = true;
            rb.gravityScale = 3f;

            // Crear groundCheck si no existe
            if (groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
                groundCheck = groundCheckObj.transform;
            }
        }

        private void Update()
        {
            // Obtener input del jugador
            GetInput();

            // Actualizar detección de suelo
            UpdateGroundDetection();

            // Actualizar temporizadores
            UpdateTimers();

            // Manejar Spin Dash si está disponible
            HandleSpinDash();
        }

        private void FixedUpdate()
        {
            // Aplicar movimiento horizontal
            HandleMovement();

            // Manejar salto
            HandleJump();

            // Aplicar fricción
            ApplyFriction();
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Obtiene el input del jugador
        /// </summary>
        private void GetInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            jumpInput = Input.GetButton("Jump");
            jumpInputDown = Input.GetButtonDown("Jump");

            // Buffer de salto
            if (jumpInputDown)
            {
                jumpBufferCounter = jumpBufferTime;
            }
        }

        #endregion

        #region Movement System

        /// <summary>
        /// Maneja el movimiento horizontal del jugador
        /// </summary>
        private void HandleMovement()
        {
            // No mover si está cargando Spin Dash
            if (isChargingSpinDash) return;

            float targetVelocity = horizontalInput * currentMoveSpeed;
            float velocityChange;

            if (isGrounded)
            {
                // Movimiento en el suelo con suavizado
                velocityChange = (targetVelocity - rb.velocity.x) * acceleration * Time.fixedDeltaTime;
                rb.velocity = new Vector2(
                    Mathf.SmoothDamp(rb.velocity.x, targetVelocity, ref velocityChange, 0.08f),
                    rb.velocity.y
                );
            }
            else
            {
                // Movimiento en el aire con control limitado y suavizado
                velocityChange = (targetVelocity - rb.velocity.x) * acceleration * airControl * Time.fixedDeltaTime;
                rb.velocity = new Vector2(
                    Mathf.SmoothDamp(rb.velocity.x, targetVelocity, ref velocityChange, 0.12f),
                    rb.velocity.y
                );
            }

            // Voltear sprite según la dirección
            if (horizontalInput != 0)
            {
                spriteRenderer.flipX = horizontalInput < 0;
            }
        }

        /// <summary>
        /// Maneja el sistema de salto con coyote time y jump buffer
        /// </summary>
        private void HandleJump()
        {
            // Condiciones para saltar
            bool canJump = (isGrounded || coyoteTimeCounter > 0f) && jumpBufferCounter > 0f;

            if (canJump)
            {
                // Realizar salto
                rb.velocity = new Vector2(rb.velocity.x, currentJumpForce);

                // Feedback visual y sonoro al saltar
                if (animator != null) animator.SetTrigger("Jump");
                if (AudioManager.Instance != null) AudioManager.Instance.PlayJumpSound();

                // Resetear contadores
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;

                Debug.Log("¡Salto ejecutado!");
            }

            // Salto variable (soltar el botón para salto más bajo)
            if (!jumpInput && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        /// <summary>
        /// Aplica fricción cuando el jugador no se está moviendo
        /// </summary>
        private void ApplyFriction()
        {
            if (isGrounded && Mathf.Abs(horizontalInput) < 0.1f && !isSpinDashing)
            {
                float frictionForce = friction * Time.fixedDeltaTime;
                rb.velocity = new Vector2(
                    Mathf.MoveTowards(rb.velocity.x, 0, frictionForce),
                    rb.velocity.y
                );
            }
        }

        #endregion

        #region Spin Dash System

        /// <summary>
        /// Maneja la mecánica de Spin Dash (solo disponible en estado Sonic)
        /// </summary>
        private void HandleSpinDash()
        {
            if (!canSpinDash) return;

            // Iniciar carga de Spin Dash
            if (Input.GetButtonDown("Fire1") && isGrounded && Mathf.Abs(horizontalInput) < 0.1f)
            {
                StartSpinDashCharge();
            }

            // Mantener carga
            if (isChargingSpinDash && Input.GetButton("Fire1"))
            {
                ChargeSpinDash();
            }

            // Ejecutar Spin Dash
            if (isChargingSpinDash && Input.GetButtonUp("Fire1"))
            {
                ExecuteSpinDash();
            }

            // Cancelar Spin Dash si se mueve
            if (isChargingSpinDash && Mathf.Abs(horizontalInput) > 0.1f)
            {
                CancelSpinDash();
            }
        }

        /// <summary>
        /// Inicia la carga del Spin Dash
        /// </summary>
        private void StartSpinDashCharge()
        {
            isChargingSpinDash = true;
            spinDashChargeCounter = 0f;
            rb.velocity = Vector2.zero; // Parar al jugador

            Debug.Log("Iniciando carga de Spin Dash...");
        }

        /// <summary>
        /// Incrementa la carga del Spin Dash
        /// </summary>
        private void ChargeSpinDash()
        {
            spinDashChargeCounter += Time.deltaTime;

            // Efecto visual de carga (TODO: añadir partículas)
            if (spinDashChargeCounter >= spinDashChargeTime)
            {
                // Máxima carga alcanzada
                Debug.Log("Spin Dash completamente cargado!");
            }
        }

        /// <summary>
        /// Ejecuta el Spin Dash
        /// </summary>
        private void ExecuteSpinDash()
        {
            isChargingSpinDash = false;
            isSpinDashing = true;

            // Calcular fuerza basada en el tiempo de carga
            float chargeRatio = Mathf.Clamp01(spinDashChargeCounter / spinDashChargeTime);
            float dashForce = Mathf.Lerp(spinDashMinForce, currentSpinDashForce, chargeRatio);

            // Aplicar fuerza en la dirección que mira el jugador
            float direction = spriteRenderer.flipX ? -1f : 1f;
            rb.velocity = new Vector2(dashForce * direction, rb.velocity.y);

            Debug.Log($"¡Spin Dash ejecutado con fuerza {dashForce}!");

            // Terminar Spin Dash después de un tiempo
            Invoke(nameof(EndSpinDash), 0.5f);
        }

        /// <summary>
        /// Cancela el Spin Dash
        /// </summary>
        private void CancelSpinDash()
        {
            isChargingSpinDash = false;
            spinDashChargeCounter = 0f;

            Debug.Log("Spin Dash cancelado.");
        }

        /// <summary>
        /// Termina el estado de Spin Dash
        /// </summary>
        private void EndSpinDash()
        {
            isSpinDashing = false;
        }

        #endregion

        #region Ground Detection

        /// <summary>
        /// Actualiza la detección de suelo
        /// </summary>
        private void UpdateGroundDetection()
        {
            wasGrounded = isGrounded;
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // Aterrizaje
            if (isGrounded && !wasGrounded)
            {
                if (animator != null) animator.SetTrigger("Land");
                if (AudioManager.Instance != null) AudioManager.Instance.PlayLandSound();
                Debug.Log("Jugador aterrizó");
            }
        }

        /// <summary>
        /// Actualiza los temporizadores de movimiento
        /// </summary>
        private void UpdateTimers()
        {
            // Coyote Time
            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            // Jump Buffer
            if (jumpBufferCounter > 0)
            {
                jumpBufferCounter -= Time.deltaTime;
            }
        }

        #endregion

        #region State Machine Integration

        /// <summary>
        /// Actualiza las estadísticas del jugador desde la máquina de estados
        /// </summary>
        /// <param name="stateData">Datos del estado actual</param>
        public void UpdateStats(PlayerStateData stateData)
        {
            currentMoveSpeed = stateData.moveSpeed;
            currentJumpForce = stateData.jumpForce;
            currentMaxSpeed = stateData.maxSpeed;
            canSpinDash = stateData.canSpinDash;
            currentSpinDashForce = stateData.spinDashForce;

            // Feedback visual y sonoro al cambiar de estado
            if (AudioManager.Instance != null) AudioManager.Instance.PlayStateChangeSound();
            if (animator != null) animator.SetTrigger("StateChange");
            if (pickupEffectPrefab != null) Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

            Debug.Log($"Estadísticas actualizadas para estado: {stateData.stateName}");
        }

        #endregion

        #region Enemy Interaction

        /// <summary>
        /// Maneja la colisión con enemigos (aplastamiento estilo Mario)
        /// </summary>
        /// <param name="other">Collider del objeto con el que colisionó</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                // Determinar si el jugador está cayendo sobre el enemigo
                if (rb.velocity.y < 0 && transform.position.y > other.transform.position.y)
                {
                    // Aplastar enemigo
                    DestroyEnemy(other.gameObject);

                    // Rebote mejorado hacia arriba
                    rb.velocity = new Vector2(rb.velocity.x, currentJumpForce * 0.7f);

                    // Efecto visual y sonoro de rebote
                    if (AudioManager.Instance != null) AudioManager.Instance.PlayBounceSound();
                    if (pickupEffectPrefab != null) Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

                    Debug.Log("¡Enemigo aplastado con rebote mejorado!");
                }
            }
        }

        /// <summary>
        /// Destruye un enemigo
        /// </summary>
        /// <param name="enemy">GameObject del enemigo a destruir</param>
        private void DestroyEnemy(GameObject enemy)
        {
            // Efectos visuales y sonoros al derrotar enemigo
            if (AudioManager.Instance != null) AudioManager.Instance.PlayEnemyDefeatSound();
            if (pickupEffectPrefab != null) Instantiate(pickupEffectPrefab, enemy.transform.position, Quaternion.identity);

            // Efecto de partículas adicional al derrotar enemigo
            if (pickupEffectPrefab != null) Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

            // Añadir puntuación y bonificación
            GameManager.GameManager.Instance?.OnEnemyDefeated();

            // TODO: Añadir efectos de partículas y sonido adicionales
            Destroy(enemy);
        }

        #endregion

        #region Debug

        /// <summary>
        /// Dibuja gizmos para debug en el editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }

        #endregion

        #region Public Getters

        public bool IsGrounded => isGrounded;
        public bool IsMoving => Mathf.Abs(rb.velocity.x) > 0.1f;
        public bool IsJumping => !isGrounded && rb.velocity.y > 0;
        public bool IsFalling => !isGrounded && rb.velocity.y < 0;
        public bool IsChargingSpinDash => isChargingSpinDash;
        public bool IsSpinDashing => isSpinDashing;
        public Vector2 Velocity => rb.velocity;

        #endregion
    }
}