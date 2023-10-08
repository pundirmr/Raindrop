using System.Collections.Generic;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Holds shared game data used among different managers (controllers).
    /// </summary>
    public class RaindropsGameState
    {
        /// <summary>
        /// Active drops that are on screen.
        /// </summary>
        public List<DropBase> ActiveDrops { get; set; }

        /// <summary>
        /// How many times the water has been hit.
        /// </summary>
        public int TotalHits { get; set; }
        
        /// <summary>
        /// How many correct answers has user got.
        /// </summary>
        public int TotalCorrect { get; set; }
        
        /// <summary>
        /// How many points has user got.
        /// </summary>
        public int Points { get; set; }
        
        /// <summary>
        /// Y coordinate in pixels (0 = top edge). 
        /// </summary>
        public int WaterPixelPosition { get; set; }

        /// <summary>
        /// Current Difficulty Settings.
        /// </summary>
        public DifficultySetting CurrentDifficulty { get; set; }

        /// <summary>
        /// Current Difficulty Settings Level.
        /// </summary>
        public int CurrentDifficultyLevel { get; set; }

        /// <summary>
        /// Difficulty scale factor.
        /// </summary>
        public float DifficultyRegulateRate { get; set; }

        public RaindropsGameState()
        {
            ActiveDrops = new List<DropBase>();
            TotalCorrect = 0;
            TotalHits = 0;
            Points = 0;
            DifficultyRegulateRate = 1f;
        }
    }
}
