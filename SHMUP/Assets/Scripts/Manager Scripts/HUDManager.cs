using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    public float score;
    public float playerHealth;

    [SerializeField] Text scoreLabel;
    [SerializeField] Slider healthBar;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = player.GetComponent<PlayerStats>().currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Update score
        scoreLabel.text = "SCORE: " + score;

        // Update health bar
        playerHealth = player.GetComponent<PlayerStats>().currentHealth;
        healthBar.value = playerHealth;
    }
}
