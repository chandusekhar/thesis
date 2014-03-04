using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;
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
            Assert.Null(router.Get(HttpMethod.Get, url));
        }

        [Fact]
        public void ReturnsNullHandlerIfMethodDoesNotMatch()
        {
            router.Register(HttpMethod.Get, url, new NopHandler());
            Assert.Null(router.Get(HttpMethod.Post, url));
        }

        [Fact]
        public void ReturnsHandlerWhenMethodAndRouteMatch()
        {
            router.Register(HttpMethod.Get, url, new NopHandler());
            Assert.NotNull(router.Get(HttpMethod.Get, url));
            Assert.IsType<NopHandler>(router.Get(HttpMethod.Get, url));
        }

        private class NopHandler : IRouteHandler
        {
            public void Handle(System.Net.HttpListenerRequest request, System.Net.HttpListenerResponse response)
            {

            }
        }
    }
}
