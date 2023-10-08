using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Switches on the level indicator.
    /// </summary>
    public class Pip : MonoBehaviour
    {
        private readonly Color glowColor = new Color(253 / 255f, 184 / 255f, 19 / 255f);
        private Unity.VectorGraphics.SVGImage svgImage;

        void Awake()
        {
            svgImage = GetComponent<Unity.VectorGraphics.SVGImage>();
        }

        public void Glow()
        {
            svgImage.color = glowColor;
        }
        public void UnGlow()
        {
            svgImage.color = Color.white;
        }
    }
}