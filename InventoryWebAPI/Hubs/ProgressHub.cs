using Microsoft.AspNetCore.SignalR;

namespace InventoryWebAPI.Hubs
{
    public class ProgressHub : Hub
    {
        public async Task SendProgress(string connectionId, int progress)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveProgress", progress);
        }
    }
}