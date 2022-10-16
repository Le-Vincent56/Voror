using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    #region FIELDS
    public Vector3 monsterPosition = new Vector3(0, 0, 0);
    Vector3 direction = new Vector3(1, 0, 0);
    Vector3 velocity = new Vector3(0, 0, 0);

    public float speed = 5f;
    public bool moving = false;

    [SerializeField] GameObject target;
    [SerializeField] GameObject player;
    [SerializeField] GameObject villagerManager;
    Vector3 targetPosition;
    Vector3 distanceToTarget = new Vector3(0, 0, 0);

    [SerializeField] Camera cam;
    float camHeight;
    float camWidth;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        player = GameObject.Find("Player");
        villagerManager = GameObject.Find("Villager Manager");
        target = player;
        monsterPosition = transform.position;
        targetPosition = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            // Find the closest target and set that as the target of attack
            target = FindClosestTarget();

            // Track player
            monsterPosition = transform.position;
            targetPosition = target.transform.position;
            distanceToTarget = targetPosition - monsterPosition;
            direction = distanceToTarget.normalized;

            // Calculate velocity
            velocity = direction * speed * Time.deltaTime;

            // Move towards player
            monsterPosition += velocity;
            transform.position = monsterPosition;
        }
    }

    public void CheckBounds()
    {
        if (monsterPosition.x < -camWidth)
        {
            monsterPosition.x = -camWidth;
        }
        else if (monsterPosition.x > camWidth)
        {
            monsterPosition.x = camWidth;
        }

        if (monsterPosition.y < -camHeight)
        {
            monsterPosition.y = -camHeight;
        }
        else if (monsterPosition.y > camHeight)
        {
            monsterPosition.y = camHeight;
        }
    }

    public GameObject FindClosestTarget()
    {
        GameObject target = player;
        foreach(GameObject villager in villagerManager.GetComponent<VillagerManager>().villagers)
        {
            if(CalculateDistance(villager) < CalculateDistance(player))
            {
                target = villager;
            } else
            {
                target = player;
            }
        }

        return target;
    }

    public float CalculateDistance(GameObject target)
    {
        Vector3 distance = GetComponent<SpriteRenderer>().bounds.center - target.GetComponent<SpriteRenderer>().bounds.center;
        return distance.magnitude;
    }
}
