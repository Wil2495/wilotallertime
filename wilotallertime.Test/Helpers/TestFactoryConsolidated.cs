using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using wilotallertime.Common.Models;
using wilotallertime.Functions.Entities;

namespace wilotallertime.Test.Helpers
{
    internal class TestFactoryConsolidated
    {
        public static ConsolidatedEntity GetConsolidatedEntity()
        {
            return new ConsolidatedEntity
            {
                Date = DateTime.Now,
                IdEmployee = 1,
                ETag = "*",
                MinutesWork = 5454,
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString()
            };
        }
        public static List<ConsolidatedEntity> GetConsolidatedsEntity()
        {
            return new List<ConsolidatedEntity>
            {
                new ConsolidatedEntity
                {
                    Date = DateTime.Now,
                    MinutesWork = 20,
                    IdEmployee = 1,
                    ETag = "*",
                    PartitionKey = "CONSOLIDATED",
                    RowKey = Guid.NewGuid().ToString()
                },
                new ConsolidatedEntity
                {
                    Date = DateTime.Now,
                    MinutesWork = 45,
                    IdEmployee = 2,
                    ETag = "*",
                    PartitionKey = "CONSOLIDATED",
                    RowKey = Guid.NewGuid().ToString()
                }
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{id}"
            };
        }


        public static DefaultHttpRequest CreateHttpRequest(Guid id, Consolidated consolidatedRequest)
        {
            string request = JsonConvert.SerializeObject(consolidatedRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Consolidated consolidatedRequest)
        {
            string request = JsonConvert.SerializeObject(consolidatedRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(List<Consolidated> consolidatedRequest)
        {
            string request = JsonConvert.SerializeObject(consolidatedRequest, Formatting.Indented);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Consolidated GetConsolidatedRequest()
        {
            return new Consolidated
            {
                Date = DateTime.Now,
                MinutesWork = 88787,
                IdEmployee = 9,
            };
        }

        public static List<Consolidated> GetConsolidatedsRequest()
        {
            return new List<Consolidated>
            {
                new Consolidated
                {
                    Date = DateTime.Now,
                    MinutesWork = 20,
                    IdEmployee = 1
                },
                new Consolidated
                {
                    Date = DateTime.UtcNow,
                    MinutesWork = 45,
                    IdEmployee = 2
                }
            };
        }



        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }
            return logger;
        }

    }
}

