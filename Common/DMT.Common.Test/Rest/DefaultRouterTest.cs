using System;
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
            Assert.False(router.Get(HttpMethod.Get, url).Success);
        }

        [Fact]
        public void ReturnsNullHandlerIfMethodDoesNotMatch()
        {
            router.Register(HttpMethod.Get, url, new NopHandler());
            Assert.False(router.Get(HttpMethod.Post, url).Success);
            Assert.Null(router.Get(HttpMethod.Post, url).Handler);
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

        [Fact]
        public void RouteParamIsOnlyAddedOnce()
        {
            router.Register(HttpMethod.Get, "/test/{name}/ready", new NopHandler());
            var route = router.Get(HttpMethod.Get, "/test/bela/ready");
            Assert.Equal("bela", route.RouteParams.Get("name"));
        }

        private class NopHandler : IRouteHandler
        {
            public void Handle(Request request, Response response)
            {
                throw new NotImplementedException();
            }
        }
    }
}
