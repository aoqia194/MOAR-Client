using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace MOAR.Patches
{
    internal class NotificationService : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        static bool Prefix()
        {
            var preset = Plugin.GetServerString();
            Plugin.DisplayMessage(preset);
            return true;
        }
    }
}
