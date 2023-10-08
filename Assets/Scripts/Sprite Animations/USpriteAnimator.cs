using UnityEngine;

namespace LumosLabs.Raindrops
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class USpriteAnimator : SpriteAnimator
    {
        /// <summary>
        /// Animations data.
        /// </summary>
        public USpriteAnimation[] animations;

        private SpriteRenderer spriteRenderer;

        protected override ISpriteAnimation[] Animations { get { return animations; } }

        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void UpdateMesh()
        {
            var frames = animations[animationId].Frames;
            if (spriteRenderer.sprite != frames[frameIndex])
                spriteRenderer.sprite = frames[frameIndex];
        }
    }
}
