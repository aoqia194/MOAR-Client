using System.Threading.Tasks;
using Comfort.Common;
using EFT;
using EFT.Communications;
using SPT.Reflection.Utils;
using UnityEngine;

namespace MOAR.Helpers
{
    public class Methods
    {
        public static void DisplayMessage(string message)
        {
            NotificationManagerClass.DisplayMessageNotification(
                message,
                ENotificationDurationType.Long,
                ENotificationIconType.EntryPoint
            );
        }

        public static async void RefreshLocationInfo()
        {
            await PatchConstants.BackEndSession.GetLevelSettings();
            // await PatchConstants.BackEndSession.GetWeatherAndTime();
        }
    }
}
