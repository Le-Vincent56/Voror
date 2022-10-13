using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    Start,
    Game,
    Pause,
    Lose
}

public enum RoundState
{
    None,
    Spawning,
    MidRound,
    EndRound,
    Lose
}

public class RoundManager : MonoBehaviour
{
    #region FIELDS
    public RoundState roundState;
    public int roundNum = 0;

    [SerializeField] EnemyManager enemyManager;
    [SerializeField] VillagerManager villagerManager;
    [SerializeField] GameObject player;

    float spawningTimer = 5f;
    float pointsTimer = 5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        roundState = RoundState.Spawning;
    }

    // Update is called once per frame
    void Update()
    {
        switch (roundState)
        {
            case RoundState.None:
                break;

            case RoundState.Spawning:
                // Increment round number
                roundNum += 1;

                // Reset enemy numbers and spawn starting enemies
                enemyManager.enemiesSpawnedThisRound = 0;
                enemyManager.maxEnemiesThisRound = enemyManager.baseEnemies + (enemyManager.bonusEnemiesPerRound * roundNum);
                enemyManager.SpawnStartingEnemies();

                // Freeze the enemies
                foreach (GameObject enemy in enemyManager.enemies)
                {
                    enemy.GetComponent<EnemyMovement>().moving = false;
                }

                // Subtract time from the spawning time
                spawningTimer -= Time.deltaTime;

                // Reset bonus timer
                if(pointsTimer <= 0)
                {
                    pointsTimer = 5f;
                }

                // Add round change condition
                if (enemyManager.enemiesSpawnedThisRound >= enemyManager.baseEnemies && spawningTimer <= 0)
                {
                    roundState = RoundState.MidRound;
                }

                break;

            case RoundState.MidRound:
                // Spawn enemies if possible
                enemyManager.SpawnEnemy();

                // Let the enemies move
                foreach(GameObject enemy in enemyManager.enemies)
                {
                    enemy.GetComponent<EnemyMovement>().moving = true;
                }

                // Add round change condition
                // - if the playe has no more health, or all the villagers are dead, lose
                if(player.GetComponent<PlayerStats>().currentHealth <= 0 || villagerManager.villagers.Count <= 0)
                {
                    roundState = RoundState.Lose;
                }

                // Add round change condition
                // - if the max number of enemies have spawned and all the enemies on the screen are dead, calculate points
                if (enemyManager.enemiesSpawnedThisRound >= enemyManager.maxEnemiesThisRound && enemyManager.enemies.Count <= 0)
                {
                    roundState = RoundState.EndRound;
                }
                break;

            case RoundState.EndRound:
                // Calculate next round enemy numbers
                enemyManager.baseEnemies += (1 * roundNum);

                // Add bonuses from alive villagers
                villagerManager.CalculateAliveBonuses();

                // Subtract time from the spawning time
                pointsTimer -= Time.deltaTime;

                // Reset spawning timer
                if(spawningTimer <= 0)
                {
                    spawningTimer = 5f;
                }

                // Add round change condition
                if(pointsTimer <= 0)
                {
                    roundState = RoundState.Spawning;
                }
                break;

            case RoundState.Lose:
                
                break;
        }
    }
}
