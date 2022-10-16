using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMaster : MonoBehaviour
{
    public float score = 0;
    public ScoreMaster instance;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        } else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
