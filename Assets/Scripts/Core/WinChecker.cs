using System.Collections.Generic;
using UnityEngine;

namespace Synapse.Core
{
    public class WinChecker : MonoBehaviour
    {
        [SerializeField] ClickableNode[] nodesToWin;
        [SerializeField] TurnCounter turnCounter;

        private void Awake()
        {
            turnCounter.OnNextTurn.AddListener((edge, counter) => CheckWinCondition(edge));
        }

        private void CheckWinCondition(Edge edge)
        {
            // All ids should just be the same and not -1
            if (nodesToWin[0].Node.Id == -1) return;
            int previousId = nodesToWin[0].Node.Id;
            for (int j = 1; j < nodesToWin.Length; j++)
                if (previousId != nodesToWin[j].Node.Id || nodesToWin[j].Node.Id == -1)
                    return;

            // Why you no work

            Debug.Log("You won!");
        }
    }
}
