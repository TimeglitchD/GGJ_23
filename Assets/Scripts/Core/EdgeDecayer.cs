using Synapse.Controls;
using System.Collections.Generic;
using UnityEngine;

namespace Synapse.Core
{
    public class EdgeDecayer : MonoBehaviour
    {
        [SerializeField] private NodeConnectionsInput connector;
        [SerializeField] private TurnCounter turnCounter;

#if UNITY_EDITOR
        [Space]
        [Header("Debug")]
        [SerializeField] private bool ShowDebugLogs = false;
#endif

        private void OnEnable()
        {
            turnCounter.OnNextTurn.AddListener(DecayEdges);
            
        }

        private void OnDisable()
        {
            turnCounter.OnNextTurn.RemoveListener(DecayEdges);
        }


        private void DecayEdges(Edge latestEdge, int turn)
        {
            Dictionary<string, Edge> edges = connector.NodeConnections.Edges;
            List<Edge> decayedEdges = new List<Edge>();
            foreach (KeyValuePair<string, Edge> pair in edges)
            {
                Edge edge = pair.Value;
                if (latestEdge == edge) continue;

                // Queue keys for removal
                if (edge.Decay())            
                    decayedEdges.Add(edge);
#if UNITY_EDITOR
                if (ShowDebugLogs)
                    Debug.Log($"Decayed edge <{edge.GetA.Label} - {edge.GetB.Label}> value: {edge.PersistenceValue}");
#endif
            }
            for (int i = 0; i < decayedEdges.Count; i++)
            {
                connector.NodeConnections.RemoveEdge(decayedEdges[i]);
#if UNITY_EDITOR
                if (ShowDebugLogs)
                    Debug.Log($"Removed edge: <{decayedEdges[i].GetA.Label} - {decayedEdges[i].GetB.Label}>");
#endif
            }


        }


        private void Reset()
        {
            connector = FindAnyObjectByType<NodeConnectionsInput>();
            if (connector == null)
            {
                Debug.LogError($"{typeof(EdgeDecayer)} requires a {typeof(NodeConnectionsInput)} to be present in the scene.");
                DestroyImmediate(this);
            }

            turnCounter = FindAnyObjectByType<TurnCounter>();
            if (connector == null)
            {
                Debug.LogError($"{typeof(EdgeDecayer)} requires a {typeof(TurnCounter)} to be present in the scene.");
                DestroyImmediate(this);
            }
        }
    }
}
