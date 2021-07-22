using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace AltAI
{
	// Token: 0x020007D3 RID: 2003
	public class RaidStrategyWorker_ImmediateAttackSmart : RaidStrategyWorker
	{
		// Token: 0x06003442 RID: 13378 RVA: 0x00121BD7 File Offset: 0x0011FDD7
		protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
		{
			return new LordJob_AssaultColony(parms.faction, true, true, false, true, true);
		}

		// Token: 0x06003443 RID: 13379 RVA: 0x00121BE9 File Offset: 0x0011FDE9
		public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
		{
			return base.CanUseWith(parms, groupKind) && parms.faction.def.canUseAvoidGrid;
		}
	}
}