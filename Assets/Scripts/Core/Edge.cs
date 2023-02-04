using System;
using UnityEngine;

namespace Synapse.Core
{
    public class Edge
    {
        private Node _a;
        private Node _b;

        private int _persistenceValue;
        public int PersistenceValue => _persistenceValue;

        public Node GetA => _a;
        public Node GetB => _b;

        public event Action OnDecayed;
        public event Action<int> OnUpdatedId;

        public Edge(Node a, Node b, int id)
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

        public void UpdateId(int id)
        {
            OnUpdatedId?.Invoke(id);
        }

        public void Remove()
        {
            OnDecayed?.Invoke();
            OnDecayed = null;
            
        }
    }
}
