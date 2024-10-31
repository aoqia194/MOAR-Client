using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using EFT.Communications;
using MOAR.Patches;
using Newtonsoft.Json;

using System.IO;
using System.Linq;
using System.Reflection;
using EFT;
using UnityEngine;
using SPT.Common.Utils;
using System;
using BepInEx.Logging;

namespace MOAR
{
    [BepInPlugin("MOAR.settings", "MOAR", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> ShowPresetOnRaidStart;
        public static string[] PresetList = [];
        public static ConfigEntry<string> currentPreset;

        public static string GetServerString()
        {
            var req = SPT.Common.Http.RequestHandler.GetJson("/moar/currentPreset");

            return req.ToString(); // no need to parse bare strings
        }

        public static string[] GetPresetsList()
        {
            return JsonConvert.DeserializeObject<Preset[]>(SPT.Common.Http.RequestHandler.GetJson("/moar/getPresets")).Select(item => item.name).ToArray();
        }

        public static string SetPresets(string preset)
        {
            var req = SPT.Common.Http.RequestHandler.PostJson($"/moar/setPreset", preset);
            return req.ToString(); // no need to parse bare strings
        }
        public static void DisplayMessage(string message)
        {
            NotificationManagerClass.DisplayMessageNotification(message, ENotificationDurationType.Long);
        }

        private void Start()
        {

            ShowPresetOnRaidStart = Config.Bind(
               "1. Main Settings",
               "Preset Banner On/Off",
               true,
               new ConfigDescription("Enable/Disable preset announce on raid start",
               null,
               new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));


            PresetList = GetPresetsList();

            if (PresetList.Length > 0)
            {
                currentPreset = Config.Bind(
                    "1. Main Settings",
                    "Moar Preset",
                    "Random",
                    new ConfigDescription("Preset to be used, default pulls a random weighted preset from the config.",
                    new AcceptableValueList<string>(PresetList),
                    new ConfigurationManagerAttributes { DefaultValue = "Random", IsAdvanced = false, ShowRangeAsPercent = false, Order = 2 }));

                currentPreset.SettingChanged += OnPresetChange;
            }

            new NotificationPatch().Enable();
            new OnMenuLoad().Enable();
        }

        private static void OnPresetChange(object sender, EventArgs e)
        {
            DisplayMessage(currentPreset.Value);
        }

    }

    public class Preset
    {
        public string name;
        public string value;
    }



}
