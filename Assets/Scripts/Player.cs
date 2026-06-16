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
    private bool wasTouchingObstacle;
    private const int StartingLives = 5;
    private int lives;
    private int maxLives;
    private Texture2D fullHeart;
    private Texture2D emptyHeart;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        gfxRenderer = GetComponentInChildren<MeshRenderer>();

        lives = StartingLives;
        maxLives = StartingLives;
        fullHeart = CreateHeartTexture(true);
        emptyHeart = CreateHeartTexture(false);
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
        
        if (touchingObstacle && !wasTouchingObstacle && lives > 0)
        {
            lives--;
        }
        wasTouchingObstacle = touchingObstacle;
    }

    private void OnGUI()
    {
        int heartSize = 40;
        int spacing = 6;
        int margin = 15;
        int totalWidth = maxLives * heartSize + (maxLives - 1) * spacing;
        int startX = Screen.width - margin - totalWidth;

        for (int i = 0; i < maxLives; i++)
        {
            Texture2D tex = i < lives ? fullHeart : emptyHeart;
            Rect r = new Rect(startX + i * (heartSize + spacing), margin, heartSize, heartSize);
            GUI.DrawTexture(r, tex);
        }
    }

    private Texture2D CreateHeartTexture(bool filled)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color red = Color.red;
        Color clear = new Color(0f, 0f, 0f, 0f);

        bool[,] inside = new bool[size, size];
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                float x = (i + 0.5f) / size * 2.6f - 1.3f;
                float y = (j + 0.5f) / size * 2.6f - 1.3f;
                float a = x * x + y * y - 1f;
                float f = a * a * a - x * x * y * y * y;
                inside[i, j] = f <= 0f;
            }
        }

        int thickness = Mathf.Max(2, size / 12);
        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < size; i++)
            {
                Color c = clear;
                if (inside[i, j])
                {
                    if (filled)
                    {
                        c = red;
                    }
                    else
                    {
                        bool border = false;
                        for (int dj = -thickness; dj <= thickness && !border; dj++)
                        {
                            for (int di = -thickness; di <= thickness && !border; di++)
                            {
                                int ni = i + di;
                                int nj = j + dj;
                                if (ni < 0 || nj < 0 || ni >= size || nj >= size || !inside[ni, nj])
                                    border = true;
                            }
                        }
                        if (border) c = red;
                    }
                }
                tex.SetPixel(i, j, c);
            }
        }

        tex.Apply();
        return tex;
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
