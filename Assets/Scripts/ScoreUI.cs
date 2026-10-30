using UnityEngine;
using Unity.Netcode;

public class ScoreUI : MonoBehaviour
{
    public GameObject restartButton;

    private int displayedScore = 0;
    private int remainingCoins = 0;
    private bool isCleared = false;

    private void Start()
    {
        if (restartButton != null)
            restartButton.SetActive(false);
    }

    private void Update()
    {
        if (GameScore.Instance == null ||
            !GameScore.Instance.IsSpawned)
            return;

        displayedScore = GameScore.Instance.Score.Value;
        remainingCoins = GameScore.Instance.RemainingCoins.Value;
        isCleared = remainingCoins == 0;

        if (restartButton != null)
            restartButton.SetActive(isCleared);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 170, 200, 40),
            "점수: " + displayedScore);

        GUI.Box(new Rect(10, 215, 200, 40),
            "남은 코인: " + remainingCoins);

        if (isCleared)
        {
            GUI.Box(
                new Rect(Screen.width / 2 - 100,
                         Screen.height / 2 - 30, 200, 60),
                "CLEAR!"
            );
        }
    }
}