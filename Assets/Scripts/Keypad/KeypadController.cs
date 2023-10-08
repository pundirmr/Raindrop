using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class KeypadController : Controller
    {
        [SerializeField] private bool isTutorial;
        public void OnKeypadClicked(int val)
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.Send(GameplayNote.OnKeypadClicked, val, isTutorial);
        }
    }
}
