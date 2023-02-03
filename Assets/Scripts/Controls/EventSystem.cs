using UnityEngine;
using UnityEngine.Events;

namespace Synapse.Controls
{
    public class EventSystem : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

#if UNITY_EDITOR
        [SerializeField] private 
#endif
        IHover _previousHoverTarget;

        public UnityEvent<GameObject> OnMouseUp;
        public UnityEvent<GameObject> OnMouseDown;

        private void Update()
        {
            // Get mouse position
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            GameObject target = null;
            if (hit.collider != null)
            {
                target = hit.collider.gameObject;
                IHover hoverTarget = hit.collider.GetComponent<IHover>();
                SetHoverTarget(hoverTarget);
                          
            } else {
                SetHoverTarget(null);
            }

            // Always detects any mouse click
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown.Invoke(target);
                Debug.Log($"Down: {target}");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnMouseUp.Invoke(target);
                Debug.Log($"Up: {target}");
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
