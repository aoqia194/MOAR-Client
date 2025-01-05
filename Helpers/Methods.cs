using System.Drawing;
using System.Threading.Tasks;
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.PrefabSettings;
using SPT.Reflection.Utils;
using UnityEngine;
using UnityEngine.Playables;

namespace MOAR.Helpers
{
    public class Methods
    {
        public static void DisplayMessage(
            string message,
            ENotificationIconType notificationType = ENotificationIconType.Quest
        )
        {
            var currentMessage = new GClass2269(
                message,
                ENotificationDurationType.Long,
                notificationType
            );

            NotificationManagerClass.DisplayNotification(currentMessage);
        }

        public static async void RefreshLocationInfo()
        {
            await PatchConstants.BackEndSession.GetLevelSettings();
            // await PatchConstants.BackEndSession.GetWeatherAndTime();
        }
    }
}
