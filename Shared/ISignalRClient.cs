namespace Shared
{
    using System.Threading.Tasks;

    public interface ISignalRClient
    {
        Task ReceiveMessage(string sender, string message);

        Task Acknowledge();

        Task NotifyUserAdded(string user);

        Task NotifyUserLeft(string user);
    }
}