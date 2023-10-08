using System;
using UnityEngine;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Abstract class for frame based sprite animations.
    /// </summary>
    public abstract class SpriteAnimator : MonoBehaviour
    {
        public class ChangedFrameEventArgs : EventArgs
        {
            public string AnimationName { get; set; }
            public int Frame { get; set; }
        }
        public class AnimationFinishedEventArgs : EventArgs
        {
            public string AnimationName { get; set; }
        }

        public delegate void ChangedFrameEventHandler(object sender, ChangedFrameEventArgs e);
        public delegate void AnimationFinishedEventHandler(object sender, AnimationFinishedEventArgs e);

        public event ChangedFrameEventHandler onChangedFrame;
        public event AnimationFinishedEventHandler onAnimationFinished;

        /// <summary>
        /// True if animation is currently unpaused.
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            protected set
            {
                playing = value;
                if (playing && frameTimer.Paused)
                    frameTimer.Unpause();
                else if (!playing && !frameTimer.Paused)
                    frameTimer.Pause();
            }
        }

        protected bool finished;
        protected bool playing;
        protected int frameIndex;
        protected Timer frameTimer;

        protected int animationId;
        protected string AnimationName { get; set; }

        // Abstract members 
        protected abstract ISpriteAnimation[] Animations { get; }
        protected abstract void UpdateMesh();

        protected virtual void Awake()
        {
            frameIndex = 0;
            frameTimer = new Timer();
            Playing = false;
            finished = false;
        }

        private void LateUpdate()
        {
            if (Playing && !finished && frameTimer.Time >= Animations[animationId].Speed)
            {
                var nextFrame = (frameIndex + 1) % Animations[animationId].Length;
                if (frameIndex > 0 && nextFrame == 0 && !Animations[animationId].Loop)
                {
                    finished = true;

                    // Animnation ended
                    if (onAnimationFinished != null)
                        onAnimationFinished(this, new AnimationFinishedEventArgs { AnimationName = AnimationName });

                }
                else
                {
                    frameIndex = nextFrame;
                    UpdateMesh();
                    frameTimer.Restart();

                    // New frame
                    if (onChangedFrame != null)
                        onChangedFrame(this, new ChangedFrameEventArgs { AnimationName = AnimationName, Frame = frameIndex });
                }
            }

            frameTimer.Update();
        }

        /// <summary>
        /// Plays an animation.
        /// </summary>
        /// <param name="animation">Animation name.</param>
        public void Play(string animation)
        {
            bool found = false;
            for(int i = 0; i < Animations.Length; i++)
            {
                if (Animations[i].Name == animation)
                {
                    AnimationName = animation;
                    animationId = i;
                    found = true;
                    break;
                }
            }
            if (!found)
                Debug.LogError(string.Format("No animation found: {0}", animation));

            Restart();
            UpdateMesh();
        }

        /// <summary>
        /// Pauses playback untill Unpause is called or a new animation is played.
        /// </summary>
        public void Pause()
        {
            Playing = false;
        }

        /// <summary>
        /// Unpauses playback.
        /// </summary>
        public void Unpause()
        {
            Playing = true;
        }

        /// <summary>
        /// Restarts animation from the first frame.
        /// </summary>
        public void Restart()
        {
            Playing = true;
            finished = false;
            frameTimer.Restart();
            frameIndex = 0;
        }
    }
}