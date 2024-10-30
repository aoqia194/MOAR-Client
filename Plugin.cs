using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOAR.ExamplePatches;
using ChatShared;
using EFT.Communications;
using EFT;

namespace MOAR
{
    [BepInPlugin("MOAR.UniqueGUID", "MOAR", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;
        public static string GetServerString()
        {
            var req = SPT.Common.Http.RequestHandler.GetJson("/moar/currentPreset");
            return req.ToString(); // no need to parse bare strings
        }
        public static void PostMessage(string message)
        {
            NotificationManagerClass.DisplayMessageNotification(message, ENotificationDurationType.Long);
        }
        // BaseUnityPlugin inherits MonoBehaviour, so you can use base unity functions like Awake() and Update()
        private void Awake()
        {
            new MoarPatch().Enable();
        }
    }
}
