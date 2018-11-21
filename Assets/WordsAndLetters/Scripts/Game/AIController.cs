using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public struct WordInfo
{
    public string fullWord;
    public string wordWithoutLetter;
}

public class AIController : MonoBehaviour
{
#region Variables
    private List<Cell> _combination;
    private Cell _selectedCell;
    private Player _player;
#endregion

#region Unity functions
    void Awake()
    {
        _player = GetComponent<Player>();
    }
#endregion

#region AI functions
    public void StartStep()
    {
        _selectedCell = null;
        _combination = null;
        GetCombination(AvailableWords(GetLettersFromField(), GameController.instance.dictionary));
        StartCoroutine(ShowCombination(_combination, _selectedCell));
    }

    ///<summary>
    /// Get all letters from feild for finding words
    ///</summary>
    public string GetLettersFromField()
    {
        var letters = "";
        for (var cellIndex = 0; cellIndex < GameController.instance.cells.Count; cellIndex++)
            if (GameController.instance.cells[cellIndex].value.letter != " ")
            {
                letters += GameController.instance.cells[cellIndex].value.letter.ToLower();
            }
        return letters;
    }

    ///<summary>
    /// Get words from dictionary that can be on the field
    ///</summary>
    public List<WordInfo> AvailableWords(string fieldLetters, List<string> dictionary)
    {
        var words = new List<WordInfo>();
       
        dictionary = dictionary.Except(_player.words.ConvertAll(pred=>pred.ToLower())).ToList();
        dictionary = dictionary.Except(_player.enemy.words.ConvertAll(pred=>pred.ToLower())).ToList();
        dictionary.Remove(GameController.instance.startWord);
        for (var wordIndex = 0; wordIndex < dictionary.Count; wordIndex++)
        {
            WordInfo word;
            word.fullWord = "";
            word.fullWord += dictionary[wordIndex];

            word.wordWithoutLetter = "";

            var letters = "";
            letters += fieldLetters;

            var countOfMissintLetter = 0;

            for (var letterIndex = 0; letterIndex < word.fullWord.Length && countOfMissintLetter <= 1; letterIndex++)
            {
                if (letters.IndexOf(word.fullWord[letterIndex]) == -1)
                {
                    countOfMissintLetter++;
                    word.wordWithoutLetter += " ";
                }
                else
                {
                    letters.Remove(letters.IndexOf(word.fullWord[letterIndex]));
                    word.wordWithoutLetter += word.fullWord[letterIndex];
                }

            }

            if (countOfMissintLetter <= 1)
                words.Add(word);
        }
        return words;
    }

    ///<summary>
    /// Get cells for selected word from dictionary. We finding new cell till don't find full combination. If we can't find next cell we try to find new word
    ///</summary>
    public List<Cell> GetCombination(List<WordInfo> dictionary)
    {
        for (var wordIndex = 0; wordIndex < dictionary.Count; wordIndex++)
        {
            var currentWord = dictionary[wordIndex];
            if (currentWord.wordWithoutLetter.Contains(" "))
            {
                foreach (var cell in GameController.instance.cells.FindAll(pred => char.ToLower(pred.value.letter[0]) == char.ToLower(currentWord.wordWithoutLetter[0])))
                {
                    var path = new List<Cell>();
                    if (cell.value.letter[0].ToString() == " ")
                    {
                        cell.SetValue(currentWord.fullWord[0].ToString());
                        _selectedCell = cell;
                    }

                    FindNextCell(path, cell, currentWord);
                    if (_combination != null)
                        return _combination;
                    else if (_selectedCell != null)
                        _selectedCell.SetValue(" ");
                }
            }
            else
            {
                for (var letterIndex = 0; letterIndex < currentWord.fullWord.Length; letterIndex++)
                {
                    currentWord.wordWithoutLetter = currentWord.wordWithoutLetter.Remove(letterIndex, 1);
                    currentWord.wordWithoutLetter = currentWord.wordWithoutLetter.Insert(letterIndex, " ");

                    foreach (var cell in GameController.instance.cells.FindAll(pred => char.ToLower(pred.value.letter[0]) == char.ToLower(currentWord.wordWithoutLetter[0])))
                    {
                        var path = new List<Cell>();
                        if (cell.value.letter[0].ToString() == " ")
                        {
                            cell.SetValue(currentWord.fullWord[0].ToString());
                            _selectedCell = cell;
                        }

                        FindNextCell(path, cell, currentWord);
                        if (_combination != null)
                            return _combination;
                        else if (_selectedCell != null)
                            _selectedCell.SetValue(" ");
                    }
                    currentWord.wordWithoutLetter = currentWord.fullWord;
                }
            }
        }
        return null;
    }

    ///<summary>
    /// Try to find next cell for selected word
    ///</summary>
    public void FindNextCell(List<Cell> path, Cell currentCell, WordInfo word)
    {
        if (_combination != null)
            return;

        var tempPath = new List<Cell>(path);

        tempPath.Add(currentCell);
        if (tempPath.Count < word.fullWord.Length)
        {
            foreach (var cell in tempPath.Last().cells.FindAll(pred => char.ToLower(word.wordWithoutLetter[tempPath.Count]) == char.ToLower(pred.value.letter[0]) && !tempPath.Contains(pred)))
            {
                if (word.wordWithoutLetter[tempPath.Count].ToString() == " ")
                {
                    cell.SetValue(word.fullWord[tempPath.Count].ToString());
                    _selectedCell = cell;
                }

                FindNextCell(tempPath, cell, word);

                if (_combination != null)
                    return;
                else if (_selectedCell != null)
                    _selectedCell.SetValue(" ");
            }
        }
        else
        {

            _combination = tempPath;
        }
    }

    ///<summary>
    /// Show selected word
    ///</summary>
    public IEnumerator ShowCombination(List<Cell> combination, Cell cell)
    {
        cell.SetBackgroundColor(Color.green);
        yield return new WaitForSeconds(0.5f);
        for (var i = 0; i < combination.Count; i++)
        {
            combination[i].SetBackgroundColor(Color.yellow);
            yield return new WaitForSeconds(0.5f);
        }
        ApplyWord();
    }
    ///<summary>
    /// Add selected word to player word list
    ///</summary>
    public void ApplyWord()
    {
        _player.SetCellCombination(_combination);
        _player.SetSelectedCell(_selectedCell);
        _player.AddWord(_player.GetWord());
    }
#endregion
}