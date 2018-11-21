using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using System.Collections;
public class NetworkController : NetworkManager
{
#region Variables
    public static NetworkController instance;

    public bool serverStarted;
    public short playerCount;

    public Player myPlayer;
#endregion

#region Unity functions
    void Awake()
    {
        playerCount = 0;
        instance = this;
    }
#endregion

#region Network functions
    ///<summary>
    /// You can read more obout this function https://docs.unity3d.com/ScriptReference/Network.OnConnectedToServer.html
    ///</summary>
    public void OnConnectedToServer()
    {
        playerCount++;
        GameController.instance.networkGame = true;

        CustomConsole.ShowMessage("Count: " + NetworkServer.active, Color.blue);
        if (playerCount == 1)
            UIController.instance.ActivateHostRoomMenu();
        else if (playerCount == 2)
        {
            if (NetworkServer.active)
                UIController.instance.ActivateHostFullRoomMenu();
            else
                UIController.instance.ActivateClientRoomMenu();
        }
        NetworkController.instance.SwitchTimer(NetworkGameController.instance.withTimer);
    }

    ///<summary>
    /// Launch this function if you want network game
    ///</summary>
    public void JoinOrCreateRoom()
    {
        UIController.instance.ActivateLoader();
        StartMatchMaker();
        
        FindInternetMatch();
        StartCoroutine(CheckConnect(6));
    }

    ///<summary>
    /// This function will be check connection success. If you have bad internet set countOfAttempts higher
    ///</summary>
    public IEnumerator CheckConnect(int countOfAttempts)
    {
        var count=0;
        while(count<countOfAttempts&&myPlayer==null)
        {
            count++;
            CustomConsole.ShowMessage("Check success");
            yield return new WaitForSeconds(5);
        }
        if(myPlayer==null)
        {
            StopGame();
            JoinOrCreateRoom();
        }
    }
    
    public void CreateRoom()
    {
        matchMaker.CreateMatch("Room", 2, true, "", "", "", 0, 0, OnRoomCreated);
    }
    public void FindInternetMatch()
    {
        matchMaker.ListMatches(0, 10, "Room", true, 0, 0, OnInternetMatchList);
    }
    ///<summary>
    /// Callback if room created
    ///</summary>
    void OnRoomCreated(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            NetworkServer.Listen(matchInfo, 9000);

            StartHost(hostInfo);
            NetworkServer.Spawn(NetworkGameController.instance.gameObject);
            serverStarted = true;
        }
    }
     ///<summary>
    /// Callback if new player added at client
    ///</summary>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        var pnc = player.GetComponent<PlayerNetworkController>();
        pnc.UpdateName("Player" + playerCount);
    }

     ///<summary>
    /// Callback if room list has free rooms
    ///</summary>
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
                matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            else
                CreateRoom();
        }
    }

    ///<summary>
    /// Callback if we joined to server
    ///</summary>
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            MatchInfo hostInfo = matchInfo;
            StartClient(hostInfo);
            serverStarted = true;
        }
    }
    ///<summary>
    /// Function for turn on/off game with timer
    ///</summary>
    public void SwitchTimer(bool timer)
    {
        if (GameController.instance.firstPlayer != null)
            GameController.instance.firstPlayer.GetNetworkController().SwitchTimer(timer);
        if (GameController.instance.secondPlayer != null)
            GameController.instance.secondPlayer.GetNetworkController().SwitchTimer(timer);
    }
    
    ///<summary>
    /// This function stop game and disconnect clients
    ///</summary>
    public void StopGame()
    {
        StopHost();
        GameController.instance.networkGame = false;
    }
#endregion
}