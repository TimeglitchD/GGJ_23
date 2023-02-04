using UnityEngine;

namespace Synapse.Core
{
    public class WinChecker : MonoBehaviour
    {
        [SerializeField] NodeMonoBehaviour[] nodesToWin;
        [SerializeField] TurnCounter turnCounter;

        private void Awake()
        {
            turnCounter.OnNextTurn.AddListener((counter) => CheckWinCondition());
        }

        private void CheckWinCondition()
        {
            // All ids should just be the same and not -1
            if (nodesToWin[0].Node.GroupId == -1) return;
            int previousId = nodesToWin[0].Node.GroupId;
            for (int j = 1; j < nodesToWin.Length; j++)
                if (previousId != nodesToWin[j].Node.GroupId || nodesToWin[j].Node.GroupId == -1)
                    return;


            // Why you no work

            Debug.Log("You won!");
        }
    }
}
