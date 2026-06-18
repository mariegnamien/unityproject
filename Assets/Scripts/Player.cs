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
    private const int StartingLives = 2;
    private int lives;
    private int maxLives;
    private Texture2D fullHeart;
    private Texture2D emptyHeart;

    [Header("Configuration du Fantôme")]
    public GameObject follower;
    public float followerSpacing = 0.25f;
    public float followerAlpha = 0.5f;
    public float followerDelay = 0.3f;
    public float followerHeightOffset = 0.6f;
    private readonly List<Transform> followers = new List<Transform>();

    private void SpawnFollower()
    {
        if (follower == null) return;

        GameObject root = new GameObject("Follower");
        GameObject visual = Instantiate(follower);

        visual.transform.SetParent(root.transform, false);

        root.transform.position = transform.position;
        root.transform.rotation = transform.rotation;

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
        controller = GetComponent<CharacterController>();
        gfxRenderer = GetComponentInChildren<Renderer>();

        lives = StartingLives;
        maxLives = StartingLives;
        fullHeart = CreateHeartTexture(true);
        emptyHeart = CreateHeartTexture(false);
    }

    void Update()
    {
        direction.z = forwardSpeed;
        direction.y += gravity * Time.deltaTime;
        if (controller.isGrounded)
        {
            direction.y = 0;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            else
            {
                direction.y += gravity * Time.deltaTime;
            }
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

        Vector3 targetPosition = transform.position;
        if (desiredLane == 2)
            targetPosition.x = laneDistance;
        else if (desiredLane == 0)
            targetPosition.x = -laneDistance;
        else
            targetPosition.x = 0;

        direction.x = (targetPosition.x - transform.position.x) * 10f;
    }

    private void FixedUpdate()
    {
        touchingObstacle = false;
        controller.Move(direction * Time.fixedDeltaTime);

        if (touchingObstacle && !wasTouchingObstacle && lives > 0)
        {
            lives--;
            if (followers.Count == 0)
                SpawnFollower();
        }
        wasTouchingObstacle = touchingObstacle;

        RecordHistory();
        UpdateFollowers();
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