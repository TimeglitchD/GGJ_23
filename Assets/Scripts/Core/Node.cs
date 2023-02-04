using System;
using System.Collections.Generic;
using UnityEngine;

namespace Synapse.Core
{
    public enum NodeType
    {
        Hacker,
        Government
    }

    [System.Serializable]
    public struct NodeData
    {
        public string Label;
        [Range(1, 10)] public  int Value;
        public NodeType NodeType;
        
    }

    public class Node
    {

        private static int NextGroupId = 0;
       
        private NodeData _nodeData;

        public int GroupId { get; set; }
        public NodeData NodeData => _nodeData;
        public bool CanInteract { get; set; }

        public int HackerConnections { get {
            int count = 0; 
            foreach (Connection connection in Connections)
                if (connection.Node.NodeData.NodeType == NodeType.Hacker)
                   count++;
                return count;
            }
        }

        public Node(NodeData nodeData)
        {
            _nodeData = nodeData;
            GroupId = -1;
        }
        public List<Connection> Connections = new List<Connection>();
        
        public int GetConnectionIndex(Node node)
        {
            for (int i = 0; i < Connections.Count; i++)
            {
                if (Connections[i].Node == node)
                return i;
            }
            return -1;;      
        }

        public int TryConnect(Node node)
        {
            int index = GetConnectionIndex(node);
            if (index >= 0) return -1;

            Connections.Add(new Connection(node, Connection.CalculateDecayValue(this, node)));
            if (GroupId != node.GroupId)
            {
                List<Node> traversedNodes = new List<Node>();
                TraverseGroup(ref traversedNodes);
            }
            return Connections.Count - 1;
        }

        public int RemoveConnection(Connection connection)
        {
            if (Connections.Contains(connection))
            {
                Node node = connection.Node;
                Connections.Remove(connection);
                if (Connections.Count == 0) CanInteract = false;
                else if (node.Connections.Count == 0) CanInteract = false;

                List<Node> traversedNodes = new List<Node>();
                if (!IsConnected(node, ref traversedNodes))
                {
                    NextGroupId++;
                    GroupId = NextGroupId;
                    traversedNodes.Clear();
                    TraverseGroup(ref traversedNodes);
                    return 1;
                }
            }
            return 0;
        }

        public int TryDisconnect(Node node)
        {
            int index = GetConnectionIndex(node);

            if (Connections.Count == 0)
                GroupId = -1;
            if (node.Connections.Count == 0)
                node.GroupId = -1;

            if (index >= 0)
            {
                Connections.RemoveAt(index);
                return 1;
            }

            List<Node> traversedNodes = new List<Node>();
            if (!IsConnected(node, ref traversedNodes))
            {
                NextGroupId++;
                GroupId = NextGroupId;
                traversedNodes.Clear();
                TraverseGroup(ref traversedNodes);
            }
            return 0;
        }

        private bool IsConnected(Node node, ref List<Node> traverseNodes)
        {
            foreach (Connection connection in Connections)
            {
                if (traverseNodes.Contains(node)) return false;
                if (node == connection.Node) return true;
                traverseNodes.Add(node);
                bool foundInRecursion = connection.Node.IsConnected(node, ref traverseNodes);
                if (foundInRecursion) return true;
            }
            return false;
        }

        private void TraverseGroup(ref List<Node> traversedNodes)
        {
            if (traversedNodes.Contains(this)) return;
            traversedNodes.Add(this);
            GroupId = traversedNodes[0].GroupId;

            foreach (Connection connection in Connections)
                connection.Node.TraverseGroup(ref traversedNodes);
        }
    }
}
