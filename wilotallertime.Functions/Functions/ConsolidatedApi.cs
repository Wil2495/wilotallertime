using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wilotallertime.Common.Responses;
using wilotallertime.Functions.Entities;

namespace wilotallertime.Functions.Functions
{
    public static class ConsolidatedApi
    {
        [FunctionName(nameof(ConsolidateProcess))]
        public static async Task<IActionResult> ConsolidateProcess(
              [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated")] HttpRequest req,
              [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
              [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
              ILogger log)
        {
            log.LogInformation($"New consolidation process, starts at: {DateTime.Now}");

            //------------------variable------------------------//
            string message = string.Empty;
            int contUpdate = 0, contNew = 0;
            //------------------variable------------------------//

            //----------------------Get data of table Times---------------------------------------------------------//
            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);
            TableQuery<TimeEntity> queryTime = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> allTimes = await timeTable.ExecuteQuerySegmentedAsync(queryTime, null);
            List<TimeEntity> toOrderTimes = allTimes.OrderBy(x => x.IdEmployee).ThenBy(x => x.Date).ToList();
            //----------------------Get data of table Times---------------------------------------------------------//

            //--------------------------Validation process------------------------------------//
            if (toOrderTimes.Count > 1)
            {
                for (int i = 0; i < toOrderTimes.Count;)
                {
                    if (toOrderTimes[i].IdEmployee == toOrderTimes[i + 1].IdEmployee)
                    {
                        //-------------------- Calcule MinutesWork and TimeConsolidated ------------------------//
                        TimeSpan workTime = toOrderTimes[i + 1].Date - toOrderTimes[i].Date;
                        DateTime timeConsolidated = new DateTime(toOrderTimes[i].Date.Year, toOrderTimes[i].Date.Month, toOrderTimes[i].Date.Day);
                        //-------------------- Calcule MinutesWork and TimeConsolidated ------------------------//

                        //-------------------------------------------------- Get info of table Consolidated ------------------------------//
                        filter = TableQuery.GenerateFilterConditionForInt("IdEmployee", QueryComparisons.Equal, toOrderTimes[i].IdEmployee);
                        string filterDate = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.Equal, timeConsolidated);
                        string combinedFilter = TableQuery.CombineFilters(filter, TableOperators.And, filterDate);

                        TableQuery<ConsolidatedEntity> queryConsolidated = new TableQuery<ConsolidatedEntity>().Where(combinedFilter);
                        TableQuerySegment<ConsolidatedEntity> allConsolidated = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsolidated, null);
                        List<ConsolidatedEntity> toOrderConsolidated = allConsolidated.ToList();
                        //-------------------------------------------------- Get info of table Consolidated ------------------------------//

                        //-------------------------------------------------- Validate data to insert or update ------------------------------//
                        if (toOrderConsolidated.Count == 0)
                        {

                            ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                            {
                                ETag = "*",
                                PartitionKey = "CONSOLIDATED",
                                RowKey = Guid.NewGuid().ToString(),
                                IdEmployee = toOrderTimes[i].IdEmployee,
                                Date = timeConsolidated,
                                MinutesWork = (int)workTime.TotalMinutes
                            };
                            TableOperation addOperation = TableOperation.Insert(consolidatedEntity);
                            await consolidatedTable.ExecuteAsync(addOperation);
                            contNew++;
                        }
                        else
                        {
                            foreach (ConsolidatedEntity items in allConsolidated)
                            {
                                items.Date = timeConsolidated;
                                items.MinutesWork += (int)workTime.TotalMinutes;
                                TableOperation addOperation = TableOperation.Replace(items);
                                await consolidatedTable.ExecuteAsync(addOperation);
                            }
                            contUpdate++;
                        }
                        //-------------------------------------------------- Validate data to insert or update ------------------------------//
                    }
                    //------------------------ Change consolidation status ----------------------------//
                    toOrderTimes[i].Consolidated = true;
                    TableOperation addOperation2 = TableOperation.Replace(toOrderTimes[i]);
                    await timeTable.ExecuteAsync(addOperation2);
                    i++;

                    //------------ validates if the next item is the last----------------------//
                    if (i + 1 == toOrderTimes.Count)
                    {
                        if (toOrderTimes[i].IdEmployee == toOrderTimes[i - 1].IdEmployee)
                        {
                            toOrderTimes[i].Consolidated = true;
                            TableOperation addOperation3 = TableOperation.Replace(toOrderTimes[i]);
                            await timeTable.ExecuteAsync(addOperation3);

                        }
                        i++;
                    }
                    //------------ validates if the next item is the last----------------------//

                    //------------------------ Change consolidation status ----------------------------//
                }
            }
            //--------------------------Validation process------------------------------------//

            message = $"STATUS: Times addeds: {contNew} , Times updateds: {contUpdate}";

            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,

            });
        }


        [FunctionName(nameof(GetAllConsolidatesByDate))]
        public static async Task<IActionResult> GetAllConsolidatesByDate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated/{date}")] HttpRequest req,
        [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
        string date,
        ILogger log)
        {
            log.LogInformation($"Get  consolidates by date: {date} completed.");
            string[] valueDate = date.Split("-");

            DateTime timeConsolidated = new DateTime(int.Parse(valueDate[0]), int.Parse(valueDate[1]), int.Parse(valueDate[2]));

            //-------------------------------------------------- Get info of table Consolidated ------------------------------//
            string filterDate = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.Equal, timeConsolidated);
            TableQuery<ConsolidatedEntity> queryConsolidated = new TableQuery<ConsolidatedEntity>().Where(filterDate);
            TableQuerySegment<ConsolidatedEntity> allConsolidated = await consolidatedTable.ExecuteQuerySegmentedAsync(queryConsolidated, null);
            List<ConsolidatedEntity> toOrderConsolidated = allConsolidated.OrderBy(x => x.IdEmployee).ThenBy(x => x.Date).ToList();
            //-------------------------------------------------- Get info of table Consolidated ------------------------------//
            string message = "Retrieved all consolidated";
            log.LogInformation(message);
            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = toOrderConsolidated
            });
        }
    }
}
