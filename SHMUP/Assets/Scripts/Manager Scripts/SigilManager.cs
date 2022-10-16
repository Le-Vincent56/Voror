using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SigilManager : MonoBehaviour
{
    [SerializeField] List<GameObject> sigilPrefabs;
    public List<GameObject> sigils;
    public List<GameObject> destroyedSigils;
    [SerializeField] CollisionManager collisionManager;
    [SerializeField] BulletManager bulletManager;
    [SerializeField] GameObject player;
    [SerializeField] VillagerManager villagerManager;

    [SerializeField] bool canSpawnSigil = false;
    [SerializeField] float maxSigilCount = 1;
    public float sigilsSpawnedThisRound = 0;
    [SerializeField] float maxSigilsPerRound = 3;
    [SerializeField] float sigilSpawnTimer = 10f;

    [SerializeField] float hermodTimer = 8f;
    [SerializeField] float lokiTimer = 5f;

    Vector3 minSpawnPoint;
    Vector3 maxSpawnPoint;

    [SerializeField] Camera cam;
    float camHeight;
    float camWidth;

    [SerializeField] bool noneActive = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set bounds
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        minSpawnPoint = new Vector3(-camWidth, -camHeight, 0);
        maxSpawnPoint = new Vector3(camWidth, camHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Activate any lasting sigil effects
        SigilEffects();

        // Remove destroyed sigils if they are all deactivated
        if (noneActive)
        {
            RemoveDestroyedSigils();
        }
        
    }

    /// <summary>
    /// Activate any sigil effects
    /// </summary>
    public void SigilEffects()
    {
        foreach(GameObject sigil in sigils)
        {
            switch (sigil.GetComponent<Sigil>().sigilName)
            {
                // If the Hermod sigil is active, gain movement speed
                case "Hermod":
                    if (sigil.GetComponent<Sigil>().active)
                    {
                        if (hermodTimer > 0)
                        {
                            // For the amount of time, add the effect
                            hermodTimer -= Time.deltaTime;
                            player.GetComponent<PlayerMove>().speed = 10f;
                        }
                        else
                        {
                            // Once time runs out, de-activate and revert the effects
                            sigil.GetComponent<Sigil>().active = false;
                            hermodTimer = 8f;
                            player.GetComponent<PlayerMove>().speed = 5f;
                        }
                        noneActive = false;

                    } else
                    {
                        noneActive = true;
                    }
                    break;

                // If the Loki sigil is active, shoot faster and have a stronger bullet
                case "Loki":
                    if (sigil.GetComponent<Sigil>().active)
                    {
                        // For the amount of time, add the effect
                        if (lokiTimer > 0)
                        {
                            lokiTimer -= Time.deltaTime;
                            bulletManager.lokiFire = true;
                        }
                        else
                        {
                            // Once time runs out, de-activate and revert the effects
                            sigil.GetComponent<Sigil>().active = false;
                            lokiTimer = 8f;
                            bulletManager.lokiFire = false;
                        }
                        noneActive = false;

                    } else
                    {
                        noneActive = true;
                    }
                    
                    break;

                // If the Thor sigil is active, activate the storm
                case "Thor":
                    if (sigil.GetComponent<Sigil>().active)
                    {
                        // If the storm finished, de-activate the effect
                        if (player.GetComponent<Storm>().stormFinished)
                        {
                            sigil.GetComponent<Sigil>().active = false;
                            noneActive = true;
                            player.GetComponent<Storm>().active = false;
                        }
                        else
                        {
                            // Otherwise, keep the effect going
                            noneActive = false;
                            player.GetComponent<Storm>().active = true;
                        }
                    }
                    break;

                case "Eir":
                    if (sigil.GetComponent<Sigil>().active)
                    {
                        noneActive = true;

                        // Fully heal the player and the villagers
                        foreach (GameObject villager in villagerManager.villagers)
                        {
                            villager.GetComponent<VillagerStats>().currentHealth = villager.GetComponent<VillagerStats>().maxHealth;
                        }

                        player.GetComponent<PlayerStats>().currentHealth = player.GetComponent<PlayerStats>().maxHealth;

                        sigil.GetComponent<Sigil>().active = false;
                        noneActive = true;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Spawn a sigil if possible
    /// </summary>
    public void SpawnSigil()
    {
        // Check if a sigil can be spawned - less than max are on the screen, less than the maximum per round has spawned, and the sigil timer has reached 0
        if (sigils.Count < maxSigilCount && sigilsSpawnedThisRound < maxSigilsPerRound && canSpawnSigil)
        {
            // Add sigil
            sigils.Add(CreateSigil());
            sigilsSpawnedThisRound++;
            canSpawnSigil = false;
        } else if (!canSpawnSigil)
        {
            if (sigilSpawnTimer > 0) // Check and reset timer
            {
                sigilSpawnTimer -= Time.deltaTime;
            }
            else
            {
                sigilSpawnTimer = 10f;
                canSpawnSigil = true;
            }
        }
    }

    /// <summary>
    ///  Create a sigil with a random sigil prefab
    /// </summary>
    /// <returns>A random sigil</returns>
    public GameObject CreateSigil()
    {
        // Create a reference with a random spawnpoint and enemy type
        GameObject sigil;
        Vector3 spawnPoint = PickSpawnPoint();

        // Instantiate the enemy
        sigil = Instantiate(PickRandomSigilSprite(), spawnPoint, Quaternion.identity, transform);

        // Return the enemy
        return sigil;
    }

    /// <summary>
    /// Pick a random sigil sprite
    /// </summary>
    /// <returns>A random sigil sprite</returns>
    public GameObject PickRandomSigilSprite()
    {
        GameObject randomSigil;

        // Pick random number
        float randVal = Random.Range(0f, 1f);
        if (randVal < 0.15f) // 15% Hermod sigil
        {
            randomSigil = sigilPrefabs[0];
        }
        else if (randVal < 0.46f) // 30% Loki sigil
        {
            randomSigil = sigilPrefabs[1];
        }
        else if(randVal < 0.76)// 30% Thor sigil
        {
            randomSigil = sigilPrefabs[2];
        } else // 25% Eir sigil
        {
            randomSigil = sigilPrefabs[3];
        }

        return randomSigil;
    }

    /// <summary>
    /// Pick a random spawn point within camera bounds
    /// </summary>
    /// <returns>A random spawn point within camera bounds</returns>
    public Vector3 PickSpawnPoint()
    {
        float randValX = Random.Range(minSpawnPoint.x, maxSpawnPoint.x);
        float randValY = Random.Range(minSpawnPoint.y, maxSpawnPoint.y);

        return new Vector3(randValX, randValY, 0);
    }

    /// <summary>
    /// Remove sigils that should be destroyed from the list of sigils
    /// </summary>
    public void RemoveDestroyedSigils()
    {
        // Check the list of destroyed sigils
        if (destroyedSigils.Count > 0)
        {
            // Compare the list of destroyed sigils with the total sigils
            for (int i = 0; i < destroyedSigils.Count; i++)
            {
                for (int j = 0; j < sigils.Count; j++)
                {
                    // If any of the destroyed sigils are equal to any of the sigils
                    // in the sigil List, remove them from the List
                    if (destroyedSigils[i].Equals(sigils[j]))
                    {
                        Destroy(sigils[j]);
                        sigils.Remove(destroyedSigils[i]);
                    }
                }
            }

            // Clear the destroyed sigils List
            destroyedSigils.Clear();
        }
    }
}
