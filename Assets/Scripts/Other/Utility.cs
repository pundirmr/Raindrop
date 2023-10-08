using UnityEngine;

namespace LumosLabs.Raindrops
{
    public class Utility
    {
        /// <summary>
        /// Convert World space coordinate to Pixel (screen 0,0 being top left corner) space coordinate.
        /// </summary>
        public static Vector2I WorldToPixels(Vector3 pos)
        {
            var _pos = Camera.main.WorldToScreenPoint(pos);
            return new Vector2I((int) (_pos.x), (int) (Screen.height - _pos.y));
        }
    }

}