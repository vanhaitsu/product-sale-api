namespace Services.Models.NotificationModels
{
    public class OneSignalNotificationModel
    {
        public string Heading {  get; set; }
        public string Content { get; set; }
        public string[] PlayerIds { get; set; }
    }
}
