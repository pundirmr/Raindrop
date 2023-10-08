using System.Timers;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Simple timer class that you can Pause and Unpause.
    /// </summary>
    public class Timer
    {
        //private System.Timers.Timer _sysTimer;

        private float timer;
        private float startTime;
        private float pauseTime;
        private float pauseTimeTotal;
        private float pauseStartTime;
        private bool paused;

        /// <summary>
        /// Elapsed time in seconds from last Restart().
        /// </summary>
        public float Time
        {
            get { return timer; }
        }

        /// <summary>
        /// True if timer is paused.
        /// </summary>
        public bool Paused
        {
            get { return paused; }
        }

        public Timer()
        {
            Restart();
        }

        /// <summary>
        /// Restarts the timer.
        /// </summary>
        public void Restart()
        {
            timer = 0;
            startTime = UnityEngine.Time.time;
            pauseTime = 0;
            pauseStartTime = 0;
            pauseTimeTotal = 0;
            paused = false;
        }

        /// <summary>
        /// Pauses the timer, you can still keep calling Update() safely. Unpause with Unpause();
        /// </summary>
        public void Pause()
        {
            paused = true;
            pauseStartTime = UnityEngine.Time.time;
        }

        /// <summary>
        /// Unpauses the timer.
        /// </summary>
        public void Unpause()
        {
            paused = false;
            pauseTimeTotal += pauseTime;
        }

        /// <summary>
        /// Updates the timer using Unity's Time.time.
        /// </summary>
        public void Update()
        {
            if (!paused)
                timer = UnityEngine.Time.time - startTime - pauseTimeTotal;
            else
                pauseTime = UnityEngine.Time.time - pauseStartTime;
        }

        private void UpdateTimer(System.Object source, ElapsedEventArgs e)
        {
            Update();
        }
    }
}