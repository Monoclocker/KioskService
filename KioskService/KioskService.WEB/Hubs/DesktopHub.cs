using KioskService.Core.DTO;
using KioskService.Core.Interfaces;
using KioskService.Core.Models;
using KioskService.WEB.HubInterfaces;
using KioskService.WEB.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace KioskService.WEB.Hubs
{
    public class DesktopHub : Hub<IDesktopHub>
    {
        IHubContext<KioskHub, IKioskHub> kioskHub;
        ILogger<DesktopHub> logger;
        IPaymentService paymentService;
        IResultsService resultsService;
        IConnectionStorage connectionStorage;

        public DesktopHub(
            IHubContext<KioskHub, IKioskHub> kioskHub,
            IConnectionStorage connectionStorage,
            IPaymentService paymentService,
            IResultsService resultsService,
            ILogger<DesktopHub> logger
        )
        {
            this.kioskHub = kioskHub;
            this.connectionStorage = connectionStorage;
            this.paymentService = paymentService;
            this.resultsService = resultsService;
            this.logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            Response<IEnumerable<string>> currentConnectedIDs = new Response<IEnumerable<string>>()
            {
                statusCode = 200,
                date = DateTime.Now,
                data = connectionStorage
            };

            await Clients.Caller.ConnectedToService(currentConnectedIDs);
        }
        public async Task GetPayment(Request<int> request)
        {
            Payment? payment = await paymentService.GetPayment(request.data);

            Response<Payment> response = new Response<Payment>() 
            {
                date = DateTime.UtcNow,
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

            await Clients.Caller.GetPayment(response);
        }
        public async Task ProceedRefund(Request<int> request)
        {
            await kioskHub.Clients
                .User(request.deviceId)
                .ProceedRefund(request);
        }
        public async Task ResultsRequest(Request request)
        {
            await kioskHub.Clients.User(request.deviceId)
                .ResultsRequest(request);
        }
        public async Task GetResults(Request<int> request)
        {
            Core.Models.Results? entity = await resultsService.GetResults(request.data);

            Response<Core.Models.Results> response = new Response<Core.Models.Results>()
            {
                date = DateTime.UtcNow,
            };

            if (entity is null)
            {
                response.statusCode = 404;
                response.message = "Обращение к несуществующей сверке";
            }
            else
            {
                response.statusCode = 200;
                response.data = entity;
            }

            await Clients.Caller.GetResults(response);
        }
        public async Task GetPreviousResults(Request<int> request)
        {
            int pageNum = request.data <= 0 ? 1 : request.data;

            PaginatedList<ResultsPreview> page 
                = await resultsService.GetPreviousResults(request.deviceId, pageNum);

            Response<PaginatedList<ResultsPreview>> response 
                = new Response<PaginatedList<ResultsPreview>>()
            {
                date = DateTime.UtcNow,
                deviceId = string.Empty,
                statusCode = 200,
                data = page,
            };

            await Clients.Caller.SavedResults(response);
        }
        public async Task GetPreviousPayment(Request<int> request)
        {
            int pageNum = request.data <= 0 ? 1 : request.data;

            PaginatedList<PaymentPreview> page
                = await paymentService.GetPreviousTransactions(request.deviceId, pageNum);

            Response<PaginatedList<PaymentPreview>> response
                = new Response<PaginatedList<PaymentPreview>>()
                {
                    statusCode = 200,
                    deviceId = string.Empty,
                    date = DateTime.UtcNow,
                    data = page
                };

            await Clients.Caller.SavedTransactions(response);
        }
        public async Task SendSettings(Request<object> request)
        {
            await kioskHub.Clients
                .User(request.deviceId)
                .SendSettings(request);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            logger.LogInformation($"Приложение {Context.UserIdentifier ?? "без идентификатора"} " +
                $"отключено");

            if (exception is not null)
            {
                logger.LogError($"Ошибка {exception.GetType()}: {exception.Message}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
