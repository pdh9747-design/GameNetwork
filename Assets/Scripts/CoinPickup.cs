using UnityEngine;
using Unity.Netcode;

public class CoinPickup : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        NetworkObject player = other.GetComponent<NetworkObject>();
        if (player == null) return;

        if (GameScore.Instance == null)
        {
            Debug.LogError("GameScore가 네트워크에 생성되지 않았습니다.");
            return;
        }

        GameScore.Instance.AddScore();
        Debug.Log("현재 점수: " + GameScore.Instance.Score.Value);

        NetworkObject coin = GetComponent<NetworkObject>();
        if (coin != null && coin.IsSpawned)
            coin.Despawn();
    }
}