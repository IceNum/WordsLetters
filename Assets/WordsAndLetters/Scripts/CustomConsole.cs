using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Message
{
    public string message;
    public Color color;

    public Message(string message, Color color)
    {
        this.message = message;
        this.color = color;
    }
    public override string ToString()
    {
        return message;
    }
}

///<summary>
/// Custom console allow show messages at screen replace this messages in debug log. For showing message Write "CustomConsole.ShowMessage("Message")" or
/// "CustomConsole.ShowMessage("Message",Color.green)". That script help debug at mobile devices and web project if you have not access to devece log
///</summary>

public class CustomConsole : MonoBehaviour
{
    public static CustomConsole instance;
    public Color defaultMessageColor = Color.black;
    public bool replaceInDebug;
    public bool hideConsole;
    public float duration;
    public int fontSize;
    private GUIStyle _style;

    private List<Message> _messages;

    private float _timer;
    void Awake()
    {
        instance = this;
        _style = new GUIStyle();
        _messages = new List<Message>();
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (_messages.Count > 0)
        {
            _timer += Time.deltaTime / duration;
            if (_timer > 1)
            {
                _messages.RemoveAt(0);
                _timer = 0;
            }
        }
    }

    private void _showMessage(object message, Color color)
    {
        if (message != null)
            _messages.Add(new Message(message.ToString(), color));
        else
            _messages.Add(new Message("Null", color));

        if (replaceInDebug)
            Debug.Log(message);
    }

    ///<summary>
    /// Show message with default color
    ///</summary>
    public static void ShowMessage(object message)
    {
        CustomConsole.instance._showMessage(message, CustomConsole.instance.defaultMessageColor);
    }
     ///<summary>
    /// Show message with special color
    ///</summary>
    public static void ShowMessage(object message, Color color)
    {
        CustomConsole.instance._showMessage(message, color);
    }
    void OnGUI()
    {
        if (hideConsole||_messages.Count==0)
            return;
        int panelWeight = ((_maxMessageLength()+2) * fontSize)/2;
        _style.fontSize = fontSize;
        GUI.Box(new Rect(0, 5, panelWeight, (fontSize + 5) * _messages.Count), "");
        for (var messageIndex = 0; messageIndex < _messages.Count; messageIndex++)
        {
            _style.normal.textColor = _messages[messageIndex].color;
            GUI.Label(new Rect(5, (5 + fontSize) * messageIndex, 500, fontSize), _messages[messageIndex].message, _style);
        }
    }
    int _maxMessageLength()
    {
        var temp = _messages[0].message.Length;
        for (var i = 0; i < _messages.Count; i++)
            if (temp < _messages[i].message.Length)
                temp = _messages[i].message.Length;
        return temp;

    }
}
