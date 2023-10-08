using UnityEngine;
using Unity.VectorGraphics;

namespace LumosLabs.Raindrops
{
    [RequireComponent(typeof(SVGImage))]
    public class SVGSpriteAnimator : SpriteAnimator
    {
        /// <summary>
        /// Animations data.
        /// </summary>
        public SVGSpriteAnimation[] animations;

        private SVGImage svgRenderer;

        protected override ISpriteAnimation[] Animations { get { return animations; } }

        protected override void Awake()
        {
            base.Awake();
            svgRenderer = GetComponent<SVGImage>();
        }

        protected override void UpdateMesh()
        {
            var frames = animations[animationId].Frames;
            if (svgRenderer.sprite != frames[frameIndex])
                svgRenderer.sprite = frames[frameIndex];
        }
    }
}
