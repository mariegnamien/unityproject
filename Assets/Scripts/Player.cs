using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    private float invincibilityTimer = 0f;

    private const int StartingLives = 2;
    private int lives;
    private int maxLives;
    private Texture2D fullHeart;
    private Texture2D emptyHeart;

    [Header("Écran de Fin")]
    public GameObject gameOverUI;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private Animator animator;
    private float originalHeight;
    private Vector3 originalCenter;
    public float slideSpeed = 18f;
    public float slideDuration = 0.8f;
    private bool isSliding;
    private bool isJumping;
    private int normalLayer;
    private int slideLayer;
    private int jumpLayer;

    [Header("Configuration du Fantôme")]
    public GameObject follower;
    public float followerSpacing = 0.25f;
    public float followerAlpha = 0.5f;
    public float followerDelay = 0.3f;
    public float followerHeightOffset = 1.5f;
    private readonly List<Transform> followers = new List<Transform>();

    private void SpawnFollower()
    {
        if (follower == null) return;

        GameObject root = new GameObject("Follower");
        GameObject visual = Instantiate(follower);

        visual.transform.SetParent(root.transform, false);

        root.transform.position = transform.position;
        root.transform.rotation = transform.rotation;

        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.material.HasProperty("_Color"))
            {
                Color c = r.material.color;
                c.a = followerAlpha;
                r.material.color = c;
            }
        }

        followers.Add(root.transform);
    }

    private struct Snapshot
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
    }
    private readonly List<Snapshot> history = new List<Snapshot>();

    void Start()
    {
        Time.timeScale = 1f;

        controller = GetComponent<CharacterController>();
        gfxRenderer = GetComponentInChildren<Renderer>();
        animator = GetComponentInChildren<Animator>();

        normalLayer = gameObject.layer;
        slideLayer = LayerMask.NameToLayer("SlidePlayer");
        jumpLayer = LayerMask.NameToLayer("JumpPlayer");

        if (slideLayer == -1) Debug.LogError("SlidePlayer layer introuvable !");
        if (jumpLayer == -1) Debug.LogError("JumpPlayer layer introuvable !");

        lives = StartingLives;
        maxLives = StartingLives;
        fullHeart = CreateHeartTexture(true);
        emptyHeart = CreateHeartTexture(false);
        originalHeight = controller.height;
        originalCenter = controller.center;

        // Masquer l'écran de fin au début du jeu
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        direction.z = isSliding ? slideSpeed : forwardSpeed;

        if (controller.isGrounded)
        {
            direction.y = -1f;
            isJumping = false;

            if (!isSliding)
                gameObject.layer = normalLayer;

            animator.SetBool("IsGrounded", true);

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetBool("IsGrounded", false);
                Jump();
            }
        }
        else
        {
            direction.y += gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && !isSliding && controller.isGrounded)
        {
            StartCoroutine(Slide());
        }

        Vector3 targetPosition = transform.position;
        if (desiredLane == 2)
            targetPosition.x = laneDistance;
        else if (desiredLane == 0)
            targetPosition.x = -laneDistance;
        else
            targetPosition.x = 0;

        direction.x = (targetPosition.x - transform.position.x) * 10f;

        if (touchingObstacle && !wasTouchingObstacle)
        {
            if (Time.time >= invincibilityTimer && lives > 0)
            {
                lives--; // On enlève 1 vie
                invincibilityTimer = Time.time + 1.2f;

                if (followers.Count == 0)
                {
                    SpawnFollower();
                }

                if (lives <= 0)
                {
                    GameOver();
                }
            }
        }
        wasTouchingObstacle = touchingObstacle;
        touchingObstacle = false;
    }

    private void GameOver()
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. On fige le temps du jeu
        Time.timeScale = 0f;
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetTrigger("Slide");
        gameObject.layer = slideLayer;

        controller.height = originalHeight * 0.5f;
        controller.center = new Vector3(originalCenter.x, controller.height / 2f, originalCenter.z);

        yield return new WaitForSeconds(slideDuration);

        controller.height = originalHeight;
        controller.center = originalCenter;

        gameObject.layer = normalLayer;
        isSliding = false;
    }

    private void Jump()
    {
        isJumping = true;
        direction.y = jumpForce;
        animator.SetTrigger("Jump");
        gameObject.layer = jumpLayer;
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return;

        controller.Move(direction * Time.fixedDeltaTime);
        RecordHistory();
        UpdateFollowers();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isSliding || isJumping) return;

        if (hit.normal.y < 0.5f)
        {
            touchingObstacle = true;
        }
    }

    private void RecordHistory()
    {
        history.Add(new Snapshot
        {
            time = Time.time,
            position = transform.position,
            rotation = transform.rotation
        });

        float oldestNeeded = Time.time - followerDelay * (followers.Count + 1) - 0.5f;
        while (history.Count > 1 && history[0].time < oldestNeeded)
            history.RemoveAt(0);
    }

    private void UpdateFollowers()
    {
        for (int i = 0; i < followers.Count; i++)
        {
            float targetTime = Time.time - followerDelay * (i + 1);
            Snapshot snap = SampleHistory(targetTime);

            Vector3 pos = snap.position;
            pos.z -= (i + 1) * followerSpacing;
            pos.y += followerHeightOffset;
            followers[i].position = pos;
            followers[i].rotation = snap.rotation;
        }
    }

    private Snapshot SampleHistory(float targetTime)
    {
        if (history.Count == 0)
            return new Snapshot { time = targetTime, position = transform.position, rotation = transform.rotation };

        if (targetTime <= history[0].time)
            return history[0];

        for (int i = history.Count - 1; i >= 0; i--)
        {
            if (history[i].time <= targetTime)
            {
                if (i == history.Count - 1)
                    return history[i];

                Snapshot a = history[i];
                Snapshot b = history[i + 1];
                float span = b.time - a.time;
                float t = span > 0f ? (targetTime - a.time) / span : 0f;
                return new Snapshot
                {
                    time = targetTime,
                    position = Vector3.Lerp(a.position, b.position, t),
                    rotation = Quaternion.Slerp(a.rotation, b.rotation, t)
                };
            }
        }
        return history[history.Count - 1];
    }

    private void OnGUI()
    {
        int heartSize = 40;
        int spacing = 6;
        int margin = 15;
        int totalWidth = maxLives * heartSize + (maxLives - 1) * spacing;
        int startStartX = Screen.width - margin - totalWidth;

        for (int i = 0; i < maxLives; i++)
        {
            Texture2D tex = i < lives ? fullHeart : emptyHeart;
            Rect r = new Rect(startStartX + i * (heartSize + spacing), margin, heartSize, heartSize);
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
                    if (filled) c = red;
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
}