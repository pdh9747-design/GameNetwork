using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections;

public class RestartGame : MonoBehaviour
{
    private bool restarting = false;

    public void Restart()
    {
        if (restarting) return;

        restarting = true;
        StartCoroutine(RestartCoroutine());
    }

    private IEnumerator RestartCoroutine()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();

            while (NetworkManager.Singleton != null &&
                   NetworkManager.Singleton.IsListening)
            {
                yield return null;
            }
        }

        SceneManager.LoadScene(sceneIndex);
    }
}