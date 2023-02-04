using System.Linq;
using UnityEngine;

namespace Synapse.Core
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] NodeMonoBehaviour _startNode;

        private void Start()
        {
            _startNode.Node.GroupId = 0;
            _startNode.Node.CanInteract = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                RestartGame();
        }

        [ContextMenu("Reset")]
        public void RestartGame()
        {
            Debug.LogWarning("Restarting game.");
            var restartables = FindObjectsOfType<MonoBehaviour>().OfType<IRestartable>();
            foreach (var restartable in restartables)
                restartable.Restart();
        }

    }
}
