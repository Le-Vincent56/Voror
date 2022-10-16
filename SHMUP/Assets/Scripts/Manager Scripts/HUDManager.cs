using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    public float playerHealth;
    public float roundNum;

    public GameObject score;
    [SerializeField] Text scoreLabel;
    [SerializeField] Slider healthBar;
    [SerializeField] Text roundLabel;

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("Score");
        playerHealth = player.GetComponent<PlayerStats>().currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Update score
        scoreLabel.text = "SCORE: " + score.GetComponent<ScoreMaster>().score;

        // Update round
        roundLabel.text = "ROUND: " + roundNum;

        // Update health bar
        playerHealth = player.GetComponent<PlayerStats>().currentHealth;
        healthBar.value = playerHealth;
    }
}
