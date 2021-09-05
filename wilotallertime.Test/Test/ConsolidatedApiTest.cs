using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using wilotallertime.Common.Models;
using wilotallertime.Functions.Functions;
using wilotallertime.Test.Helpers;
using Xunit;


namespace wilotallertime.Test.Test
{
    public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactoryConsolidated.CreateLogger();

        [Fact]
        public async void GetAllConsolidatedByDate_Should_Return_200()
        {
            //Arrenge
            MockCloudTableConsolidated mockConsolidate = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<Consolidated> consolidatedRequest = TestFactoryConsolidated.GetConsolidatedsRequest();
            DefaultHttpRequest request = TestFactoryConsolidated.CreateHttpRequest(consolidatedRequest);


            //Act
            IActionResult response = await ConsolidatedApi.GetAllConsolidatesByDate(request, mockConsolidate, "2000-05-07", logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        
    }
}
