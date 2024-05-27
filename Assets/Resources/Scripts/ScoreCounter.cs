using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField]
    private Text textScores;
    [SerializeField]
    private Text subTextScores;
    private int score;

    public void addScore() 
    {
        score++;
        string textScore = "Score " + System.Convert.ToString(score);
        textScores.text = textScore;
        subTextScores.text = textScore;
    }
}
