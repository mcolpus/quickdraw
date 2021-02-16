using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class MatchOptionPanel : MonoBehaviour {
    public Text matchNameText;
    private MatchInfoSnapshot match;
    private standardAnnouncement onMatchClicked;
    private NetworkMatch.DataResponseDelegate<MatchInfo> onMatchJoined;

    public delegate void MatchJoinedDel(bool success, string extendedInfo, MatchInfo matchInfo);

    public void Setup(MatchInfoSnapshot _match,standardAnnouncement _onMatchClicked, NetworkMatch.DataResponseDelegate<MatchInfo> _onMatchJoined)
    {
        match = _match;
        onMatchClicked = _onMatchClicked;
        onMatchJoined = _onMatchJoined;
        matchNameText.text = match.name;
    }

    public void OnMatchClicked()
    {
        if(onMatchClicked != null)
            onMatchClicked();
        MyNetworkManager.mySingleton.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, onMatchJoined);
        //MyNetworkManager.mySingleton.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, MyNetworkManager.singleton.OnMatchJoined);
    }
    

}
