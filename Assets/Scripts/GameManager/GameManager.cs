using UnityEngine;
using UnityEngine.SceneManagement;

namespace SuperJoshua.GameManager
{
    /// <summary>
    /// Gestor principal del juego que maneja puntuación, vidas, niveles y estado general
    /// Implementa el patrón Singleton para acceso global
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton Implementation

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject gameManagerObj = new GameObject("GameManager");
                        _instance = gameManagerObj.AddComponent<GameManager>();
                        DontDestroyOnLoad(gameManagerObj);
                    }
                }
                return _instance;
            }
        }

        #endregion

        [Header("Configuración del Juego")]
        [SerializeField] private int startingLives = 3;
        [SerializeField] private int extraLifeScoreThreshold = 10000;
        [SerializeField] private int coinsForExtraLife = 100;

        [Header("Configuración de Puntuación")]
        [SerializeField] private int enemyKillBonus = 100;
        [SerializeField] private int levelCompleteBonus = 1000;
        [SerializeField] private float scoreMultiplierDuration = 10f;

        // Estado del juego
        private int currentScore = 0;
        private int currentLives;
        private int currentLevel = 1;
        private int coinCount = 0;
        private bool isGamePaused = false;
        private bool isGameOver = false;

        // Multiplicadores y bonificaciones
        private float currentScoreMultiplier = 1f;
        private float scoreMultiplierTimer = 0f;
        private int nextExtraLifeScore;

        // Eventos para notificar cambios en el estado del juego
        public System.Action<int> OnScoreChanged;
        public System.Action<int> OnLivesChanged;
        public System.Action<int> OnCoinsChanged;
        public System.Action<int> OnLevelChanged;
        public System.Action OnGameOver;
        public System.Action OnGamePaused;
        public System.Action OnGameResumed;

        #region Unity Lifecycle

        private void Awake()
        {
            // Implementar singleton
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar estado del juego
            InitializeGameState();
        }

        private void Update()
        {
            // Actualizar multiplicador de puntuación
            UpdateScoreMultiplier();

            // Manejar input de pausa
            HandlePauseInput();
        }

        #endregion

        #region Game State Management

        /// <summary>
        /// Inicializa el estado del juego
        /// </summary>
        private void InitializeGameState()
        {
            currentLives = startingLives;
            nextExtraLifeScore = extraLifeScoreThreshold;
            isGameOver = false;
            isGamePaused = false;

            Debug.Log("Juego inicializado - Vidas: " + currentLives);
        }

        /// <summary>
        /// Reinicia el juego desde el principio
        /// </summary>
        public void RestartGame()
        {
            currentScore = 0;
            currentLives = startingLives;
            currentLevel = 1;
            coinCount = 0;
            currentScoreMultiplier = 1f;
            scoreMultiplierTimer = 0f;
            nextExtraLifeScore = extraLifeScoreThreshold;
            isGameOver = false;
            isGamePaused = false;

            // Notificar cambios
            OnScoreChanged?.Invoke(currentScore);
            OnLivesChanged?.Invoke(currentLives);
            OnCoinsChanged?.Invoke(coinCount);
            OnLevelChanged?.Invoke(currentLevel);

            // Recargar la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            Debug.Log("Juego reiniciado");
        }

        /// <summary>
        /// Termina el juego
        /// </summary>
        public void GameOver()
        {
            if (isGameOver) return;

            isGameOver = true;
            Time.timeScale = 0f; // Pausar el juego

            OnGameOver?.Invoke();

            Debug.Log("¡Game Over! Puntuación final: " + currentScore);
        }

        #endregion

        #region Score System

        /// <summary>
        /// Añade puntos al marcador
        /// </summary>
        /// <param name="points">Cantidad de puntos a añadir</param>
        public void AddScore(int points)
        {
            int adjustedPoints = Mathf.RoundToInt(points * currentScoreMultiplier);
            currentScore += adjustedPoints;

            // Verificar si se ha ganado una vida extra por puntuación
            CheckForExtraLifeByScore();

            OnScoreChanged?.Invoke(currentScore);

            Debug.Log($"Puntos añadidos: {adjustedPoints} (multiplicador: {currentScoreMultiplier:F1}x)");
        }

        /// <summary>
        /// Establece un multiplicador de puntuación temporal
        /// </summary>
        /// <param name="multiplier">Multiplicador a aplicar</param>
        /// <param name="duration">Duración del multiplicador en segundos</param>
        public void SetScoreMultiplier(float multiplier, float duration = -1f)
        {
            currentScoreMultiplier = multiplier;
            scoreMultiplierTimer = duration > 0 ? duration : scoreMultiplierDuration;

            Debug.Log($"Multiplicador de puntuación activado: {multiplier}x por {scoreMultiplierTimer} segundos");
        }

        /// <summary>
        /// Actualiza el multiplicador de puntuación
        /// </summary>
        private void UpdateScoreMultiplier()
        {
            if (currentScoreMultiplier > 1f && scoreMultiplierTimer > 0f)
            {
                scoreMultiplierTimer -= Time.deltaTime;

                if (scoreMultiplierTimer <= 0f)
                {
                    currentScoreMultiplier = 1f;
                    Debug.Log("Multiplicador de puntuación terminado");
                }
            }
        }

        /// <summary>
        /// Verifica si el jugador ha ganado una vida extra por puntuación
        /// </summary>
        private void CheckForExtraLifeByScore()
        {
            if (currentScore >= nextExtraLifeScore)
            {
                AddLife();
                nextExtraLifeScore += extraLifeScoreThreshold;
                Debug.Log("¡Vida extra ganada por puntuación!");
            }
        }

        #endregion

        #region Lives System

        /// <summary>
        /// Añade una vida al jugador
        /// </summary>
        public void AddLife()
        {
            currentLives++;
            OnLivesChanged?.Invoke(currentLives);

            Debug.Log("Vida añadida. Vidas actuales: " + currentLives);
        }

        /// <summary>
        /// Quita una vida al jugador
        /// </summary>
        public void LoseLife()
        {
            if (isGameOver) return;

            currentLives--;
            OnLivesChanged?.Invoke(currentLives);

            if (currentLives <= 0)
            {
                GameOver();
            }
            else
            {
                Debug.Log("Vida perdida. Vidas restantes: " + currentLives);
                // TODO: Implementar respawn del jugador
            }
        }

        #endregion

        #region Coin System

        /// <summary>
        /// Añade monedas al contador
        /// </summary>
        /// <param name="amount">Cantidad de monedas a añadir</param>
        public void AddCoins(int amount = 1)
        {
            coinCount += amount;
            OnCoinsChanged?.Invoke(coinCount);

            // Verificar si se ha ganado una vida extra por monedas
            if (coinCount >= coinsForExtraLife)
            {
                AddLife();
                coinCount -= coinsForExtraLife; // Restar las monedas utilizadas
                Debug.Log("¡Vida extra ganada por monedas!");
            }
        }

        #endregion

        #region Level Management

        /// <summary>
        /// Avanza al siguiente nivel
        /// </summary>
        public void NextLevel()
        {
            currentLevel++;

            // Bonificación por completar nivel
            AddScore(levelCompleteBonus);

            OnLevelChanged?.Invoke(currentLevel);

            Debug.Log("Nivel completado. Avanzando al nivel " + currentLevel);

            // TODO: Cargar la siguiente escena del nivel
        }

        /// <summary>
        /// Carga un nivel específico
        /// </summary>
        /// <param name="levelNumber">Número del nivel a cargar</param>
        public void LoadLevel(int levelNumber)
        {
            currentLevel = levelNumber;
            OnLevelChanged?.Invoke(currentLevel);

            // TODO: Implementar carga de escenas por número de nivel
            string sceneName = "Level" + levelNumber.ToString("00");
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("No se puede cargar el nivel: " + sceneName);
            }
        }

        #endregion

        #region Pause System

        /// <summary>
        /// Maneja el input de pausa
        /// </summary>
        private void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Alterna el estado de pausa del juego
        /// </summary>
        public void TogglePause()
        {
            if (isGameOver) return;

            isGamePaused = !isGamePaused;

            if (isGamePaused)
            {
                Time.timeScale = 0f;
                OnGamePaused?.Invoke();
                Debug.Log("Juego pausado");
            }
            else
            {
                Time.timeScale = 1f;
                OnGameResumed?.Invoke();
                Debug.Log("Juego reanudado");
            }
        }

        /// <summary>
        /// Pausa el juego
        /// </summary>
        public void PauseGame()
        {
            if (!isGamePaused)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Reanuda el juego
        /// </summary>
        public void ResumeGame()
        {
            if (isGamePaused)
            {
                TogglePause();
            }
        }

        #endregion

        #region Enemy and Collectible Handling

        /// <summary>
        /// Maneja cuando el jugador derrota a un enemigo
        /// </summary>
        /// <param name="enemyType">Tipo de enemigo derrotado</param>
        public void OnEnemyDefeated(string enemyType = "Normal")
        {
            int bonus = enemyKillBonus;

            // Diferentes bonificaciones según el tipo de enemigo
            switch (enemyType.ToLower())
            {
                case "boss":
                    bonus *= 10;
                    break;
                case "special":
                    bonus *= 3;
                    break;
                case "normal":
                default:
                    break;
            }

            AddScore(bonus);
            Debug.Log($"Enemigo {enemyType} derrotado. Bonificación: {bonus}");
        }

        #endregion

        #region Public Properties

        public int CurrentScore => currentScore;
        public int CurrentLives => currentLives;
        public int CurrentLevel => currentLevel;
        public int CoinCount => coinCount;
        public bool IsGamePaused => isGamePaused;
        public bool IsGameOver => isGameOver;
        public float CurrentScoreMultiplier => currentScoreMultiplier;
        public float ScoreMultiplierTimeLeft => scoreMultiplierTimer;

        #endregion

        #region Save/Load System (Future Implementation)

        // TODO: Implementar sistema de guardado y carga
        // TODO: Implementar high scores
        // TODO: Implementar configuraciones del juego (volumen, controles, etc.)

        #endregion
    }
}