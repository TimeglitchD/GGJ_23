using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Controls
{
    public class EventSystem : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layers;

#if UNITY_EDITOR
        [Space]
        [Header("Debug")]
        [SerializeField] private bool ShowDebugLogs = false;
#endif

        IHover _previousHoverTarget;
        public UnityEvent<GameObject> OnMouseUp;
        public UnityEvent<GameObject> OnMouseDown;
        private Vector2 _currentMousePosition;
        public Vector2 CurrentMousePosition => _currentMousePosition;

        private void Update()
        {
            // Get mouse position
            Vector3 current3dMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _currentMousePosition = new Vector2(current3dMousePosition.x, current3dMousePosition.y);


            RaycastHit2D[] hit = Physics2D.RaycastAll(_currentMousePosition, Vector2.zero, float.MaxValue, _layers);
            GameObject target = null;

            if (hit.Length > 0)
            {
                target = hit[0].collider.gameObject;
                IHover hoverTarget = hit[0].collider.GetComponent<IHover>();
                SetHoverTarget(hoverTarget);
                          
            } else {
                SetHoverTarget(null);
            }

            // Always detects any mouse click
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown?.Invoke(target);
#if UNITY_EDITOR
                if (ShowDebugLogs) Debug.Log($"Down: {target}");
#endif
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp?.Invoke(target);
#if UNITY_EDITOR
                if (ShowDebugLogs) Debug.Log($"Up: {target}");
#endif
            }

            // Maybe click support
        }

        private void SetHoverTarget(IHover hoverTarget)
        {
            if (_previousHoverTarget == hoverTarget)
            {
                if (hoverTarget == null) return;
                hoverTarget.OnHover();
                return;
            };

            if (_previousHoverTarget != null) _previousHoverTarget.OnExit();
            if (hoverTarget != null) hoverTarget.OnEnter();            
        }

        private void Reset()
        {
            _camera = Camera.main;
        }
    }
}
