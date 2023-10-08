using UnityEngine;
namespace LumosLabs.Raindrops
{
    [RequireComponent(typeof(Unity.VectorGraphics.SVGImage))]
    public class SVGButton : MonoBehaviour
    {
        public Sprite normal;
        public Sprite highlighted;
        public Sprite pressed;

        private Unity.VectorGraphics.SVGImage svgImage;

        void Start()
        {
            svgImage = GetComponent<Unity.VectorGraphics.SVGImage>();
        }

        /// <summary>
        /// Called by UI.
        /// </summary>
        public void OnMouseEnter()
        {
            svgImage.sprite = highlighted;
        }

        /// <summary>
        /// Called by UI.
        /// </summary>
        public void OnMouseExit()
        {
            svgImage.sprite = normal;
        }

        /// <summary>
        /// Called by UI.
        /// </summary>
        public void OnMouseUp()
        {
            svgImage.sprite = highlighted;
        }

        /// <summary>
        /// Called by UI.
        /// </summary>
        public void OnMouseDown()
        {
            svgImage.sprite = pressed;
        }
    }
}
