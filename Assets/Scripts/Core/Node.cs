using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Synapse.Core
{
    public class Node
    {
        private string _id;
        private string _label;
        [Range(1, 10)] private int _value;
        public string Id => _id;
        public string Label => _label;
        public int Value => _value;

        private List<Node> _connectedNodes = new List<Node>();
        public List<Node> ConnectedNodes => _connectedNodes;
        public bool IsConnected(Node node) => _connectedNodes.Contains(node);

        public Node(string id, string label, int value)
        {
            _id = id;
            _label = label;
            _value = value;
        }

        public void Traverse(ref List<Node> nodes)
        {
            if (nodes.Contains(this)) return;
            nodes.Add(this);
            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Traverse(ref nodes);
        }

        public static string GetEdgeId(Node a, Node b) => a.GetHashCode().ToString() + b.GetHashCode().ToString();
    }
}
