using Comfort.Common;
using EFT;
using EFT.Communications;
using SPT.Reflection.Utils;

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
            if (PatchConstants.BackEndSession != null)
                await PatchConstants.BackEndSession.GetLevelSettings();
            // await PatchConstants.BackEndSession.GetWeatherAndTime();
        }

        public static AddSpawnRequest GetPlayersCoordinatesAndLevel()
        {
            var position = Singleton<GameWorld>.Instance.MainPlayer.Position;
            var location = Singleton<GameWorld>.Instance.MainPlayer.Location;

            return new AddSpawnRequest
            {
                map = location,
                position = new Ixyz
                {
                    x = position.x,
                    y = position.y,
                    z = position.z,
                },
            };
        }
    }
}
