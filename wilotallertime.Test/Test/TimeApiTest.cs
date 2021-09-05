using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using wilotallertime.Common.Models;
using wilotallertime.Functions.Entities;
using wilotallertime.Functions.Functions;
using wilotallertime.Test.Helpers;
using Xunit;

namespace wilotallertime.Test.Tests
{
    public class TimeApiTest
    {
        private readonly ILogger logger = TestFactoryTime.CreateLogger();

        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactoryTime.GetTimeRequest();
            DefaultHttpRequest request = TestFactoryTime.CreateHttpRequest(timeRequest);
            //Act
            IActionResult response = await TimeApi.CreateTime(request, mockTimes, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }



        [Fact]
        public async void GetAllTimeById_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactoryTime.GetTimeRequest();
            Guid id = Guid.NewGuid();

            TableOperation find = TableOperation.Retrieve<TimeEntity>("TIME", id.ToString());
            TableResult action = await mockTimes.ExecuteAsync(find);
            TimeEntity timeEntity = (TimeEntity)action.Result;

            DefaultHttpRequest request = TestFactoryTime.CreateHttpRequest(id, timeRequest);
            //Act
            IActionResult response =  TimeApi.GetTime(request, timeEntity, id.ToString(),logger);

           //Assert
           OkObjectResult result = (OkObjectResult)response;
           Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
       }
       


        [Fact]
        public async void GetAllTimes_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<Time> times = TestFactoryTime.GetTimesRequest();
            DefaultHttpRequest request = TestFactoryTime.CreateHttpRequest(times);
            //Act
            IActionResult response = await TimeApi.GetAllTimes(request, mockTimes, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }




        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactoryTime.GetTimeRequest();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactoryTime.CreateHttpRequest(timeId, timeRequest);
            //Act
            IActionResult response = await TimeApi.UpdateTime(request, mockTimes, timeId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }





        [Fact]
        public async void DeleteTime_Should_Return_200()
        {
            //Arrenge
            MockCloudTableTimes mockTimes = new MockCloudTableTimes(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Time timeRequest = TestFactoryTime.GetTimeRequest();
            Guid timeId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactoryTime.CreateHttpRequest(timeId, timeRequest);
            TimeEntity timeEntity = TestFactoryTime.GetTimeEntity();
            //Act
            IActionResult response = await TimeApi.DeleteTime(request, timeEntity, mockTimes, timeId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}

