using UnityEngine;
using Unity.Netcode;

public class ScoreUI : MonoBehaviour
{
    private int displayedScore = 0;

    private void Update()
    {
        if (GameScore.Instance != null &&
            GameScore.Instance.IsSpawned)
        {
            displayedScore = GameScore.Instance.Score.Value;
        }
    }

    private void OnGUI()
    {
        GUI.Box(
            new Rect(10, 170, 200, 40),
            "Á¡¼ö: " + displayedScore
        );
    }
}