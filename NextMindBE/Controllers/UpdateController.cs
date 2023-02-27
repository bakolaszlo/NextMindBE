using Microsoft.AspNetCore.Mvc;
using NextMindBE.Data;
using NuGet.Protocol;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NextMindBE.Controllers
{
    [ApiController]
    public class UpdateController : ControllerBase
    {

        [HttpGet]
        [Route("/api/updates")]
        public async Task<IActionResult> GetUpdates(CancellationToken cancellationToken)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");

            while (!cancellationToken.IsCancellationRequested)
            {
                var data = TriggerData.Data; // get the data to send to the client
                var eventString = $"data: {data}\n\n";
                //TriggerData.Data = null;
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes(eventString));
                await Response.Body.FlushAsync();
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // wait for 5 seconds before sending the next event
            }

            return new EmptyResult();
        }
    }
}
