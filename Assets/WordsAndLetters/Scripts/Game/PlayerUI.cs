using UnityEngine;
using UnityEngine.UI;


///<summary>
/// Class for updating UI for players
///</summary>
public class PlayerUI : MonoBehaviour
{

#region Variables
    public Image timerImage;

    public Text scoreText;
    public Text nameText;

    private float _timer;
    private bool _timerOn;
#endregion

#region Unity functions
    void Update()
    {
        if (_timerOn)
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime / GameController.instance.timerDuration;
                timerImage.fillAmount = _timer;
            }
            else
            {
                StopTimer();
                GameController.instance.EndTimer();
            }
        }
    }
#endregion

#region Player UI controller
    public void StopTimer()
    {
        _timerOn = false;
        timerImage.enabled = false;
    }

    public void StartTimer()
    {
        CustomConsole.ShowMessage("Test");
        _timerOn = true;
        timerImage.enabled = true;
        _timer = 1;
    }

    public void ActivateTimerImage(bool activate)
    {
        timerImage.fillAmount = 1;
        timerImage.enabled = activate;
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
#endregion

}