using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BulletManager : MonoBehaviour
{
    #region FIELDS
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] List<GameObject> bullets;
    [SerializeField] GameObject player;
    List<Vector3> bulletPositions;
    List<Vector3> bulletDirections;
    List<Vector3> bulletVelocities;
    List<int> destroyedBulletIndexes = new List<int>();
    Vector3 playerPosition;
    Vector3 playerDirection;
    public float bulletSpeed = 8f;

    [SerializeField] Camera cam;
    float camHeight;
    float camWidth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        camHeight = cam.orthographicSize;
        camWidth = cam.orthographicSize * cam.aspect;

        playerPosition = player.GetComponent<PlayerMove>().playerPosition;
        playerDirection = player.GetComponent<PlayerMove>().direction;
    }

    // Update is called once per frame
    void Update()
    {
        // Update player-based variables
        playerPosition = player.GetComponent<PlayerMove>().playerPosition;
        playerDirection = player.GetComponent<PlayerMove>().direction;

        // Check if any bullets have been destroyed
        for(int i = 0; i < bullets.Count; i++)
        {
            if (OutOfBounds(bullets[i]))
            {
                destroyedBulletIndexes.Add(i);
            }
        }

        // Destroy any bullets that need to be destroyed
        if(destroyedBulletIndexes.Count > 0)
        {
            foreach (int index in destroyedBulletIndexes)
            {
                Destroy(bullets[index]);
                bullets.RemoveAt(index);
                bulletPositions.RemoveAt(index);
                bulletDirections.RemoveAt(index);
                bulletVelocities.RemoveAt(index);
            }

            destroyedBulletIndexes.Clear();
        }
        
        // Move all the bullets that have been fired
        for(int i = 0; i < bullets.Count; i++)
        {
            bulletVelocities[i] = bulletDirections[i] * bulletSpeed * Time.deltaTime;
            bulletPositions[i] += bulletVelocities[i];
            bullets[i].transform.position = bulletPositions[i];
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        GameObject bulletReference = Instantiate(bulletPrefab);

        Vector3 bulletPosition = playerPosition;
        Vector3 bulletDirection = playerDirection;
        Vector3 bulletVelocity = bulletDirection * bulletSpeed;

        bullets.Add(bulletReference);
        bulletPositions.Add(bulletPosition);
        bulletDirections.Add(bulletDirection);
        bulletVelocities.Add(bulletVelocity);
    }

    public bool OutOfBounds(GameObject bullet)
    {
        if (bullet.transform.position.x > camWidth || bullet.transform.position.x < -camWidth ||
            bullet.transform.position.y > camHeight || bullet.transform.position.y < -camHeight)
        {
            return true;
        }
        else return false;
    }
}
