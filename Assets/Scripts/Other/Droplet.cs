using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Falling down droplets controller.
    /// </summary>
    public class Droplet : DropBase
    {
        public SpriteAnimator dropletAnimator;

        public new void Initialize(Equation equation, Vector3 position, float speed)
        {
            transform.localPosition = Vector3.zero;
            base.Initialize(equation, position, speed);
            dropType = DropletType.DROPLET;
            dropletAnimator.Play("Fall"); // Plays default animation

            //This had to be done because ObjectPool class resets the scale of object on Release(), which is not desired here.
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            
        }

        void spriteAnimator_onAnimationFinished(object sender, SpriteAnimator.AnimationFinishedEventArgs e)
        {
            //Debug.Log("spriteAnimator_onAnimationFinished()");
            if (e.AnimationName == "Splash" || e.AnimationName == "Explode" || e.AnimationName == "Evaporate")
            {
                OnDeath();
            }
        }

        /// <summary>
        /// Animates the splash effect (water hit) and calls onDeath event once finished.
        /// </summary>
        public void Splash()
        {
            dropletAnimator.Play("Splash");
            signAnimator.gameObject.SetActive(false);
            operandsText.gameObject.SetActive(false);

            gameObject.layer = UnityConstants.Layers.DestroyedDrops;
            //AudioManager.Instance.PlaySingle(splashSound, 1f);
        }

        /// <summary>
        /// Animates the explode effect (correct guess) and calls onDeath event once finished.
        /// </summary>
        /// <param name="playSound">Should the burst sound be played</param>
        public void Explode(bool playSound = false)
        {
            dropletAnimator.Play("Explode");
            signAnimator.gameObject.SetActive(false);
            operandsText.gameObject.SetActive(false);

            gameObject.layer = UnityConstants.Layers.DestroyedDrops;

            // Only the first exploded droplet should play sfx
            /*if (playSound)
                AudioManager.Instance.PlaySingle(burstSound, 1f);*/
        }

        /// <summary>
        /// Animates the evaporate effect (when sun clears it) and calls onDeath event once finished.
        /// </summary>
        public void Evaporate()
        {
            dropletAnimator.Play("Evaporate");
            signAnimator.gameObject.SetActive(false);
            operandsText.gameObject.SetActive(false);

            gameObject.layer = UnityConstants.Layers.DestroyedDrops;
        }

        /// <summary>
        /// IPoolListener callback.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            dropletAnimator.onAnimationFinished -= spriteAnimator_onAnimationFinished;
        }

        /// <summary>
        /// IPoolListener callback.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            dropletAnimator.onAnimationFinished += spriteAnimator_onAnimationFinished;
        }
    }
}
