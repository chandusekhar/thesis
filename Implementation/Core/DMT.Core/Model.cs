﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMT.Core.Interfaces;

namespace DMT.Core
{
    public class Model : IModel
    {
        private List<INode> nodesList;
        private Dictionary<IId, INode> nodeDic;

        public ICollection<INode> Nodes
        {
            get { return nodesList; }
        }

        public Model()
            : this(new List<INode>())
        {

        }

        /// <summary>
        /// Instantiates a new Model object.
        /// </summary>
        /// <param name="nodes">Root nodes of the components. A new list will be instantiated with the specified element.</param>
        public Model(IEnumerable<INode> nodes)
        {
            this.nodesList = new List<INode>(nodes);
        }

        public IDictionary<IId, INode> GetNodeDictionary()
        {
            if (nodeDic == null)
            {
                nodeDic = this.nodesList.ToDictionary(n => n.Id);
            }

            return nodeDic;
        }

        public bool HasNode(INode node)
        {
            return GetNodeDictionary().ContainsKey(node.Id);
        }
    }
}
