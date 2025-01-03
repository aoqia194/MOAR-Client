using System.Reflection;
using EFT;
using HarmonyLib;
using MOAR.Helpers;
using SPT.Reflection.Patching;

namespace MOAR.Patches
{
    public class NotificationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(
                typeof(MetricsEventsClass),
                nameof(MetricsEventsClass.SetLocationLoaded)
            );
        }

        [PatchPrefix]
        static bool Prefix()
        {
            if (!Settings.ShowPresetOnRaidStart.Value)
                return false;

            Methods.DisplayMessage(
                "Current preset is " + Routers.GetAnnouncePresetName() + ", good luck."
            );

            return true;
        }
    }
}
