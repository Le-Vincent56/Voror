using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    #region FIELDS
    public Vector3 enemyBulletPosition = new Vector3(0, 0, 0);
    public Vector3 enemyBulletDirection = new Vector3(1, 0, 0);
    Vector3 velocity = new Vector3(0, 0, 0);

    public float speed = 10f;
    public float damage = 10f;

    public GameObject target;
    [SerializeField] GameObject player;
    [SerializeField] GameObject villagerManager;
    Vector3 targetPosition;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        villagerManager = GameObject.Find("Villager Manager");
        target = FindClosestTarget();
        targetPosition = target.transform.position;
        enemyBulletDirection = new Vector3(targetPosition.x - enemyBulletPosition.x, targetPosition.y - enemyBulletPosition.y, 0).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate velocity
        velocity = enemyBulletDirection * speed * Time.deltaTime;

        // Add the velocity to the bullet position
        enemyBulletPosition += velocity;

        // Draw the bullet at the position
        transform.position = enemyBulletPosition;

        // Rotate the bullet to look in the direction its going towards
        if (enemyBulletDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.back, enemyBulletDirection);
        }
    }

    /// <summary>
    /// Finds the closest target to attack
    /// </summary>
    /// <returns>A GameObject that represents the closest target</returns>
    public GameObject FindClosestTarget()
    {
        GameObject target = player;
        foreach (GameObject villager in villagerManager.GetComponent<VillagerManager>().villagers)
        {
            if (CalculateDistance(villager) < CalculateDistance(player))
            {
                target = villager;
            }
            else
            {
                target = player;
            }
        }

        return target;
    }

    /// <summary>
    /// Calculate the distance to a target
    /// </summary>
    /// <param name="target">The target to calculate the distance between</param>
    /// <returns>A magnitude of the vector between this object and the target</returns>
    public float CalculateDistance(GameObject target)
    {
        Vector3 distance = GetComponent<SpriteRenderer>().bounds.center - target.GetComponent<SpriteRenderer>().bounds.center;
        return distance.magnitude;
    }
}
