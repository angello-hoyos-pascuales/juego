using UnityEngine;

namespace SuperJoshua.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAvatar : MonoBehaviour
    {
        [Header("Sprites del Avatar")]
        [SerializeField] private Sprite joshuaIdle;
        [SerializeField] private Sprite joshuaWalk;
        [SerializeField] private Sprite joshuaJump;
        [SerializeField] private Sprite sonicIdle;
        [SerializeField] private Sprite sonicWalk;
        [SerializeField] private Sprite sonicJump;

        [Header("Animaciones del Avatar")]
        [SerializeField] private Animator animator;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetIdle();
        }

        public void SetIdle()
        {
            spriteRenderer.sprite = joshuaIdle;
        }

        public void SetWalk()
        {
            spriteRenderer.sprite = joshuaWalk;
        }

        public void SetJump()
        {
            spriteRenderer.sprite = joshuaJump;
        }

        public void SetSonicIdle()
        {
            spriteRenderer.sprite = sonicIdle;
        }

        public void SetSonicWalk()
        {
            spriteRenderer.sprite = sonicWalk;
        }

        public void SetSonicJump()
        {
            spriteRenderer.sprite = sonicJump;
        }

        public void PlayTransitionToSonic()
        {
            if (animator != null)
            {
                animator.SetTrigger("TransformToSonic");
            }
        }

        public void PlayTransitionToJoshua()
        {
            if (animator != null)
            {
                animator.SetTrigger("TransformToJoshua");
            }
        }
    }
}
