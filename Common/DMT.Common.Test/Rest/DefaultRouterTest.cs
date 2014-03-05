﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;
using DMT.Common.Rest.Router;
using Xunit;

namespace DMT.Common.Test.Rest
{
    public class DefaultRouterTest
    {
        const string url = "/test";
        DefaultRouter router = new DefaultRouter();

        [Fact]
        public void WhenNoRouteRegisteredReturnsNullForHandlers()
        {
            Assert.True(router.Get(HttpMethod.Get, url).Success);
        }

        [Fact]
        public void ReturnsNullHandlerIfMethodDoesNotMatch()
        {
            router.Register(HttpMethod.Get, url, new NopHandler());
            Assert.True(router.Get(HttpMethod.Post, url).Success);
        }

        [Fact]
        public void ReturnsHandlerWhenMethodAndRouteMatch()
        {
            router.Register(HttpMethod.Get, url, new NopHandler());
            Assert.True(router.Get(HttpMethod.Get, url).Success);
            Assert.IsType<NopHandler>(router.Get(HttpMethod.Get, url).Handler);
        }

        [Fact]
        public void RouteCanHaveParameters()
        {
            router.Register(HttpMethod.Get, "/test/{name}", new NopHandler());
            var route = router.Get(HttpMethod.Get, "/test/bela");
            Assert.True(route.Success);
            Assert.Equal("bela", route.RouteParams.Get("name"));
        }

        private class NopHandler : IRouteHandler
        {
            public void Handle(System.Net.HttpListenerRequest request, System.Net.HttpListenerResponse response)
            {

            }
        }
    }
}
