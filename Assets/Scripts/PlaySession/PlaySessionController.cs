using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class PlaySessionController : Controller
    {
        private ISession _session;

        #region Initialization

        protected override void Initialize()
        {
            var store = Services.Get<IModelStoreService>();
        }

        protected override void RegisterCommands(IMessageRegistrar commands)
        {
            commands.AddListener(PlaySessionCmd.Start, HandleStartCommand);
            commands.AddListener(PlaySessionCmd.Reset, HandleResetCommand);
        }

        protected override void RegisterNotifications(IMessageRegistrar notifications)
        {
            notifications.AddListener(GameplayNote.End, HandleSessionEnded);
        }
        #endregion

        private void HandleStartCommand()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.Send(GameplayCmd.Start);
        }

        private void HandleResetCommand()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.Send(GameplayCmd.Reset);
        }

        private void HandleSessionEnded()
        {
            var messenger = Services.Get<IMessengerService>();
            messenger.Send(MetadataCmd.CompleteSession);
            messenger.Send(PlaySessionNote.Completed);
        }
    }
}
