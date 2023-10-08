using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public static class GameplayNote
    {
        public static readonly MsgId OnModelInitialized =
            new MsgId("Raindrops.GameplayNote.OnModelInitialized", MsgKind.Notification);

        public static readonly MsgId<int, bool> OnKeypadClicked =
            new MsgId<int, bool>("Raindrops.GameplayNote.KeypadTapped", MsgKind.Notification);

        public static readonly MsgId SpawnDroplet =
            new MsgId("Raindrops.GameplayNote.SpawnDroplet", MsgKind.Notification);

        public static readonly MsgId SpawnSun =
            new MsgId("Raindrops.GameplayNote.SpawnSun", MsgKind.Notification);

        public static readonly MsgId End =
            new MsgId("Raindrops.GameplayNote.End", MsgKind.Notification);
    }
}
