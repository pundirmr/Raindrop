using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class WaterController : Controller
    {
        [SerializeField] private Water waterComponent;

        protected override void RegisterCommands(IMessageRegistrar commands)
        {

        }

        protected override void RegisterNotifications(IMessageRegistrar notifications)
        {
            notifications.AddListener(WaterNote.Initialize, HandleInitialize);
            notifications.AddListener(WaterNote.Reset, HandleReset);
            notifications.AddListener(WaterNote.OnDropHit, HandleDropHit);
            notifications.AddListener(WaterNote.SetWaterLevel, HandleWaterLevel);
        }

        private void HandleDropHit()
        {
            waterComponent.MoveWaterUp();
            if (waterComponent.Level > 3)
            {
                Services.Get<IMessengerService>().Send(GameplayNote.End);
            }
        }

        private void HandleInitialize()
        {
            waterComponent.Initialize();
        }

        private void HandleReset()
        {
            waterComponent.OnGameReset();
        }

        private void HandleWaterLevel(int level)
        {
            waterComponent.SetWaterLevel(level);
        }
    }
}
