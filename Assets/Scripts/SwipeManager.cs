using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    [Header("References")]
    public Player playerScript;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [Header("Settings")]
    [SerializeField] private float minSwipeDistance = 50f;

    void Update()
    {
        // Ignore inputs if the game is paused or over
        if (Time.timeScale == 0f) return;

        // 1. Record the screen position when the player first touches/clicks
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        // 2. Record the screen position when the player releases the touch/click
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            DetectSwipe();
        }
    }

    // Calculates the distance and direction of the swipe to trigger player actions
    void DetectSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;

        // Check if the movement is long enough to be considered a swipe
        if ((Mathf.Abs(swipeVector.x) > minSwipeDistance || Mathf.Abs(swipeVector.y) > minSwipeDistance) && playerScript != null)
        {
            // Compare X axis and Y axis to see if the swipe was mostly horizontal or vertical
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                // Horizontal Swipe
                if (swipeVector.x > 0)
                    playerScript.MoveRight();
                else
                    playerScript.MoveLeft();
            }
            else
            {
                // Vertical Swipe
                if (swipeVector.y > 0)
                    playerScript.TriggerJump();
                else
                    playerScript.TriggerSlide();
            }
        }
    }
}