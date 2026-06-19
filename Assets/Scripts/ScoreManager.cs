using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public Transform playerTransform;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public float pointsPerUnit = 5f; // 5 points per unit of distance

    public Player playerScript;
    public float timeToIncreaseSpeed = 10f; // Every 10 seconds, the speed increases
    public float speedIncrement = 2f; // +2 to speed at each stage
    public int multiplierIncrement = 1; // +1 to the multiplier at each stage

    private float startZ;
    private int currentMultiplier = 1;
    private float speedTimer;

    void Start()
    {
        // We register the player's starting Z position
        if (playerTransform != null)
        {
            startZ = playerTransform.position.z;
        }

        UpdateMultiplierUI();
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 1. Calculating the score by distance
        float distanceCalculated = playerTransform.position.z - startZ;

        // Distance × Basis Points × Current Multiplier (stored in a 'long' to prevent overflow during calculation)
        long rawScore = (long)(distanceCalculated * pointsPerUnit * currentMultiplier);

        // We clamp the score to the absolute maximum value of an int (2,147,483,647)
        int finalScore = (int)Mathf.Min(rawScore, int.MaxValue);

        // Score displayed (The "D8" forces at least 8 digits, but will naturally grow to 9 or 10 digits if needed)
        scoreText.text = finalScore.ToString("D8");

        // 2. Time management for speed and multiplier
        speedTimer += Time.deltaTime;

        if (speedTimer >= timeToIncreaseSpeed)
        {
            IncreaseDifficulty();
            speedTimer = 0f; // We're resetting the timer for the next stage
        }
    }

    void IncreaseDifficulty()
    {
        // We're increasing the score multiplier
        currentMultiplier += multiplierIncrement;
        UpdateMultiplierUI();

        // Safely increase the player's forward speed directly in their script
        if (playerScript != null)
        {
            playerScript.forwardSpeed += speedIncrement;
            Debug.Log("Speed increased! New speed: " + playerScript.forwardSpeed + " | Multiplier : x" + currentMultiplier);
        }
    }

    void UpdateMultiplierUI()
    {
        if (multiplierText != null)
        {
            multiplierText.text = "X" + currentMultiplier;
        }
    }
}