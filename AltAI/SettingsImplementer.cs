using Verse;

namespace AltAI
{
    // Token: 0x02000006 RID: 6
    [StaticConstructorOnStartup]
    public class SettingsImplementer
    {
        // Token: 0x0600000F RID: 15 RVA: 0x00002750 File Offset: 0x00000950
        static SettingsImplementer()
        {
            LordJob_AssaultColony.sapperSmartChance = LoadedModManager.GetMod<AltAISettings>().settings.repSapperSmartChance;
        }
    }
}