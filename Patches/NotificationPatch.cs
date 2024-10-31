using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

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
            if (!Plugin.ShowPresetOnRaidStart.Value) return false;
            var preset = Plugin.GetServerString();
            Plugin.DisplayMessage(preset);
            return true;
        }
    }
}
