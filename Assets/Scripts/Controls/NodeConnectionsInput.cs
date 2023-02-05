using Synapse.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Controls {
    public class NodeConnectionsInput : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;

        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private ConnectionLifetimeManager _connectionLifetimeManager;
       
        public UnityEvent<NodeMonoBehaviour> OnStartConnection;
        public UnityEvent<NodeMonoBehaviour, NodeMonoBehaviour> OnConnectionSuccess;
        public UnityEvent<NodeMonoBehaviour, NodeMonoBehaviour> OnConnectionFailed;
        public UnityEvent<NodeMonoBehaviour, Vector3> OnConnectionCanceled;

        NodeMonoBehaviour _connecterPathStart;

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
            _eventSystem.OnMouseDown.AddListener(OnDown);
            _eventSystem.OnMouseUp.AddListener(OnUp);
        }

        private void OnDisable()
        {
            _eventSystem.OnMouseDown.RemoveListener(OnDown);
            _eventSystem.OnMouseUp.RemoveListener(OnUp);
        }

        private void OnDown(GameObject go) => StartPathConnecter(go);
        private void OnUp(GameObject go) => EndPathConnector(go);

        private void StartPathConnecter(GameObject go)
        {
            if (go == null) return;
            NodeMonoBehaviour node = go.GetComponent<NodeMonoBehaviour>();
            if (node == null) return;
            if (!node.Node.CanInteract) return;

            _connecterPathStart = node;
            //Debug.Log("Starting path");
            OnStartConnection?.Invoke(_connecterPathStart);
        }

        private void Update()
        {
            if (_connecterPathStart == null)
            {
                _lineRenderer.enabled = false;
                return;
            }
            _lineRenderer.enabled = true;
            _lineRenderer.SetPositions(new Vector3[2] { _connecterPathStart.transform.position, _eventSystem.CurrentMousePosition });
        }


        private void EndPathConnector(GameObject go)
        {
            //Debug.Log("Ending path");
            // Has no start of path. Cannot connect
            if (_connecterPathStart == null) return;

            // Debug
            _lineRenderer.enabled = false;

            // Cancel path if no viable target has been found
            if (go == null)
            {
                OnConnectionCanceled?.Invoke(_connecterPathStart, _eventSystem.CurrentMousePosition);
                _connecterPathStart = null;
                //Debug.Log("No path connected.");
                return;
            }

            NodeMonoBehaviour node = go.GetComponent<NodeMonoBehaviour>();
            if (node == null || node == _connecterPathStart)
            {
                OnConnectionCanceled?.Invoke(_connecterPathStart, _eventSystem.CurrentMousePosition);
                _connecterPathStart = null;
                //Debug.Log("No path connected.");
                return;
            }

            if (!_connecterPathStart.NeighbouringNodes.Contains(node))
            {
                OnConnectionFailed?.Invoke(_connecterPathStart, node);
                //Debug.Log($"Path Failed:{_connecterPathStart.gameObject.name} - {node.gameObject.name} ");
                _connecterPathStart = null;
                return;
            }

            int index = _connecterPathStart.Node.TryConnect(node.Node);
            if (index >= 0)
            {
                // Start applying decay first
                _connectionLifetimeManager.ApplyDecayToAll();

                // Then spawn a new connection
                _connectionLifetimeManager.SpawnConnection(_connecterPathStart, node);

                //Debug.Log($"Path Connected:{_connecterPathStart.gameObject.name} - {node.gameObject.name} ");
                OnConnectionSuccess?.Invoke(_connecterPathStart, node);
                _connecterPathStart = null;
                return;
            }

            //Debug.Log($"Already has path:{_connecterPathStart.gameObject.name} - {node.gameObject.name} ");
            _connecterPathStart = null;
            return;
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
