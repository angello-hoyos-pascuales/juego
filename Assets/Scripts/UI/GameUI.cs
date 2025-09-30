using UnityEngine;
using UnityEngine.UI;
using SuperJoshua.GameManager;
using SuperJoshua.Player;

namespace SuperJoshua.UI
{
    /// <summary>
    /// Controlador de UI principal que muestra información del juego en pantalla
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text livesText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text transformationText;
        [SerializeField] private Slider transformationBar;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Transformation UI")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color transformedColor = Color.cyan;
        [SerializeField] private Color warningColor = Color.red;

        // Referencias
        private GameManager.GameManager gameManager;
        private PlayerStateMachine playerStateMachine;

        #region Unity Lifecycle

        private void Start()
        {
            // Obtener referencias
            gameManager = GameManager.GameManager.Instance;
            playerStateMachine = FindObjectOfType<PlayerStateMachine>();

            // Suscribirse a eventos
            if (gameManager != null)
            {
                gameManager.OnScoreChanged += UpdateScore;
                gameManager.OnLivesChanged += UpdateLives;
                gameManager.OnCoinsChanged += UpdateCoins;
                gameManager.OnLevelChanged += UpdateLevel;
                gameManager.OnGamePaused += ShowPausePanel;
                gameManager.OnGameResumed += HidePausePanel;
                gameManager.OnGameOver += ShowGameOverPanel;
            }

            // Inicializar UI
            InitializeUI();
        }

        private void Update()
        {
            // Actualizar información de transformación
            UpdateTransformationUI();
        }

        private void OnDestroy()
        {
            // Desuscribirse de eventos
            if (gameManager != null)
            {
                gameManager.OnScoreChanged -= UpdateScore;
                gameManager.OnLivesChanged -= UpdateLives;
                gameManager.OnCoinsChanged -= UpdateCoins;
                gameManager.OnLevelChanged -= UpdateLevel;
                gameManager.OnGamePaused -= ShowPausePanel;
                gameManager.OnGameResumed -= HidePausePanel;
                gameManager.OnGameOver -= ShowGameOverPanel;
            }
        }

        #endregion

        #region UI Initialization

        /// <summary>
        /// Inicializa todos los elementos de UI
        /// </summary>
        private void InitializeUI()
        {
            // Crear Canvas si no existe
            if (GetComponent<Canvas>() == null)
            {
                SetupCanvas();
            }

            // Crear elementos de UI automáticamente si no están asignados
            if (scoreText == null) CreateScoreText();
            if (livesText == null) CreateLivesText();
            if (coinsText == null) CreateCoinsText();
            if (levelText == null) CreateLevelText();
            if (transformationText == null) CreateTransformationText();
            if (transformationBar == null) CreateTransformationBar();

            // Actualizar valores iniciales
            if (gameManager != null)
            {
                UpdateScore(gameManager.CurrentScore);
                UpdateLives(gameManager.CurrentLives);
                UpdateCoins(gameManager.CoinCount);
                UpdateLevel(gameManager.CurrentLevel);
            }

            // Ocultar paneles inicialmente
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        /// <summary>
        /// Configura el Canvas principal
        /// </summary>
        private void SetupCanvas()
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            gameObject.AddComponent<GraphicRaycaster>();
        }

        #endregion

        #region UI Creation Methods

        /// <summary>
        /// Crea el texto de puntuación
        /// </summary>
        private void CreateScoreText()
        {
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(transform);

            scoreText = scoreObj.AddComponent<Text>();
            scoreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            scoreText.fontSize = 36;
            scoreText.color = Color.white;
            scoreText.text = "Score: 0";

            RectTransform rect = scoreText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(20, -20);
            rect.sizeDelta = new Vector2(300, 50);
        }

