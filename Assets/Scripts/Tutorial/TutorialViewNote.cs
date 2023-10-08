using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public static class TutorialViewNote
    {
        public static readonly MsgId NextStep =
            new MsgId("Raindrops.TutorialViewNote.NextStep", MsgKind.Notification);
        public static readonly MsgId Restart =
            new MsgId("Raindrops.TutorialViewNote.Restart", MsgKind.Notification);
        public static readonly MsgId SpawnDroplet =
            new MsgId("Raindrops.TutorialViewNote.SpawnDroplet", MsgKind.Notification);
        public static readonly MsgId SpawnSun =
            new MsgId("Raindrops.TutorialViewNote.SpawnSun", MsgKind.Notification);
    }
}
