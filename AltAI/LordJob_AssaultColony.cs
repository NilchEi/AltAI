﻿using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace AltAI
{
	// Token: 0x02000753 RID: 1875
	public class LordJob_AssaultColony : LordJob
	{
		public static int sapperSmartChance;

		public override bool GuiltyOnDowned
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060030BD RID: 12477 RVA: 0x0010E53E File Offset: 0x0010C73E
		public LordJob_AssaultColony()
		{
		}

		// Token: 0x060030BE RID: 12478 RVA: 0x0010E55C File Offset: 0x0010C75C
		public LordJob_AssaultColony(SpawnedPawnParams parms)
		{
			this.assaulterFaction = parms.spawnerThing.Faction;
			this.canKidnap = false;
			this.canTimeoutOrFlee = false;
			this.canSteal = false;
		}

		// Token: 0x060030BF RID: 12479 RVA: 0x0010E5AC File Offset: 0x0010C7AC
		public LordJob_AssaultColony(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = true, bool sappers = false, bool useAvoidGridSmart = false, bool canSteal = true)
		{
			this.assaulterFaction = assaulterFaction;
			this.canKidnap = canKidnap;
			this.canTimeoutOrFlee = canTimeoutOrFlee;
			this.sappers = sappers;
			this.useAvoidGridSmart = useAvoidGridSmart;
			this.canSteal = canSteal;
		}

		// Token: 0x060030C0 RID: 12480 RVA: 0x0010E604 File Offset: 0x0010C804
		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil lordToil = null;
			if (this.sappers)
			{
				lordToil = new LordToil_AssaultColonySappers();
				if (this.useAvoidGridSmart)
				{
					int num = Rand.Range(1, 100);
					bool smartcheck = num <= sapperSmartChance;
					if (smartcheck)
					{
						lordToil.useAvoidGrid = true;
					}
				}
				stateGraph.AddToil(lordToil);
				Transition transition = new Transition(lordToil, lordToil, true, true);
				transition.AddTrigger(new Trigger_PawnLost(PawnLostCondition.Undefined, null));
				stateGraph.AddTransition(transition, false);
			}
			LordToil lordToil2 = new LordToil_AssaultColony(false);
			if (this.useAvoidGridSmart)
			{
				lordToil2.useAvoidGrid = true;
			}
			stateGraph.AddToil(lordToil2);
			LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, false, true);
			lordToil_ExitMap.useAvoidGrid = true;
			stateGraph.AddToil(lordToil_ExitMap);
			if (this.sappers)
			{
				Transition transition2 = new Transition(lordToil, lordToil2, false, true);
				transition2.AddTrigger(new Trigger_NoFightingSappers());
				stateGraph.AddTransition(transition2, false);
			}
			if (this.assaulterFaction.def.humanlikeFaction)
			{
				if (this.canTimeoutOrFlee)
				{
					Transition transition3 = new Transition(lordToil2, lordToil_ExitMap, false, true);
					if (lordToil != null)
					{
						transition3.AddSource(lordToil);
					}
					transition3.AddTrigger(new Trigger_TicksPassed(this.sappers ? LordJob_AssaultColony.SapTimeBeforeGiveUp.RandomInRange : LordJob_AssaultColony.AssaultTimeBeforeGiveUp.RandomInRange));
					transition3.AddPreAction(new TransitionAction_Message("MessageRaidersGivenUpLeaving".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
					stateGraph.AddTransition(transition3, false);
					Transition transition4 = new Transition(lordToil2, lordToil_ExitMap, false, true);
					if (lordToil != null)
					{
						transition4.AddSource(lordToil);
					}
					FloatRange floatRange = new FloatRange(0.25f, 0.35f);
					float randomInRange = floatRange.RandomInRange;
					transition4.AddTrigger(new Trigger_FractionColonyDamageTaken(randomInRange, 900f));
					transition4.AddPreAction(new TransitionAction_Message("MessageRaidersSatisfiedLeaving".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
					stateGraph.AddTransition(transition4, false);
				}
				if (this.canKidnap)
				{
					LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Kidnap().CreateGraph()).StartingToil;
					Transition transition5 = new Transition(lordToil2, startingToil, false, true);
					if (lordToil != null)
					{
						transition5.AddSource(lordToil);
					}
					transition5.AddPreAction(new TransitionAction_Message("MessageRaidersKidnapping".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
					transition5.AddTrigger(new Trigger_KidnapVictimPresent());
					stateGraph.AddTransition(transition5, false);
				}
				if (this.canSteal)
				{
					LordToil startingToil2 = stateGraph.AttachSubgraph(new LordJob_Steal().CreateGraph()).StartingToil;
					Transition transition6 = new Transition(lordToil2, startingToil2, false, true);
					if (lordToil != null)
					{
						transition6.AddSource(lordToil);
					}
					transition6.AddPreAction(new TransitionAction_Message("MessageRaidersStealing".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
					transition6.AddTrigger(new Trigger_HighValueThingsAround());
					stateGraph.AddTransition(transition6, false);
				}
			}
			Transition transition7 = new Transition(lordToil2, lordToil_ExitMap, false, true);
			if (lordToil != null)
			{
				transition7.AddSource(lordToil);
			}
			transition7.AddTrigger(new Trigger_BecameNonHostileToPlayer());
			transition7.AddPreAction(new TransitionAction_Message("MessageRaidersLeaving".Translate(this.assaulterFaction.def.pawnsPlural.CapitalizeFirst(), this.assaulterFaction.Name), null, 1f));
			stateGraph.AddTransition(transition7, false);
			return stateGraph;
		}

		// Token: 0x060030C1 RID: 12481 RVA: 0x0010E9EC File Offset: 0x0010CBEC
		public override void ExposeData()
		{
			Scribe_References.Look<Faction>(ref this.assaulterFaction, "assaulterFaction", false);
			Scribe_Values.Look<bool>(ref this.canKidnap, "canKidnap", true, false);
			Scribe_Values.Look<bool>(ref this.canTimeoutOrFlee, "canTimeoutOrFlee", true, false);
			Scribe_Values.Look<bool>(ref this.sappers, "sappers", false, false);
			Scribe_Values.Look<bool>(ref this.useAvoidGridSmart, "useAvoidGridSmart", false, false);
			Scribe_Values.Look<bool>(ref this.canSteal, "canSteal", true, false);
		}

		// Token: 0x04001AA5 RID: 6821
		private Faction assaulterFaction;

		// Token: 0x04001AA6 RID: 6822
		private bool canKidnap = true;

		// Token: 0x04001AA7 RID: 6823
		private bool canTimeoutOrFlee = true;

		// Token: 0x04001AA8 RID: 6824
		private bool sappers;

		// Token: 0x04001AA9 RID: 6825
		private bool useAvoidGridSmart;

		// Token: 0x04001AAA RID: 6826
		private bool canSteal = true;

		// Token: 0x04001AAB RID: 6827
		private static readonly IntRange AssaultTimeBeforeGiveUp = new IntRange(26000, 38000);

		// Token: 0x04001AAC RID: 6828
		private static readonly IntRange SapTimeBeforeGiveUp = new IntRange(33000, 38000);
	}
}