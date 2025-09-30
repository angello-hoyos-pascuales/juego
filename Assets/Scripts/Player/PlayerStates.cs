using UnityEngine;

namespace SuperJoshua.Player
{
    /// <summary>
    /// Enumeración que define los diferentes estados del jugador
    /// </summary>
    public enum PlayerState
    {
        Joshua,    // Estado normal del jugador
        Sonic      // Estado transformado con poderes de Sonic
    }

    /// <summary>
    /// Clase que maneja los diferentes estados del jugador y sus propiedades
    /// </summary>
    [System.Serializable]
    public class PlayerStateData
    {
        [Header("Configuración del Estado")]
        public PlayerState state;
        public string stateName;

        [Header("Sprites")]
        public Sprite idleSprite;
        public Sprite[] walkSprites;
        public Sprite jumpSprite;
        public Sprite fallSprite;

        [Header("Estadísticas de Movimiento")]
        public float moveSpeed = 5f;
        public float jumpForce = 12f;
        public float maxSpeed = 10f;

        [Header("Habilidades Especiales")]
        public bool canSpinDash = false;
        public bool isInvincible = false;
        public float spinDashForce = 20f;

        [Header("Efectos Visuales")]
        public Color playerTint = Color.white;
        public GameObject[] effectPrefabs;
    }
}