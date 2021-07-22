using UnityEngine;
using Verse;

namespace AltAI
{
    // Token: 0x02000005 RID: 5
    public class AltAISettings : Mod
    {
        // Token: 0x0600000C RID: 12 RVA: 0x0000255C File Offset: 0x0000075C
        public AltAISettings(ModContentPack content) : base(content)
        {
            this.settings = base.GetSettings<AltAIModSettings>();
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002574 File Offset: 0x00000774
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.Label("repSapperSmartChanceExp".Translate(), -1f, null);
            listing_Standard.Gap(4f);
            string text = this.settings.repSapperSmartChance.ToString();
            listing_Standard.TextFieldNumeric<int>(ref this.settings.repSapperSmartChance, ref text, 0, 100);
            listing_Standard.End();
            base.DoSettingsWindowContents(inRect);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002738 File Offset: 0x00000938
        public override string SettingsCategory()
        {
            return "Alternative AI";
        }

        // Token: 0x04000007 RID: 7
        public readonly AltAIModSettings settings;
    }
}
