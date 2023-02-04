using System;
using System.Collections.Generic;
using UnityEngine;

namespace Synapse.Core
{
    public class Node
    {
        public int Id = -1;
        private string _label;
        [Range(1, 10)] private int _value;
        public string Label => _label;
        public int Value => _value;
        
        private List<Node> _connectedNodes = new List<Node>();
        private List<Node> _nodesInRange = new List<Node>();

        public event Action<int> OnIdChanged;

        public bool HasConnections => _connectedNodes.Count > 0;
        public bool IsDirectlyConnected(Node node) => _connectedNodes.Contains(node);
        public bool IsInRange(Node node) => _nodesInRange.Contains(node);
        

        public Node(int id, string label, int value, List<Node> InRange)
        {
            Id = id;
            _label = label;
            _value = value;
            _nodesInRange = InRange;
        }

        public void SetId(int id)
        {
            Id = id;
            OnIdChanged?.Invoke(id);
        }

        public void Add(Node node) => _connectedNodes.Add(node);
        public void Remove(Node node) => _connectedNodes.Remove(node);

        public void Traverse(ref List<Node> nodes)
        {
            if (nodes.Contains(this)) return;
            nodes.Add(this);
            for (int i = 0; i < _connectedNodes.Count; i++)
                _connectedNodes[i].Traverse(ref nodes);
        }

        public bool IsInNetwork(Node node)
        {
            List<Node> traversedNodes = new List<Node>();
            Traverse(ref traversedNodes);
            return traversedNodes.Contains(node);
        }             

        public static string GetEdgeKey(Node a, Node b) => a.GetHashCode().ToString() + b.GetHashCode().ToString();
    }
}
