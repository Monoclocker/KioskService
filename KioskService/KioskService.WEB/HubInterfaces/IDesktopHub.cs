using KioskService.Core.DTO;
using KioskService.Core.Models;

namespace KioskService.WEB.HubInterfaces
{
    public interface IDesktopHub
    {
        public Task Error(Response response);
        public Task KioskError(Response response);
        public Task KioskConnected(Response<string> dto);
        public Task ConnectedToService(Response<IEnumerable<string>> dto);
        public Task KioskDisconnected(Response<string> dto);
        public Task SendSavePaymentResult(Response<PaymentPreview> dto);
        public Task GetPayment(Response<Payment> dto);
        public Task NewPayment(Response<PaymentPreview> dto);
        public Task RefundResponse(Response<int> dto);
        public Task SetSettingsResponse(Response dto);
        public Task GetResults(Response<Core.Models.Results> dto);
        public Task NewResults(Response<ResultsPreview> dto);
        public Task SavedResults(Response<PaginatedList<ResultsPreview>> dto);
        public Task SavedTransactions(Response<PaginatedList<PaymentPreview>> dto);
    }
}
