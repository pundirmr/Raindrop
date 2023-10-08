using System.Collections.Generic;
using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Regulates and updates game dificulty.
    /// </summary>
    public class DifficultyManager
    {
        /// <summary>
        /// Treshold (correct answers) to reach the next difficulty level.
        /// </summary>
        public int NextUpTreshold
        {
            get { return nextUpTreshold; }
        }
        
        private RaindropsGameState gameState
        {
            get
            {
                return trialsModel.gameState;
            }
        }
        private readonly List<DifficultySetting> difficultySettings;

        private int currentDifficulty;
        private int nextUpTreshold;

        private float lastWiepoutTime;

        private TrialsModel trialsModel;

        public DifficultyManager(TrialsModel trialsModel)
        {
            this.trialsModel = trialsModel;
            
            this.difficultySettings = new List<DifficultySetting>{
                    new DifficultySetting {
                        Treshold = 0, MaxDroplets = 1, 
                        AddRanges = new Vector2I(10, 5), SubRanges = new Vector2I(10, 5) 
                    },
                    new DifficultySetting {
                        Treshold = 5, MaxDroplets = 2, 
                        AddRanges = new Vector2I(10, 10), SubRanges = new Vector2I(10, 10), MulRanges = new Vector2I(5,5)
                    },
                    new DifficultySetting {
                        Treshold = 10, MaxDroplets = 3, 
                        AddRanges = new Vector2I(20, 10), SubRanges = new Vector2I(10, 10), MulRanges = new Vector2I(5, 5), DivRanges = new Vector2I(3, 5) 
                    },
                    new DifficultySetting {
                        Treshold = 20, MaxDroplets = 4, 
                        AddRanges = new Vector2I(20, 10), SubRanges = new Vector2I(20, 10), MulRanges = new Vector2I(10, 5), DivRanges = new Vector2I(4, 6) 
                    },
                    new DifficultySetting {
                        Treshold = 35, MaxDroplets = 5, 
                        AddRanges = new Vector2I(25, 10), SubRanges = new Vector2I(30, 10), MulRanges = new Vector2I(10, 10), DivRanges = new Vector2I(4, 9) 
                    },
                    new DifficultySetting {
                        Treshold = 50, MaxDroplets = 6, 
                        AddRanges = new Vector2I(15, 15), SubRanges = new Vector2I(30, 10), MulRanges = new Vector2I(10, 10), DivRanges = new Vector2I(5, 9) 
                    },
                    new DifficultySetting {
                        Treshold = 70, MaxDroplets = 7, 
                        AddRanges = new Vector2I(20, 15), SubRanges = new Vector2I(30, 15), MulRanges = new Vector2I(10, 10), DivRanges = new Vector2I(6, 9) 
                    },
                    new DifficultySetting {
                        Treshold = 90, MaxDroplets = 8, 
                        AddRanges = new Vector2I(20, 20), SubRanges = new Vector2I(30, 15), MulRanges = new Vector2I(11, 11), DivRanges = new Vector2I(7, 9) 
                    },
                    new DifficultySetting {
                        Treshold = 115, MaxDroplets = 9, 
                        AddRanges = new Vector2I(20, 20), SubRanges = new Vector2I(30, 15), MulRanges = new Vector2I(12, 12), DivRanges = new Vector2I(9, 9) 
                    },
                    new DifficultySetting {
                        Treshold = 140, MaxDroplets = 10, 
                        AddRanges = new Vector2I(20, 20), SubRanges = new Vector2I(30, 15), MulRanges = new Vector2I(12, 12), DivRanges = new Vector2I(9, 9) 
                    }};

            lastWiepoutTime = 0f;

            currentDifficulty = 0;
            if (difficultySettings.Count > 1)
                nextUpTreshold = difficultySettings[1].Treshold;
            gameState.CurrentDifficulty = difficultySettings[currentDifficulty];
        }

        /// <summary>
        /// Update regulate rate when the screen has ben cleared of all droplets.
        /// </summary>
        /// <param name="playTime">Play time in seconds when wipeout occured.</param>
        public void WipedOutScreen(float playTime)
        {
            currentDifficulty = Mathf.Max(0, currentDifficulty - 2);
            lastWiepoutTime = playTime;

            gameState.DifficultyRegulateRate = Mathf.Min(1.0f, gameState.DifficultyRegulateRate);
            gameState.CurrentDifficulty = difficultySettings[currentDifficulty];
            gameState.CurrentDifficultyLevel = currentDifficulty;

            Debug.Log(string.Format("<color=red>Moving back onto {0} difficulty.</color>", currentDifficulty));
        }

        /// <summary>
        /// Continuously scales game difficulty based on droplets position on screen.
        /// </summary>
        /// <param name="playTime">Play time in seconds.</param>
        public void Update(float playTime)
        {
            float regulateRate = 0f;
            float depthRatio = MaxDepthRatio(UnityEngine.Screen.height); // 320 from the legacy code
            float depthWeight = 2.0f;

            regulateRate = 1.0f + depthWeight - (2.0f * depthWeight * depthRatio);

            float regulateMin = 0.5f;
            regulateRate = Mathf.Max(regulateMin, Mathf.Min(3.0f, regulateRate));

            float populateSeconds = 5.0f;
            float populateRate = (playTime - lastWiepoutTime) / populateSeconds;
            populateRate = Mathf.Max(0.01f, Mathf.Min(1.0f, populateRate));
            regulateRate *= populateRate;

            // Assign final rate
            gameState.DifficultyRegulateRate = regulateRate;

            //if (DeveloperCheats.ForceNextDifficultyLevel)
            //{
            //    if(currentDifficulty < difficultySettings.Count - 1)
            //        MoveToNextDifficulty();
            //    DeveloperCheats.ForceNextDifficultyLevel = false;
            //}
        }

        /// <summary>
        /// Attempts to update difficulty based on total correct answers.
        /// </summary>e
        public void CheckForNewDifficulty()
        {
            if (currentDifficulty < difficultySettings.Count - 1)
            {
                var nextUp = difficultySettings[currentDifficulty + 1];
                float nextUpTreshold = nextUp.Treshold;
                nextUpTreshold = nextUpTreshold / gameState.DifficultyRegulateRate;

                if (gameState.TotalCorrect >= nextUpTreshold)
                {
                    MoveToNextDifficulty();
                }

                this.nextUpTreshold = (int)nextUpTreshold;
            }
        }

        /// <summary>
        /// Moves to next difficulty level.
        /// </summary>
        private void MoveToNextDifficulty()
        {
            currentDifficulty++;
            gameState.CurrentDifficulty = difficultySettings[currentDifficulty];
            gameState.CurrentDifficultyLevel = currentDifficulty;

            Debug.Log(string.Format("<color=red>Moving forward onto {0} difficulty.</color>", currentDifficulty));
        }

        /// <summary>
        /// Returns the maximum depth ratio in droplets list.
        /// Depth ratio 0 equals the droplet being at the top of the scene, while 1 being on the bottom.
        /// </summary>
        /// <returns></returns>
        private float MaxDepthRatio(int screenHeight)
        {
            float max = 0f;
            foreach (var droplet in gameState.ActiveDrops)
            {
                float depthRatio = Utility.WorldToPixels(droplet.transform.position).y / (float)screenHeight;
                if (max < depthRatio)
                    max = depthRatio;
            }

            return Mathf.Min(max, 1.0f);
        }
    }
}