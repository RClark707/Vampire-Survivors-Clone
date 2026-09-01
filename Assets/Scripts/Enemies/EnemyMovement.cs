using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Enemy enemy;
    Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, enemy.movementSpeed * Time.deltaTime);
        CheckEnemyPlayerDistance();
    }

    void CheckEnemyPlayerDistance()
    {
        float separation = (playerTransform.position - transform.position).magnitude;

        if (separation >= 50f) // is the enemy beyond rendering?
        {
            Destroy(gameObject);
        }

        // we can check here to see if this is the nearest enemy to the player too
    }
}
