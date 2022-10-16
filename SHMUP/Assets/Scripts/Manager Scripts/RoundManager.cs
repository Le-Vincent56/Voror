using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject score;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] VillagerManager villagerManager;
    [SerializeField] SigilManager sigilManager;
    [SerializeField] GameObject player;
    [SerializeField] HUDManager HUDManager;

    float spawningTimer = 5f;
    float pointsTimer = 5f;

    bool initialSpawned = false;
    bool completedEndChecks = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("Score");
        roundState = RoundState.Spawning;
        score.GetComponent<ScoreMaster>().score = 0;
    }

    // Update is called once per frame
    void Update()
    {

        switch (roundState)
        {
            case RoundState.None:
                break;

            case RoundState.Spawning:
                // Reset enemy numbers and spawn starting enemies
                if (!initialSpawned)
                {
                    // Increment round number
                    roundNum += 1;
                    HUDManager.roundNum = roundNum;
                    sigilManager.sigilsSpawnedThisRound = 0;

                    // Set enemy spawning variables and spawn starting enemies
                    enemyManager.enemiesSpawnedThisRound = 0;
                    enemyManager.maxEnemiesThisRound = enemyManager.baseEnemies + (enemyManager.bonusEnemiesPerRound * roundNum);
                    enemyManager.SpawnStartingEnemies();

                    // Freeze the enemies and prevent them from being damaged
                    foreach (GameObject enemy in enemyManager.enemies)
                    {
                        enemy.GetComponent<EnemyMovement>().moving = false;
                        enemy.GetComponent<EnemyStats>().canBeDamaged = false;
                    }
                    initialSpawned = true;
                    completedEndChecks = false;
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

                // Spawn sigils if possible
                sigilManager.SpawnSigil();

                // Let the enemies move and allow them to be damaged
                foreach(GameObject enemy in enemyManager.enemies)
                {
                    enemy.GetComponent<EnemyMovement>().moving = true;
                    enemy.GetComponent<EnemyStats>().canBeDamaged = true;
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
                if (!completedEndChecks)
                {
                    // Calculate next round enemy numbers
                    enemyManager.baseEnemies += (1 * roundNum);

                    // Add bonuses from alive villagers
                    villagerManager.CalculateAliveBonuses();
                    initialSpawned = false;
                    completedEndChecks = true;
                }                

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
                SceneManager.LoadScene("Lose Screen");
                break;
        }
    }
}
