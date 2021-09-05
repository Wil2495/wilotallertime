using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using wilotallertime.Common.Models;
using wilotallertime.Common.Responses;
using wilotallertime.Functions.Entities;

namespace wilotallertime.Functions.Functions
{
    public static class TimeApi
    {
        [FunctionName(nameof(CreateTime))]
        public static async Task<IActionResult> CreateTime(
       [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "time")] HttpRequest req,
       [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
         ILogger log)
        {
            log.LogInformation("Recieved a new time.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Time time = JsonConvert.DeserializeObject<Time>(requestBody);

            ;

            if (ValidateJsonTime(time) != null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = ValidateJsonTime(time)
                });
            }

            TimeEntity timeEntity = new TimeEntity
            {
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = (int)time.IdEmployee,
                Date = (DateTime)time.Date,
                Type = (int)time.Type,
                Consolidated = false,
            };

            TableOperation addOperation = TableOperation.Insert(timeEntity);
            await timeTable.ExecuteAsync(addOperation);

            string message = "New time stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeEntity

            });
        }

        private static string ValidateJsonTime(Time time)
        {
            if (time?.IdEmployee == null)
            {
                return "The request must have an IdEmployee";
            }
            if (time?.Date == null)
            {
                return "The request must have a Date.";
            }
            if (time?.Type == null)
            {
                return "The request must have a Type.";
            }
            return null;
        }
    }
}
