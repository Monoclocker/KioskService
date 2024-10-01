using KioskService.Core.DTO;
using KioskService.Core.Models;

namespace KioskService.WEB.HubInterfaces
{
    public interface IKioskHub
    {
        public Task KioskConnectionError(Response dto);
        public Task SendSettings(Request<Settings> dto);
        public Task ProceedRefund(Request<int> dto);
        public Task ResultsRequest(Request dto);

    }
}
