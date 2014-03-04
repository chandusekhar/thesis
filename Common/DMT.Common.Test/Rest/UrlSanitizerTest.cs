using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Common.Rest;
using Xunit;

namespace DMT.Common.Test.Rest
{
    public class UrlSanitizerTest
    {
        UrlSanitizer us = new UrlSanitizer();

        [Fact]
        public void CutsOfTrailingSlashWhenPresent()
        {
            var url1 = "/test/";
            var url2 = "/test";

            Assert.Equal("/test", us.Sanitize(url1));
            Assert.Equal("/test", us.Sanitize(url2));
        }

        [Fact]
        public void CutsOfQueryString()
        {
            var url1 = "/test/?hello=world";
            var url2 = "/test?hello=world";

            Assert.Equal("/test", us.Sanitize(url1));
            Assert.Equal("/test", us.Sanitize(url2));
        }
    }
}
