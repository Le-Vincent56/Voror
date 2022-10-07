using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> enemyPrefabs;
    public List<GameObject> enemies;
    [SerializeField] List<GameObject> destroyedEnemies;
    [SerializeField] GameObject HUD;
    [SerializeField] Camera cam;

    float maxEnemyCount = 5;
    float spawnCooldown = 3f;
    bool canSpawnEnemy = true;


    float camHeight;
    float camWidth;

    Vector3 minSpawnPoint;
    Vector3 maxSpawnPoint;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        minSpawnPoint = new Vector3(-camWidth, -camHeight, 0);
        maxSpawnPoint = new Vector3(camWidth, camHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn Enemies
        SpawnEnemy();

        // Check for collision color changing
        BlinkOnCollision();

        // Check for destroyed enemies and remove them
        CheckDestroy();
        RemoveDestroyedEnemies();
    }

    public void BlinkOnCollision()
    {
        foreach(GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyStats>().collided)
            {
                if(enemy.GetComponent<EnemyStats>().colorTimer > 0)
                {
                    enemy.GetComponent<SpriteRenderer>().color = Color.red;
                    enemy.GetComponent<EnemyStats>().colorTimer -= Time.deltaTime;
                } else
                {
                    enemy.GetComponent<EnemyStats>().collided = false;
                }
            } else
            {
                enemy.GetComponent<EnemyStats>().colorTimer = 0.5f;
                enemy.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// Spawn an enemy
    /// </summary>
    public void SpawnEnemy()
    {
        // If an enemy can be spawned, spawn an enemy
        // If not, wait until there are less enemies than the max amount,
        // or until the cooldown resets
        if(enemies.Count < maxEnemyCount && canSpawnEnemy)
        {
            enemies.Add(CreateEnemy());
            canSpawnEnemy = false;
        } else if (!canSpawnEnemy)
        {
            if(spawnCooldown > 0)
            {
                spawnCooldown -= Time.deltaTime;
            } else
            {
                spawnCooldown = 3f;
                canSpawnEnemy = true;
            }
            
        }
    }

    /// <summary>
    /// Instantiate an enemy within a value
    /// </summary>
    /// <returns>A GameObject representing the enemy</returns>
    public GameObject CreateEnemy()
    {
        // Create a reference with a random spawnpoint and enemy type
        GameObject enemy;
        Vector3 spawnPoint = PickSpawnPoint();

        // Instantiate the enemy
        enemy = Instantiate(PickRandomEnemy(), spawnPoint, Quaternion.identity, transform);

        // Return the enemy
        return enemy;
    }

    /// <summary>
    /// Generates a random spawnpoint given a range
    /// </summary>
    /// <returns>A new vector representing the random spawnpoint</returns>
    public Vector3 PickSpawnPoint()
    {
        float randValX = Random.Range(minSpawnPoint.x, maxSpawnPoint.x);
        float randValY = Random.Range(minSpawnPoint.y, maxSpawnPoint.y);

        return new Vector3(randValX, randValY, 0);
    }

    /// <summary>
    /// Pick s a Random Enemy from a list of Prefabs
    /// </summary>
    /// <returns>A randomly chosen enemy</returns>
    public GameObject PickRandomEnemy()
    {
        GameObject randomEnemy;

        // Pick random number
        float randVal = Random.Range(0f, 1f);
        if (randVal < 0.15f) // 15% quick enemy
        {
            randomEnemy = enemyPrefabs[0];
        }
        else if (randVal < 0.29f) // 15% tanky enemy
        {
            randomEnemy = enemyPrefabs[1];
        }
        else if (randVal < 0.39f) // 10% strong enemy
        {
            randomEnemy = enemyPrefabs[2];
        }
        else if (randVal < 0.49f) // 10% ranged enemy
        {
            randomEnemy = enemyPrefabs[3];
        }
        else // 50% standard enemy
        {
            randomEnemy = enemyPrefabs[4];
        }

        return randomEnemy;
    }

    /// <summary>
    /// Checks to see if an enemy should be destroyed
    /// </summary>
    public void CheckDestroy()
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy.GetComponent<EnemyStats>().currentHealth <= 0)
            {
                destroyedEnemies.Add(enemy);
                HUD.GetComponent<HUDManager>().score += enemy.GetComponent<EnemyStats>().value;
            }
        }
    }

    /// <summary>
    /// Removes the destroyed enemies from the list of enemies
    /// </summary>
    public void RemoveDestroyedEnemies()
    {
        // Check the list of destroyed bullets
        if (destroyedEnemies.Count > 0)
        {
            // Compare the list of destroyed bullets with the total bullets
            for (int i = 0; i < destroyedEnemies.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // If any of the destroyed bullets are equal to any of the bullets
                    // in the bulletList, remove them from the list
                    if (destroyedEnemies[i].Equals(enemies[j]))
                    {
                        Destroy(enemies[j]);
                        enemies.Remove(destroyedEnemies[i]);
                    }
                }
            }

            // Clear the destroyed bullets list
            destroyedEnemies.Clear();
        }
    }
}
