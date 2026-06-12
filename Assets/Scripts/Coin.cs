using UnityEngine;

public class Coin : MonoBehaviour
{
    private float startY; // Pour stocker la hauteur de départ de la pièce

    private void Start()
    {
        // On sauvegarde la position Y initiale de la pièce avant qu'elle ne bouge
        startY = transform.position.y;

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.Register(this);
        }
    }

    // Cette fonction est appelée par le CoinManager à chaque frame
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
            // 1. On dit au manager d'ajouter 1 pièce au compteur global
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.AddCoin(1);
            }

            // 2. On détruit la pièce (elle disparaît de la scène)
            Destroy(gameObject);
        }
    }
}