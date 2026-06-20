using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    [Header("Références")]
    public Player playerScript; // Glisse ton joueur ici dans l'émetteur

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [Header("Réglages")]
    [Tooltip("Distance minimum en pixels pour que le glissement soit détecté")]
    [SerializeField] private float minSwipeDistance = 50f;

    void Update()
    {
        if (Time.timeScale == 0f) return; // Si le jeu est en pause/GameOver, on ne swipe pas

        // 1. Quand on CLIQUE
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        // 2. Quand on RELÂCHE
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }
    }

    void DetectSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;

        if (swipeVector.magnitude > minSwipeDistance && playerScript != null)
        {
            // Horizontal vs Vertical
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                // Mouvement Horizontal
                if (swipeVector.x > 0)
                    playerScript.MoveRight();
                else
                    playerScript.MoveLeft();
            }
            else
            {
                // Mouvement Vertical
                if (swipeVector.y > 0)
                    playerScript.TriggerJump();
                else
                    playerScript.TriggerSlide();
            }
        }
    }
}