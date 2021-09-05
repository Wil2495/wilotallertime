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
    public class TestFactoryTime
    {


        public static TimeEntity GetTimeEntity()
        {
            return new TimeEntity
            {
                Consolidated = false,
                Date = DateTime.UtcNow,
                IdEmployee = 78,
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                Type = 0
            };
        }
        public static List<TimeEntity> GetTimesEntity()
        {
            return new List<TimeEntity>
            {
                new TimeEntity
                {
                    Consolidated = false,
                    Date = DateTime.UtcNow,
                    IdEmployee = 50,
                    ETag = "*",
                    PartitionKey = "TIME",
                    RowKey = Guid.NewGuid().ToString(),
                    Type = 0
                },
                new TimeEntity
                {
                    Consolidated = false,
                    Date = DateTime.UtcNow,
                    IdEmployee = 50,
                    ETag = "*",
                    PartitionKey = "TIME",
                    RowKey = Guid.NewGuid().ToString(),
                    Type = 1
                }
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id, Time timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Time timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(List<Time> timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest, Formatting.Indented);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }





        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }


        public static Time GetTimeRequest()
        {
            return new Time
            {
                Consolidated = false,
                Date = DateTime.UtcNow,
                IdEmployee = 5,
                Type = 5
            };
        }

        public static List<Time> GetTimesRequest()
        {
            return new List<Time>
            {
                new Time {
                    Consolidated = false,
                    Date = DateTime.UtcNow,
                    IdEmployee = 9,
                    Type = 0
                },
                new Time {
                    Consolidated = false,
                    Date = DateTime.UtcNow,
                    IdEmployee = 9 ,
                    Type =1
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
