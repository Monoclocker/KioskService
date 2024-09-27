using Microsoft.AspNetCore.SignalR.Client;

namespace KioskExample
{
    internal class Program
    {
        static HubConnection connection;
        static async Task Main(string[] args)
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5238/kiosk")
                .Build();

            connection.On<string>("set_settings", (settings) =>
            {
                Console.WriteLine(settings);
            });

            connection.On("connection_error", () =>
            {
                Console.WriteLine("Не задан ID устройства");
            });

            await connection.StartAsync();


            Console.ReadKey();
        }
    }
}
