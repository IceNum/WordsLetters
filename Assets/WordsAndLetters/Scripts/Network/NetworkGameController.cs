using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
public class NetworkGameController : NetworkBehaviour
{
    public static NetworkGameController instance;
    [SyncVar]
    public string startWord;
    [SyncVar]
    public bool withTimer;
    void Awake()
    {
        instance = this;

    }
    public void StartGame(string word)
    {
        CmdStartGame(word);
    }
    [ClientRpc]
    private void RpcStartGame(string word)
    {
        GameController.instance.StartGame(word);
        NetworkController.instance.myPlayer.enemy.GetHumanController().enabled = false;
    }
    [Command]
    private void CmdStartGame(string word)
    {
        RpcStartGame(word);
    }

    


}
