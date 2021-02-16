using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class ScoringUI : MonoBehaviour {
    public static ScoringUI singleton;

    public GameObject playerScorePanelPrefab;
    public Transform playerPanelParent;
    [SerializeField]
    private List<ScoringUI_PlayerPanel> panels = new List<ScoringUI_PlayerPanel>();
	
    void Awake()
    {
        if(singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        DisplayScores();
        GameController.RoundEnd += UpdateScores;
    }

    public void DisplayScores()
    {
        foreach(Player p in GameController.current.players)
        {
            GameObject obj = Instantiate(playerScorePanelPrefab) as GameObject;
            obj.transform.SetParent(playerPanelParent);
            ScoringUI_PlayerPanel panel = obj.GetComponent<ScoringUI_PlayerPanel>();
            panel.Setup(p.name, p.score, p.displaySprite);
            panels.Add(panel);
        }
    }

    public void UpdateScores()
    {
        for(int i=0;i< GameController.current.players.Count; i++)
        {
            panels[i].SetScore(GameController.current.players[i].score);
        }
    }
    

}
