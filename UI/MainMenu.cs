using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MainMenu : MonoBehaviour {
    public static MainMenu singleton;

    public GameObject mainPanel;
    public GameObject playPanel;
    public GameObject onlineSetupPanel;
    public GameObject connectingPanel;
    public enum PANELS { main, play, onlineSetup, connecting};
    public PANELS currentPanel = PANELS.main;

    public Text onlineRoomNameInput;
    public Text directServerIpInput;
    public Text connectionInfo;
    public Text ipInUse;
    
    private MyNetworkManager myNetworkManager;

    public RectTransform serverListRect;
    public GameObject serverEntryPrefab;

    void Awake()
    {
        if(singleton == null)
            singleton = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        myNetworkManager = MyNetworkManager.mySingleton;
    }

    void SetPanel(PANELS panel)
    {
        currentPanel = panel;
        mainPanel.SetActive(false);
        //playPanel.SetActive(false);
        onlineSetupPanel.SetActive(false);
        connectingPanel.SetActive(false);
        switch(currentPanel)
        {
        case PANELS.main:
            mainPanel.SetActive(true);
            break;
        case PANELS.play:
            //playPanel.SetActive(true);
            break;
        case PANELS.onlineSetup:
            onlineSetupPanel.SetActive(true);
            break;
        case PANELS.connecting:
            connectingPanel.SetActive(true);
            break;
        }
    }

    void SetConnectingInfo()
    {
        SetPanel(PANELS.connecting);
        connectionInfo.text = "Connecting to..." + myNetworkManager.networkAddress + " port: " + myNetworkManager.networkPort;
    }

    //called by MynetworkManager on the client
    public void OnClientConnect()
    {
        Debug.Log("client connect");
        SetPanel(PANELS.play);
        ipInUse.text = "ip: " + myNetworkManager.networkAddress + " port: " + myNetworkManager.networkPort;
        
    }

    //called by MyNetworkManager on server when a client joins
    public void OnClientJoined()
    {
        Debug.Log("client joined");
        playPanel.GetComponent<CharacterSelectMenu>().UpdateAllPanels();
    }

    #region button presses
    public void PressBack()
    {
        SetPanel(PANELS.main);
        myNetworkManager.StopServer();
        myNetworkManager.StopHost();
        myNetworkManager.StopClient();
    }
    public void PressPlayLocal()
    {
        myNetworkManager.StartHost();
        myNetworkManager.GameType = GAMETYPES.local;
        SetPanel(PANELS.play);
    }
    public void PressPlayOnline()
    {
        SetPanel(PANELS.onlineSetup);
    }
    public void PressLanHost()
    {
        SetPanel(PANELS.play);
        myNetworkManager.GameType = GAMETYPES.lan;
        myNetworkManager.StartHost();
        
    }
    public void PressLanJoin()
    {
        myNetworkManager.GameType = GAMETYPES.lan;
        myNetworkManager.networkAddress = "127.0.0.1";
        SetConnectingInfo();
        myNetworkManager.StartClient();
    }
    
    public void PressLanServer()
    {
        myNetworkManager.GameType = GAMETYPES.lan;
        SetPanel(PANELS.play);
        myNetworkManager.StartServer();
    }

    public void PressJoinDirect()
    {
        if(directServerIpInput.text == "" || directServerIpInput.text == null)
            return;

        myNetworkManager.GameType = GAMETYPES.online;
        myNetworkManager.networkAddress = directServerIpInput.text;
        SetConnectingInfo();
        myNetworkManager.StartClient();
    }


    public void PressOnlineHost()
    {
        if(onlineRoomNameInput.text == "" || onlineRoomNameInput.text == null)
            return;

        SetPanel(PANELS.play);
        myNetworkManager.StartMatchMaker();
        myNetworkManager.matchMaker.CreateMatch(onlineRoomNameInput.text, (uint)4,true, "", "", "", 0, 0, MatchCreated);
        
    }
    void MatchCreated(bool success,string extendedInfo, MatchInfo matchInfo)
    {
        myNetworkManager.OnMatchCreate(success, extendedInfo, matchInfo);
        if(success)
        {
            Debug.Log("match successfully made. ID: " + matchInfo.networkId);
            myNetworkManager.GameType = GAMETYPES.online;
        }
        else
        {
            Debug.Log("match failed ID: " + matchInfo.networkId);
        }
        
    }

    public void PressOnlineJoin()
    {
        myNetworkManager.StartMatchMaker();
        myNetworkManager.matchMaker.ListMatches(0, 4, "", true, 0, 0, OnGUIMatchList);
    }
    public void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if(matches.Count == 0)
        {
            Debug.Log("no matches to join");
            return;
        }
        
        foreach(Transform t in serverListRect)
            Destroy(t.gameObject);

        for(int i = 0; i < matches.Count; ++i)
        {
            GameObject o = Instantiate(serverEntryPrefab) as GameObject;
            o.GetComponent<MatchOptionPanel>().Setup(matches[i], () => SetConnectingInfo(),MatchJoined);
            o.transform.SetParent(serverListRect, false);
            
        }
    }
    void MatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        SetPanel(PANELS.play);
        if(success)
        {
            Debug.Log("match successfully joined. ID: " + matchInfo.networkId);
            myNetworkManager.GameType = GAMETYPES.online;
        }
        else
        {
            Debug.Log("failed to join match ID: " + matchInfo.networkId);
        }
        myNetworkManager.OnMatchJoined(success, extendedInfo, matchInfo);

    }


   

    #endregion
}
