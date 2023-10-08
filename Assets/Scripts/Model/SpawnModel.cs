using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Decides when to spawn new droplets and assigns their launch properties (position, speed).
    /// </summary>
    public class SpawnModel : Model
    {
        /// <summary>
        /// Current spawn period for the next drop.
        /// </summary>
        public float SpawnPeriod
        {
            get { return spawnPeriod; }
        }

        protected RaindropsGameState gameState;

        public int TotalDrops { get; set; }

        private readonly float baseSpawnPeriod;
        protected readonly float baseSpeed;

        protected Vector2 spawnRangeX;
        protected Vector2 spawnRangeY;

        private readonly Vector2 colliderOffset;
        private readonly Vector2 colliderSize;

        private Timer spawnTimer;
        private float spawnPeriod;
        private int sunOccuranceFrequency;
        public float waterPixelPosition;

        public SpawnModel(IMessenger messenger, RaindropsGameState gameState, float spawnPeriod, float baseSpeed, Vector2 colliderOffset, Vector2 colliderSize, int sunOccuranceFrequency) : base(messenger)
        {
            TotalDrops = 0;
            baseSpawnPeriod = spawnPeriod;
            this.gameState = gameState;
            this.baseSpeed = baseSpeed;
            this.colliderOffset = colliderOffset;
            this.colliderSize = colliderSize;
            this.sunOccuranceFrequency = sunOccuranceFrequency;
            waterPixelPosition = 0;

            spawnTimer = new Timer();
        }

        public void SetSpawnRange(Vector2 spawnX, Vector2 spawnY)
        {
            this.spawnRangeX = spawnX;
            this.spawnRangeY = spawnY;
        }

        public void Reset()
        {
            TotalDrops = 0;
            waterPixelPosition = 0;
            spawnTimer = new Timer();
        }

        public void OnViewUpdateListener()
        {
            spawnTimer.Update();
            if(ShouldSpawnNow())
            {
                Messenger.Send(GameplayNote.SpawnDroplet);
                TotalDrops++;
            }

            if(ShouldSpawnSun())
            {
                Messenger.Send(GameplayNote.SpawnSun);
                TotalDrops++;
            }
        }

        /// <summary>
        /// Resolves whether a new droplet should be spawned.
        /// </summary>
        /// <returns>True if a new droplet should spawn.</returns>
        public virtual bool ShouldSpawnNow()
        {
            spawnPeriod = baseSpawnPeriod;
            spawnPeriod = Mathf.Pow(0.99f, gameState.TotalCorrect);
            spawnPeriod /= gameState.DifficultyRegulateRate;

            bool spawn = (spawnTimer.Time >= spawnPeriod && !MaxSpawned()) || gameState.ActiveDrops.Count < 1;
            if (spawn)
                spawnTimer.Restart();

            return spawn;
        }

        /// <summary>
        /// Gets a random valid spawn position.
        /// </summary>
        /// <returns>Spawn position vector.</returns>
        public virtual Vector3 GetSpawnPosition()
        {
            Vector2 position = Vector2.zero;

            bool doesOverlap = false;
            // If there is no such position then just return whatever one
            int timeout = 50;

            var droplet = new GameObject("drop", typeof(RectTransform));

            do
            {
                doesOverlap = false;

                position.x = Random.Range(spawnRangeX.x, spawnRangeX.y);
                position.y = Random.Range(spawnRangeY.x, spawnRangeY.y);

                foreach (var drop in gameState.ActiveDrops)
                {
                    droplet.GetComponent<RectTransform>().position = position;
                    if (droplet.GetComponent<RectTransform>().Overlaps(drop.GetComponent<RectTransform>()))
                    {
                        doesOverlap = true;
                        break;
                    }
                }

            } while (doesOverlap && timeout-- > 0);

            GameObject.Destroy(droplet);

            return position;
        }

        /// <summary>
        /// Gets spawn speed based on current game state.
        /// </summary>
        /// <returns></returns>
        public virtual float GetSpawnSpeed()
        {
            var speed = baseSpeed + 3.5f * gameState.TotalHits;
            if (gameState.TotalCorrect <= 40)
            {
                speed *= Mathf.Pow(0.985f, gameState.TotalCorrect);
            }
            else if (gameState.TotalCorrect > 40 && gameState.TotalCorrect <= 60)
            {
                speed *= Mathf.Pow(0.985f, 40);
                speed *= Mathf.Pow(0.992f, gameState.TotalCorrect - 40);
            }
            else
            {
                speed *= Mathf.Pow(0.985f, 40);
                speed *= Mathf.Pow(0.992f, 20);
                speed *= Mathf.Pow(0.996f, gameState.TotalCorrect - 60);
            }
            speed += 1.5f;

            speed = waterPixelPosition / speed;

            //speed /= 100f; // Convert from pixels/sec to Unity's units/sec

            return speed;
        }

        /// <summary>
        /// Resolves if spawned droplet should be sun.
        /// </summary>
        /// <returns>True if sun should be spawned.</returns>
        public virtual bool ShouldSpawnSun()
        {
            bool spawnSun = false;
            if (TotalDrops > 0)
            {
                if (TotalDrops <= 160)
                    spawnSun = (TotalDrops % sunOccuranceFrequency == 0);
                else
                    spawnSun = ((TotalDrops % (2 * sunOccuranceFrequency)) == 0);
            }
            return spawnSun;
        }

        /// <summary>
        /// Resolves whether we reached maximum spawned droplets.
        /// </summary>
        /// <returns>True if the spawn limit has been hit.</returns>
        private bool MaxSpawned()
        {
            return gameState.ActiveDrops.Count > (gameState.CurrentDifficulty.MaxDroplets - gameState.TotalHits);
        }
    }

}