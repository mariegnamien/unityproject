using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Transform playerTransform;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public float pointsPerUnit = 5f;
    public Player playerScript;
    public float timeToIncreaseSpeed = 10f;
    public float speedIncrement = 2f;
    public int multiplierIncrement = 1;

    private float startZ;
    private int currentMultiplier = 1;
    private float speedTimer;
    private int finalScore;

    void Start()
    {
        // Save where the player started on the Z axis
        if (playerTransform != null)
        {
            startZ = playerTransform.position.z;
        }

        UpdateMultiplierUI();
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calculate how far the player traveled along the Z axis
        float distanceCalculated = playerTransform.position.z - startZ;

        // Calculate score: Distance * Points * Multiplier, then convert to a whole number (int)
        finalScore = (int)(distanceCalculated * pointsPerUnit * currentMultiplier);

        // "D8" forces the text to display at least 8 digits (e.g., 00000125)
        scoreText.text = finalScore.ToString("D8");

        // Handle the timer to increase difficulty over time
        speedTimer += Time.deltaTime;
        if (speedTimer >= timeToIncreaseSpeed)
        {
            IncreaseDifficulty();
            speedTimer = 0f; // Reset timer for the next difficulty spike
        }
    }

    // Increases game speed and score multiplier every X seconds
    void IncreaseDifficulty()
    {
        currentMultiplier += multiplierIncrement;
        UpdateMultiplierUI();

        // Directly increase the speed variable inside the Player script
        if (playerScript != null)
        {
            playerScript.forwardSpeed += speedIncrement;
            Debug.Log("Speed increased! New speed: " + playerScript.forwardSpeed);
        }
    }

    void UpdateMultiplierUI()
    {
        if (multiplierText != null)
        {
            multiplierText.text = "X" + currentMultiplier;
        }
    }

    // Called by the Player script when the game ends to display the final score
    public int GetFinalScore()
    {
        return finalScore;
    }
}