using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] List<GameObject> bulletPrefabs;
    [SerializeField] GameObject enemyBulletPrefab;
    [SerializeField] GameObject player;
    [SerializeField] EnemyManager enemyManager;
    public List<GameObject> bulletList;
    public List<GameObject> destroyedBullets;
    public List<GameObject> enemyBulletList;
    public List<GameObject> destroyedEnemyBullets;
    public int numOfBulletSpawn = 1;

    public bool lokiFire = false;
    [SerializeField] bool canFire = true;
    public float fireCooldown = 0.35f;

    [SerializeField] Camera cam;
    float camHeight;
    float camWidth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        #region PLAYER BULLETS
        // Fire cooldown checks
        if (!canFire)
        {
            fireCooldown -= Time.deltaTime;
        }

        if (fireCooldown <= 0)
        {
            canFire = true;
            if (lokiFire)
            {
                fireCooldown = 0.15f;
            } else
            {
                fireCooldown = 0.35f;
            }
        }

        if (player.GetComponent<Storm>().active)
        {
            canFire = false;
        }

        // Check bounds
        CheckBounds();

        // Remove destroyed bullets
        RemoveDestroyedBullets();
        #endregion

        #region ENEMY BULLETS
        foreach(GameObject enemy in enemyManager.enemies)
        {
            if (enemy.GetComponent<EnemyStats>().ranged && enemy.GetComponent<EnemyStats>().canBeDamaged)
            {
                if (!enemy.GetComponent<EnemyStats>().canFire)
                {
                    enemy.GetComponent<EnemyStats>().fireCooldown -= Time.deltaTime;
                } else
                {
                    SpawnEnemyBullet(enemy);
                    enemy.GetComponent<EnemyStats>().canFire = false;
                }

                if(enemy.GetComponent<EnemyStats>().fireCooldown <= 0)
                {
                    enemy.GetComponent<EnemyStats>().canFire = true;
                    enemy.GetComponent<EnemyStats>().fireCooldown = 2f;
                }
            }
        }

        // Check bounds
        CheckEnemyBounds();

        // Remove enemy bullets
        RemoveDestroyedEnemyBullets();
        #endregion
    }

    /// <summary>
    /// Spawn a player bullet
    /// </summary>
    public void SpawnBullet()
    {
        // Create bullet reference
        GameObject bulletReference;

        if (lokiFire)
        {
            bulletReference = Instantiate(bulletPrefabs[1]);
        }
        else
        {
            bulletReference = Instantiate(bulletPrefabs[0]);
        }

        // Set bullet position
        bulletReference.GetComponent<Bullet>().bulletPosition = player.GetComponent<PlayerMove>().playerPosition;

        // Add the reference to the list of bullets
        bulletList.Add(bulletReference);
    }

    /// <summary>
    /// Spawn an enemy bullet
    /// </summary>
    /// <param name="enemy">The enemy firing the bullet</param>
    public void SpawnEnemyBullet(GameObject enemy)
    {
        // Create bullet reference
        GameObject enemyBulletReference;

        enemyBulletReference = Instantiate(enemyBulletPrefab);

        // Set bullet position
        enemyBulletReference.GetComponent<EnemyBullet>().enemyBulletPosition = enemy.GetComponent<EnemyMovement>().monsterPosition;

        // Add the reference to the list of bullets
        enemyBulletList.Add(enemyBulletReference);
    }

    /// <summary>
    /// Destroys a bullet if it leaves the screen
    /// </summary>
    public void CheckBounds()
    {
        foreach(GameObject bullet in bulletList)
        {
            if (bullet.transform.position.x - bullet.GetComponent<SpriteRenderer>().size.x > camWidth)
            {
                destroyedBullets.Add(bullet);
            } else if (bullet.transform.position.x + bullet.GetComponent<SpriteRenderer>().size.x < -camWidth)
            {
                destroyedBullets.Add(bullet);
            }

            if (bullet.transform.position.y - bullet.GetComponent<SpriteRenderer>().size.x > camHeight)
            {
                destroyedBullets.Add(bullet);
            }
            else if (bullet.transform.position.y + bullet.GetComponent<SpriteRenderer>().size.x < -camHeight)
            {
                destroyedBullets.Add(bullet);
            }
        }
    }

    /// <summary>
    /// Destroys an enemy bullet if it leaves the screen
    /// </summary>
    public void CheckEnemyBounds()
    {
        foreach (GameObject bullet in enemyBulletList)
        {
            if (bullet.transform.position.x - bullet.GetComponent<SpriteRenderer>().size.x > camWidth)
            {
                destroyedEnemyBullets.Add(bullet);
            }
            else if (bullet.transform.position.x + bullet.GetComponent<SpriteRenderer>().size.x < -camWidth)
            {
                destroyedEnemyBullets.Add(bullet);
            }

            if (bullet.transform.position.y - bullet.GetComponent<SpriteRenderer>().size.x > camHeight)
            {
                destroyedEnemyBullets.Add(bullet);
            }
            else if (bullet.transform.position.y + bullet.GetComponent<SpriteRenderer>().size.x < -camHeight)
            {
                destroyedEnemyBullets.Add(bullet);
            }
        }
    }

    /// <summary>
    /// Remove bullets that are meant to be destroyed
    /// </summary>
    public void RemoveDestroyedBullets()
    {
        // Check the list of destroyed bullets
        if(destroyedBullets.Count > 0)
        {
            // Compare the list of destroyed bullets with the total bullets
            for(int i = 0; i < destroyedBullets.Count; i++)
            {
                for(int j = 0; j < bulletList.Count; j++)
                {
                    // If any of the destroyed bullets are equal to any of the bullets
                    // in the bulletList, remove them from the list
                    if (destroyedBullets[i].Equals(bulletList[j]))
                    {
                        Destroy(bulletList[j]);
                        bulletList.Remove(destroyedBullets[i]);
                    }
                }
            }

            // Clear the destroyed bullets list
            destroyedBullets.Clear();
        }
    }

    /// <summary>
    /// Remove enemy bullets that are meant to be destroyed
    /// </summary>
    public void RemoveDestroyedEnemyBullets()
    {
        // Check the list of destroyed bullets
        if (destroyedEnemyBullets.Count > 0)
        {
            // Compare the list of destroyed bullets with the total bullets
            for (int i = 0; i < destroyedEnemyBullets.Count; i++)
            {
                for (int j = 0; j < enemyBulletList.Count; j++)
                {
                    // If any of the destroyed bullets are equal to any of the bullets
                    // in the bulletList, remove them from the list
                    if (destroyedEnemyBullets[i].Equals(enemyBulletList[j]))
                    {
                        Destroy(enemyBulletList[j]);
                        enemyBulletList.Remove(destroyedEnemyBullets[i]);
                    }
                }
            }

            // Clear the destroyed bullets list
            destroyedEnemyBullets.Clear();
        }
    }

    /// <summary>
    /// Fire a bullet
    /// </summary>
    /// <param name="context">Input context</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        // Check if the player can fire
        if (canFire)
        {
            // Spawn a bullet and trigger cooldown
            SpawnBullet();
            canFire = false;
        }
    }
}
