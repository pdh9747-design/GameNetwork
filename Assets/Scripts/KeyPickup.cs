
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null && !inventory.hasKey)
            {
                inventory.AddItem("Key");

                Debug.Log("ø≠ºË∏¶ »πµÊ«ﬂΩ¿¥œ¥Ÿ!");

                Destroy(gameObject);
            }
        }
    }
}
