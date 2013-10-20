using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DMT.Core.Test.Utils
{
    static class AssertEx
    {
        public static async void ThrowsAsync(Type expectedException, Func<Task> testCode)
        {
            Type actualException = null;
            try
            {
                await testCode();
            }
            catch (Exception ex)
            {
                actualException = ex.GetType();
            }

            Assert.Equal(expectedException, actualException);
        }
    }
}
