using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace ZTACS.Client.Services
{
    public class FingerprintService
    {
        private readonly IJSRuntime _jsRuntime;

        public FingerprintService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<string> GetFingerprintAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("getFingerprint");
        }
    }
}
