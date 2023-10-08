using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class PlaySessionNote
    {
        public static readonly MsgId Completed =
                new MsgId("Raindrops.PlaySessionNote.Completed", MsgKind.Notification);
    }
}
