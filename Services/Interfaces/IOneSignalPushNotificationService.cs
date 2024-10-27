
using Services.Models.NotificationModels;

namespace Services.Interfaces
{

    public interface IOneSignalPushNotificationService
    {
        Task<bool> SendNotificationAsync(OneSignalNotificationModel oneSignalNotificationModel);
    }

}
