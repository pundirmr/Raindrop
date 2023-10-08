using System;
using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Sprite animation implemented using SVGs as assets.
    /// </summary>
    [Serializable]
    public class SVGSpriteAnimation : ISpriteAnimation
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private float speed;

        [SerializeField]
        private bool loop;

        [SerializeField]
        private Sprite[] frames;

        public Sprite[] Frames
        {
            get { return frames; }
        }
        
        public string Name
        {
            get { return name; }
        }
        
        public float Speed
        {
            get { return speed; }
        }
        
        public bool Loop
        {
            get { return loop; }
        }

        public int Length
        {
            get { return frames.Length; }
        }
    }
}
