using UnityEngine;
using KaboomStudio;
using System.Xml;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

#region Variables
    public static GameController instance;

    public GameObject localPlayer;
    public List<string> dictionary;
    public GameObject field;
    public List<Cell> cells;

    public Player firstPlayer;
    public Player secondPlayer;
    public Player currentPlayer;

    public List<Cell> centerLine;

    public string startWord;
    public float timerDuration;
    public bool gameWithTimer;
    public bool networkGame;
    public bool isGame;
#endregion

#region Unity functions
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        LoadDictionary();
    }
#endregion

#region Setters
    public void SetLetterToCurrenPlayer(string letter)
    {
        currentPlayer.GetHumanController().SetLetter(letter);

        UIController.instance.ActivateKeyboard(false);
    }

    public void SetTimer(bool playWithTimer)
    {
        gameWithTimer = playWithTimer;
        if (networkGame)
            NetworkController.instance.SwitchTimer(playWithTimer);
    }

    public void SetLocalGame(bool withBot)
    {
        ClearPlayers();

        var firstPlayerTemp = Instantiate(localPlayer) as GameObject;
        var secondPlayerTemp = Instantiate(localPlayer) as GameObject;


        firstPlayer = firstPlayerTemp.GetComponent<Player>();
        secondPlayer = secondPlayerTemp.GetComponent<Player>();

        if (withBot)
        {
            secondPlayer.type = PlayerType.AI;
            dictionary.Shuffle();
        }
        else
            secondPlayer.type = PlayerType.Human;
        networkGame = false;
    }
#endregion

