using Microsoft.AspNetCore.SignalR;

namespace MyMvcAuthProject.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNewBookNotification(string bookTitle, string authorName)
        {
            await Clients.All.SendAsync("ReceiveNotification", bookTitle, authorName);
        }
    }
}
