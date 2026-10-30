using UnityEngine;
using Unity.Netcode;

public class GameScore : NetworkBehaviour
{
    public static GameScore Instance;

    public NetworkVariable<int> Score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<int> RemainingCoins = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        Instance = this;
    }

    public override void OnNetworkDespawn()
    {
        if (Instance == this)
            Instance = null;
    }

    public void AddScore()
    {
        if (IsServer)
            Score.Value++;
    }

    public void RegisterCoin()
    {
        if (IsServer)
            RemainingCoins.Value++;
    }

    public void CollectCoin()
    {
        if (IsServer && RemainingCoins.Value > 0)
            RemainingCoins.Value--;
    }

    public void RequestStartGame()
    {
        if (IsServer)
            GameStarted.Value = true;
        else
            StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        GameStarted.Value = true;
    }
}