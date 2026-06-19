using UnityEngine;

public class Coin : MonoBehaviour
{
    private float startY;

    private void Start()
    {
        // On sauvegarde la position Y initiale de la pièce avant qu'elle ne bouge
        startY = transform.position.y;

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.Register(this);
        }
    }

    // appelée par CoinManager à chaque frame
    public void ApplyHover(float offset)
    {
        Vector3 newPosition = transform.position;
        newPosition.y = startY + offset; // Hauteur de départ + la vague de surgrèvement
        transform.position = newPosition;
    }

    private void OnDestroy()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.Unregister(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // On vérifie si l'objet qui touche la pièce a bien le tag "Player"
        if (other.CompareTag("Player"))
        {
            // + 1 pièce au compteur global
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(1);
            }

            // On détruit la pièce 
            Destroy(gameObject);
        }
    }
}
