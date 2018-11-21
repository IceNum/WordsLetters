using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public enum NetworkPlayerType
{
    server,
    client
}

///<summary>
/// Combination of functions like 
///_________________________
/// void Test(){CmdTest();}
/// [Command]
/// void CmdTest(){RpcTest();}
/// [ClientRpc]
/// void RpcTest(){CustomConsole.ShowMessage("Test")}
///_________________________
/// Allow lounch function on all clients
///</summary>


public class PlayerNetworkController : NetworkBehaviour
{
#region Variables
    [SyncVar]
    public Vector2 cellIndex;
    [SyncVar]
    public string cellValue;
    [SyncVar]
    public NetworkPlayerType type;
    [SyncVar]
    public string playerName;
    [SyncVar]
    public string word;

    public NetworkConnection conn;
    public short id;

    private Player _player;

    public List<Vector2> combinationIndexes;
#endregion

#region Unity functions
    void Awake()
    {
        _player = GetComponent<Player>();
    }
    void OnDestroy()
    {
        if (NetworkController.instance.serverStarted)
        {
            if (type == NetworkPlayerType.server)
            {
                NetworkController.instance.StopServer();
                NetworkController.instance.serverStarted = false;

                UIController.instance.ActivateStartMenu();
            }
            else
            {
                NetworkController.instance.StopHost();
            }
        }
        NetworkController.instance.playerCount--;
        GameController.instance.isGame=false;
    }
#endregion


#region Network functions
    public override void OnStartClient()
    {
        NetworkController.instance.OnConnectedToServer();
        
        if (NetworkController.instance.playerCount == 1)
        {
            GameController.instance.firstPlayer = _player;
            type = NetworkPlayerType.server;
        }
        else
        {
            GameController.instance.secondPlayer = _player;
            type = NetworkPlayerType.client;
        }
        gameObject.name = playerName;
        _player.playerName = playerName;
        UIController.instance.SyncPlayersNames();
    }

    public override void OnStartLocalPlayer()
    {
        NetworkController.instance.myPlayer = _player;
    }
#endregion

#region Network player functions
    public IEnumerator ShowCombination()
    {
        GameController.instance.GetCellWithIndex(cellIndex).SetBackgroundColor(Color.green);
        GameController.instance.GetCellWithIndex(cellIndex).SetValue(cellValue);

        yield return new WaitForSeconds(0.5f);
        for (var i = 0; i < combinationIndexes.Count; i++)
        {
            GameController.instance.GetCellWithIndex(combinationIndexes[i]).SetBackgroundColor(Color.yellow);
            yield return new WaitForSeconds(0.5f);
        }
        _player.AddWord(word);
    }
#endregion
    
#region Allow enemy word
    public void AllowEnemyWord(GameObject enemy)
    {
        CmdAllowEnemyWord(enemy);
    }
    [ClientRpc]
    private void RpcAllowEnemyWord(GameObject enemy)
    {
        if (isLocalPlayer)
            return;
        enemy.GetComponent<PlayerNetworkController>().AddWord(_player.enemy.GetNetworkController().word, _player.GetNetworkController().netId.Value);
        UIController.instance.ActivatePopupWaitConfirmation(false);
    }
    [Command]
    private void CmdAllowEnemyWord(GameObject enemy)
    {
        RpcAllowEnemyWord(enemy);
    }
#endregion

#region Cancel enemy word
    public void CancelEnemyWord(GameObject enemy)
    {
        CmdCancelEnemyWord(enemy);
    }
    [ClientRpc]
    private void RpcCancelEnemyWord(GameObject enemy)
    {
        if (isLocalPlayer)
            return;
        enemy.GetComponent<Player>().RestartStep();
        UIController.instance.ActivatePopupWaitConfirmation(false);
    }
    [Command]
    private void CmdCancelEnemyWord(GameObject enemy)
    {
        RpcCancelEnemyWord(enemy);
    }
#endregion


#region Update name
    public void UpdateName(string newName)
    {
        CmdUpdateName(newName);
    }
    [ClientRpc]
    private void RpcUpdateName(string newName)
    {
        CustomConsole.ShowMessage("Name: " + newName, Color.red);
        gameObject.name = newName;
        _player.playerName = newName;
        playerName = newName;
        UIController.instance.SyncPlayersNames();
    }
    [Command]
    private void CmdUpdateName(string newName)
    {
        RpcUpdateName(newName);
    }
#endregion


#region SwitchTimer
 public void SwitchTimer(bool timer)
    {
        if (!isLocalPlayer)
            return;
        CmdSwitchTimer(timer);
    }

    [ClientRpc]
    private void RpcSwitchTimer(bool timer)
    {
        GameController.instance.gameWithTimer = timer;
        UIController.instance.gameWithTimer.isOn = timer;
    }
    [Command]
    private void CmdSwitchTimer(bool timer)
    {
        RpcSwitchTimer(timer);
    }
#endregion


#region Add word
    public void AddWord(string word, uint enemyID)
    {
        CmdAddWord(word, enemyID);
    }
    [ClientRpc]
    private void RpcAddWord(string word, uint enemyID)
    {
        if (NetworkController.instance.myPlayer.GetNetworkController().netId.Value == enemyID)
        {
            StartCoroutine(ShowCombination());
        }
        else
        {
            _player.AddWord(word);
        }
    }
    [Command]
    private void CmdAddWord(string word, uint enemyID)
    {
        RpcAddWord(word, enemyID);
    }
#endregion


#region Add index
    public void AddNewIndex(Vector2 index,string selectedWord)
    {
        CmdAddNewIndex(index,selectedWord);
    }
    [ClientRpc]
    private void RpcAddNewIndex(Vector2 index,string selectedWord)
    {
        combinationIndexes.Add(index);
        word=selectedWord;
    }
    [Command]
    private void CmdAddNewIndex(Vector2 index,string selectedWord)
    {
        RpcAddNewIndex(index,selectedWord);
    }
#endregion

#region Clean indexes
    public void ClearIndexes()
    {
        CmdClearIndexes();
    }
    [ClientRpc]
    private void RpcClearIndexes()
    {
        combinationIndexes.Clear();
        word = "";
    }
    [Command]
    private void CmdClearIndexes()
    {
        RpcClearIndexes();
    }
#endregion


#region Sync cell
    public void SyncCell(Vector2 index, string value)
    {
        CmdSyncCell(index, value);
    }
    [ClientRpc]
    private void RpcSyncCell(Vector2 index, string value)
    {
       // CustomConsole.ShowMessage(index + " " + value);
        cellIndex = index;
        cellValue = value;

    }
    [Command]
    private void CmdSyncCell(Vector2 index, string value)
    {
        RpcSyncCell(index, value);
    }
#endregion


#region Show warning
    public void ShowWarning(uint enemyID,string word)
    {
        CmdShowWarning(enemyID,word);
    }
    [ClientRpc]
    private void RpcShowWarning(uint enemyID, string word)
    {
        if (NetworkController.instance.myPlayer.GetNetworkController().netId.Value == enemyID)
            UIController.instance.ActivateWarning(true, word);
        else
            UIController.instance.ActivatePopupWaitConfirmation(true);
    }
    [Command]
    private void CmdShowWarning(uint enemyID,string word)
    {
        RpcShowWarning(enemyID,word);
    }
#endregion
}