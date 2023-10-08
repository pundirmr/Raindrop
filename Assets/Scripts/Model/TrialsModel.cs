using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class TrialsModel : Model
    {
        // Game state information shared among managers
        public RaindropsGameState gameState;

        public const float DropletVisibilityTreshold = 2.7f;

        // Data only for game manager
        public int totalCorrections;
        public int lastCorrections;
        public int totalTries;
        public int totalTriesCorrections;
        public int totalKeyboardTries;
        public float avgCorrectResponseTime;

        public float lastDropletSpeed;

        // Gets us new equations
        public EquationsManager equationsManager;

        // Controls difficulty of the game
        public DifficultyManager difficultyManager;

        // Handles when we should spawn new droplets
        public SpawnModel spawnModel;

        // Pauses
        public bool paused;

        public Timer playTimer;

        // Channel that plays the ambient
        public int ambientSoundId;

        public ObjectPool dropletsPool;
        public ObjectPool sunsPool;
        public string currentTypedNumberStr;
        public float waterPixelPosition;

        public SettingsModel settingsModel;

        public TrialsModel(IMessenger messenger) : base(messenger)
        {
            gameState = new RaindropsGameState();         

            // Initialize difficulty manager with list of difficulty settings
            difficultyManager = new DifficultyManager(this);

            equationsManager = new EquationsManager(gameState);

            totalCorrections = 0;
            lastCorrections = 0;
            totalTries = 0;
            totalTriesCorrections = 0;
            totalKeyboardTries = 0;
            lastDropletSpeed = 0;
            avgCorrectResponseTime = 0;
            paused = false;
            currentTypedNumberStr = "";
            playTimer = new Timer();
        }

        public void OnGameReset()
        {
            for (int i = 0; i < gameState.ActiveDrops.Count; i++)
            {
                if (gameState.ActiveDrops[i].GetType() == typeof(Droplet))
                {
                    dropletsPool.Release(gameState.ActiveDrops[i].gameObject);
                }
                else
                {
                    sunsPool.Release(gameState.ActiveDrops[i].gameObject);
                }
                gameState.ActiveDrops[i].gameObject.gameObject.SetActive(false);
            }
            gameState.ActiveDrops.Clear();
            /*dropletsPool?.Reset();
            sunsPool?.Reset();*/
            gameState = new RaindropsGameState();
            // Initialize difficulty manager with list of difficulty settings
            difficultyManager = new DifficultyManager(this);

            equationsManager = new EquationsManager(gameState);

            // Initialize, default values the rest
            playTimer = new Timer();

            totalCorrections = 0;
            lastCorrections = 0;
            totalTries = 0;
            totalTriesCorrections = 0;
            totalKeyboardTries = 0;
            lastDropletSpeed = 0;
            avgCorrectResponseTime = 0;
            paused = false;
        }
    }
}
