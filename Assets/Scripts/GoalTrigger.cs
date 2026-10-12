
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameObject clearText;
    private bool cleared = false;

    private void Start()
    {
        if (clearText != null)
        {
            clearText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cleared)
            return;

        if (other.CompareTag("Player"))
        {
            cleared = true;

            if (clearText != null)
            {
                clearText.SetActive(true);
            }

            Debug.Log("CLEAR!");
        }
    }
}
