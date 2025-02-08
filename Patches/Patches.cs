using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using Comfort.Common;
using Comfort.Logs;
using EFT;
using EFT.Game.Spawning;
using HarmonyLib;
using MOAR.Helpers;
using Newtonsoft.Json;
using Sirenix.Serialization;
using SPT.Custom.CustomAI;
using SPT.Reflection.Patching;
using UnityEngine;

namespace MOAR.Patches
{
    public class BotZoneDumper : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocationScene), nameof(LocationScene.Awake));
        }

        [PatchPostfix]
        public static void Postfix(LocationScene __instance)
        {
            foreach (var zone in __instance.BotZones)
            {
                Logger.LogInfo($"BotZone name: {zone.NameZone} ID: {zone.Id}");
            }
        }
    }

    public class SpawnPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(SpawnSystemClass),
                "GInterface418.ValidateSpawnPosition"
            );
        }

        [PatchPrefix]
        static bool Prefix(ref bool __result)
        {
            __result = true;

            return false;
        }
    }

    public class SpawnPatch2 : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotZone), nameof(BotZone.smethod_0));
        }

        [PatchPrefix]
        static bool Prefix(ref bool __result)
        {
            __result = true;

            return false;
        }
    }

    public class SpawnPatch3 : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotZone), nameof(BotZone.IsValid));
        }

        [PatchPrefix]
        static bool Prefix(ref bool __result)
        {
            __result = true;

            return false;
        }
    }

    public class AddEnemyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotsGroup).GetMethod(
                nameof(BotsGroup.AddEnemy),
                BindingFlags.Public | BindingFlags.Instance
            );
        }

        [PatchPrefix]
        protected static bool PatchPrefix(
            BotsGroup __instance,
            IPlayer person,
            EBotEnemyCause cause
        )
        {
            if (__instance == null || person == null || !person.IsAI)
                return true;
            // // We only care about bot groups adding you as an enemy
            if (
                __instance.Side != person.Side
                || person.Side == EPlayerSide.Savage
                || __instance.Side == EPlayerSide.Savage
            )
            {
                return true;
            }

            // if (cause == EBotEnemyCause.initial)
            //     return true;
            // Get the ID's of all group members

            // List<BotOwner> groupMemberList = AiHelpers.GetAllMembers(__instance);

            // Check if the the bot group was created by this mod
            List<BotOwner> groupMemberList = SPT.Custom.CustomAI.AiHelpers.GetAllMembers(
                __instance
            );

            // IEnumerable<BotOwner> activatedBots = Singleton<IBotGame>
            //     .Instance
            //     .BotsController
            //     .Bots
            //     .BotOwners;

            // Plugin.LogSource.LogWarning("----++++----");
            // Vector3 temp = new();
            // List<BotOwner> filteredBots = groupMemberList.ApplyFilter(m =>
            // {
            //     if (temp.IsZero())
            //     {
            //         temp = m.Memory.ActivatedPos;
            //         return true;
            //     }
            //     return temp == m.Memory.ActivatedPos;
            // });

            // // int[] groupAreaId = groupMemberList.Select(m => m.AIData.PlaceInfo.AreaId).ToArray();
            // //
            // // string[] groupMemberEntryPoints = groupMemberList
            // //     .Select(m => m.Profile.AccountId)
            // //     .ToArray();

            // Plugin.LogSource.LogWarning("-------------");

            // //     return true;
            // string[] groupMemberGroupIds = groupMemberList
            //     .Select(m => m.Profile.Info.GroupId)
            //     .ToArray();

            // string[] groupMemberTeamIds = groupMemberList
            //     .Select(m => m.Profile.Info.TeamId)
            //     .ToArray();

            // string[] groupMemberAccountIds = groupMemberList.Select(m => m.AccountId).ToArray();

            // string[] groupMemberProfileIDs = groupMemberList.Select(m => m.Profile.Id).ToArray();

            // person.Profile.Stats.Eft.Victims.Any(v => groupMemberIDs.Contains(v.ProfileId)
            // {
            //     Plugin.LogSource.LogWarning(
            //         "Preventing BotsGroup::AddEnemy from running due to EBotEnemyCause.addBotAtGroup because the victim was in a bot group created by this mod"
            //     );
            //     return false;
            // }
            // __instance.CoverPointMaster.BotZone.Id;

            Plugin.LogSource.LogWarning(
                person.TeamId
                    + " > "
                    + person.GroupId
                    + " > "
                    + person.AccountId
                    + " > "
                    + person.ProfileId
                    + " > "
                    + cause
                    + " > "
                    + groupMemberList.ContainsPlayer(person)
                    + " > "
            // + person.AIData.Player.TeamId
            // + " > "
            // + string.Join(", ", groupTeamId)
            // + string.Join(", ", groupMemberTime)
            // + " groupMemberEntryPoints "
            // + groupMemberEntryPoints.Contains(person.Profile.Info.EntryPoint)
            // + " GroupId "
            // + groupMemberGroupIds.Contains(person.Profile.Info.GroupId)
            // + " TeamIds "
            // + groupMemberTeamIds.Contains(person.Profile.Info.TeamId)
            // + " AccountId "
            // + groupMemberAccountIds.Contains(person.Profile.AccountId)
            // + " ProfileID "
            // + groupMemberProfileIDs.Contains(person.Profile.Id)
            );

            if (cause == EBotEnemyCause.initial || groupMemberList.ContainsPlayer(person))
            {
                // if (__instance.IsEnemy(person))
                // {
                //     __instance.RemoveEnemy(person);
                // }

                // if (!__instance.IsAlly(person))
                // {
                //     __instance.Allies.Add(person);
                // }

                Plugin.LogSource.LogWarning("Make bot peaceful against" + person.Profile.Id);

                return false;
            }

            // Plugin.LogSource.LogInfo(
            //     "You are now an enemy of "
            //         + string.Join(", ", groupMemberIDs)
            //         + " due to reason: "
            //         + cause.ToString()
            // );

            return true;
        }
    }

    public class BotSpawnerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BotSpawner), nameof(BotSpawner.GetGroupAndSetEnemies));
        }

        [PatchPostfix]
        static void Postfix(ref BotsGroup __result, BotOwner bot, BotZone zone)
        {
            if (bot.Side == EPlayerSide.Bear || bot.Side == EPlayerSide.Usec)
            {
                FieldInfo botSpawnerAllPlayersFieldInfo = AccessTools.Field(
                    typeof(BotSpawner),
                    "_allPlayers"
                );

                FieldInfo botSpawnerDeadBodiesControllerFieldInfo = AccessTools.Field(
                    typeof(BotSpawner),
                    "_deadBodiesController"
                );

                BotSpawner botSpawner =
                    Singleton<GameWorld>.Instance.gameObject.GetComponent<BotSpawner>();

                DeadBodiesController deadBodiesController = (DeadBodiesController)
                    botSpawnerDeadBodiesControllerFieldInfo.GetValue(botSpawner);

                List<Player> _allPlayers =
                    (List<Player>)botSpawnerAllPlayersFieldInfo.GetValue(botSpawner);
                EPlayerSide side = bot.Profile.Info.Side;
                List<BotOwner> list = [.. botSpawner.method_4(bot)];

                BotsGroup group =
                    new(
                        zone,
                        botSpawner.BotGame,
                        bot,
                        list,
                        deadBodiesController,
                        _allPlayers,
                        true
                    );
                group.TargetMembersCount = 5;
                botSpawner.Groups.Add(zone, side, group, true);
                group.Lock();

                botSpawner.method_5(bot);

                __result = group;
            }
        }
    }

    public class SniperPatch : ModulePatch
    {
        private static double Sq(double n)
        {
            return n * n;
        }

        private static double Pt(double a, double b)
        {
            return Math.Sqrt(Sq(a) + Sq(b));
        }

        public static double GetDistance(
            double x,
            double y,
            double z,
            double mX,
            double mY,
            double mZ
        )
        {
            x = Math.Abs(x - mX);
            y = Math.Abs(y - mY);
            z = Math.Abs(z - mZ);

            return Pt(Pt(x, z), y);
        }

        public static double GetVectorDistance(Vector3 v1, Vector3 v2)
        {
            return GetDistance(v1.x, v1.y, v1.z, v2.x, v2.y, v2.z);
        }

        public static BotZone FindFarthestZone(List<BotZone> botZones, Vector3 referencePoint)
        {
            if (botZones == null || botZones.Count == 0)
            {
                throw new ArgumentException("The botZones list cannot be null or empty.");
            }

            // Order the zones by distance in descending order
            var orderedZones = botZones
                .OrderBy(botZone => GetVectorDistance(botZone.CenterOfSpawnPoints, referencePoint))
                .ToList();

            // Get the last half of the list
            int halfCount = orderedZones.Count / 2;
            var lastHalfZones = orderedZones.Skip(halfCount).ToList();

            // Select a random zone from the last half
            System.Random random = new();
            int randomIndex = random.Next(lastHalfZones.Count);

            return lastHalfZones[randomIndex];
        }

        static BotZone GetNearestZone(List<BotZone> zones, string name, bool isPmc)
        {
            System.Random random = new();

            if (!isPmc)
            {
                foreach (BotZone zone in zones)
                {
                    if (zone.NameZone == name)
                    {
                        return zone;
                    }
                }
            }
            // Generate a random index between 0 and the count of the list (exclusive)
            int randomIndex = random.Next(zones.Count);

            // Return the element at the random index
            return zones[randomIndex];
        }

        static PatrolWay GetRandomPatrol(PatrolWay[] patrol)
        {
            // Create a random number generator
            System.Random random = new();

            // Generate a random index between 0 and the count of the list (exclusive)
            int randomIndex = random.Next(patrol.Length);

            // Return the element at the random index
            return patrol[randomIndex];
        }

        static bool IsNameInBotzones(List<BotZone> zones, string name)
        {
            // Create a random number generator
            System.Random random = new();

            foreach (BotZone zone in zones)
            {
                if (zone.NameZone == name)
                {
                    return true;
                }
            }
            // Generate a random index between 0 and the count of the list (exclusive)

            return false;
        }

        public static string GetBotZoneNameById(SpawnPointParams[] spawnPoints, string id)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.Id != null && spawnPoint.BotZoneName != null && spawnPoint.Id == id)
                {
                    return spawnPoint.BotZoneName;
                }
            }
            return ""; // Return null if the ID is not found
        }

        public static void SetBotZoneName(SpawnPointParams[] spawnPoints, string id, string newName)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].Id == id)
                {
                    spawnPoints[i].BotZoneName = newName;
                }
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(SpawnPointManagerClass),
                nameof(SpawnPointManagerClass.smethod_1)
            );
        }

        [PatchPostfix]
        static void Postfix(ref SpawnPointMarker[] __result, SpawnPointParams[] parameters)
        {
            Plugin.LogSource.LogInfo("Attempting spawnzone updates");
            if (
                __result == null
                || parameters == null
                || __result.Length == 0
                || parameters.Length == 0
            )
            {
                Plugin.LogSource.LogInfo("MOAR: We hit the error case, skipping implementation");
                return;
            }

            List<BotZone> snipeZones = new List<BotZone>();
            List<BotZone> botZones = new List<BotZone>();

            foreach (SpawnPointMarker zone in __result)
            {
                if (zone == null)
                    continue;
                var botzoneExists = !zone.BotZone.IsNullOrDestroyed();
                if (botzoneExists)
                {
                    if (zone.BotZone.SnipeZone)
                    {
                        snipeZones.Add(zone.BotZone);
                    }
                    else
                    {
                        botZones.Add(zone.BotZone);
                    }
                }
            }
            // Plugin.LogSource.LogInfo("1");
            if (botZones.Count == 0 || snipeZones.Count == 0)
                return;

            List<BotZone> nonSniperZones = botZones.ApplyFilter(zone => !zone.SnipeZone);
            // Plugin.LogSource.LogInfo("2");
            for (int index = 0; index < __result.Length; index++)
            {
                SpawnPointMarker zone = __result[index];
                if (
                    zone == null
                    || zone.SpawnPoint.Categories == ESpawnCategoryMask.None
                    || zone.SpawnPoint.Categories.ContainPlayerCategory()
                )
                    continue;
                // Plugin.LogSource.LogInfo("3");
                var botzoneDoesNotExist = zone.BotZone.IsNullOrDestroyed();
                // Plugin.LogSource.LogInfo("4");
                string botZoneName = GetBotZoneNameById(parameters, zone.Id);
                // bool isPmc = botZoneName.Contains("pmc");

                if (botzoneDoesNotExist)
                {
                    if (
                        IsNameInBotzones(snipeZones, botZoneName)
                        || botZoneName.ToLower().Contains("custom_snipe")
                    )
                    {
                        if (botZoneName.ToLower().Contains("custom_snipe"))
                        {
                            SetBotZoneName(parameters, zone.Id, "");
                            botZoneName = "";
                        }

                        BotZone RandomBotZone = GetNearestZone(snipeZones, botZoneName, false);

                        int newVal =
                            RandomBotZone.MaxPersons > 0 ? RandomBotZone.MaxPersons + 1 : 5;

                        AccessTools
                            .Field(typeof(BotZone), "_maxPersons")
                            .SetValue(RandomBotZone, newVal);

                        // for (int i = 0; i < RandomBotZone.PatrolWays.Length; i++)
                        // {
                        //     if (RandomBotZone.PatrolWays[i].PatrolType == PatrolType.patrolling)
                        //     {
                        //         RandomBotZone.PatrolWays[i].PatrolType = PatrolType.patrolling;
                        //     }
                        // }

                        // RandomBotZone.SpawnPointMarkers =
                        // [
                        //     .. RandomBotZone.SpawnPointMarkers,
                        //     zone,
                        // ];

                        zone.BotZone = RandomBotZone;
                    }
                    else
                    {
                        BotZone RandomBotZone = GetNearestZone(nonSniperZones, botZoneName, false);

                        // if (RandomBotZone.name != botZoneName)
                        //     SetBotZoneName(parameters, zone.Id, RandomBotZone.name);

                        // if (!RandomBotZone.SpawnPointMarkers.Contains(zone))
                        // {
                        //     RandomBotZone.SpawnPointMarkers =
                        //     [
                        //         .. RandomBotZone.SpawnPointMarkers,
                        //         zone,
                        //     ];
                        // }

                        if (RandomBotZone.MaxPersons != -1)
                        {
                            AccessTools
                                .Field(typeof(BotZone), "_maxPersons")
                                .SetValue(RandomBotZone, -1);
                        }

                        zone.BotZone = RandomBotZone;
                    }
                    // else
                    // {
                    //     if (zone.BotZone.name != botZoneName)
                    //         SetBotZoneName(parameters, zone.Id, zone.BotZone.name);
                }
            }
            Plugin.LogSource.LogInfo("Spawnszone updates complete");
        }
    }

    public class NotificationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            if (!Settings.ShowPresetOnRaidStart.Value)
                return false;

            List<string> messages =
            [
                ", good luck!",
                ", may the bots ever be in your favour.",
                ", you're probably screwed.",
                ", may your raids be bug-free.",
                ", enjoy the dumpster fire.",
                ", hope you brought snacks.",
                ", good luck, seriously.",
                ", prepare to be crushed.",
                ", you’re about to get wrecked.",
                ", enjoy the show.",
                ", good luck, you'll need it.",
                ", enjoy the carnage.",
                ", try not to rage-quit.",
                ", don’t say I didn’t warn you.",
                ", best of luck surviving that.",
                ", it’s going to be a long day for you.",
                ", be water my friend.",
                ", let the feelings of dread pass over you.",
                ", black a leg!",
                ", it's about to get ugly. Enjoy.",
            ];

            System.Random randNum = new();

            int aRandomPos = randNum.Next(messages.Count);

            string currName = messages[aRandomPos];

            Methods.DisplayMessage(
                "Current preset is " + Routers.GetAnnouncePresetName() + currName,
                EFT.Communications.ENotificationIconType.EntryPoint
            );

            return true;
        }
    }
}
