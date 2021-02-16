using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Announcments : MyNetworkBehaviour {
    private static Announcments _current;
    public static Announcments current
    {
        get
        {
            if(_current != null)
                return _current;
            Debug.LogError("no Announcments");
            return null;
        }
        set
        {
            _current = value;
        }
    }

    void Awake()
    {
        if(_current == null)
        {
            _current = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public Text Message;
    public GameObject announcmentsPanel;

    void Start()
    {
        announcmentsPanel.SetActive(false);
        GameController.playerWins += PlayerWins;
        SceneManager.sceneLoaded += OnNewLevel;
    }

    void OnNewLevel(Scene aScene, LoadSceneMode aMode)
    {
        announcmentsPanel.SetActive(false);
        if(isServer)
            RpcOnNewLevel();
    }
    [ClientRpc]
    void RpcOnNewLevel()
    {
        announcmentsPanel.SetActive(false);
    }

    public void PlayerWins(Player player)
    {
        Message.text = player.name + " Wins!!!";
        announcmentsPanel.SetActive(true);
        RpcPlayerWins(player.name);
    }
    [ClientRpc]
    void RpcPlayerWins(string player)
    {
        Message.text = player + " Wins!!!";
        announcmentsPanel.SetActive(true);
    }


}
