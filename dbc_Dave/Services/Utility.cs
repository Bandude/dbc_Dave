
using Microsoft.JSInterop;

namespace dbc_Dave.Services
{
    public class Utility
    {
        private IJSRuntime _jsRuntime;

        public Utility(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task UpdateTextAreaHeight(string elementId)
        {
            await _jsRuntime.InvokeVoidAsync("UpdateTextAreaHeight", elementId);
        }

       
    }
}
