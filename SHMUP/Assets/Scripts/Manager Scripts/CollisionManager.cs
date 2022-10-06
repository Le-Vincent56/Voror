using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] List<GameObject> enemies;
    [SerializeField] GameObject player;

    public bool recentCollision = false;
    public float collisionTimer = 3f;
    public float blinkingTime = 0f;
    public float blinkingPeriod = 0.5f;


    // Update is called once per frame
    void Update()
    {
        // Check if there has been enough time to check for another collision
        if(collisionTimer <= 0)
        {
            recentCollision = false;
            collisionTimer = 3f;
            blinkingTime = 0f;
        }

        // Check if there's been a recent collision
        if (recentCollision == true)
        {
            collisionTimer -= Time.deltaTime;

            // Give the player a blinking red effect
            blinkingTime += Time.deltaTime;

            if (blinkingTime < blinkingPeriod)
            {
                player.GetComponent<SpriteRenderer>().color = Color.red;
            } else
            {
                player.GetComponent<SpriteRenderer>().color = Color.white;
            }

            if(blinkingTime >= 2 * blinkingPeriod)
            {
                blinkingTime -= (2 * blinkingPeriod);
            }

        } else
        {
            // Set default color
            player.GetComponent<SpriteRenderer>().color = Color.white;
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        // Check each object within the list
        foreach (GameObject enemy in enemies)
        {
            // If there are no current collisions, check for collisions
            if (recentCollision == false)
            {
                if (AABBCollision(player, enemy))
                {
                    recentCollision = true;

                    // Change color if colliding
                    player.GetComponent<PlayerStats>().currentHealth -= enemy.GetComponent<EnemyStats>().damage;
                    player.GetComponent<SpriteRenderer>().color = Color.red;
                    enemy.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }

    /// <summary>
    /// Check collisions using Axis-Aligned Bounding Boxes
    /// </summary>
    /// <param name="object1">The first collider</param>
    /// <param name="object2">The second collider</param>
    /// <returns>A boolean stating whether there was a collision or not</returns>
    public bool AABBCollision(GameObject object1, GameObject object2)
    {
        // Get min and max bounds
        float BMaxX = object2.GetComponent<SpriteRenderer>().bounds.max.x;
        float BMinX = object2.GetComponent<SpriteRenderer>().bounds.min.x;
        float BMaxY = object2.GetComponent<SpriteRenderer>().bounds.max.y;
        float BMinY = object2.GetComponent<SpriteRenderer>().bounds.min.y;

        float AMaxX = object1.GetComponent<SpriteRenderer>().bounds.max.x;
        float AMinX = object1.GetComponent<SpriteRenderer>().bounds.min.x;
        float AMaxY = object1.GetComponent<SpriteRenderer>().bounds.max.y;
        float AMinY = object1.GetComponent<SpriteRenderer>().bounds.min.y;

        // If all conditions are met, return true, otherwise return false
        if (BMinX < AMaxX && BMaxX > AMinX && BMaxY > AMinY && BMinY < AMaxY)
        {
            return true;
        }
        else return false;
    }
}
