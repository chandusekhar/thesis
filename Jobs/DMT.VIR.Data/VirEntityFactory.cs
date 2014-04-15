using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Entities;
using DMT.Core.Interfaces;
using DMT.Matcher.Data.Interfaces;

namespace DMT.VIR.Data
{
    [Export(typeof(IEntityFactory))]
    public class VirEntityFactory : CoreEntityFactory
    {
        private readonly static Dictionary<string, Type> nodeTypes = new Dictionary<string, Type>
        {
            { "Person", typeof(Person) },
            { "Group", typeof(Group) },
            { "Membership", typeof(Membership) },
            { "SemesterValuation", typeof(SemesterValuation) },
            { "CommunityScore", typeof(CommunityScore) },

        };

        public VirEntityFactory()
        {
        }

        public override INode CreateNode(string typeInfo)
        {
            Type t = nodeTypes[typeInfo];
            return (INode)Activator.CreateInstance(t, this);
        }

        public override IEdge CreateEdge(INode nodeA, INode nodeB, EdgeDirection direction)
        {
            return new VirEdge(nodeA, nodeB, direction, this);
        }
    }
}
