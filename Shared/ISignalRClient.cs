namespace Shared
{
    using System.Threading.Tasks;

    public interface ISignalRClient
    {
        Task ReceiveMessage(string sender, string message);

        Task Acknowledge();
    }
}