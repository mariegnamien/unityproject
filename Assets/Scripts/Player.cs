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

    private Renderer gfxRenderer;
    private bool touchingObstacle;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        gfxRenderer = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;
        direction.y += gravity * Time.deltaTime;
        if (controller.isGrounded)
        {
            direction.y = 0; // or -1
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
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
        // Calcul de la position cible en X
        Vector3 targetPosition = transform.position;
        if (desiredLane == 2)
            targetPosition.x = laneDistance;
        else if (desiredLane == 0)
            targetPosition.x = -laneDistance;
        else
            targetPosition.x = 0;

        // Déplacement latéral via direction, pas transform.position
        direction.x = (targetPosition.x - transform.position.x) * 10f;
    }

    private void FixedUpdate()
    {
        touchingObstacle = false;
        controller.Move(direction * Time.fixedDeltaTime); // la formule utilisée dans move nous renverra un vecteur avec le nombre d'unités pour un déplacement sur un appel, fixedupdate est appelée 50 fois.
        gfxRenderer.material.color = touchingObstacle ? Color.green : Color.white;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.5f)
            touchingObstacle = true;
    }

    private void Jump()
    {
        direction.y = jumpForce;

    }
}
