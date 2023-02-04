using Synapse.Controls;
using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Core
{
    public class TurnCounter : MonoBehaviour
    {
        int turnCounter = 0;
        [SerializeField] private NodeConnectionsInput connector;

        public UnityEvent<Edge, int> OnNextTurn;

#if UNITY_EDITOR
        [Space]
        [Header("Debug")]
        [SerializeField] private bool ShowDebugLogs = false;
#endif

        private void Awake()
        {
            connector.OnEdgeCreated.AddListener(NextTurn);
        }

        private void NextTurn(Edge edge)
        {
            turnCounter++;
            OnNextTurn?.Invoke(edge, turnCounter);
#if UNITY_EDITOR
            if(ShowDebugLogs)
                Debug.Log($"Turn counter: {turnCounter}");
#endif
        }

        private void Reset()
        {
            connector = FindAnyObjectByType<NodeConnectionsInput>();
            if (connector == null)
            {
                Debug.LogError($"{typeof(TurnCounter)} requires a {typeof(NodeConnectionsInput)} to be present in the scene.");
                DestroyImmediate(this);
            }
        }
    }
}
