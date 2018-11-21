using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum PlayerType
{
    AI,
    Human
}


public class Player : MonoBehaviour
{
    
#region Variables
    public List<string> words;

    public Player enemy;
    public PlayerType type;

    public string playerName;

    public Cell cell;

    private List<Cell> _combination;
    private HumanController _humanController;
    private AIController _aiController;
    private PlayerUI _ui;
    private PlayerNetworkController _pnc;
#endregion

#region Unity functions
    ///<summary>
    /// Initialization player variables
    ///</summary>
    void Awake()
    {
        _combination = new List<Cell>();
        _humanController = GetComponent<HumanController>();
        _aiController = GetComponent<AIController>();
        _ui = GetComponent<PlayerUI>();
        _pnc = GetComponent<PlayerNetworkController>();
    }
#endregion

#region Getters
    public Cell GetSelectedCell()
    {
        return cell;
    }

    public List<Cell> GetCellCombination()
    {
        return _combination;
    }

    public PlayerUI GetPlayerUI()
    {
        return _ui;
    }

    public HumanController GetHumanController()
    {
        return _humanController;
    }

    public string GetWord()
    {
        var word = "";

        for (var i = 0; i < _combination.Count; i++)
            word += _combination[i].value.letter;

        return word;
    }

    public int GetScore()
    {
        var count = 0;
        for (var i = 0; i < words.Count; i++)
            count += words[i].Length;
        return count;
    }
#endregion

#region Setters
    ///<summary>
    /// Set new name for player
    ///</summary>
    public void SetName(string name)
    {
        playerName = name;
        if (GameController.instance.networkGame)
        {
            _pnc.UpdateName(name);
        }
    }

    ///<summary>
    /// Set field cell
    ///</summary>
    public void SetSelectedCell(Cell cell)
    {
        this.cell = cell;
        if (GameController.instance.networkGame && cell != null)
            _pnc.SyncCell(cell.index, cell.value.letter);
    }

    ///<summary>
    /// Update combination
    ///</summary>
    public void SetCellCombination(List<Cell> combination)
    {
        _combination = combination;
    }
#endregion

#region Control player fuctions
    ///<summary>
    /// Start step for new player
    ///</summary>
    public void StartStep()
    {
        if (type == PlayerType.Human)// Set player controller Human or AI
            _humanController.StartStep();
        else
            _aiController.StartStep();

        if (GameController.instance.gameWithTimer)// If game with timer we start timer ui else we show user frame for current player
            _ui.StartTimer();
        else
            _ui.ActivateTimerImage(true);
    }

    ///<summary>
    /// Add letter to new word
    ///</summary>
    public void AddToCombination(Cell newCell)
    {
        SoundController.instance.SelectSound();
        _combination.Add(newCell);
        _combination.Last().SetBackgroundColor(Color.yellow);
        if (GameController.instance.networkGame)
            _pnc.AddNewIndex(newCell.index, GetWord());

        UIController.instance.ShowWord(GetWord());
        newCell = null;
    }

    ///<summary>
    /// Cancel cell if player select some cell
    ///</summary>
    public void CancelSelectedCell()
    {
        if (cell != null)
        {
            cell.HideBackground();
            cell.SetValue("");
            cell = null;
        }
    }

    ///<summary>
    /// Cancel cell if player select some cells
    ///</summary>
    public void CancelCombination()
    {
        for (var i = 0; i < _combination.Count; i++)
            _combination[i].HideBackground();
        _combination.Clear();
        if (GameController.instance.networkGame)
            _pnc.ClearIndexes();
    }

    ///<summary>
    /// New word will added if this word don't have both player in word list and this dictionary contains this word (or enemy confirm this word)
    ///</summary>
    public void TryAddNewWord()
    {
        if (_combination.Find(pred => pred == cell) != null)
        {
            if (words.FindAll(pred => pred.ToLower() == GetWord().ToLower()).Count == 0 &&
            enemy.words.FindAll(pred => pred.ToLower() == GetWord().ToLower()).Count == 0 &&
            _combination.Count != 0 &&
            GetWord().ToLower() != GameController.instance.startWord.ToLower())
            {
                if (GameController.instance.dictionary.Find(pred => pred.ToLower() == GetWord().ToLower()) != null)
                    if (GameController.instance.networkGame)
                        _pnc.AddWord(GetWord(), enemy.GetNetworkController().netId.Value);
                    else
                        AddWord(GetWord());
                else
                {
                    if (GameController.instance.networkGame)
                        _pnc.ShowWarning(enemy.GetNetworkController().netId.Value, GetWord());
                    else
                        UIController.instance.ActivateWarning(true);
                }
                Debug.Log("Next step");
            }
            else
            {
                UIController.instance.ActivatePopupWordUsed(true);
                RestartStep();
            }
        }
        else
        {
            RestartStep();
            UIController.instance.ActivatePopupWithoutLetter(true);
        }
    }

    ///<summary>
    /// Add word to player wordList
    ///</summary>
    public void AddWord(string word)
    {
        UIController.instance.ShowWord(word);
        words.Add(word);
        EndStep();

        UpdateScore();
        _ui.ActivateTimerImage(false);

        GameController.instance.ShowAccessibleCells();

        GameController.instance.CheckStep(enemy);
        cell = null;
    }

    public void EndStep()
    {
        if (type == PlayerType.Human)
            _humanController.EndStep();
    }

    public void RestartStep()
    {
        CancelSelectedCell();
        CancelCombination();

        GameController.instance.ShowAccessibleCells();

        UIController.instance.ShowWord("");

        Debug.Log("Restart Step");
        if (type == PlayerType.Human)
            _humanController.StartStep();
    }

    public void UpdateScore()
    {
        _ui.SetScore(GetScore());
    }
    public void UpdateName()
    {
        _ui.SetName(playerName);
    }
#endregion

#region Network fuctions
    public PlayerNetworkController GetNetworkController()
    {
        return _pnc;
    }
#endregion

}