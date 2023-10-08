using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;
using LumosLabs.Shared;

namespace LumosLabs.Raindrops
{
    public class RAutoPlayer : AutoPlayer
    {
        [SerializeField] private float _stepDelay;

        public override void StartAutoPlay()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.AddListener(AutoPlayCmd.Start, HandleStart);
        }

        private void HandleStart()
        {
            StartCoroutine(DelayStep());
        }

        private IEnumerator DelayStep()
        {
            var messenger = Services.Get<IMessengerService>();
            while (true)
            {
                yield return new WaitForSeconds(_stepDelay);

                var correct = Random.Range(0f, 1f) < TargetPerformance;

                if (correct)
                {
                    messenger.Send(AutoPlayNote.Answer);
                }
            }
        }
    }
}
