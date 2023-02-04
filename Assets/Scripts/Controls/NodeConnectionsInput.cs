using Synapse.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Controls {

    public class NodeConnectionsInput : MonoBehaviour
    {
        [SerializeField] private Material[] _materials;

        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private LineRenderer _lineRenderer; 

        private NodeConnecter _nodeConnections = new NodeConnecter();
        public NodeConnecter NodeConnections => _nodeConnections;

        private ClickableNode _currentStartNode;

        public UnityEvent<Edge> OnEdgeCreated;

        public UnityEvent<ClickableNode> OnStartConnection;
        public UnityEvent<ClickableNode, ClickableNode> OnConnectionSuccess;
        public UnityEvent<ClickableNode, ClickableNode> OnConnectionFailed;
        public UnityEvent<ClickableNode, Vector3> OnConnectionCanceled;

        ClickableNode _lastNode;

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
            if (node.Node.Id < 0) return;

            _currentStartNode = node;
            OnStartConnection?.Invoke(_currentStartNode);

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

            _lastNode = _currentStartNode;
            if (go != null)
            {
                ClickableNode node = go.GetComponent<ClickableNode>();
                if (node != null)
                {
                    Node a = _currentStartNode.Node;
                    Node b = node.Node;

                    if (!_nodeConnections.CanConnect(a, b))
                    {
#if UNITY_EDITOR
                        if (ShowDebugLogs)
                            Debug.Log($"Connection <{a.Label} - {b.Label}> already exists.");
#endif
                        OnConnectionFailed?.Invoke(_currentStartNode, node);
                    }
                    else
                    {
                        OnConnectionSuccess?.Invoke(_currentStartNode, node);

                        // TODO dirty edge drawer
                        GameObject edgeObject = new GameObject();
                        LineRenderer line = edgeObject.AddComponent<LineRenderer>();
                        line.material = _materials[_currentStartNode.Node.Id % _materials.Length];
                        line.SetPositions(new Vector3[2] { _currentStartNode.transform.position, node.transform.position});
                        

                        Edge edge = _nodeConnections.ConnectNodes(a, b);
                        edge.OnDecayed += () => Destroy(line.gameObject);
                        edge.OnUpdatedId += (id) => line.material = _materials[id % _materials.Length];
                        node.ConnectToNetwork();
#if UNITY_EDITOR
                        if (ShowDebugLogs) 
                            Debug.Log($"Connecting: <{a.Label} - {b.Label}>");
#endif
                        OnEdgeCreated?.Invoke(edge);
                        
                    }
                }
                _currentStartNode = null;
                return;
            }
#if UNITY_EDITOR
            if (ShowDebugLogs && _currentStartNode) Debug.Log($"Connection stopped: {_currentStartNode.Node.Label}");
#endif

            OnConnectionCanceled?.Invoke(_currentStartNode, _eventSystem.CurrentMousePosition);
            _currentStartNode = null;
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
