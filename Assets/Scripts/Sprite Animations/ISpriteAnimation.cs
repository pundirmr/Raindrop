namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Animation interface that has no reference to graphic assets.
    /// </summary>
    public interface ISpriteAnimation
    {
        /// <summary>
        /// Animation name.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Animation speed (time in seconds needed for frame change to occur). 
        /// </summary>
        float Speed { get; }

        /// <summary>
        /// True if animation should be played again after finishing.
        /// </summary>
        bool Loop { get; }

        /// <summary>
        /// Length in frames of the animation.
        /// </summary>
        int Length { get; }
    }
}