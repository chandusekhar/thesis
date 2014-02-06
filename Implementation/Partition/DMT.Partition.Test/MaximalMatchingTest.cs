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
            var mm = new MaximalMatching(Circle4());
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
            var mm = new MaximalMatching(Circle4());
            var matching = mm.Find();

            Assert.Equal(2, matching.Count());
            
        }

        private static List<INode> Circle4()
        {
            INode n1 = EntityHelper.CreateNode();
            INode n2 = EntityHelper.CreateNode();
            INode n3 = EntityHelper.CreateNode();
            INode n4 = EntityHelper.CreateNode();

            n1.ConnectTo(n2, EdgeDirection.Both);
            n2.ConnectTo(n3, EdgeDirection.Both);
            n3.ConnectTo(n4, EdgeDirection.Both);
            n4.ConnectTo(n1, EdgeDirection.Both);

            List<INode> nodes = new List<INode> { n1, n2, n3, n4 };
            return nodes;
        }
    }
}
