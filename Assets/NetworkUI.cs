using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    private string status = "연결 대기 중";

    public void StartHost()
    {
        if (NetworkManager.Singleton == null)
        {
            status = "NetworkManager 없음";
            return;
        }

        if (NetworkManager.Singleton.IsListening) return;

        bool success = NetworkManager.Singleton.StartHost();
        status = success ? "Host 실행 중" : "Host 실행 실패";
    }

    public void StartClient()
    {
        if (NetworkManager.Singleton == null)
        {
            status = "NetworkManager 없음";
            return;
        }

        if (NetworkManager.Singleton.IsListening) return;

        bool success = NetworkManager.Singleton.StartClient();
        status = success ? "Client 연결 시도 중" : "Client 실행 실패";
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 220, 150), "Coin Game");

        if (GUI.Button(new Rect(20, 40, 200, 30), "Start Host"))
            StartHost();

        if (GUI.Button(new Rect(20, 75, 200, 30), "Start Client"))
            StartClient();

        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            status = NetworkManager.Singleton.IsHost
                ? "Host 실행 중"
                : NetworkManager.Singleton.IsConnectedClient
                    ? "Client 연결됨"
                    : "연결 중";
        }

        GUI.Label(new Rect(20, 115, 200, 30), status);
    }
}