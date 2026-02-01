namespace BlazorApp1.Models
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsVisible { get; set; } = true;
    }

    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Info
    }
}