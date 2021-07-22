using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace AltAI
{
    // Token: 0x02000004 RID: 4
    public static class CustomGenAI
    {
        // Token: 0x06000010 RID: 16 RVA: 0x000028AC File Offset: 0x00000AAC
        public static bool MachinesLike(Faction machineFaction, Pawn p)
        {
            return (p.Faction != null || !p.NonHumanlikeOrWildMan() || (p.HostFaction == machineFaction && !p.IsPrisoner)) && (!p.IsPrisoner || p.HostFaction != machineFaction) && (p.Faction == null || !p.Faction.HostileTo(machineFaction));
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002910 File Offset: 0x00000B10
        public static bool CanUseItemForWork(Pawn p, Thing item)
        {
            return !item.IsForbidden(p) && p.CanReserveAndReach(item, PathEndMode.ClosestTouch, p.NormalMaxDanger(), 1, -1, null, false);
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002948 File Offset: 0x00000B48
        public static bool CanBeArrestedBy(this Pawn pawn, Pawn arrester)
        {
            return pawn.RaceProps.Humanlike && (!pawn.InAggroMentalState || !pawn.HostileTo(arrester)) && !pawn.HostileTo(Faction.OfPlayer) && (!pawn.IsPrisonerOfColony || !pawn.Position.IsInPrisonCell(pawn.Map));
        }

        // Token: 0x06000013 RID: 19 RVA: 0x000029A8 File Offset: 0x00000BA8
        public static bool InDangerousCombat(Pawn pawn)
        {
            Region root = pawn.GetRegion(RegionType.Set_Passable);
            bool found = false;
            RegionTraverser.BreadthFirstTraverse(root, (Region r1, Region r2) => r2.Room == root.Room, (Region r) => r.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn).Any(delegate (Thing t)
            {
                Pawn pawn2 = t as Pawn;
                if (pawn2 != null && !pawn2.Downed && (float)(pawn.Position - pawn2.Position).LengthHorizontalSquared < 144f && pawn2.HostileTo(pawn.Faction))
                {
                    found = true;
                    return true;
                }
                return false;
            }), 9, RegionType.Set_Passable);
            return found;
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002A10 File Offset: 0x00000C10
        public static IntVec3 RandomRaidDest(IntVec3 raidSpawnLoc, Map map)
        {
            IEnumerable<Building> source = from b in map.listerBuildings.allBuildingsColonist
                                           where !b.def.building.ai_combatDangerous && !b.def.building.isInert
                                           select b;
            bool flag = source.Any<Building>();
            if (flag)
            {
                for (int i = 0; i < 500; i++)
                {
                    Building t = source.RandomElement<Building>();
                    IntVec3 intVec = t.RandomAdjacentCell8Way();
                    bool flag2 = intVec.Walkable(map) && map.reachability.CanReach(raidSpawnLoc, intVec, PathEndMode.OnCell, TraverseMode.PassAllDestroyableThings, Danger.Deadly);
                    if (flag2)
                    {
                        return intVec;
                    }
                }
            }
            Pawn pawn;
            bool flag3 = (from x in map.mapPawns.FreeColonistsSpawned
                          where map.reachability.CanReach(raidSpawnLoc, x, PathEndMode.OnCell, TraverseMode.PassAllDestroyableThings, Danger.Deadly)
                          select x).TryRandomElement(out pawn);
            IntVec3 result;
            if (flag3)
            {
                result = pawn.Position;
            }
            else
            {
                IntVec3 intVec2;
                bool flag4 = CellFinderLoose.TryGetRandomCellWith((IntVec3 x) => map.reachability.CanReach(raidSpawnLoc, x, PathEndMode.OnCell, TraverseMode.PassAllDestroyableThings, Danger.Deadly), map, 1000, out intVec2);
                if (flag4)
                {
                    result = intVec2;
                }
                else
                {
                    result = map.Center;
                }
            }
            return result;
        }

        // Token: 0x06000015 RID: 21 RVA: 0x00002B58 File Offset: 0x00000D58
        public static bool EnemyIsNear(Pawn p, float radius)
        {
            bool flag = !p.Spawned;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = p.Position.Fogged(p.Map);
                List<IAttackTarget> potentialTargetsFor = p.Map.attackTargetsCache.GetPotentialTargetsFor(p);
                for (int i = 0; i < potentialTargetsFor.Count; i++)
                {
                    IAttackTarget attackTarget = potentialTargetsFor[i];
                    bool flag3 = !attackTarget.ThreatDisabled(p);
                    if (flag3)
                    {
                        bool flag4 = flag2 || !attackTarget.Thing.Position.Fogged(attackTarget.Thing.Map);
                        if (flag4)
                        {
                            bool flag5 = p.Position.InHorDistOf(((Thing)attackTarget).Position, radius);
                            if (flag5)
                            {
                                return true;
                            }
                        }
                    }
                }
                result = false;
            }
            return result;
        }
    }
}
