using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] List<GameObject> villagerPrefabs;
    public List<GameObject> villagers;
    [SerializeField] List<GameObject> destroyedVillagers;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] GameObject score;

    Vector3 minSpawnPoint;
    Vector3 maxSpawnPoint;

    float maxVillagerCount = 3;

    [SerializeField] Camera cam;
    float camHeight;
    float camWidth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("Score");

        // Set bounds
        camHeight = cam.orthographicSize / 2;
        camWidth = camHeight * cam.aspect;

        minSpawnPoint = new Vector3(-camWidth, -camHeight, 0);
        maxSpawnPoint = new Vector3(camWidth, camHeight, 0);

        // Spawn the villagers
        SpawnVillagers();
    }

    // Update is called once per frame
    void Update()
    {
        // Blink on being damaged
        BlinkOnCollision();

        // Check for destroyed villagers and remove them
        CheckDestroyedVillagers();
        RemoveDestroyedVillagers();
    }

    /// <summary>
    /// Calculate the score bonuses for how many villagers are currently alive
    /// </summary>
    public void CalculateAliveBonuses()
    {
        // Add bonuses to the score depending on how many villagers are alive
        switch (villagers.Count)
        {
            case 0:
                break;

            case 1:
                score.GetComponent<ScoreMaster>().score += 5;
                break;

            case 2:
                score.GetComponent<ScoreMaster>().score += 10;
                break;

            case 3:
                score.GetComponent<ScoreMaster>().score += 20;
                break;
        }
    }

    /// <summary>
    /// Make colliding villagers blink
    /// </summary>
    public void BlinkOnCollision()
    {
        foreach (GameObject villager in villagers)
        {
            if (villager.GetComponent<VillagerStats>().collided)
            {
                if (villager.GetComponent<VillagerStats>().collisionTimer > 0)
                {
                    villager.GetComponent<SpriteRenderer>().color = Color.blue;
                    villager.GetComponent<VillagerStats>().collisionTimer -= Time.deltaTime;
                }
                else
                {
                    villager.GetComponent<VillagerStats>().collided = false;
                }
            }
            else
            {
                villager.GetComponent<VillagerStats>().collisionTimer = 1.5f;
                villager.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    /// <summary>
    /// Spawn the max amount of villagers
    /// </summary>
    public void SpawnVillagers()
    {
        for(int i = 0; i < maxVillagerCount; i++)
        {
            villagers.Add(CreateVillager());
        }
    }

    /// <summary>
    /// Creates a randomly generated villager at a random point within the inner area of the screen
    /// </summary>
    /// <returns>A randomly generated villager within a random point of the inner area of the screen</returns>
    public GameObject CreateVillager()
    {
        // Create a reference with a random spawnpoint and enemy type
        GameObject villager;
        Vector3 spawnPoint = PickSpawnPoint();

        // Instantiate the enemy
        villager = Instantiate(PickRandomVillagerSprite(), spawnPoint, Quaternion.identity, transform);

        // Return the enemy
        return villager;
    }

    /// <summary>
    /// Pick a random villager sprite
    /// </summary>
    /// <returns>A random villager object with a random sprite</returns>
    public GameObject PickRandomVillagerSprite()
    {
        GameObject randomVillager;

        // Pick random number
        float randVal = Random.Range(0f, 1f);
        if (randVal < 0.34f) // 33% male enemy
        {
            randomVillager = villagerPrefabs[0];
        }
        else if (randVal < 0.67f) // 33% female enemy
        {
            randomVillager = villagerPrefabs[1];
        }
        else // 33% old enemy
        {
            randomVillager = villagerPrefabs[2];
        }

        return randomVillager;
    }

    /// <summary>
    /// Pick a random spawn point within certain bounds
    /// </summary>
    /// <returns>A Vector3 that represents the spawn point</returns>
    public Vector3 PickSpawnPoint()
    {
        float randValX = Random.Range(minSpawnPoint.x, maxSpawnPoint.x);
        float randValY = Random.Range(minSpawnPoint.y, maxSpawnPoint.y);

        return new Vector3(randValX, randValY, 0);
    }

    /// <summary>
    /// Check to see if any villagers should be destroyed
    /// </summary>
    public void CheckDestroyedVillagers()
    {
        foreach (GameObject villager in villagers)
        {
            if (villager.GetComponent<VillagerStats>().currentHealth <= 0)
            {
                destroyedVillagers.Add(villager);
            }
        }
    }

    /// <summary>
    /// Remove any villagers that are meant to be destroyed
    /// </summary>
    public void RemoveDestroyedVillagers()
    {
        // Check the list of destroyed villagers
        if (destroyedVillagers.Count > 0)
        {
            // Compare the list of destroyed villagers with the total villagers
            for (int i = 0; i < destroyedVillagers.Count; i++)
            {
                for (int j = 0; j < villagers.Count; j++)
                {
                    // If any of the destroyed villagers are equal to any of the villagers
                    // in the villager List, remove them from the List
                    if (destroyedVillagers[i].Equals(villagers[j]))
                    {
                        Destroy(villagers[j]);
                        villagers.Remove(destroyedVillagers[i]);
                    }
                }
            }

            // Clear the destroyed villager List
            destroyedVillagers.Clear();
        }
    }
}
