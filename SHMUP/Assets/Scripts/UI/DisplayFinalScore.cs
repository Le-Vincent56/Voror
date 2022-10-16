using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFinalScore : MonoBehaviour
{
    [SerializeField] Text finalScore;
    [SerializeField] GameObject score;
    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("Score");
        finalScore.text = "FINAL SCORE: " + score.GetComponent<ScoreMaster>().score;
    }
}
