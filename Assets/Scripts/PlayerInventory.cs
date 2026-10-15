
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKey = false;

    public List<string> collectedItems = new List<string>();

    public void AddItem(string itemName)
    {
        collectedItems.Add(itemName);

        for (int i = 0; i < collectedItems.Count; i++)
        {
            Debug.Log("È¹µæÇÑ ¾ÆÀÌÅÛ: " + collectedItems[i]);
        }

        if (itemName == "Key")
        {
            hasKey = true;
        }
    }
}
