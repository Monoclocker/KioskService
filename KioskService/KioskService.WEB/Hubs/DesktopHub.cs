using KioskService.Core.DTO;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Services;
using KioskService.WEB.Utils;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class DesktopHub : Hub
    {
        DataSender dataSender;
        ILogger<DesktopHub> logger;
        IPaymentService paymentService;
        IConnectionStorage connectionStorage;
        DatabaseContext context;

        public DesktopHub(
            DataSender dataSender,
            IConnectionStorage connectionStorage,
            IPaymentService paymentService,
            ILogger<DesktopHub> logger,
            DatabaseContext context
        )
        {
            this.dataSender = dataSender;
            this.connectionStorage = connectionStorage;
            this.paymentService = paymentService;
            this.logger = logger;
            this.context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync(DesktopEventsNames.ConnectedToServiceEvent, connectionStorage);
            await base.OnConnectedAsync();
        }

        public async Task GetPayment(Request<Guid> request)
        {
            Payment? payment = await paymentService.GetPayment(request.data);

            Response<Payment> response = new Response<Payment>() 
            {
                date = DateTime.Now,
                deviceId = request.deviceId,
            };

            if (payment == null) 
            {
                response.statusCode = 404;
            }
            else
            {
                response.statusCode = 200;
                response.data = payment;
            }

            await Clients.Caller.SendAsync(DesktopEventsNames.GetPaymentEvent, response);
        }

        public async Task ProceedRefund(Request<Guid> request)
        {
            await dataSender.SendRefundMessageToKiosk(request);   
        }

        public async Task SendSetings(Request<Settings> request)
        {
            await dataSender.SendSettingsToKiosk(request);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            logger.LogInformation($"Приложение {Context.ConnectionId} отключено");

            if (exception is not null)
            {
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
