using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] RoundManager roundManager;
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> enemyPrefabs;
    public List<GameObject> enemies;
    [SerializeField] List<GameObject> destroyedEnemies;
    [SerializeField] Camera cam;
    [SerializeField] CollisionManager collisionManager;
    [SerializeField] VillagerManager villagerManager;
    [SerializeField] GameObject score;
    public int maxEnemiesThisRound = 3;
    public int enemiesSpawnedThisRound = 0;
    public int baseEnemies = 1;
    public int bonusEnemiesPerRound = 1;

    public float spawnCooldown = 3f;
    [SerializeField] bool canSpawnEnemy = true;


    float camHeight;
    float camWidth;

    Vector3 minSpawnPoint;
    Vector3 maxSpawnPoint;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("Score");

        // Set bounds
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        minSpawnPoint = new Vector3(-camWidth, -camHeight, 0);
        maxSpawnPoint = new Vector3(camWidth, camHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Blink on being damaged
        BlinkOnCollision();

        // Check for destroyed enemies and remove them
        CheckDestroyedEnemies();
        RemoveDestroyedEnemies();
    }

    /// <summary>
    /// Blink the enemy on collision
    /// </summary>
    public void BlinkOnCollision()
    {
        foreach(GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyStats>().collided)
            {
                if (enemy.GetComponent<EnemyStats>().canBeDamaged)
                {
                    if (enemy.GetComponent<EnemyStats>().damageColorTimer > 0)
                    {
                        enemy.GetComponent<SpriteRenderer>().color = Color.red;
                        enemy.GetComponent<EnemyStats>().damageColorTimer -= Time.deltaTime;
                    }
                    else
                    {
                        enemy.GetComponent<EnemyStats>().collided = false;
                    }
                } else
                {
                    if (enemy.GetComponent<EnemyStats>().invincibleColorTimer > 0)
                    {
                        enemy.GetComponent<SpriteRenderer>().color = Color.gray;
                        enemy.GetComponent<EnemyStats>().invincibleColorTimer -= Time.deltaTime;
                    }
                    else
                    {
                        enemy.GetComponent<EnemyStats>().collided = false;
                    }
                }
            } else
            {
                enemy.GetComponent<EnemyStats>().invincibleColorTimer = 0.5f;
                enemy.GetComponent<EnemyStats>().damageColorTimer = 0.5f;
                enemy.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// Spawn an enemy
    /// </summary>
    public void SpawnStartingEnemies()
    {
        for (int i = 0; i < baseEnemies; i++)
        {
            enemies.Add(CreateEnemy());
            enemiesSpawnedThisRound++;
        }
    }

    public void SpawnEnemy()
    {
        // If an enemy can be spawned, spawn an enemy
        // If not, wait until there are less enemies than the max amount,
        // or until the cooldown resets
        if (enemiesSpawnedThisRound < maxEnemiesThisRound && canSpawnEnemy)
        {
            enemies.Add(CreateEnemy());
            enemiesSpawnedThisRound++;
            canSpawnEnemy = false;
        }
        else if (!canSpawnEnemy)
        {
            if (spawnCooldown > 0)
            {
                spawnCooldown -= Time.deltaTime;
            }
            else if(spawnCooldown < 0)
            {
                if(roundManager.roundNum < 3)
                {
                    spawnCooldown = 3f;
                } else if(roundManager.roundNum < 9)
                {
                    spawnCooldown = 2f;
                } else if(roundManager.roundNum >= 9)
                {
                    spawnCooldown = 1f;
                }
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
        Vector3 spawnPoint = Vector3.zero;
        bool validSpawnPoint = false;

        // Test if the spawn point is valid, if not, generate a new one
        while (!validSpawnPoint)
        {
            Vector3 temporarySpawnPoint = PickSpawnPoint();
            GameObject tempEnemy;
            tempEnemy = Instantiate(PickRandomEnemy(), temporarySpawnPoint, Quaternion.identity, transform);

            // A spawnpoint is valid depending if there is a villager collision
            foreach(GameObject villager in villagerManager.villagers)
            {
                if (collisionManager.AABBCollision(tempEnemy, villager))
                {
                    Destroy(tempEnemy);
                    validSpawnPoint = false;
                } else
                {
                    Destroy(tempEnemy);
                    spawnPoint = temporarySpawnPoint;
                    validSpawnPoint = true;
                }
            }
        }
        
        // Set the final enemy
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
    public void CheckDestroyedEnemies()
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy.GetComponent<EnemyStats>().currentHealth <= 0)
            {
                destroyedEnemies.Add(enemy);
                score.GetComponent<ScoreMaster>().score += enemy.GetComponent<EnemyStats>().value;
            }
        }
    }

    /// <summary>
    /// Removes the destroyed enemies from the list of enemies
    /// </summary>
    public void RemoveDestroyedEnemies()
    {
        // Check the list of destroyed enemies
        if (destroyedEnemies.Count > 0)
        {
            // Compare the list of destroyed enemies with the total enemies
            for (int i = 0; i < destroyedEnemies.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    // If any of the destroyed enemies are equal to any of the enemies
                    // in the enemy List, remove them from the List
                    if (destroyedEnemies[i].Equals(enemies[j]))
                    {
                        Destroy(enemies[j]);
                        enemies.Remove(destroyedEnemies[i]);
                    }
                }
            }

            // Clear the destroyed enemies List
            destroyedEnemies.Clear();
        }
    }
}
