using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] BulletManager bulletManager;
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> villagers;
    [SerializeField] VillagerManager villagerManager;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] SigilManager sigilManager;

    public bool recentCollision = false;
    public float collisionTimer = 3f;
    public float blinkingTime = 0f;
    public float blinkingPeriod = 0.5f;

    // Update is called once per frame
    void Update()
    {
        #region PLAYER COLLISIONS
        // Check if there has been enough time to check for another collision
        if (collisionTimer <= 0)
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
        }

        // Check each object within the list
        foreach (GameObject enemy in enemyManager.enemies)
        {
            if(enemy != null)
            {
                // If there are no current collisions, check for collisions
                if (recentCollision == false && enemy.GetComponent<EnemyStats>().canBeDamaged)
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
        #endregion

        #region PLAYER BULLET COLLISIONS
        // Check each bullet to see if it collides with an enemy
        foreach (GameObject bullet in bulletManager.bulletList)
        {
            foreach (GameObject enemy in enemyManager.enemies)
            {
                if(enemy != null)
                {
                    if (AABBCollision(bullet, enemy))
                    {
                        // Tell the enemy it has been collided with
                        enemy.GetComponent<EnemyStats>().collided = true;

                        // If it can, take damage
                        if (enemy.GetComponent<EnemyStats>().canBeDamaged)
                        {
                            enemy.GetComponent<EnemyStats>().currentHealth -= bullet.GetComponent<Bullet>().damage;
                        }

                        // Destroy the bullet
                        bulletManager.destroyedBullets.Add(bullet);
                    }
                }
            }
        }
        #endregion

        #region ENEMY BULLET COLLISIONS
        foreach (GameObject bullet in bulletManager.enemyBulletList)
        {
            foreach (GameObject villager in villagerManager.villagers)
            {
                if (villager != null)
                {
                    if (AABBCollision(bullet, villager))
                    {
                        // Tell the villager it has been collided with
                        villager.GetComponent<VillagerStats>().collided = true;

                        // Damage the villager
                        villager.GetComponent<VillagerStats>().currentHealth -= bullet.GetComponent<EnemyBullet>().damage;

                        // Destroy the bullet
                        bulletManager.destroyedEnemyBullets.Add(bullet);
                    }
                }
            }

            if(player != null && !recentCollision)
            {
                if(AABBCollision(bullet, player))
                {
                    // Trigger a recent collision
                    recentCollision = true;

                    // Damage the player
                    player.GetComponent<PlayerStats>().currentHealth -= bullet.GetComponent<EnemyBullet>().damage;
                    player.GetComponent<SpriteRenderer>().color = Color.red;

                    // Destroy the bullet
                    bulletManager.destroyedEnemyBullets.Add(bullet);
                }
            }
        }
        #endregion

        #region VILLAGER COLLISIONS
        // Check collisions between each villager and each enemy
        foreach (GameObject villager in villagerManager.villagers)
        {
            foreach (GameObject enemy in enemyManager.enemies)
            {
                if (enemy != null)
                {
                    if (!villager.GetComponent<VillagerStats>().collided)
                    {
                        if (AABBCollision(villager, enemy))
                        {
                            // Tell the villager it has experienced a collision and take damage
                            villager.GetComponent<VillagerStats>().collided = true;
                            villager.GetComponent<VillagerStats>().currentHealth -= enemy.GetComponent<EnemyStats>().damage;
                        }
                    }
                }
            }
        }
        #endregion

        #region SIGIL COLLISION
        foreach (GameObject sigil in sigilManager.sigils)
        {
            if(AABBCollision(player, sigil))
            {
                sigil.GetComponent<Sigil>().active = true;
                sigil.transform.position = new Vector3(999, 999, 99);
                sigilManager.destroyedSigils.Add(sigil);
            }
        }

        #region STORM COLLISION
        if (player.GetComponent<Storm>().active)
        {
            if(player.GetComponent<Storm>().stormState == StormState.Damaging)
            {
                foreach(GameObject enemy in enemyManager.enemies)
                {
                    if(enemy != null)
                    {
                        if(CircleCollision(enemy, player.GetComponent<Storm>().stormCrosshair))
                        {
                            enemy.GetComponent<EnemyStats>().currentHealth -= player.GetComponent<Storm>().stormDamage;
                        }
                    }
                }
            }
        }
        #endregion
        #endregion
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

    /// <summary>
    /// Detect collision between two GameObjects using Circles
    /// </summary>
    /// <param name="spaceship">The spaceship GameObject</param>
    /// <param name="asteroid">The asteroid GameObject</param>
    /// <returns>A boolean stating if the two GameObjects are colliding</returns>
    public bool CircleCollision(GameObject spaceship, GameObject asteroid)
    {
        // Get the radii of the two GameObjects
        Vector3 spaceshipRadiusVector = spaceship.GetComponent<SpriteRenderer>().bounds.max - spaceship.GetComponent<SpriteRenderer>().bounds.center;
        float spaceshipRadius = spaceshipRadiusVector.magnitude;

        Vector3 asteroidRadiusVector = asteroid.GetComponent<SpriteRenderer>().bounds.max - asteroid.GetComponent<SpriteRenderer>().bounds.center;
        float asteroidRadius = asteroidRadiusVector.magnitude;

        // Combine the radii
        float unitedRadiiSquared = Mathf.Pow(spaceshipRadius + asteroidRadius, 2);

        // Get distances between the centers of the two GameObjects
        Vector3 distance = asteroid.GetComponent<SpriteRenderer>().bounds.center - spaceship.GetComponent<SpriteRenderer>().bounds.center;
        float distanceF = distance.magnitude;
        float distanceSquared = Mathf.Pow(distanceF, 2);

        // If the distance between the two centers are less than the sum of their radii, return true
        // otherwise, return false
        if (distanceSquared < unitedRadiiSquared)
        {
            return true;
        }
        else return false;
    }
}
