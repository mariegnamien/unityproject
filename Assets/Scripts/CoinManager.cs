using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 85f;

    [Header("Hover Settings")]
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float floatAmplitude = 0.2f;

    [Header("UI Component")]
    [SerializeField] private TextMeshProUGUI coinText;

    private List<Coin> coins = new List<Coin>();
    private int playerCoins = 0;

    #region Singleton
    public static CoinManager Instance;

    public void Awake()
    {
        // Simple Singleton pattern: ensures only one instance of CoinManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        UpdateCoinUI();
    }
    #endregion

    public void Update()
    {
        // Mathf.Sin creates a smooth wave (-1 to 1) based on time to make coins bob up and down
        float hoverOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        // Loop backwards to safely remove coins from the list if they are collected/destroyed
        for (int i = coins.Count - 1; i >= 0; i--)
        {
            if (coins[i] != null)
            {
                // Rotate the coin smoothly on the Y axis
                coins[i].transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

                // Apply the hover floating effect
                coins[i].ApplyHover(hoverOffset);
            }
            else
            {
                // Clean up the list if the coin object no longer exists
                coins.RemoveAt(i);
            }
        }
    }

    // Adds coins to the counter and refreshes the display
    public void AddCoin(int amount)
    {
        playerCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = playerCoins.ToString();
        }
    }

    // Adds a newly spawned coin to the tracking list
    public void Register(Coin coin)
    {
        if (!coins.Contains(coin))
        {
            coins.Add(coin);
        }
    }

    // Removes a coin from the tracking list
    public void Unregister(Coin coin)
    {
        if (coins.Contains(coin))
        {
            coins.Remove(coin);
        }
    }

    // Called by the Player script when the game ends
    public int GetCoinsCount()
    {
        return playerCoins;
    }
}