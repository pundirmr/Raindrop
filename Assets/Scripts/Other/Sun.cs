using System;
using UnityEngine;
using LumosLabs.Raindrops;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Falling down sun controller.
    /// </summary>
    public class Sun : DropBase
    {
        public SpriteAnimator spriteAnimator;

        public event DropletEventHandler onAppearedOnScreen;
        private bool appearedOnScreen;
        private int sunLoopId;

        public new void Initialize(Equation equation, Vector3 position, float speed)
        {
            base.Initialize(equation, position, speed);
            dropType = DropletType.SUN;
            spriteAnimator.Play("Idle");
        }

        void spriteAnimator_onAnimationFinished(object sender, SpriteAnimator.AnimationFinishedEventArgs e)
        {
            if (e.AnimationName == "Explode")
            {
                OnDeath();
            }
        }

        /// <summary>
        /// Animates the explode effect (correct guess and water hit) and calls onDeath event once finished.
        /// </summary>
        /// <param name="playSound">Should the burst sound be played</param>
        public void Explode(bool playSound = false)
        {
            //AudioManager.Instance.StopChannel(sunLoopId);
            if (playSound)
                //AudioManager.Instance.PlaySingle(sunBurst, 1f);

            gameObject.layer = UnityConstants.Layers.DestroyedDrops;

            signAnimator.gameObject.SetActive(false);
            operandsText.gameObject.SetActive(false);

            spriteAnimator.Play("Explode");
        }

        protected override void Update()
        {
            base.Update();
            if (!appearedOnScreen && transform.position.y < 2.7f && gameObject.layer != UnityConstants.Layers.DestroyedDrops) // 2.7f because the pivot is on center
            {
                appearedOnScreen = true;
                //sunLoopId = AudioManager.Instance.PlayLooped(sunLoop, 1f);

                if (onAppearedOnScreen != null)
                    onAppearedOnScreen(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// IPoolListener callback.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            spriteAnimator.onAnimationFinished -= spriteAnimator_onAnimationFinished;
        }

        /// <summary>
        /// IPoolListener callback.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            appearedOnScreen = false;
            spriteAnimator.onAnimationFinished += spriteAnimator_onAnimationFinished;
        }
    }
}
