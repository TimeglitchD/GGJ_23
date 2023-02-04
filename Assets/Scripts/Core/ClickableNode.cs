using UnityEngine;

namespace Synapse.Core
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class ClickableNode : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private string _label;
        [SerializeField] [Range(0, 10)] private int _value = 10;

        private Node _node;
        public Node Node => _node;

        private void Awake()
        {
            _node = new Node(_id, _label, _value);
        }

        private void Reset()
        {
            _id = gameObject.name;
            _label = gameObject.name;
        }

    }
}
