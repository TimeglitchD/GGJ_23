using Synapse.Controls;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Synapse.Core
{
    [System.Serializable]
    public struct GoalData
    {
        public NodeMonoBehaviour GoalNode;
        public int ConnectionCount;
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] NodeMonoBehaviour _startNode;
        [SerializeField] GoalData[] _goalNodes;

        [SerializeField] NodeConnectionsInput _input;

        private void Awake()
        {
            _input.OnConnectionSuccess.AddListener((a, b) => ComputeTurn());
        }

        private void Start()
        {
            _startNode.Node.GroupId = 0;
            _startNode.Node.CanInteract = true;
            _startNode.ConnectToNetwork();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                RestartGame();
        }

        private void ComputeTurn()
        {
            if (CheckForWin()) 
                Debug.Log("You've won!");
        }

        private bool CheckForWin()
        {
            List<Node> traverseGroup = new List<Node>();
            
            int goalsAchieved = 0;
            int groupId = _goalNodes[0].GoalNode.Node.GroupId;
            for (int i = 1; i <_goalNodes.Length; i++)
                if (_goalNodes[i].GoalNode.Node.GroupId == groupId)
                    goalsAchieved++;
           
            Debug.Log("Goals achieved: " + goalsAchieved);
            return goalsAchieved >= _goalNodes.Length;
        }

        [ContextMenu("Reset")]
        public void RestartGame()
        {
            Debug.LogWarning("Restarting game.");
            var restartables = FindObjectsOfType<MonoBehaviour>().OfType<IRestartable>();
            foreach (var restartable in restartables)
                restartable.Restart();

            Start();
        }

    }
}
