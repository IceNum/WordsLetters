using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
#region Variables
    public static UIController instance;

    public GameObject warningPopup;
    public Text warningContent;
    [Space]

    public GameObject waitConfirmation;
    [Space]
    public GameObject popupWithoutLetter;
    public GameObject popupWordUsed;
    [Space]

    public GameObject startMenu;
    public GameObject gameSettingsMenu;
    public GameObject multiplayerMenu;
    public GameObject rules;
    public GameObject gameUI;
    [Space]

    public InputField firstPlayerName;
    public InputField secondPlayerName;
    public GameObject waitSecondPlayer;

    public Button PlayButton;
    public Toggle gameWithTimer;
    [Space]
    public GameObject gameOverPopup;
    public Text gameOverContent;
    public GameObject goToMenu;
    public GameObject restartGruop;
    [Space]
    public Text currentWord;

    [Space]
    public Image firstPlayerTimerImage;
    public Text firstPlayerScoreText;
    public Text firstPlayerNameText;
    [Space]
    public Image secondPlayerTimerImage;
    public Text secondPlayerScoreText;
    public Text secondPlayerNameText;

    [Space]
    public GameObject keyboard;

    [Space]
    public GameObject loader;
#endregion

#region Unity functions
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ActivateStartMenu();
    }
#endregion

#region Switch ui screens
    public void ActivateGameUI()
    {
        gameUI.SetActive(true);
        startMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        loader.SetActive(false);

        GameController.instance.field.SetActive(true);
    }
    public void ActivateMultiplayerMenu()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        loader.SetActive(false);
    }
    public void ActivateLocalGameSettingsMenu()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(true);

        firstPlayerName.gameObject.SetActive(true);
        firstPlayerName.text = "Player1";
        firstPlayerName.interactable = true;

        secondPlayerName.gameObject.SetActive(true);
        secondPlayerName.text = "Player2";
        secondPlayerName.interactable = true;

        PlayButton.interactable = true;
        waitSecondPlayer.SetActive(false);
        loader.SetActive(false);
    }
    public void ActivateHostRoomMenu()
    {
        GameController.instance.field.SetActive(false);
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(true);

        firstPlayerName.interactable = true;

        secondPlayerName.gameObject.SetActive(false);
        secondPlayerName.interactable = false;
        waitSecondPlayer.SetActive(true);
        PlayButton.gameObject.SetActive(true);
        PlayButton.interactable = false;
        loader.SetActive(false);
    }
    public void ActivateHostFullRoomMenu()
    {
        GameController.instance.field.SetActive(false);
        secondPlayerName.gameObject.SetActive(true);
        secondPlayerName.interactable = false;
        waitSecondPlayer.SetActive(false);
        PlayButton.interactable = true;
        loader.SetActive(false);
    }
    public void ActivateClientRoomMenu()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(true);

        firstPlayerName.interactable = false;
        secondPlayerName.interactable = true;

        secondPlayerName.gameObject.SetActive(true);
        secondPlayerName.interactable = true;
        waitSecondPlayer.SetActive(false);
        PlayButton.gameObject.SetActive(false);
        loader.SetActive(false);
    }
    public void ActivateSinglePlayerMenu()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(true);

        secondPlayerName.gameObject.SetActive(true);
        secondPlayerName.text = "Bot";
        secondPlayerName.interactable = false;

        PlayButton.interactable = true;
        PlayButton.gameObject.SetActive(true);
        waitSecondPlayer.SetActive(false);
        loader.SetActive(false);
    }
    public void ActivateStartMenu()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
        GameController.instance.field.SetActive(false);
        loader.SetActive(false);
        rules.SetActive(false);
    }
    public void ActivateRules()
    {
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
        GameController.instance.field.SetActive(false);
        loader.SetActive(false);
        rules.SetActive(true);
    }
    public void ActivateLoader()
    {
        loader.SetActive(true);
        gameUI.SetActive(false);
        startMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        gameSettingsMenu.SetActive(false);
        GameController.instance.field.SetActive(false);
    }

    public void HideGameOverPopup()
    {
        gameOverPopup.SetActive(false);
    }
#endregion

#region Activate popups
    public void ActivateWarning(bool activate)
    {
        warningPopup.SetActive(activate);
        warningContent.text = "The word is not in the dictionary";
    }

    public void ActivateWarning(bool activate, string word)
    {
        warningPopup.SetActive(activate);
        warningContent.text = "The word \"" + word + "\" is not in the dictionary";
    }


    public void ActivatePopupWithoutLetter(bool activate)
    {
        popupWithoutLetter.SetActive(activate);
    }

    public void ActivatePopupWordUsed(bool activate)
    {
        popupWordUsed.SetActive(activate);
    }

    public void ActivatePopupWaitConfirmation(bool activate)
    {
        waitConfirmation.SetActive(activate);
    }
    public void ActivateKeyboard(bool activate)
    {
        keyboard.SetActive(activate);
    }
    public void ShowWord(string word)
    {
        currentWord.text = word;
    }

    public void ShowGameOverPopup(string winner, bool networkGame)
    {
        gameOverPopup.SetActive(true);
        gameUI.SetActive(false);
        if (networkGame)
        {
            gameOverContent.text = winner + " won. Restart?";
            restartGruop.SetActive(false);
            goToMenu.SetActive(true);
        }
        else
        {
            gameOverContent.text = winner + " won. Restart?";
            restartGruop.SetActive(true);
            goToMenu.SetActive(false);
        }
    }
#endregion
   
#region Network functions
    public void SyncPlayersNames()
    {
        if (GameController.instance.firstPlayer != null)
            firstPlayerName.text = GameController.instance.firstPlayer.playerName;
        if (GameController.instance.secondPlayer != null)
            secondPlayerName.text = GameController.instance.secondPlayer.playerName;
    }
#endregion
}
