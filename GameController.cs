using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameController : MyNetworkBehaviour {
    private static GameController _current;
    public static GameController current
    {
        get
        {
            if(_current != null)
                return _current;
            Debug.LogError("no gamecontroller");
            return null;
        }
        set
        {
            _current = value;
        }
    }

    
    private GAMETYPES gameType;
    public GAMETYPES GameType { get; set; }
    
    public List<Stage> stages = new List<Stage>();
    public List<Player> players;
    public List<NetworkConnection> connections = new List<NetworkConnection>();
    public List<short> controllerIds = new List<short>();

    public CHARACTERTYPES typesForReference;
    public List<GameObject> characterPrefabs;

    public bool RunOnStart;
    private int stageIndex;

    public delegate void gameAnnouncement();
    public static event gameAnnouncement OnPlayersEnter;
    public static event gameAnnouncement RoundEnd;
    public delegate void gameAnnouncement2(Player player);
    public static event gameAnnouncement2 playerWins;
    

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
        Debug.Log("game controller now active");
    }

    public int RegisterNewConnection(NetworkConnection networkConnection)
    {
        connections.Add(networkConnection);
        return connections.Count - 1;//return position in list
    }

    


    void Start()
    {
        if(RunOnStart)
            StartGame();
    }

    public void StartGame(List<Player> _players)
    {
        players = _players;
        StartCoroutine(Run());
    }
    public void StartGame()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        if(!isServer)
            yield break;

        stageIndex = UnityEngine.Random.Range(0, stages.Count);
        while(true)
        {
            if(RunOnStart)
            {
                yield return StartCoroutine(LoadStage(-1, ""));
            }
            else
            {
                yield return StartCoroutine(LoadStage(stages[stageIndex].stageNumber, stages[stageIndex].stageName));
            }
            
            stageIndex = (stageIndex + 1) % stages.Count;

            List<Player> remaining = new List<Player>();
            remaining.AddRange(players);
            while(true)
            {
                foreach(Player p in remaining)
                {
                    if(p.playerController.dead == true)
                    {
                        remaining.Remove(p);
                        break;
                    }
                }
                if(remaining.Count <= 1)
                {
                    if(remaining.Count == 1)
                    {
                        remaining[0].score++;
                        print(remaining[0].name + " won round");
                        if(playerWins != null)
                            playerWins(remaining[0]);

                        RpcUpdateScores(players.IndexOf(remaining[0]));
                    }
                    //while(true)
                    //{
                    //    yield return null;
                    //    if(MyInputs.GetButtonDown(1, Tags.jump))
                    //        break;
                    //}
                    
                    AnnounceRoundEnd();
                    yield return new WaitForSeconds(3);
                    break;
                }
                yield return null;
            }

            foreach(Player p in players)
            {
                if(p.score == 20)
                {
                    print(p.name + " won the game!");
                    yield break;
                }
            }

            yield return null;
        }
    }

    [Server]
    IEnumerator LoadStage(int stageNumber, string stageName)
    {
        if(stageNumber != -1)
        {
            //SceneManager.LoadScene(stageNumber);
            MyNetworkManager.mySingleton.OnCharacterSelection = false;
            SceneManager.LoadScene(stageName);
            yield return new WaitForSeconds(0.5f);
            NetworkManager.singleton.ServerChangeScene(stageName);
            
        }
        Debug.LogWarning("stage loaded time start");
        yield return new WaitForSeconds(2f);
        Debug.LogWarning("stage loaded time over");


        Transform spawnPosParent = GameObject.Find(Tags.spawnLocations).transform;
        if(spawnPosParent == null) Debug.LogError("no SpawnLocations found");
        List<Vector2> startPoints = new List<Vector2>();
        foreach(Transform child in spawnPosParent)
        {
            startPoints.Add(child.position);
        }
        startPoints.Shuffle();

        for(int i = 0; i < players.Count; i++)
        {
            GameObject obj = Instantiate(characterPrefabs[(int)players[i].characterType]);

            obj.transform.position = startPoints[i];
            players[i].playerController = obj.GetComponent<PlayerController>();
            obj.GetComponentInChildren<SpriteRenderer>().color = players[i].spriteColor;
            players[i].playerController.hatRenderer.sprite = players[i].hat;
            if(GameType== GAMETYPES.local)
            {
                players[i].playerController.playerControlNumber = players[i].controllerNumber;
                //this was set by CharacterSelectMenu based on which panel was used
                ClientScene.AddPlayer( (short)players[i].controllerNumber);
                MyNetworkManager.mySingleton.SpawnAsPlayer(obj, connections[0], (short)players[i].controllerNumber);
                
            }
            else
            {
                players[i].playerController.playerControlNumber = 0;
                MyNetworkManager.mySingleton.SpawnAsPlayer(obj, connections[i], (short)0);
            }
            
            

            
        }
        AnnouncePlayersEnter();

    }


    //void UpdateScoresInClients()
    //{
    //    List<int> scores = new List<int>();
    //    foreach(Player p in players)
    //    {
    //        scores.Add(p.score);
    //    }
    //    RpcUpdateScores(scores);
    //}
    [ClientRpc]
    void RpcUpdateScores(int winnerIndex)
    {
        if(isClientOnly)
            players[winnerIndex].score++;
    }

    #region announcements
    void AnnouncePlayersEnter()
    {
        if(OnPlayersEnter != null)
            OnPlayersEnter();
        if(isServer)
            RpcAnnouncePlayersEnter();
    }
    [ClientRpc]
    void RpcAnnouncePlayersEnter()
    {
        if(OnPlayersEnter != null)
            OnPlayersEnter();
    }

    void AnnounceRoundEnd()
    {
        if(RoundEnd != null)
            RoundEnd();
        if(isServer)
            RpcAnnounceRoundEnd();
    }
    [ClientRpc]
    void RpcAnnounceRoundEnd()
    {
        if(RoundEnd != null)
            RoundEnd();
    }
    #endregion


    [Serializable]
    public class Stage
    {
        public int stageNumber;
        public string stageName;
    }
	
}

[Serializable]
public class Player
{
    public string name;
    public int controllerNumber;
    public CHARACTERTYPES characterType;
    public Color spriteColor = Color.white;
    public int score;
    public PlayerController playerController;
    public Sprite hat;
    public Sprite displaySprite;
}
