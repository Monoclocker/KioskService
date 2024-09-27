using Microsoft.AspNetCore.SignalR.Client;

namespace DesktopExample
{
    internal class Program
    {
        static HubConnection connection;

        static async Task Main(string[] args)
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5238/desktop")
                .Build();

            connection.On<string>("kiosk_connected", (deviceId) =>
            {
                Console.WriteLine(deviceId);
            });

            connection.On<IEnumerable<string>>("connected_to_service", (ids) =>
            {
                foreach (var id in ids)
                {
                    Console.WriteLine(id);
                }
            });

            connection.On<string>("kiosk_error", (error) =>
            {
                Console.WriteLine(error);
            });


            await connection.StartAsync();

            Console.ReadKey();

        }
    }
}
