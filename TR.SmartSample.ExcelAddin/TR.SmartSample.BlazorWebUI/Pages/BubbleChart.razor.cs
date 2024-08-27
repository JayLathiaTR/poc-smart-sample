using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using TR.SmartSample.BlazorWebUI.Models;

namespace TR.SmartSample.BlazorWebUI.Pages
{
    [SupportedOSPlatform("browser")]
    public partial class BubbleChart
    {
        [Inject, AllowNull]
        private IJSRuntime JSRuntime { get; set; }
        private IJSObjectReference? JSModule { get; set; }
        private bool IsLoading { get; set; } = false;
        private string? ErrorMessage { get; set; }

        [Inject, AllowNull]
        private HttpClient HttpClient { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./Pages/BubbleChart.razor.js");

                //// To avoid cached JS files, use following versioned JS call
                //JSModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", $"./Pages/BubbleChart.razor.js?v={DateTime.Now.Ticks}");
            }
        }

        /// <summary>
        /// Function to create the starter table as source for the bubble chart.
        /// </summary>
        private async Task CreateTable()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                //https://smart-sample.free.beeceptor.com/getinventories
                //mocked-datas/inventories.json
                var data = await HttpClient.GetFromJsonAsync<List<InventoryDetails>>("https://smart-sample.free.beeceptor.com/getinventories");
                await JSModule.InvokeVoidAsync("createTable", data);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error occurred while fetching data. Details: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Function to create the actual bubble chart.
        /// </summary>
        private async Task CreateBubbleChart() =>
            await JSModule.InvokeVoidAsync("createBubbleChart");

        [JSImport("createTable", "BubbleChart")]
        internal static partial Task CreateImportedTable();

        [JSImport("createBubbleChart", "BubbleChart")]
        internal static partial Task RunCreateChart();

        [JSInvokable]
        public static async Task CreateBubbles()
        {
            await JSHost.ImportAsync("BubbleChart", "../Pages/BubbleChart.razor.js");
            await CreateImportedTable();
            await RunCreateChart();
        }

        [JSInvokable]
        public static string SayHelloBubble(string name)
        {
            return $"Hello Bubble, {name} from BubbleChart!";
        }
    }
}
