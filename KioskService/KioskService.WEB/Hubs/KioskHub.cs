using KioskService.Core.DTO;
using KioskService.Core.Exceptions;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using KioskService.Persistance.Database;
using KioskService.WEB.Interfaces;
using KioskService.WEB.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace KioskService.WEB.Hubs
{
    public class KioskHub: Hub
    {
        DataSender dataSender;
        ILogger<KioskHub> logger;
        IPaymentService paymentService;
        IConnectionStorage connectionStorage;
        DatabaseContext context;

        public KioskHub(
            DataSender dataSender,
            ILogger<KioskHub> logger,
            IPaymentService paymentService,
            IConnectionStorage connectionStorage,
            DatabaseContext context
        ) 
        {
            this.dataSender = dataSender;
            this.logger = logger;
            this.paymentService = paymentService;
            this.connectionStorage = connectionStorage;
            this.context = context;
        }

        public override async Task OnConnectedAsync()
        {
            string deviceId = Context.UserIdentifier!;

            await dataSender.NotifyDesktopAboutKioskConnect(deviceId);

            await base.OnConnectedAsync();
        }

        public async Task SavePayment(Request<Payment> body)
        {
            Guid newPaymentId = await paymentService.SavePayment(body.data);

            Response<Guid> response = new Response<Guid>()
            {
                statusCode = 200,
                deviceId = body.deviceId,
                date = DateTime.UtcNow,
                data = newPaymentId
            };

            string responseJson = JsonSerializer.Serialize(response);

            logger.LogInformation($"Запрос выполнен, тело ответа: {responseJson}");

            await dataSender.SendPaymentSaveResultToDesktop(response);
        }

        public async Task RefundResult(Response<Guid> response) 
        {
            if (response.statusCode == 200)
            {
                try
                {
                    await paymentService.ProceedRefund(response.data);
                }
                catch (InvalidPaymentTranscation)
                {
                    response.statusCode = 400;
                    response.message = "Ошибка при отмене транзакции на сервере";
                }
            }

            await dataSender.SendRefundResultToDesktop(response);
        }

        public async Task SetSettingsResult(Response response)
        {
            await dataSender.SendSetSettingsResultToDesktop(response);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string deviceId = Context.UserIdentifier!;

            connectionStorage.Delete(deviceId);

            await dataSender.NotifyDesktopAboutKioskDisconnect(deviceId);

            logger.LogInformation($"Киоск {deviceId} отключён");

            if (exception is not null)
            {
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