#region Game controller functions
    public void StartGame(string word)
    {
        field.SetActive(true);

        for (var i = 0; i < cells.Count; i++)
            cells[i].SetValue(" ");

        UIController.instance.ActivateGameUI();
        startWord = word;

        if (networkGame)
        {
            NetworkGameController.instance.startWord = word;
            for (var i = 0; i < centerLine.Count; i++)
                centerLine[i].SetValue(NetworkGameController.instance.startWord[i].ToString());
        }
        else
            for (var i = 0; i < centerLine.Count; i++)
                centerLine[i].SetValue(word[i].ToString());

        gameWithTimer = UIController.instance.gameWithTimer.isOn;


        ShowAccessibleCells();

        InitPlayers();

        firstPlayer.StartStep();
        currentPlayer = firstPlayer;
        isGame = true;
    }
    ///<summary>
    /// Function for play button
    ///</summary>
    public void Play()
    {
        if (networkGame)
        {
            NetworkGameController.instance.startWord = dictionary.FindAll(pred => pred.Length == 5)[Random.Range(0, dictionary.FindAll(pred => pred.Length == 5).Count)];
            NetworkGameController.instance.StartGame(NetworkGameController.instance.startWord);
        }
        else
            StartGame(dictionary.FindAll(pred => pred.Length == 5)[Random.Range(0, dictionary.FindAll(pred => pred.Length == 5).Count)]);
    }

    ///<summary>
    /// Initialization links for player prefabs
    ///</summary>
    public void InitPlayers()
    {
        firstPlayer.enemy = secondPlayer;
        secondPlayer.enemy = firstPlayer;

        currentPlayer = firstPlayer;

        firstPlayer.GetPlayerUI().timerImage = UIController.instance.firstPlayerTimerImage;
        firstPlayer.GetPlayerUI().nameText = UIController.instance.firstPlayerNameText;
        firstPlayer.GetPlayerUI().scoreText = UIController.instance.firstPlayerScoreText;

        secondPlayer.GetPlayerUI().timerImage = UIController.instance.secondPlayerTimerImage;
        secondPlayer.GetPlayerUI().nameText = UIController.instance.secondPlayerNameText;
        secondPlayer.GetPlayerUI().scoreText = UIController.instance.secondPlayerScoreText;

        firstPlayer.playerName = UIController.instance.firstPlayerName.text;
        secondPlayer.playerName = UIController.instance.secondPlayerName.text;

        firstPlayer.UpdateScore();
        secondPlayer.UpdateScore();

        firstPlayer.UpdateName();
        secondPlayer.UpdateName();
    }

    ///<summary>
    /// Every cell at field has index for identification. That function get Cell with special index
    ///</summary>
    public Cell GetCellWithIndex(Vector2 index)
    {
        return cells.Find(pred => pred.index == index);
    }

    ///<summary>
    /// Destroy players after gameover
    ///</summary>
    public void ClearPlayers()
    {
        if (firstPlayer != null)
            Destroy(firstPlayer.gameObject);
        if (secondPlayer != null)
            Destroy(secondPlayer.gameObject);
    }

    ///<summary>
    /// If field is full we show game over popup with winner name
    ///</summary>
    public void CheckStep(Player nextPlayer)
    {
        if (cells.FindAll(pred => pred.value.letter == " ").Count > 0)
        {
            nextPlayer.StartStep();
            currentPlayer.GetPlayerUI().StopTimer();
            currentPlayer = nextPlayer;
        }
        else
        {
            var winner = "";
            if (firstPlayer.GetScore() > secondPlayer.GetScore())
                winner = firstPlayer.playerName;
            else if (firstPlayer.GetScore() < secondPlayer.GetScore())
                winner = secondPlayer.playerName;
            else
                winner = firstPlayer.playerName + " & " + secondPlayer.playerName;
            UIController.instance.ShowGameOverPopup(winner, networkGame);
            isGame = false;
            ClearPlayers();
        }
    }

    ///<summary>
    /// Mark cells for adding letter
    ///</summary>
    public void ShowAccessibleCells()
    {
        for (var i = 0; i < cells.Count; i++)
            if (cells[i].value.letter == " " && cells[i].cells.FindAll(pred => pred.value.letter != " ").Count != 0)
                cells[i].SetBackgroundColor(new Color(0.7f, 0.7f, 0.7f));
            else
                cells[i].HideBackground();
    }

    ///<summary>
    /// If timer finish step turn moves to the next player
    ///</summary>
    public void EndTimer()
    {
        currentPlayer.CancelCombination();
        currentPlayer.CancelSelectedCell();
        currentPlayer.EndStep();
        CheckStep(currentPlayer.enemy);
        UIController.instance.ActivatePopupWithoutLetter(false);
        UIController.instance.ActivateWarning(false);
        UIController.instance.ActivatePopupWordUsed(false);
        UIController.instance.ActivatePopupWaitConfirmation(false);
    }

    ///<summary>
    /// Add word to current player word list
    ///</summary>
    public void AddWordToCurrentPlayer()
    {
        if (networkGame)
            NetworkController.instance.myPlayer.GetNetworkController().AllowEnemyWord(currentPlayer.gameObject);
        else
            currentPlayer.AddWord(currentPlayer.GetWord());
    }

    ///<summary>
    /// Restart cell combination of current player
    ///</summary>
    public void RestartCurrentPlayerStep()
    {
        if (networkGame)
        {
            if (NetworkController.instance.myPlayer.GetNetworkController().netId == currentPlayer.GetNetworkController().netId)
                currentPlayer.RestartStep();
            else
                NetworkController.instance.myPlayer.GetNetworkController().CancelEnemyWord(currentPlayer.gameObject);
        }
        else
            currentPlayer.RestartStep();
    }

    ///<summary>
    /// Check word of current player. If word added step turn moves to the next player
    ///</summary>
    public void CheckCurrentPlayerWord()
    {
        if (networkGame)
        {
            if (NetworkController.instance.myPlayer.GetNetworkController().netId == currentPlayer.GetNetworkController().netId)
                currentPlayer.TryAddNewWord();
        }
        else
            currentPlayer.TryAddNewWord();
    }

    ///<summary>
    /// Back to main menu
    ///</summary>
    public void BackToMenu()
    {
        field.SetActive(false);
        isGame = false;
        UIController.instance.ActivateStartMenu();
        if (networkGame)
            NetworkController.instance.StopGame();
    }

    ///<summary>
    /// Load dictionary from xml file
    ///</summary>
    private void LoadDictionary()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("Dictionary");

        var dictionaryDocument = new XmlDocument();
        dictionaryDocument.LoadXml(textAsset.text);

        var dicionaryNodes = dictionaryDocument.DocumentElement.ChildNodes;

        dictionary.Clear();

        for (var i = 0; i < dicionaryNodes.Count; i++)
            dictionary.Add(dicionaryNodes[i].InnerText);
    }

    ///<summary>
    /// Update players names
    ///</summary>
    public void UpdateFirstPlayerName(string name)
    {
        firstPlayer.SetName(name);
    }

    public void UpdateSecondPlayerName(string name)
    {
        secondPlayer.SetName(name);
    }
#endregion

}