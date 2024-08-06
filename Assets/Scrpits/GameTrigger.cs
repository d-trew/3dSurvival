using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameTrigger : MonoBehaviour
{
    public GameObject promptPanel; // Reference to the panel UI
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject target; // Reference to the target GameObject
    private bool playerInRange = false;
    private bool gameActive = false;
    private float gameDuration = 30f; // Duration of the game in seconds
    private float timeRemaining;
    private int score = 0;

    void Start()
    {
        // Ensure the prompt is initially hidden
        promptPanel.SetActive(false);
        timerText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player enters");
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }

    public void ShowPrompt()
    {
        promptPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        promptText.text = "Do you want to play a game?";
    }

    public void HidePrompt()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        promptPanel.SetActive(false);
    }

    public void OnYesButtonClicked()
    {
        if (playerInRange)
        {
            StartGame();
        }
    }

    public void OnNoButtonClicked()
    {
        HidePrompt();
    }

    public void StartGame()
    {
        Debug.Log("Game Started!");
        HidePrompt();

        // Start the game timer
        timeRemaining = gameDuration;
        score = 0;
        UpdateUI();

        timerText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        gameActive = true;

        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        while (timeRemaining > 0)
        {
            timerText.text = "Time: " + timeRemaining.ToString("F1") + "s";
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        EndGame();
    }

    private void EndGame()
    {
        gameActive = false;
        timerText.gameObject.SetActive(false);
        scoreText.text += " - Game Over!";
        Debug.Log("Game Over! Your final score is: " + score);
    }

    public void RegisterHit(string targetTag)
    {
        Debug.LogError("HIT");
        if (!gameActive) return;

        switch (targetTag)
        {
            case "TargetCenter":
                score += 10; // Highest points for center hit
                break;
            case "TargetMiddle":
                score += 5; // Fewer points for middle hit
                break;
            case "TargetOuter":
                score += 2; // Lowest points for outer hit
                break;
        }
        UpdateUI();
        Debug.Log("Hit! Current score: " + score);
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
    }
}
