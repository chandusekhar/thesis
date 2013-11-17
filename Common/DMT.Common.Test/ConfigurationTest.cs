using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DMT.Common.Test
{
    public class ConfigurationTest
    {
        [Fact]
        public void EnvironmentSetToTest()
        {
            Assert.Equal(Environment.Test, Configuration.Current.Environment);
        }

        [Fact]
        public void DefaultEnvironmentOverriddenByEnvironmentVariable()
        {
            Configuration.OverrideCurrent(new Configuration());

            System.Environment.SetEnvironmentVariable(Configuration.EnvironmentKey, "production");
            Assert.Equal(Environment.Production, Configuration.Current.Environment);
            System.Environment.SetEnvironmentVariable(Configuration.EnvironmentKey, null);

            // clear config values
            Configuration.OverrideCurrent(new Configuration());
        }
    }
}
