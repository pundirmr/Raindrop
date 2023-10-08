using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class TrialTutorialStep : TutorialStep
    {
        [SerializeField] private int _dropCount;
        [SerializeField] private int _sunCount;
        [SerializeField] private bool _spawnAtOnce;

        protected override void OnStartStep()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.AddListener(TutorialViewNote.Restart, Restart);
            StartCoroutine(StepImpl());
        }

        protected override void OnCancelStep()
        {
            
        }

        protected override void OnComplete()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.RemoveListener(TutorialViewNote.Restart, Restart);
        }

        private void Restart()
        {
            StopAllCoroutines();
            StartCoroutine(StepImpl());
        }

        private IEnumerator StepImpl()
        {
            yield return new WaitForSeconds(1);
            var messenger = Services.Get<IMessengerService>();
            if (!_spawnAtOnce)
            {
                for (int i = 0; i < _dropCount; i++)
                {
                    messenger.Send(TutorialViewNote.SpawnDroplet);
                    yield return TemporaryListener.Wait(messenger, TutorialViewNote.NextStep);
                }
            }
            else
            {
                for (int i = 0; i < _dropCount; i++)
                {
                    messenger.Send(TutorialViewNote.SpawnDroplet);
                }
                messenger.Send(TutorialViewNote.SpawnSun);
                yield return TemporaryListener.Wait(messenger, TutorialViewNote.NextStep);
            }
            CompleteStep();
        }
    }
}
