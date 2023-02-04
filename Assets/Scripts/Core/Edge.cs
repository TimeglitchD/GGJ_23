using System;
using UnityEngine;

namespace Synapse.Core
{
    public class Edge
    {
        private Node _a;
        private Node _b;

        private int _persistenceValue;

        public Node GetA => _a;
        public Node GetB => _b;

        public event Action OnDecayed;
        public static Edge CreateConnection(Node a, Node b)
        {
            a.ConnectedNodes.Add(b);
            b.ConnectedNodes.Add(a);
            return new Edge(a, b);
        }


        public Edge(Node a, Node b)
        {
            _a = a;
            _b = b;
            
            _persistenceValue = Math.Max(a.Value, b.Value);
        }

        public bool Decay()
        {
            _persistenceValue--;
            if (_persistenceValue <= 0)
            {
                Remove();
                return true;
                // Destroy or pool itself
                // Visuals, etc
            }
            return false;
        }



        public void Remove()
        {
            OnDecayed?.Invoke();
            OnDecayed = null;
        }
    }
}
