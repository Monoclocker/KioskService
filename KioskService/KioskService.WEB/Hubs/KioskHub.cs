using KioskService.Core.DTO;
using KioskService.Core.Exceptions;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using KioskService.WEB.HubInterfaces;
using KioskService.WEB.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class KioskHub: Hub<IKioskHub>
    {
        IHubContext<DesktopHub, IDesktopHub> desktopHub;
        ILogger<KioskHub> logger;
        IPaymentService paymentService;
        IResultsService resultsService;
        IConnectionStorage connectionStorage;

        public KioskHub(
            IHubContext<DesktopHub, IDesktopHub> desktopHub,
            ILogger<KioskHub> logger,
            IPaymentService paymentService,
            IResultsService resultsService,
            IConnectionStorage connectionStorage
        ) 
        {
            this.desktopHub = desktopHub;
            this.logger = logger;
            this.paymentService = paymentService;
            this.resultsService = resultsService;
            this.connectionStorage = connectionStorage;
        }

        public override async Task OnConnectedAsync()
        {
            string deviceId = Context.UserIdentifier!;

            Response<string> response = new Response<string>()
            {
                statusCode = 200,
                data = deviceId,
                date = DateTime.UtcNow
            };

            await desktopHub.Clients.All
                .KioskConnected(response);

            await base.OnConnectedAsync();
        }
        public async Task SavePayment(Request<Payment> body)
        {
            int newPaymentId = await paymentService.SavePayment(body.data);

            Response<PaymentPreview> response = new Response<PaymentPreview>()
            {
                statusCode = 200,
                deviceId = body.deviceId,
                date = DateTime.UtcNow,
                data = new PaymentPreview()
                {
                    id = newPaymentId,
                    sum = body.data.sum,
                    localDate = body.data.localTime
                }
            };

            await desktopHub.Clients.All.SendSavePaymentResult(response);
        }
        public async Task RefundResult(Response<int> response) 
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

            await desktopHub.Clients.All
                .RefundResponse(response);
        }
        public async Task SetSettingsResult(Response response)
        {
            await desktopHub.Clients
                .All
                .SetSettingsResponse(response);
        }
        public async Task ResultsResponse(Response<Core.Models.Results> response)
        {
            Response<ResultsPreview> responseToDesktop = new Response<ResultsPreview>()
            {
                deviceId = response.deviceId,
                date = DateTime.UtcNow,
                statusCode = response.statusCode,
                stackTrace = response.stackTrace,
                errorType = response.errorType,
                message = response.message,
            };

            if (response.statusCode == 200)
            {
                int newId = await resultsService.SaveResults(response.data!);

                responseToDesktop.data = new ResultsPreview()
                {
                    id = newId,
                    localDate = response.date.ToLocalTime(),
                };
            }

            await desktopHub.Clients.All.NewResults(responseToDesktop);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string deviceId = Context.UserIdentifier!;

            connectionStorage.Delete(deviceId);

            Response<string> response = new Response<string>()
            {
                statusCode = 200,
                data = deviceId,
                date = DateTime.UtcNow
            };


            logger.LogInformation($"Киоск {deviceId} отключён");

            if (exception is not null)
            {
                response.statusCode = 500;
                response.stackTrace = exception.StackTrace;
                response.message = exception.Message;
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await desktopHub.Clients.All.KioskDisconnected(response);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
