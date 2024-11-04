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
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            if (!Settings.ShowPresetOnRaidStart.Value)
                return false;

            Methods.DisplayMessage(
                "Current preset is " + Routers.GetCurrentPresetName() + ", good luck."
            );

            return true;
        }
    }
}
