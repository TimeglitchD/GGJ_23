using UnityEngine;

namespace Synapse.Core
{
   
    public class ConnectionMonobehaviour : MonoBehaviour
    {
        [SerializeField] private TextMesh _text;
        [SerializeField] private SpriteRenderer _renderer;
        public Connection Connection { get; set; }
        public NodeMonoBehaviour NodeMonoBehaviour { get; set; }

        public TextMesh Text => _text;
        public SpriteRenderer Renderer => _renderer;
        
    }
}