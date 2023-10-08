using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class RRestartStateController : GameStateController
    {
        protected override int State => GameState.Restart;

        protected override void OnEnterState()
        {
            // Send reset messages here
            var messenger = Services.Get<IMessengerService>();
            messenger.Send(PlaySessionCmd.Reset);
        }

        protected override void OnExitState()
        {

        }
    }
}
