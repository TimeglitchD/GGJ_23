using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Core
{
    public enum VisibleState
    {
        Hidden,
        Anonymous,
        Visible,
        Disabled
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class NodeMonoBehaviour : MonoBehaviour, IRestartable
    {
        [SerializeField] private NodeData _nodeData;
        [SerializeField] private List<NodeMonoBehaviour> _neighbouringNodes = new List<NodeMonoBehaviour>();

        [SerializeField] private UnityEvent OnStartDrawPath;
        [SerializeField] private UnityEvent OnEndDrawPath;

        public List<NodeMonoBehaviour> NeighbouringNodes => _neighbouringNodes;

        private VisibleState _visibleState = VisibleState.Visible;
        private Node _node;
        public Node Node => _node;

#if UNITY_EDITOR
        [SerializeField] [Range(0.01f, 10.0f)] private float _maxDistance = 2;
#endif

        private void OnDrawGizmosSelected()
        {
            foreach (var item in _neighbouringNodes)
                Gizmos.DrawLine(transform.position, item.transform.position);
        }


        private void Awake()
        {
            _node = new Node(_nodeData);
            Hide();
        }

        public void Hide()
        {
            if (_visibleState == VisibleState.Hidden) return;
            _visibleState = VisibleState.Anonymous;
            gameObject.SetActive(false);
        }

        public void RevealFromFogOfWar()
        {
            if (_visibleState == VisibleState.Visible) return;
            _visibleState = VisibleState.Anonymous;
            gameObject.SetActive(true);
        }

        public void ConnectToNetwork()
        {
            _visibleState = VisibleState.Visible;
            foreach (NodeMonoBehaviour node in _neighbouringNodes)
                node.RevealFromFogOfWar();
            gameObject.SetActive(true);
        }

        public void StartPathConnection()
        {

            OnStartDrawPath?.Invoke();
        }


        public void EndPathConnection()
        {
            OnEndDrawPath?.Invoke();
        }

        public void Restart()
        {
            Hide();
            Node.Connections.Clear();
            Node.CanInteract = false;
            new Node(_nodeData);
        }


#if UNITY_EDITOR
        private void Reset()
        {
            _nodeData = new NodeData()
            {
                Label = gameObject.name,
                Value = 1,
                NodeType = NodeType.Government,
            };
            FindNeighbours();
        }

        [ContextMenu("Find neighbours")]
        private void FindNeighbours()
        {
            _neighbouringNodes.Clear();
            NodeMonoBehaviour[] others = FindObjectsOfType<NodeMonoBehaviour>();
            foreach (var item in others)
            {
                if (Vector3.Distance(item.transform.position, transform.position) < _maxDistance)
                    _neighbouringNodes.Add(item);
            }
        }
#endif
    }
}
