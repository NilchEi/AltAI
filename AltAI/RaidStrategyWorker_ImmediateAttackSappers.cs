﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace AltAI
{
    // Token: 0x02000003 RID: 3
    public class RaidStrategyWorker_ImmediateAttackSappers : RaidStrategyWorker
    {
		public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
		{
			return this.PawnGenOptionsWithSappers(parms.faction, groupKind).Any<PawnGroupMaker>() && base.CanUseWith(parms, groupKind);
		}

		public override float MinimumPoints(Faction faction, PawnGroupKindDef groupKind)
		{
			return Mathf.Max(base.MinimumPoints(faction, groupKind), this.CheapestSapperCost(faction, groupKind));
		}

		public override float MinMaxAllowedPawnGenOptionCost(Faction faction, PawnGroupKindDef groupKind)
		{
			return this.CheapestSapperCost(faction, groupKind);
		}

		private float CheapestSapperCost(Faction faction, PawnGroupKindDef groupKind)
		{
			IEnumerable<PawnGroupMaker> enumerable = this.PawnGenOptionsWithSappers(faction, groupKind);
			if (!enumerable.Any<PawnGroupMaker>())
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to get MinimumPoints for ",
					base.GetType().ToString(),
					" for faction ",
					faction.ToString(),
					" but the faction has no groups with sappers. groupKind=",
					groupKind
				}), false);
				return 99999f;
			}
			float num = 9999999f;
			foreach (PawnGroupMaker pawnGroupMaker in enumerable)
			{
				foreach (PawnGenOption pawnGenOption in from op in pawnGroupMaker.options
														where op.kind.canBeSapper
														select op)
				{
					if (pawnGenOption.Cost < num)
					{
						num = pawnGenOption.Cost;
					}
				}
			}
			return num;
		}

		public override bool CanUsePawnGenOption(PawnGenOption opt, List<PawnGenOption> chosenOpts)
		{
			return chosenOpts.Count != 0 || opt.kind.canBeSapper;
		}

		public override bool CanUsePawn(Pawn p, List<Pawn> otherPawns)
		{
			return (otherPawns.Count != 0 || SappersUtility.IsGoodSapper(p) || SappersUtility.IsGoodBackupSapper(p)) && (!p.kindDef.canBeSapper || !RimWorld.SappersUtility.HasBuildingDestroyerWeapon(p) || SappersUtility.IsGoodSapper(p));
		}

		private IEnumerable<PawnGroupMaker> PawnGenOptionsWithSappers(Faction faction, PawnGroupKindDef groupKind)
		{
			if (faction.def.pawnGroupMakers == null)
			{
				return Enumerable.Empty<PawnGroupMaker>();
			}
			return faction.def.pawnGroupMakers.Where(delegate (PawnGroupMaker gm)
			{
				if (gm.kindDef == groupKind && gm.options != null)
				{
					return gm.options.Any((PawnGenOption op) => op.kind.canBeSapper);
				}
				return false;
			});
		}

		protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
		{
			return new LordJob_AssaultColony(parms.faction, true, true, true, true, true);
		}
	}
}
