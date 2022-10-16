using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    #region FIELDS
    public float damage;
    public float maxHealth;
    public float currentHealth;
    public float value;
    public bool collided;
    public float damageColorTimer = 0.5f;
    public float invincibleColorTimer = 0.5f;
    public bool canBeDamaged = false;
    public bool ranged;
    public bool canFire;
    public float fireCooldown;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
