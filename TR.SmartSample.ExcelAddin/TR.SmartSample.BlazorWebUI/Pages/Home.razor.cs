using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace TR.SmartSample.BlazorWebUI.Pages
{
    public partial class Home
    {
        [Inject, AllowNull]
        private IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference JSModule { get; set; } = default!;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("Hit OnAfterRenderAsync in Home.razor.cs in Console!");
                JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/Home.razor.js");

                // To avoid cached JS files, use following versioned JS call
                //JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Pages/Home.razor.js?v={DateTime.Now.Ticks}");
            }
        }

        private string DynamicValue { get; set; } = string.Empty;

        private async Task HelloButton() =>
            await JSModule.InvokeVoidAsync("helloButton", DynamicValue);

        [JSInvokable]
        public static string SayHelloHome(string name)
        {
            return $"Hello Home, {name} from Home!";
        }
    }
}
