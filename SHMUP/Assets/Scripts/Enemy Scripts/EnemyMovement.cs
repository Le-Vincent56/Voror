using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    #region FIELDS
    Vector3 monsterPosition = new Vector3(0, 0, 0);
    Vector3 direction = new Vector3(1, 0, 0);
    Vector3 velocity = new Vector3(0, 0, 0);

    public float speed = 5f;

    [SerializeField] GameObject player;
    Vector3 playerPosition;
    Vector3 distanceToPlayer = new Vector3(0, 0, 0);
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        monsterPosition = transform.position;
        playerPosition = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Track player
        monsterPosition = transform.position;
        playerPosition = player.transform.position;
        distanceToPlayer = playerPosition - monsterPosition;
        direction = distanceToPlayer.normalized;

        // Calculate velocity
        velocity = direction * speed * Time.deltaTime;

        // Move towards player
        monsterPosition += velocity;
        transform.position = monsterPosition;
    }
}
