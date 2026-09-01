using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Player player;
    Rigidbody2D rb;

    [HideInInspector]
    public Vector2 movementDirection;
    [HideInInspector]
    public float prevMoveX;
    [HideInInspector]
    public float prevMoveY; // not used by anything
    [HideInInspector]
    public Vector2 lastMoveDirection;
    [HideInInspector]
    public MapController mc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mc = FindAnyObjectByType<MapController>();
        lastMoveDirection = new Vector2(1, 0f);
        player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    // Called at regular intervals, and doesn't sometimes pause like the regular update. Necessary for physics motion
    private void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector2(moveX, moveY);

        if (movementDirection.x != 0)
        {
            prevMoveX = movementDirection.x;
            lastMoveDirection = new Vector2(prevMoveX, 0f);
        }
        if (movementDirection.y != 0)
        {
            prevMoveY = movementDirection.y;
            lastMoveDirection = new Vector2(0f, prevMoveY);
        }
        if (movementDirection.x != 0 && movementDirection.y != 0)
        {
            lastMoveDirection = new Vector2(prevMoveX, prevMoveY);
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(movementDirection.x * player.movementSpeed, movementDirection.y * player.movementSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Used by the Map Controller to determine which chunk the player is in
        if (collision.CompareTag("Chunk") && mc.currentChunk != collision.gameObject)
        {
            mc.currentChunk = collision.gameObject;
        }
    }
}
