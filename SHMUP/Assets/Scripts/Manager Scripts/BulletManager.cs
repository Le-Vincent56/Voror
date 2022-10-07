using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject player;
    public List<GameObject> bulletList;
    public List<GameObject> destroyedBullets;

    bool canFire = true;
    float fireCooldown = 0.5f;

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
        // Fire cooldown checks
        if (!canFire)
        {
            fireCooldown -= Time.deltaTime;
        }

        if(fireCooldown <= 0)
        {
            canFire = true;
            fireCooldown = 0.5f;
        }

        // Check bounds
        CheckBounds();

        // Remove destroyed bullets
        RemoveDestroyedBullets();
    }

    public void SpawnBullet()
    {
        // Create bullet reference
        GameObject bulletReference;
        bulletReference = Instantiate(bulletPrefab);

        // Set bullet position
        bulletReference.GetComponent<Bullet>().bulletPosition = player.GetComponent<PlayerMove>().playerPosition;

        // Add the reference to the list of bullets
        bulletList.Add(bulletReference);
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
