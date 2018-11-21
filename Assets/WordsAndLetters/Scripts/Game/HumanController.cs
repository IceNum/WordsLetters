using UnityEngine;
using System.Linq;
using KaboomStudio;

public class HumanController : MonoBehaviour
{
#region Variables
    private bool _waitCell;
    private bool _waitLetter;
    private bool _waitCombination;
    private Player _player;
#endregion

#region Unity fuctions
    void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        if (_waitCell)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition).GetVector2(), Vector2.zero);
                if (hit)
                {
                    if (_player.GetSelectedCell() != null)
                        _player.GetSelectedCell().HideBackground();

                    _player.SetSelectedCell(hit.transform.GetComponent<Cell>());

                    if (_player.GetSelectedCell().value.letter == " " && _player.GetSelectedCell().cells.FindAll(pred => pred.value.letter != " ").Count != 0)
                    {
                        SoundController.instance.SelectSound();
                        GameController.instance.ShowAccessibleCells();
                        _player.GetSelectedCell().SetBackgroundColor(Color.green);
                        _waitLetter = true;
                        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                            UIController.instance.ActivateKeyboard(true);
                    }
                    else
                    {
                        _player.SetSelectedCell(null);
                        _waitLetter = false;
                    }
                }
            }
        }
        else if (_waitCombination)
        {
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition).GetVector2(), Vector2.zero);
                if (hit)
                {
                    var _tempCell = hit.transform.GetComponent<Cell>();
                    if (_player.GetCellCombination().Find(pred => pred == _tempCell) == null && _tempCell.value.letter != " ")
                    {
                        if (_player.GetCellCombination().Count == 0)
                        {
                            _player.AddToCombination(_tempCell);
                        }
                        else
                        {
                            if (_player.GetCellCombination().Last().cells.Find(pred => pred == _tempCell) != null)
                                _player.AddToCombination(_tempCell);
                            else
                                _player.CancelCombination();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _player.TryAddNewWord();
            }
        }

    }

    void OnGUI()
    {
        if (_waitLetter)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                SetLetter(e.keyCode.ToString());
            }
        }
    }
#endregion

#region Human controller fuctions
    public void SetLetter(string letter)
    {
        SoundController.instance.SelectSound();
        _waitLetter = false;
        _waitCell = false;
        _waitCombination = true;

        _player.GetSelectedCell().SetValue(letter);
        if (GameController.instance.networkGame)
            _player.GetNetworkController().SyncCell(_player.GetSelectedCell().index, _player.GetSelectedCell().value.letter);
        _player.GetSelectedCell().SetBackgroundColor(Color.blue);
        if (_player.cell.value.letter == " ")
            _player.RestartStep();
    }

    public void StartStep()
    {
        _waitCell = true;
        _waitLetter = false;
        _waitCombination = false;
    }

    public void EndStep()
    {
        _waitCell = false;
        _waitLetter = false;
        _waitCombination = false;
    }
#endregion
}