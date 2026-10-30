using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    public GameObject startButton;

    private void Update()
    {
        if (GameScore.Instance != null &&
            GameScore.Instance.GameStarted.Value)
        {
            if (startButton != null)
                startButton.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (GameScore.Instance != null)
        {
            GameScore.Instance.RequestStartGame();
        }
    }
}