using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Core.Partition;
using DMT.Module.Common.Service;
using DMT.Partition.Module.Remote;
using Xunit;

namespace DMT.Partition.Module.Test
{
    public class MatcherRegistryTest
    {
        PartitionEntityFactory factory = new PartitionEntityFactory();
        MatcherRegistry registry = new MatcherRegistry();

        [Fact]
        public void GetPartitionByIdReturnsNullForEmptyRegistry()
        {
            Assert.Null(registry.GetByPartitionId(factory.CreateId()));
        }

        [Fact]
        public void GetPartitionByIdReturnsAPartition()
        {
            var partition = factory.CreatePartition();
            registry.AddMatcher(new MatcherInfo { Partition = partition });

            var result = registry.GetByPartitionId(partition.Id);
            Assert.NotNull(result);
            Assert.Equal(partition, result.Partition);
        }

        [Fact]
        public void GetPartitionByIdReturnsNullOnNoMatch()
        {
            var partition = factory.CreatePartition();
            registry.AddMatcher(new MatcherInfo { Partition = partition });

            var result = registry.GetByPartitionId(factory.CreateId());
            Assert.Null(result);
        }
    }
}
