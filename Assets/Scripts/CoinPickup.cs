using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class CoinPickup : NetworkBehaviour
{
    private static readonly HashSet<ulong> collectedCoins = new HashSet<ulong>();

    [SerializeField] private float pickupDistance = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer || !IsSpawned) return;

        NetworkObject player = other.GetComponent<NetworkObject>();
        if (player == null) return;

        float distance = Vector3.Distance(
            transform.position,
            player.transform.position
        );

        if (distance > pickupDistance)
        {
            Debug.Log("ÄÚÀÎ È¹µæ °Å¸®°¡ ÃÊ°úµÇ¾î °ÅºÎµÊ");
            return;
        }

        ulong coinId = NetworkObjectId;

        if (collectedCoins.Contains(coinId)) return;
        collectedCoins.Add(coinId);

        if (GameScore.Instance != null)
            GameScore.Instance.AddScore();

        NetworkObject coin = GetComponent<NetworkObject>();
        if (coin != null && coin.IsSpawned)
            coin.Despawn();
    }
}