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

    public class SniperPatch : ModulePatch
    {
        static BotZone GetRandomBotZone(List<BotZone> zones, string name)
        {
            // Create a random number generator
            System.Random random = new();

            foreach (BotZone zone in zones)
            {
                if (zone.NameZone == name)
                {
                    return zone;
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
            Plugin.LogSource.LogInfo("Start");
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
                if (botzoneDoesNotExist)
                {
                    string botZoneName = GetBotZoneNameById(parameters, zone.Id);
                    // Plugin.LogSource.LogInfo(
                    //     "No Botzone" + zone.name + "-" + zone.Sides + "-" + botZoneName
                    // );

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

                        BotZone RandomBotZone = GetRandomBotZone(snipeZones, botZoneName);

                        int newVal =
                            RandomBotZone.MaxPersons > 0 ? RandomBotZone.MaxPersons + 1 : 5;

                        AccessTools
                            .Field(typeof(BotZone), "_maxPersons")
                            .SetValue(RandomBotZone, newVal);

                        for (int i = 0; i < RandomBotZone.PatrolWays.Length; i++)
                        {
                            if (RandomBotZone.PatrolWays[i].PatrolType == PatrolType.patrolling)
                            {
                                RandomBotZone.PatrolWays[i].PatrolType = PatrolType.reserved;
                            }
                        }

                        RandomBotZone.SpawnPointMarkers =
                        [
                            .. RandomBotZone.SpawnPointMarkers,
                            zone,
                        ];

                        // Plugin.LogSource.LogInfo(
                        //     RandomBotZone.NameZone
                        //         + "-"
                        //         + RandomBotZone.SpawnPointMarkers.Count
                        //         + " - "
                        //         + RandomBotZone.MaxPersons
                        // );

                        zone.BotZone = RandomBotZone;
                    }
                    else
                    {
                        BotZone RandomBotZone = GetRandomBotZone(botZones, botZoneName);
                        RandomBotZone.SpawnPointMarkers =
                        [
                            .. RandomBotZone.SpawnPointMarkers,
                            zone,
                        ];

                        // PatrolWay RandomPatrolZone = GetRandomPatrol(RandomBotZone.PatrolWays);
                        // RandomBotZone.PatrolWays = [.. RandomBotZone.PatrolWays, RandomPatrolZone.];

                        // int newVal =
                        //     RandomBotZone.MaxPersons > 0 ? RandomBotZone.MaxPersons + 1 : 10;
                        if (RandomBotZone.MaxPersons != -1)
                        {
                            AccessTools
                                .Field(typeof(BotZone), "_maxPersons")
                                .SetValue(RandomBotZone, -1);
                        }

                        // Plugin.LogSource.LogInfo(
                        //     RandomBotZone.SpawnPointMarkers.Count + " - " + RandomBotZone.MaxPersons
                        // );

                        zone.BotZone = RandomBotZone;
                    }
                }
            }

            // foreach (SpawnPointMarker zone in __result)
            // {
            //     var botzoneExists = !zone.BotZone.IsNullOrDestroyed();
            //     string botZoneName = GetBotZoneNameById(parameters, zone.Id);
            //     if (botzoneExists)
            //     {
            //         Plugin.LogSource.LogInfo(
            //             botZoneName + " has an assigned Botzone " + zone.BotZone.NameZone
            //         );
            //     }
            //     else
            //     {
            //         Plugin.LogSource.LogInfo(botZoneName + " is STILL missing a botzone ");
            //     }
            // }
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
