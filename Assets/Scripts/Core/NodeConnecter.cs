using System.Collections.Generic;
using UnityEngine;

namespace Synapse.Core
{
    public class NodeConnecter
    {
        public static int CurrentId = 0;

        private Dictionary<string, Edge> _edges = new Dictionary<string, Edge>();
        public Dictionary<string, Edge> Edges => _edges;
       
        public bool CanConnect(Node a, Node b)
        {
            if (!a.IsInRange(b) && !b.IsInRange(a)) return false;
            if (a.IsDirectlyConnected(b)) return false;
            return true;
        }

        public Edge ConnectNodes(Node a, Node b)
        {
            a.Add(b);
            b.Add(a);

            Merge(a, b);

            Edge edge = new Edge(a, b, a.Id);
            _edges.Add(Node.GetEdgeKey(a, b), edge);

            return edge;
        }

        public void DisconnectNodes(Node a, Node b)
        {
            if (TryGetEdgeKey(a, b, out string edgeKey))
                RemoveEdge(_edges[edgeKey]);   
        }

        private void Merge(Node a, Node b)
        {
            // One adapts to the other
            if (a.Id == b.Id) return;

            Node dominantNode = (a.Id > b.Id) ? b : a;
            Node otherNode = (dominantNode == a) ? b : a;
            UpdateIds(dominantNode, otherNode.Id);
        }

        public void RemoveEdge(Edge edge)
        {
            Node a = edge.GetA;
            Node b = edge.GetB;
            a.Remove(b);
            b.Remove(a);

            SplitGroups(a, b);

            TryGetEdgeKey(a, b, out string edgeKey);
            _edges.Remove(edgeKey);
        }

        private void SplitGroups(Node a, Node b)
        {
            if (a.IsInNetwork(b)) return;

            if (!a.HasConnections) a.Id = -1;
            else
            {
                CurrentId++;
                UpdateIds(a, CurrentId);
                Debug.Log($"A split into {CurrentId}");
            }
            if (!b.HasConnections) b.Id = -1;
            else
            {
                CurrentId++;
                UpdateIds(b, CurrentId);
                Debug.Log($"B split into {CurrentId}");
            }
        }

        public void UpdateIds(Node node, int newId)
        {
            List<Node> traversedNodes = new List<Node>();
            node.Traverse(ref traversedNodes);
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < traversedNodes.Count; i++)
            {
                node.Id = newId;

                foreach (KeyValuePair<string, Edge> pair in _edges)
                {
                    Edge edge = pair.Value;
                    Node a = edge.GetA;
                    Node b = edge.GetB;
                    if (a == traversedNodes[i] || b == traversedNodes[i] && !edges.Contains(edge))
                    {
                        edges.Add(edge);
                        edge.UpdateId(newId);
                    }
                }
            }
        }

        private bool TryGetEdgeKey(Node a, Node b, out string edgeKey)
        {
            edgeKey = Node.GetEdgeKey(a, b);
            if (!_edges.ContainsKey(edgeKey))
                edgeKey = Node.GetEdgeKey(b, a);
            if (!_edges.ContainsKey(edgeKey))
            {
                edgeKey = "";
                return false;
            }
            return true;    
        }
    }
}