        /// <summary>
        /// Crea el texto de vidas
        /// </summary>
        private void CreateLivesText()
        {
            GameObject livesObj = new GameObject("LivesText");
            livesObj.transform.SetParent(transform);

            livesText = livesObj.AddComponent<Text>();
            livesText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            livesText.fontSize = 36;
            livesText.color = Color.white;
            livesText.text = "Lives: 3";

            RectTransform rect = livesText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -20);
            rect.sizeDelta = new Vector2(200, 50);
        }

        /// <summary>
        /// Crea el texto de monedas
        /// </summary>
        private void CreateCoinsText()
        {
            GameObject coinsObj = new GameObject("CoinsText");
            coinsObj.transform.SetParent(transform);

            coinsText = coinsObj.AddComponent<Text>();
            coinsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            coinsText.fontSize = 28;
            coinsText.color = Color.yellow;
            coinsText.text = "Coins: 0";

            RectTransform rect = coinsText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(20, -80);
            rect.sizeDelta = new Vector2(200, 40);
        }

        /// <summary>
        /// Crea el texto de nivel
        /// </summary>
        private void CreateLevelText()
        {
            GameObject levelObj = new GameObject("LevelText");
            levelObj.transform.SetParent(transform);

            levelText = levelObj.AddComponent<Text>();
            levelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            levelText.fontSize = 28;
            levelText.color = Color.white;
            levelText.text = "Level: 1";

            RectTransform rect = levelText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-20, -80);
            rect.sizeDelta = new Vector2(150, 40);
        }

        /// <summary>
        /// Crea el texto de transformación
        /// </summary>
        private void CreateTransformationText()
        {
            GameObject transformObj = new GameObject("TransformationText");
            transformObj.transform.SetParent(transform);

            transformationText = transformObj.AddComponent<Text>();
            transformationText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            transformationText.fontSize = 32;
            transformationText.color = normalColor;
            transformationText.text = "Joshua";
            transformationText.alignment = TextAnchor.MiddleCenter;

            RectTransform rect = transformationText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -30);
            rect.sizeDelta = new Vector2(200, 50);
        }

        /// <summary>
        /// Crea la barra de transformación
        /// </summary>
        private void CreateTransformationBar()
        {
            GameObject barObj = new GameObject("TransformationBar");
            barObj.transform.SetParent(transform);

            transformationBar = barObj.AddComponent<Slider>();
            transformationBar.minValue = 0f;
            transformationBar.maxValue = 1f;
            transformationBar.value = 0f;

            RectTransform rect = transformationBar.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1);
            rect.anchorMax = new Vector2(0.5f, 1);
            rect.anchoredPosition = new Vector2(0, -90);
            rect.sizeDelta = new Vector2(300, 20);

            // Configurar colores
            ColorBlock colors = transformationBar.colors;
            colors.normalColor = Color.cyan;
            colors.highlightedColor = Color.cyan;
            colors.pressedColor = Color.cyan;
            colors.selectedColor = Color.cyan;
            transformationBar.colors = colors;

            // Inicialmente oculta
            barObj.SetActive(false);
        }

        #endregion

        #region UI Update Methods

        /// <summary>
        /// Actualiza el texto de puntuación
        /// </summary>
        private void UpdateScore(int newScore)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {newScore:N0}";
            }
        }

        /// <summary>
        /// Actualiza el texto de vidas
        /// </summary>
        private void UpdateLives(int newLives)
        {
            if (livesText != null)
            {
                livesText.text = $"Lives: {newLives}";
            }
        }

        /// <summary>
        /// Actualiza el texto de monedas
        /// </summary>
        private void UpdateCoins(int newCoins)
        {
            if (coinsText != null)
            {
                coinsText.text = $"Coins: {newCoins}";
            }
        }

        /// <summary>
        /// Actualiza el texto de nivel
        /// </summary>
        private void UpdateLevel(int newLevel)
        {
            if (levelText != null)
            {
                levelText.text = $"Level: {newLevel}";
            }
        }

        /// <summary>
        /// Actualiza la UI de transformación
        /// </summary>
        private void UpdateTransformationUI()
        {
            if (playerStateMachine == null || transformationText == null) return;

            bool isTransformed = playerStateMachine.IsTransformed;
            float timeLeft = playerStateMachine.TransformationTimeLeft;

            // Actualizar texto
            if (isTransformed)
            {
                transformationText.text = $"SONIC ({timeLeft:F1}s)";

                // Cambiar color según tiempo restante
                if (timeLeft <= 3f)
                {
                    transformationText.color = Color.Lerp(warningColor, transformedColor,
                        Mathf.PingPong(Time.time * 3f, 1f));
                }
                else
                {
                    transformationText.color = transformedColor;
                }
            }
            else
            {
                transformationText.text = "JOSHUA";
                transformationText.color = normalColor;
            }

            // Actualizar barra
            if (transformationBar != null)
            {
                transformationBar.gameObject.SetActive(isTransformed);
                if (isTransformed)
                {
                    transformationBar.value = timeLeft / 15f; // Duración total de transformación
                }
            }
        }

        #endregion

        #region Panel Management

        /// <summary>
        /// Muestra el panel de pausa
        /// </summary>
        private void ShowPausePanel()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        /// <summary>
        /// Oculta el panel de pausa
        /// </summary>
        private void HidePausePanel()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        /// <summary>
        /// Muestra el panel de game over
        /// </summary>
        private void ShowGameOverPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reinicia el juego (para botón de UI)
        /// </summary>
        public void RestartGame()
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
        }

        /// <summary>
        /// Pausa/reanuda el juego (para botón de UI)
        /// </summary>
        public void TogglePause()
        {
            if (gameManager != null)
            {
                gameManager.TogglePause();
            }
        }

        #endregion
    }
}