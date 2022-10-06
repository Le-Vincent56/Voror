using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> enemies;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the player reaches 0 health, destroy it
        if (player.GetComponent<PlayerStats>().currentHealth <= 0)
        {
            Destroy(player);
        }
    }
}
