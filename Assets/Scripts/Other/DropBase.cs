using System;
using UnityEngine;
using TMPro;
namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Types of spawnable droplets.
    /// </summary>
    public enum DropletType
    {
        DROPLET = 0,
        SUN
    }

    /// <summary>
    /// Base for Droplet and Sun.
    /// </summary>
    public abstract class DropBase : MonoBehaviour
    {
        public delegate void DropletEventHandler(object sender, EventArgs e);

        public event DropletEventHandler onWaterCollision;
        public event DropletEventHandler onDeath;

        public SpriteAnimator signAnimator;

        public Equation Equation
        {
            get { return equation; }
        }
        public float Speed
        {
            get { return speed; }
        }
        public float LifeTime
        {
            get { return lifeTimer.Time; }
        }
        public DropletType DropType
        {
            get { return dropType; }
        }
        
        protected Equation equation;
        protected float speed;
        protected bool hitWater;
        protected DropletType dropType;

        protected TextMeshProUGUI operandsText;

        protected Timer lifeTimer;
        protected bool paused;

        protected virtual void Awake()
        {
            operandsText = GetComponentInChildren<TextMeshProUGUI>();
            lifeTimer = new Timer();
        }

        protected void Initialize(Equation equation, Vector3 position, float speed)
        {
            this.equation = equation;
            this.speed = speed;

            transform.position = position;
            hitWater = false;
            lifeTimer.Restart();

            operandsText.text = string.Format("{0}\n{1}", equation.OperandA, equation.OperandB);
            signAnimator.Play(equation.OperatorType.ToString()); // Makes sure we're displaying the correct sign
        }

        protected virtual void Update()
        {
            if (!hitWater)
            {
                if(!paused)
                    transform.position += new Vector3(0f, -speed * Time.deltaTime, 0f);
                lifeTimer.Update();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Water"))
            {
                hitWater = true;
                if (onWaterCollision != null)
                    onWaterCollision(this, EventArgs.Empty);
            }
        }

        protected void OnDeath()
        {
            if (onDeath != null)
                onDeath(this, EventArgs.Empty);
        }

        /// <summary>
        /// Pauses and hides the equation.
        /// </summary>
        public void Pause()
        {
            paused = true;
            lifeTimer.Pause();
            signAnimator.gameObject.SetActive(false);
            operandsText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Unpauses the drop.
        /// </summary>
        public void Unpause()
        {
            paused = false;
            lifeTimer.Unpause();
            signAnimator.gameObject.SetActive(true);
            operandsText.gameObject.SetActive(true);
        }

        /// <summary>
        /// IPoolListener callback.
        /// </summary>
        public virtual void OnDisable()
        {
            equation = null;
            speed = 0f;
        }

        /// <summary>
        /// IPoolListener callback.
        /// </summary>
        public virtual void OnEnable()
        {
            gameObject.layer = UnityConstants.Layers.Default;
            operandsText.gameObject.SetActive(true);
            signAnimator.gameObject.SetActive(true);
        }
    }
}
