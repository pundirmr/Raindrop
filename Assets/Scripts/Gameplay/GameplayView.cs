using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LumosLabs.Raindrops
{
    public class GameplayView : MonoBehaviour
    {
        public Animator sunAnimator;
        public Animator keypadAnimator;
        public Animator pipAnimator;
        public Animator bubblesAnimator;
        public GameObject dropPrefab;
        public GameObject sunPrefab;
        public TextMeshProUGUI answerInput;
        public Water water;
        public RectTransform spawnAreaRect;
        public Transform DropsAndSunParent;

        public delegate void OnUpdate();
        public event OnUpdate OnUpdateEvent;

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

        public void OnGameReset()
        {
            answerInput.text = "";
        }

        public void SunAppear()
        {
            sunAnimator.SetBool("sunActive", true);
        }

        public void SunDisappear()
        {
            sunAnimator.SetBool("sunActive", false);
        }

        public void KeypadDisappear()
        {
            keypadAnimator.SetTrigger("GameEnd");
        }

        public void PipDisappear()
        {
            pipAnimator.SetTrigger("SlideOut");
        }

        public void BubblesAppear()
        {
            bubblesAnimator.SetTrigger("GameEnd");
        }
    }
}
