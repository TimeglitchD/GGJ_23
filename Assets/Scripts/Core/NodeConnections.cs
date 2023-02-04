using System.Collections.Generic;

namespace Synapse.Core
{
    public class NodeConnections
    {
        private Dictionary<string, Edge> _edges = new Dictionary<string, Edge>();
        public Dictionary<string, Edge> Edges => _edges;
       
        public bool CanConnect(Node a, Node b)
        {
            for (int i = 0; i < a.ConnectedNodes.Count; i++)
                if (a.ConnectedNodes[i] == b)
                    return false;
            return true;
        }

        public Edge ConnectNodes(Node a, Node b)
        {        
            a.ConnectedNodes.Add(b);
            b.ConnectedNodes.Add(a);

            Edge edge = new Edge(a, b);
            _edges.Add(Node.GetEdgeId(a, b), edge);
            return edge;
        }

        public void DisconnectNodes(Node a, Node b)
        {
            if (TryGetEdgeKey(a, b, out string edgeKey))
            {
                a.ConnectedNodes.Remove(b);
                b.ConnectedNodes.Remove(a);
                _edges.Remove(edgeKey);
            }
        }

        public void RemoveEdge(Edge edge)
        {
            Node a = edge.GetA;
            Node b = edge.GetB;
            a.ConnectedNodes.Remove(b);
            b.ConnectedNodes.Remove(a);

            TryGetEdgeKey(a, b, out string edgeKey);
            _edges.Remove(edgeKey);
        }

        private bool TryGetEdgeKey(Node a, Node b, out string edgeKey)
        {
            edgeKey = Node.GetEdgeId(a, b);
            if (!_edges.ContainsKey(edgeKey))
                edgeKey = Node.GetEdgeId(b, a);
            if (!_edges.ContainsKey(edgeKey))
            {
                edgeKey = "";
                return false;
            }
            return true;
               
        }
    }
}
