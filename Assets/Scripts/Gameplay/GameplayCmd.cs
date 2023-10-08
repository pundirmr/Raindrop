using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public static class GameplayCmd
    {
        public static readonly MsgId Start =
            new MsgId("Raindrops.GameplayCmd.Start", MsgKind.Command);
        public static readonly MsgId Reset =
            new MsgId("Raindrops.GameplayCmd.Reset", MsgKind.Command);
    }
}
