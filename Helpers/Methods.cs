using EFT.Communications;

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
    }
}
