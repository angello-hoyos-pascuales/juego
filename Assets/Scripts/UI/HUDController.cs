using UnityEngine;
using UnityEngine.UI;
using SuperJoshua.GameManager;

namespace SuperJoshua.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private Text coinsText;
        [SerializeField] private Text livesText;

        private void Start()
        {
            UpdateHUD();
            GameManager.Instance.OnCoinsChanged += UpdateCoins;
            GameManager.Instance.OnLivesChanged += UpdateLives;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnCoinsChanged -= UpdateCoins;
            GameManager.Instance.OnLivesChanged -= UpdateLives;
        }

        private void UpdateHUD()
        {
            UpdateCoins(GameManager.Instance.CoinCount);
            UpdateLives(GameManager.Instance.CurrentLives);
        }

        private void UpdateCoins(int coins)
        {
            if (coinsText != null)
                coinsText.text = $"Monedas/Anillos: {coins}";
        }

        private void UpdateLives(int lives)
        {
            if (livesText != null)
                livesText.text = $"Vidas: {lives}";
        }
    }
}
