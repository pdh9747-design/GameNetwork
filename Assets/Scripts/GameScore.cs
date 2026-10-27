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
}