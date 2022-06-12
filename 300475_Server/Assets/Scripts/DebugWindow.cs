using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugWindow : MonoBehaviour
{
    public Text text;

    // Use this for initialization
    void Start ()
    {

    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    void FixedUpdate(){
        Application.logMessageReceived += LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        if (text.text.Length > 300)
        {
            text.text = message + "\n";
        }
        else
        {
            text.text += message + "\n";
        }
    }
}
