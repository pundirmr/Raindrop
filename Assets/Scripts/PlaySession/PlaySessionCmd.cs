using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class PlaySessionCmd
    {
        public static readonly MsgId Start = new MsgId("Raindrops.PlaySessionCmd.Start", MsgKind.Command);
        public static readonly MsgId Reset = new MsgId("Raindrops.PlaySessionCmd.Reset", MsgKind.Command);
    }
}
