using UnityEngine;
using LumosLabs.Shared.Pillar;
using TMPro;

namespace LumosLabs.Raindrops
{
    public class TutorialView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI hintText;

        public Animator sunAnimator;
        public RectTransform spawnAreaRect;
        public Transform DropsAndSunParent;
        public Water water;
        public GameObject dropPrefab;
        public GameObject sunPrefab;
        public TextMeshProUGUI answerInput;

        public delegate void OnUpdate();
        public event OnUpdate OnUpdateEvent;

        public void SetHintText(string text)
        {
            hintText.text = text;
        }

        public void Initialize()
        {
            answerInput.text = "";
        }

        public void SetAnswerText(string newNumber)
        {
            answerInput.text = newNumber;
        }

        void Update()
        {
            if (OnUpdateEvent != null)
            {
                OnUpdateEvent();
            }
        }

        public void SunAppear()
        {
            sunAnimator.SetBool("sunActive", true);
        }

        public void SunDisappear()
        {
            sunAnimator.SetBool("sunActive", false);
        }
    }
}
