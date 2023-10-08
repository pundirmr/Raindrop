using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class RPlayStateController : GameStateController
    {
        protected override int State => GameState.Play;

        protected override void OnEnterState()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.AddListener(PlaySessionNote.Completed, HandleSessionCompleted);
            messenger.Send(PlaySessionCmd.Start);
        }

        protected override void OnExitState()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.RemoveListener(PlaySessionNote.Completed, HandleSessionCompleted);
        }

        private void HandleSessionCompleted()
        {
            ExitState();
        }
    }
}
