using Microsoft.AspNetCore.SignalR;

namespace CVManagement.Web.Hubs
{
    public interface INotificationHub
    {
        Task SendNotification( string message);
    }
    public sealed class NotificationHub : Hub<INotificationHub>
    {
       
    }
}
