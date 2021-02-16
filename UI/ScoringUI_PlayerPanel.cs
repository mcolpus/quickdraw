using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringUI_PlayerPanel : MonoBehaviour {
    public Text nameText;
    public Text scoreText;
    public Image playerSprite;


    public void Setup(string name, int score, Sprite sprite)
    {
        nameText.text = name;
        scoreText.text = "Score: " + score;
        playerSprite.sprite = sprite;
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
	
}
