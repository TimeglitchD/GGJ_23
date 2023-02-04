using UnityEngine;

namespace Synapse.Core
{
    [System.Serializable]
    public class Connection
    {
        public Node Node;
        public int DecayValue;
        
        public Connection(Node node, int decayValue)
        {
            Node = node;
            DecayValue = decayValue;
        }

        public bool Decay()
        {
            DecayValue--;
            return (DecayValue <= 0);
        }

        public static int CalculateDecayValue(Node a, Node b)
        {
            return a.NodeData.Value;//Mathf.Max(a.NodeData.Value, b.NodeData.Value);
        }
    }
}