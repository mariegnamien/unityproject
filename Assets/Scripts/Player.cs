using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed;

    private int desiredLane = 1;
    public float laneDistance;

    public float jumpForce;
    public float gravity = -20;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;
        direction.y += gravity * Time.deltaTime;
        if (controller.isGrounded)
        {
            direction.y = 0; // or -1
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
            else
            {
                direction.y += gravity * Time.deltaTime;
            }
        }
        //gather inputs for the desired lane
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if(desiredLane == 3)
            {
                desiredLane = 2;
            }

        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if(desiredLane == -1)
            {
                desiredLane = 0;
            }
        }
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
      
        }
        else if(desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;

        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, 80 * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime); // la formule utilisée dans move nous renverra un vecteur avec le nombre d'unités pour un déplacement sur un appel, fixedupdate est appelée 50 fois.

    }

    private void Jump()
    {
        direction.y = jumpForce;

    }
}
