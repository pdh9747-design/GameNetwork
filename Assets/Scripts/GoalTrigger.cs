
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private bool cleared = false;

    private void OnTriggerEnter(Collider other)
    {
        if (cleared)
            return;

        if (other.CompareTag("Player"))
        {
            cleared = true;
            Debug.Log("CLEAR!");
        }
    }
}
