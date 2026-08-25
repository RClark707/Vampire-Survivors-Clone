using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float movementSpeed;

    private Rigidbody2D rb;
    [HideInInspector] // we want to make this public, but keep the inspector clear
    public Vector2 movementDirection;
    [HideInInspector]
    public float prevMoveX;
    [HideInInspector]
    public float prevMoveY;

    public GameObject currentChunk;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if (movementDirection.x != 0) { prevMoveX = movementDirection.x; }
        if (movementDirection.y != 0) { prevMoveY = movementDirection.y; }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(movementDirection.x * movementSpeed, movementDirection.y * movementSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Chunk" && currentChunk != collision.gameObject)
        {
            currentChunk = collision.gameObject;
        }
    }
}
