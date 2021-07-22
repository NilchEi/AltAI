using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AltAI
{
	// Token: 0x02000778 RID: 1912
	public class LordToil_AssaultColonySappers : LordToil
	{
		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x060031A2 RID: 12706 RVA: 0x00113083 File Offset: 0x00111283
		private LordToilData_AssaultColonySappers Data
		{
			get
			{
				return (LordToilData_AssaultColonySappers)this.data;
			}
		}

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x060031A3 RID: 12707 RVA: 0x0001015A File Offset: 0x0000E35A
		public override bool AllowSatisfyLongNeeds
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x060031A4 RID: 12708 RVA: 0x000100E1 File Offset: 0x0000E2E1
		public override bool ForceHighStoryDanger
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060031A5 RID: 12709 RVA: 0x00113090 File Offset: 0x00111290
		public LordToil_AssaultColonySappers()
		{
			this.data = new LordToilData_AssaultColonySappers();
		}

		// Token: 0x060031A6 RID: 12710 RVA: 0x00112FFC File Offset: 0x001111FC
		public override void Init()
		{
			base.Init();
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.Drafting, OpportunityType.Critical);
		}

		// Token: 0x060031A7 RID: 12711 RVA: 0x001130A4 File Offset: 0x001112A4
		public override void UpdateAllDuties()
		{
			if (!this.Data.sapperDest.IsValid && this.lord.ownedPawns.Any<Pawn>())
			{
				this.Data.sapperDest = GenAI.RandomRaidDest(this.lord.ownedPawns[0].Position, base.Map);
			}
			List<Pawn> list = null;
			if (this.Data.sapperDest.IsValid)
			{
				list = new List<Pawn>();
				for (int i = 0; i < this.lord.ownedPawns.Count; i++)
				{
					Pawn pawn = this.lord.ownedPawns[i];
					if (SappersUtility.IsGoodSapper(pawn))
					{
						list.Add(pawn);
					}
				}
				if (list.Count == 0 && this.lord.ownedPawns.Count >= 2)
				{
					Pawn pawn2 = null;
					int num = 0;
					for (int j = 0; j < this.lord.ownedPawns.Count; j++)
					{
						if (SappersUtility.IsGoodBackupSapper(this.lord.ownedPawns[j]))
						{
							int level = this.lord.ownedPawns[j].skills.GetSkill(SkillDefOf.Mining).Level;
							if (pawn2 == null || level > num)
							{
								pawn2 = this.lord.ownedPawns[j];
								num = level;
							}
						}
					}
					if (pawn2 != null)
					{
						list.Add(pawn2);
					}
				}
			}
			for (int k = 0; k < this.lord.ownedPawns.Count; k++)
			{
				Pawn pawn3 = this.lord.ownedPawns[k];
				if (list != null && list.Contains(pawn3))
				{
					pawn3.mindState.duty = new PawnDuty(DutyDefOf.Sapper, this.Data.sapperDest, -1f);
				}
				else if (!list.NullOrEmpty<Pawn>())
				{
					float randomInRange;
					if (pawn3.equipment != null && pawn3.equipment.Primary != null && pawn3.equipment.Primary.def.IsRangedWeapon)
					{
						randomInRange = LordToil_AssaultColonySappers.EscortRadiusRanged.RandomInRange;
					}
					else
					{
						randomInRange = LordToil_AssaultColonySappers.EscortRadiusMelee.RandomInRange;
					}
					pawn3.mindState.duty = new PawnDuty(DutyDefOf.Escort, list.RandomElement<Pawn>(), randomInRange);
				}
				else
				{
					pawn3.mindState.duty = new PawnDuty(DutyDefOf.AssaultColony);
				}
			}
		}

		// Token: 0x060031A8 RID: 12712 RVA: 0x00113314 File Offset: 0x00111514
		public override void Notify_ReachedDutyLocation(Pawn pawn)
		{
			this.Data.sapperDest = IntVec3.Invalid;
			this.UpdateAllDuties();
		}

		// Token: 0x04001AF5 RID: 6901
		private static readonly FloatRange EscortRadiusRanged = new FloatRange(15f, 19f);

		// Token: 0x04001AF6 RID: 6902
		private static readonly FloatRange EscortRadiusMelee = new FloatRange(23f, 26f);
	}
}
