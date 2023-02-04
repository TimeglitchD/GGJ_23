using Synapse.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Controls {

    public class NodeConnectionsInput : MonoBehaviour
    {
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private LineRenderer _lineRenderer; 

        private NodeConnections _nodeConnections = new NodeConnections();
        public NodeConnections NodeConnections => _nodeConnections;

        private ClickableNode _currentStartNode;

        public UnityEvent BeforeConnected;
        public UnityEvent<Edge> OnConnected;

#if UNITY_EDITOR
        [Space]
        [Header("Debug")]
        [SerializeField] private bool ShowDebugLogs = false;
#endif

        private void Start()
        {
            if (_lineRenderer.enabled)
            {
                Debug.LogWarning($"Automatically disabling {typeof(LineRenderer)} component on {_lineRenderer.gameObject.name}");
                _lineRenderer.enabled = false;
            }
        }

        private void OnEnable()
        {
            _eventSystem.OnMouseDown.AddListener(TryStartConnecting);
            _eventSystem.OnMouseUp.AddListener(TryConnecting);
        }

        private void OnDisable()
        {
            _eventSystem.OnMouseDown.RemoveListener(TryStartConnecting);
            _eventSystem.OnMouseUp.RemoveListener(TryConnecting);
        }

        private void TryStartConnecting(GameObject go)
        {
            if (go == null) return;
            ClickableNode node = go.GetComponent<ClickableNode>();
            if (node == null) return;

            _currentStartNode = node;
          
#if UNITY_EDITOR
            if (ShowDebugLogs) Debug.Log($"Started connection for: {_currentStartNode.Node.Label}");
#endif
        }

        private void Update()
        {
            if (_currentStartNode == null)
            {
                _lineRenderer.enabled = false;
                return;
            }
            _lineRenderer.enabled = true;
            _lineRenderer.SetPositions(new Vector3[2] { _currentStartNode.transform.position, _eventSystem.CurrentMousePosition });
        }

        private void TryConnecting(GameObject go)
        {
            if (_currentStartNode == null) return;
            _lineRenderer.enabled = false;

            Node a = _currentStartNode.Node;
            _currentStartNode = null;

            if (go != null)
            {
                ClickableNode node = go.GetComponent<ClickableNode>();
                if (node != null)
                {
                    Node b = node.Node;
                    if (!_nodeConnections.CanConnect(a, b))
                    {
#if UNITY_EDITOR
                        Debug.Log($"Connection <{a.Label} - {b.Label}> already exists.");
#endif
                    }
                    else
                    {
                        BeforeConnected?.Invoke();
                        Edge edge = _nodeConnections.ConnectNodes(a, b);
#if UNITY_EDITOR
                        if (ShowDebugLogs) Debug.Log($"Connecting: <{a.Label} - {b.Label}>");
#endif
                        OnConnected?.Invoke(edge);
                    }
                }
                return;
            }
            EndConnection();
        }
       

   

        private void EndConnection()
        {
#if UNITY_EDITOR
            if (ShowDebugLogs && _currentStartNode) Debug.Log($"Connection stopped: {_currentStartNode.Node.Label}");
#endif

        }   

        private void Reset()
        {
            _eventSystem = FindAnyObjectByType<EventSystem>();
            if (_eventSystem == null)
            {
                Debug.LogError($"{typeof(NodeConnectionsInput)} requires a {typeof(EventSystem)} to be present in the scene.");
                DestroyImmediate(this);
            }
        }
    }
}
