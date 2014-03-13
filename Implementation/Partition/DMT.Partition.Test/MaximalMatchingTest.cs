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

        [Fact]
        public void TreeGraphDoesNotGetStuckInALoop()
        {
            INode root = EntityHelper.CreateNode();
            INode leaf1 = EntityHelper.CreateNode();
            INode leaf2 = EntityHelper.CreateNode();

            root.ConnectTo(leaf1, EdgeDirection.Both);
            root.ConnectTo(leaf2, EdgeDirection.Both);

            var mm = new MaximalMatching(new[] { root, leaf1, leaf2 });
            var matching = mm.Find();

            Assert.Equal(1, matching.Count());
        }
    }
}
