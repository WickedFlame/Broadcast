﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Broadcast.Test
{
    [TestClass]
    public class RequestPublisherTests
    {
        [TestMethod]
        public void RequestHandlerTest()
        {
            var requestHandler = new RequestHandler();
            var publisher = new RequestPublisher<Request>(requestHandler);
            publisher.Handle(new Request(5));

            Assert.IsTrue(requestHandler.ID == 5);
        }

        [TestMethod]
        public async Task AsyncRequestHandlerTest()
        {
            var requestHandler = new RequestHandler();
            var publisher = new RequestPublisher<Request>(requestHandler);
            await publisher.HandleAsync(new Request(5));

            Assert.IsTrue(requestHandler.ID == 5);
        }

        [TestMethod]
        public void RequestHandlerWithResultTest()
        {
            var requestHandler = new ResultRequestHandler();
            var publisher = new RequestPublisher<ResultRequest, int>(requestHandler);
            var id = publisher.Handle(new ResultRequest(5));

            Assert.IsTrue(id == 5);
        }

        [TestMethod]
        public async Task AsyncRequestHandlerWithResultTest()
        {
            var requestHandler = new ResultRequestHandler();
            var publisher = new RequestPublisher<ResultRequest, int>(requestHandler);
            var id = await publisher.HandleAsync(new ResultRequest(5));

            Assert.IsTrue(id == 5);
        }
        

        class Request : IRequest
        {
            public Request(int id)
            {
                ID = id;
            }

            public int ID { get; set; }
        }

        class RequestHandler : IRequestHandler<Request>
        {
            public void Handle(Request request)
            {
                ID = request.ID;
            }

            public int ID { get; set; }
        }

        class ResultRequest : IRequest<int>
        {
            public ResultRequest(int id)
            {
                ID = id;
            }

            public int ID { get; set; }
        }

        class ResultRequestHandler : IRequestHandler<ResultRequest, int>
        {
            public int Handle(ResultRequest request)
            {
                return request.ID;
            }
        }
    }
}
