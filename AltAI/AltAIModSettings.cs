using Verse;

namespace AltAI
{
    // Token: 0x02000004 RID: 4
    public class AltAIModSettings : ModSettings
    {
        // Token: 0x0600000A RID: 10 RVA: 0x0000249C File Offset: 0x0000069C
        public override void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.repSapperSmartChance, "repSapperSmartChance", 20, false);
            base.ExposeData();
        }

        // Token: 0x04000006 RID: 6
        public int repSapperSmartChance;
    }
}