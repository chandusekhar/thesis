using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;
using DMT.Core.Test.Utils;
using Xunit;

namespace DMT.Partition.Test
{
    public class MaximalMatchingTest
    {
        [Fact]
        public void GettingTheMatchingShouldThrowExceptionIfGraphHasNotBeenProcessed()
        {
            Assert.Throws(typeof(InvalidOperationException), () => new MaximalMatching(new List<INode>()).Matching);
        }

        [Fact]
        public void AfterFindMatchingIsPopulated()
        {
            var mm = new MaximalMatching(new List<INode>());
            mm.Find();
            Assert.NotNull(mm.Matching);
        }

        [Fact]
        public void MatchingIsTheSameAsWhatFindReturns()
        {
            var mm = new MaximalMatching(GraphHelper.Circle4());
            var matching = mm.Find();

            Assert.Equal(matching, mm.Matching);
        }

        [Fact]
        public void EmptyInputListProducesEmptyMatching()
        {
            var mm = new MaximalMatching(new List<INode>());
            Assert.Empty(mm.Find());
        }

        [Fact]
        public void Circle4HasTwoEdgesInMatching()
        {
            var mm = new MaximalMatching(GraphHelper.Circle4());
            var matching = mm.Find();

            Assert.Equal(2, matching.Count());
            
        }
    }
}
