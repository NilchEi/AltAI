using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AltAI
{
    public static class SappersUtility
    {
		public static bool IsGoodSapper(Pawn p)
		{
			return SappersUtility.HasBuildingDestroyerWeapon(p) && SappersUtility.CanMineReasonablyFast(p);
		}

		// Token: 0x060031AD RID: 12717 RVA: 0x001133B1 File Offset: 0x001115B1
		public static bool IsGoodBackupSapper(Pawn p)
		{
			return SappersUtility.CanMineReasonablyFast(p);
		}

		// Token: 0x060031AE RID: 12718 RVA: 0x001133C8 File Offset: 0x001115C8
		private static bool CanMineReasonablyFast(Pawn p)
		{
			return p.RaceProps.Humanlike && !p.skills.GetSkill(SkillDefOf.Mining).TotallyDisabled && !StatDefOf.MiningSpeed.Worker.IsDisabledFor(p) && p.skills.GetSkill(SkillDefOf.Mining).Level >= 4;
		}

		// Token: 0x060031AF RID: 12719 RVA: 0x00113428 File Offset: 0x00111628
		public static bool HasBuildingDestroyerWeapon(Pawn p)
		{
			if (p.equipment == null || p.equipment.Primary == null)
			{
				return false;
			}
			List<Verb> allVerbs = p.equipment.Primary.GetComp<CompEquippable>().AllVerbs;
			for (int i = 0; i < allVerbs.Count; i++)
			{
				if (allVerbs[i].verbProps.ai_IsBuildingDestroyer)
				{
					return true;
				}
			}
			return false;
		}
	}
}
