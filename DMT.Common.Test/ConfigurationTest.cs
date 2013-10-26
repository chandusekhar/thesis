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
        public ConfigurationTest()
        {
            ClearInitializedFlag();
        }

        [Fact]
        public void EnvironmentSetToTest()
        {
            Assert.Equal(Environment.TEST, Configuration.GetEnvironment());
        }

        [Fact]
        public void DefaultEnvironmentOverriddenByEnvironmentVariable()
        {
            System.Environment.SetEnvironmentVariable(Configuration.EnvironmentKey, "production");
            Assert.Equal(Environment.PRODUCTION, Configuration.GetEnvironment());
            System.Environment.SetEnvironmentVariable(Configuration.EnvironmentKey, null);
        }

        private void ClearInitializedFlag()
        {
            var field = typeof(Configuration).GetField("isEnvSet", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, false);
        }
    }
}
