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
    [Export(typeof(IMatcherEntityFactory))]
    public class VirEntityFactory : CoreEntityFactory, IMatcherEntityFactory
    {
        private readonly static Dictionary<string, Type> nodeTypes = new Dictionary<string, Type>
        {
            { "Person", typeof(Person) },
            { "Group", typeof(Group) },
            { "Membership", typeof(Membership) },
            { "SemesterValuation", typeof(SemesterValuation) },
            { "CommunityScore", typeof(CommunityScore) },

        };

        [Import]
        private IEntityFactory baseFactory;

        public VirEntityFactory()
        {
            this.baseFactory = this;
        }

        public override INode CreateNode(string typeInfo)
        {
            Type t = nodeTypes[typeInfo];
            return (INode)Activator.CreateInstance(t, this.baseFactory);
        }

        public override IEdge CreateEdge(INode nodeA, INode nodeB, EdgeDirection direction)
        {
            return new VirEdge(nodeA, nodeB, direction, this.baseFactory);
        }

        public IPattern CreatePattern()
        {
            return new Pattern();
        }

        public IPatternNode CreatePatternNode()
        {
            return new PatternNode(this.baseFactory);
        }


        public IPatternNode CreatePatternNode(string name)
        {
            return new PatternNode(name, this.baseFactory);
        }
    }
}
