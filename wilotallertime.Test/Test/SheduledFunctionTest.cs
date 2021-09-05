
using System;
using wilotallertime.Functions.Functions;
using wilotallertime.Test.Helpers;
using Xunit;

namespace wilotallertime.Test.Tests
{
    public class SheduledFunctionTest
    {


        [Fact]
        public void ScheduledFuction_Should_Log_Message()
        {
            // Arrange
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactoryConsolidated.CreateLogger(LoggerTypes.List);

            // Add
            ScheduledFunction.Run(null, mockTimes, mockConsolidated, logger);
            string message = logger.Logs[0];

            // Assert
            Assert.Contains($"New consolidation process, starts at: {DateTime.Now}", message);
        }
    }
}

