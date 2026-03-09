using UnityEngine;
using TMPro; //Biblioteca para moverle a los textos

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
        }
    }

    public void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timerText == null) return;

        if (timeToDisplay < 0) timeToDisplay = 0;

        // Formato MM:SS
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // De mamon se pone rojo el texto cuando casi termina
        if (timeToDisplay <= 10f)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }
}