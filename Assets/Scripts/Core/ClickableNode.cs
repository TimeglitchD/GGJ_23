using System.Collections.Generic;
using UnityEngine;

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
    public class ClickableNode : MonoBehaviour
    {
        [SerializeField] private string _label;
        [SerializeField] [Range(0, 10)] private int _value = 10;
        [SerializeField] private ClickableNode[] _nodesInRange;
        [SerializeField] private bool _isStartPoint = false;
        
        public bool IsStartPoint => _isStartPoint;
        private VisibleState _visibleState;

        private Node _node;
        public Node Node => _node;


        private void OnDrawGizmosSelected()
        {
            foreach (var item in _nodesInRange)
            {
                Gizmos.DrawLine(transform.position, item.transform.position);
            }
        }


        private void Awake()
        {
            List<Node> nodesInRange = new List<Node>();
            for (int i = 0; i < _nodesInRange.Length; i++)
                nodesInRange.Add(_nodesInRange[i].Node);

            int id = -1;
            if (!_isStartPoint)
                gameObject.SetActive(false);
            else
                id = 0; // Supports only one startpoint right now.
            
            _node = new Node(id, _label, _value, nodesInRange);                  
        }

        private void Start()
        {
            if (_isStartPoint)
                ConnectToNetwork();            
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
            foreach (ClickableNode node in _nodesInRange)
                node.RevealFromFogOfWar();
            gameObject.SetActive(true);

            // Reveal label, stats, etc?
        }

        private void Reset()
        {
            _label = gameObject.name;
        }
    }
}
