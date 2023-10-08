using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    public class WaterNote
    {
        public static readonly MsgId Initialize =
            new MsgId("Raindrops.WaterNote.Initialize", MsgKind.Notification);
        public static readonly MsgId Reset =
            new MsgId("Raindrops.WaterNote.Reset", MsgKind.Notification);
        public static readonly MsgId OnDropHit =
            new MsgId("Raindrops.WaterNote.OnDropHit", MsgKind.Notification);
        public static readonly MsgId<int> SetWaterLevel =
            new MsgId<int>("Raindrops.WaterNote.SetWaterLevel", MsgKind.Notification);
    }
}
