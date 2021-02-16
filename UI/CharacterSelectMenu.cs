using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterSelectMenu : MyNetworkBehaviour {
    public enum HATS { red, Blue}

    public List<CharacterSelectPanel> characterSelectPanels;
    
    public List<PanelData> panelDatas;
    public List<string> RandomNames;
    public List<CharacterOption> characterOptions;
    public List<HatOption> hatOptions;

    public MenuPlayer menuPlayer;//way to issue updates to server


    public override void OnStartServer()
    {
        Debug.Log("server");
        base.OnStartServer();
        //reset to no characters entered
        for(int i = 0; i < characterSelectPanels.Count; i++)
        {
            panelDatas[i].open = false;
            panelDatas[i].name = GetRandomName("");
            characterSelectPanels[i].UpdateData(panelDatas[i]);
        }
    }
    
    void OnDisable()
    {
        //this is just for cosmetics so that when selection loaded they start off all empty
        for(int i = 0; i < characterSelectPanels.Count; i++)
        {
            panelDatas[i].open = false;
            characterSelectPanels[i].UpdateData(panelDatas[i]);
        }
    }

    public void StartGame()
    {
        if(!isServer)
            return;

        List<Player> players = new List<Player>();
        for(int i = 0; i < characterSelectPanels.Count; i++)
        {
            if(characterSelectPanels[i].open)
            {
                Player p = new Player();
                p.name = panelDatas[i].name;
                p.controllerNumber = i;
                p.characterType = GetCharacterOption(panelDatas[i].characterOptionNumber).type;
                p.spriteColor = GetCharacterOption(panelDatas[i].characterOptionNumber).spriteColor;
                p.hat = GetHatOption(panelDatas[i].hatOptionNumber).sprite;
                p.displaySprite = GetCharacterOption(panelDatas[i].characterOptionNumber).sprite;
                players.Add(p);
            }
        }
        if(players.Count >= 1)
        {
            RpcSetGameControllerData();
            GameController.current.players = players;
            GameController.current.StartGame(players);
        }
    }
    [ClientRpc]
    void RpcSetGameControllerData()
    {
        List<Player> players = new List<Player>();
        for(int i = 0; i < characterSelectPanels.Count; i++)
        {
            if(characterSelectPanels[i].open)
            {
                Player p = new Player();
                p.name = panelDatas[i].name;
                p.controllerNumber = i;
                p.characterType = GetCharacterOption(panelDatas[i].characterOptionNumber).type;
                p.spriteColor = GetCharacterOption(panelDatas[i].characterOptionNumber).spriteColor;
                p.hat = GetHatOption(panelDatas[i].hatOptionNumber).sprite;
                p.displaySprite = GetCharacterOption(panelDatas[i].characterOptionNumber).sprite;
                players.Add(p);
            }
        }
        GameController.current.players = players;
    }

    //methods called by panels
    public void RandomizeName(int panelNumber)
    {
        string exclude = panelDatas[panelNumber].name;
        panelDatas[panelNumber].name = GetRandomName(exclude);
        SendUpdateToServer(panelNumber);
    }
    string GetRandomName(string exclude)
    {
        string rnd = "";
        while(true)
        {
            rnd = RandomNames.GetRandom();
            if(rnd == exclude) continue;
            break;
        }
        return rnd;
    }
    public void OpenClose(int panelNumber)
    {
        panelDatas[panelNumber].open = !panelDatas[panelNumber].open;

        SendUpdateToServer(panelNumber);
    }
    public void NextCharacterType(int panelNumber, int amount)
    {
        panelDatas[panelNumber].characterOptionNumber += amount;
        SendUpdateToServer(panelNumber);
    }
    public void NextHatType(int panelNumber, int amount)
    {
        panelDatas[panelNumber].hatOptionNumber += amount;
        SendUpdateToServer(panelNumber);
    }
    public void SetName(int panelNumber, string _name)
    {
        panelDatas[panelNumber].name = _name;
        SendUpdateToServer(panelNumber);
    }
    void SendUpdateToServer(int panelNumber)
    {
        if(menuPlayer != null)
        {
            menuPlayer.CallUpdate(panelNumber, panelDatas[panelNumber]);
        }
    }



    [Server]
    public void UpdateAllPanels()
    {
        //used when 
        Debug.Log("update all");
        for(int i = 0; i < characterSelectPanels.Count; i++)
        {
            RpcUpdateCharacterPanel(i, panelDatas[i]);
        }
    }
    [Server]
    public void SetPanelData(int panelNumber, PanelData data)
    {
        panelDatas[panelNumber] = data;
        characterSelectPanels[panelNumber].UpdateData(panelDatas[panelNumber]);
        RpcUpdateCharacterPanel(panelNumber, data);
    }
    [ClientRpc]
    void RpcUpdateCharacterPanel(int panelNumber, PanelData data)
    {
        panelDatas[panelNumber] = data;
        characterSelectPanels[panelNumber].UpdateData(panelDatas[panelNumber]);
    }


    public void SetMenuPlayer(MenuPlayer _menuPlayer)
    {
        menuPlayer = _menuPlayer;
    }





    public CharacterOption GetCharacterOption(int index)
    {
        index = mod(index, characterOptions.Count);
        return characterOptions[index];
    }
    public HatOption GetHatOption(int index)
    {
        index = mod(index, hatOptions.Count);
        return hatOptions[index];
    }
    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }


    [System.Serializable]
    public class CharacterOption
    {
        public CHARACTERTYPES type;
        public Sprite sprite;
        public Color spriteColor = Color.white;
        [Multiline]
        public string description;
    }
    [System.Serializable]
    public class HatOption
    {
        public HATS type;
        public Sprite sprite;
    }

    [System.Serializable]
    public class PanelData
    {
        public string name;
        public bool open;
        public int characterOptionNumber;
        public int hatOptionNumber;

    }
}
