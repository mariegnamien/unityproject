using UnityEngine;
using System.Collections.Generic;
using TMPro; // <-- ajoute ça

public class CoinManager : MonoBehaviour
{
    [Header("Réglages de Rotation")]
    [SerializeField] private float rotationSpeed = 85f;

    [Header("Réglages du Flottement")]
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float floatAmplitude = 0.2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;

    private Quaternion rotation;
    private List<Coin> coins;
    private int playerCoins = 0;

    #region Singleton
    public static CoinManager Instance;
    public void Awake()
    {
        Instance = this;
        rotation = Quaternion.identity;
        coins = new List<Coin>();
        UpdateCoinUI(); // initialise à "0"
    }
    #endregion

    public void Update()
    {
        rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
        float hoverOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        foreach (Coin coin in coins)
        {
            if (coin != null)
            {
                coin.transform.rotation = rotation;
                coin.ApplyHover(hoverOffset);
            }
        }
    }

    public void AddCoin(int amount)
    {
        playerCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = playerCoins.ToString();
    }

    public void Register(Coin coin)
    {
        if (!coins.Contains(coin)) coins.Add(coin);
    }

    public void Unregister(Coin coin)
    {
        if (coins.Contains(coin)) coins.Remove(coin);
    }
}