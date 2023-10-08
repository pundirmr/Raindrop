using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Visible water (the obstacle for droplets) controller.
    /// </summary>
    public class Water : MonoBehaviour
    {
        public Pip[] pips;

        public GameObject answerBox;

        private Animator animator;

        private Vector3 _waterStartPosition;

        /// <summary>
        /// Level of the water changed when droplets hit it.
        /// </summary>
        public int Level
        {
            get { return level; }
            private set
            {
                level = value;
                if (level < 1) level = 1;
            }
        }

        //private GameState gameState;
        private int level;

        private void OnEnable()
        {
            _waterStartPosition = transform.localPosition;
        }

        public void OnGameReset()
        {
            transform.localPosition = _waterStartPosition;
            Level = 1;
            if (animator != null)
            {
                animator.SetBool("startGame", false);
                animator.SetInteger("waterLevel", 0);
                animator.Play("BeforeGame");
            }
            for (int i = 0; i < pips.Length; i++)
            {
                pips[i].UnGlow();
            }
        }

        public void Initialize()
        {
            //this.gameState = gameState;
            Level = 1;
            //gameState.WaterPixelPosition = Utility.WorldToPixels(transform.position).y;
            animator = GetComponent<Animator>();
            StartWater();
        }
        public void StartWater()
        {
            animator.SetBool("startGame", true);
        }
/*        private void Update()
        {
            gameState.WaterPixelPosition = Utility.WorldToPixels(transform.position).y;
        }*/

        /// <summary>
        /// Moves the water up by waterMovePerLevel and animates it.
        /// </summary>
        public void MoveWaterUp()
        {
            Level++;
            AnimateToCurrentLevel();
        }

        /// <summary>
        /// Moves the water down by waterMovePerLevel and animates it.
        /// </summary>
        public void MoveWaterDown()
        {
            Level--;
            AnimateToCurrentLevel();
        }

        /// <summary>
        /// Moves the water to given level position and animates it.
        /// </summary>
        public void SetWaterLevel(int level)
        {
            Level = level;
            AnimateToCurrentLevel();
        }

        /// <summary>
        /// Glows the first pip. Called once they all slide in.
        /// </summary>
        public void GlowFirstPip()
        {
            pips[0].Glow();
        }

        private void AnimateToCurrentLevel()
        {
            animator.SetInteger("waterLevel", Level);
            if (level - 1 < pips.Length)
            {
                for (int i = 0; i < level; i++)
                {
                    pips[i].Glow();
                }
            }
        }

        public void WaterInitialized()
        {
            answerBox.transform.SetParent(null);
        }

        public void Destroy()
        {
            Destroy(answerBox);
            Destroy(gameObject);
        }
    }
}