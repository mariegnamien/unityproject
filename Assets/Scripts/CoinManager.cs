using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
    private List<Coin> coins = new List<Coin>();
    private int playerCoins = 0;

    #region Singleton
    public static CoinManager Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rotation = Quaternion.identity;
        UpdateCoinUI();
    }
    #endregion

    public void Update()
    {
        rotation *= Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);
        float hoverOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        // Boucle inversée ultra importante pour nettoyer les pièces manquantes sans faire planter le jeu
        for (int i = coins.Count - 1; i >= 0; i--)
        {
            if (coins[i] != null)
            {
                coins[i].transform.rotation = rotation;
                coins[i].ApplyHover(hoverOffset);
            }
            else
            {
                coins.RemoveAt(i);
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
        if (coins == null) coins = new List<Coin>();
        if (!coins.Contains(coin)) coins.Add(coin);
    }

    public void Unregister(Coin coin)
    {
        if (coins != null && coins.Contains(coin)) coins.Remove(coin);
    }
    public int GetCoinsCount()
    {
        return playerCoins;
    }
}