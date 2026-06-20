using UnityEngine;

public class Coin : MonoBehaviour
{
    private float startY;

    private void Start()
    {
        // Save the initial Y position before the coin starts hovering
        startY = transform.position.y;

        // Register this coin to the global manager to keep track of it
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.Register(this);
        }
    }

    // This function is called by CoinManager every frame to make the coin bob up down
    public void ApplyHover(float offset)
    {
        Vector3 newPosition = transform.position;
        newPosition.y = startY + offset;
        transform.position = newPosition;
    }

    private void OnDestroy()
    {
        // Tell the manager to stop tracking this coin when it gets destroyed
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.Unregister(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the Player
        if (other.CompareTag("Player"))
        {
            // Add +1 coin to the global score counter
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(1);
            }

            // Remove the coin from the game scene
            Destroy(gameObject);
        }
    }
}