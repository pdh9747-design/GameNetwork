using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class CoinPickup : NetworkBehaviour
{
    private static readonly HashSet<ulong> collectedCoins =
        new HashSet<ulong>();

    [SerializeField] private float pickupDistance = 2.0f;

    public override void OnNetworkSpawn()
    {
        if (IsServer && GameScore.Instance != null)
        {
            GameScore.Instance.RegisterCoin();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("코인 충돌 감지: " + other.name);

        if (!IsServer || !IsSpawned)
            return;

        NetworkObject player =
            other.GetComponentInParent<NetworkObject>();

        if (player == null)
        {
            Debug.Log("플레이어 NetworkObject를 찾지 못함");
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.transform.position
        );

        if (distance > pickupDistance)
        {
            Debug.Log("획득 거리 초과: " + distance);
            return;
        }

        ulong coinId = NetworkObjectId;

        if (!collectedCoins.Add(coinId))
            return;

        if (GameScore.Instance != null)
        {
            GameScore.Instance.AddScore();
            GameScore.Instance.CollectCoin();
        }

        Debug.Log("코인 획득 성공!");

        NetworkObject coin = GetComponent<NetworkObject>();

        if (coin != null && coin.IsSpawned)
            coin.Despawn();
    }
}