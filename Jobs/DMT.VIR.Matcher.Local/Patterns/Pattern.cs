using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DMT.Common.Extensions;
using DMT.Core.Interfaces;
using DMT.Core.Interfaces.Serialization;
using DMT.Matcher.Data.Interfaces;
using DMT.VIR.Data;

namespace DMT.VIR.Matcher.Local.Patterns
{
    public class Pattern : IPattern
    {
        private Dictionary<string, PatternNode> patternNodes;

        public bool IsMatched
        {
            // TODO: cache true value
            get { return this.patternNodes.Values.All(pn => pn.IsMatched); }
        }

        public IId CurrentNode { get; set; }

        public string CurrentPatternNodeName { get; set; }

        public Pattern()
        {
            this.patternNodes = new Dictionary<string, PatternNode>();
        }

        public HashSet<INode> GetMatchedNodes()
        {
            return new HashSet<INode>(this.patternNodes.Values.Where(pn => pn.IsMatched), VirNode.EqualityComparer());
        }

        IEnumerable<INode> IPattern.GetMatchedNodes()
        {
            return this.GetMatchedNodes();
        }

        /// <summary>
        /// Add nodes to the pattern.
        /// </summary>
        /// <param name="nodes"></param>
        public void AddNodes(params PatternNode[] nodes)
        {
            foreach (var item in nodes)
            {
                this.patternNodes.Add(item.Name, item);
            }
        }

        /// <summary>
        /// Clears the matched nodes.
        /// </summary>
        public void Reset()
        {
            patternNodes.Values.ForEach(pn => pn.Reset());
        }

        /// <summary>
        /// Gets a pattern node that matches the given name.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <returns>the pattern node</returns>
        /// <exception cref="InvalidOperationException">there is more than one element OR no element</exception>
        public PatternNode GetNodeByName(string name)
        {
            return this.patternNodes[name];
        }

        /// <summary>
        /// Gets the matched node of a pattern node that matches the
        /// given name.
        /// </summary>
        /// <param name="name">name of the pattern node</param>
        /// <returns>the matched node or null if the pattern node does not have one</returns>
        /// <exception cref="InvalidOperationException">there is more than one element OR no element</exception>
        public INode GetMatchedNodeByName(string name)
        {
            return GetNodeByName(name).MatchedNode;
        }

        /// <summary>
        /// Sets the matched node for a pattern node.
        /// </summary>
        /// <param name="name">name of the pattern node</param>
        /// <param name="match">matched node to set</param>
        public void SetMatchedNodeForPatternNode(string name, INode match)
        {
            GetNodeByName(name).MatchedNode = match;
        }

        /// <summary>
        /// Determines whether the node with the given name has been matched or not.
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <returns>true only if the node is matched</returns>
        /// <exception cref="InvalidOperationException">there is more than one node with given name OR no node with the given name</exception>
        public bool HasMatchedNodeFor(string name)
        {
            var node = GetNodeByName(name);
            return node.IsMatched;
        }

        public Pattern Copy()
        {
            var p = new Pattern();
            p.patternNodes = patternNodes.Values.Select(pn => pn.Copy()).ToDictionary(pn => pn.Name);

            return p;
        }

        public void Merge(Pattern match)
        {
            foreach (var matchItem in match.patternNodes)
            {
                if (matchItem.Value.IsMatched && !patternNodes[matchItem.Key].IsMatched)
                {
                    var pn = patternNodes[matchItem.Key];
                    pn.Merge(matchItem.Value);
                }
            }
        }

        #region ISerializable

        public void Serialize(XmlWriter writer)
        {
            writer.WriteElementString("CurrentPatternNodeName", this.CurrentPatternNodeName);
            writer.WriteStartElement("CurrentNode");
            this.CurrentNode.Serialize(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("PatternNodes");
            foreach (var patternNode in this.patternNodes.Values)
            {
                writer.WriteStartElement("PatternNode");
                patternNode.Serialize(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            // pattern nodes remote edges
            // serializing remote edges with the node itself will cause collision 
            writer.WriteStartElement("RemoteEdges");
            foreach (var edge in this.patternNodes.Values.SelectMany(pn => pn.RemoteEdges).Distinct())
            {
                writer.WriteStartElement("RemoteEdge");
                edge.Serialize(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void Deserialize(XmlReader reader, IContext context)
        {
            if (reader.Name != "CurrentPatternNodeName") { reader.ReadToFollowing("CurrentPatternNodeName"); }

            this.CurrentPatternNodeName = reader.ReadElementContentAsString();

            this.CurrentNode = context.EntityFactory.CreateId();
            this.CurrentNode.Deserialize(reader, context);

            if (reader.Name != "PatternNodes") { reader.ReadToFollowing("PatternNodes"); }
            var subreader = reader.ReadSubtree();
            Dictionary<string, PatternNode> nodes = new Dictionary<string, PatternNode>();
            while (subreader.ReadToFollowing("PatternNode"))
            {
                var pn = new PatternNode(context.EntityFactory);
                pn.Deserialize(subreader.ReadSubtree(), context);
                nodes.Add(pn.Name, pn);
            }
            this.patternNodes = nodes;

            List<IMatchEdge> remoteEdges = new List<IMatchEdge>();
            if (reader.Name != "RemoteEdges") { reader.ReadToFollowing("RemoteEdges"); }
            subreader = reader.ReadSubtree();
            while (subreader.ReadToFollowing("RemoteEdge"))
            {
                IMatchEdge e = (IMatchEdge)context.EntityFactory.CreateEdge();
                e.Deserialize(subreader.ReadSubtree(), context);
                remoteEdges.Add(e);
            }

            foreach (var pn in this.patternNodes.Values)
            {
                if (pn.IsMatched)
                {
                    pn.RemoteEdges = remoteEdges.Where(e =>
                    {
                        return e.EndA.Id.Equals(pn.MatchedNode.Id)
                            || e.EndB.Id.Equals(pn.MatchedNode.Id);
                    });
                }
            }
        }

        #endregion
    }
}
