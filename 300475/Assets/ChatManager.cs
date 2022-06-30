using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    public InputField chatInput;

    public GameObject chatPanel;

    public GameObject messagePrefab;

    public Transform messageContainer;

    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public static bool isOn = false;

    // Start is called before the first frame update
    void Start()
    {
        chatPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && chatInput.text == ""){
            if(isOn){
                chatPanel.SetActive(false);
                isOn = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else{
                chatPanel.SetActive(true);
                isOn = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if(isOn){
            if(Input.GetKeyDown(KeyCode.Return)){
                if(chatInput.text != ""){
                    if(chatInput.text != "/clear_chat"){
                        ClientSend.ChatMessage(chatInput.text);
                        chatInput.text = "";
                    }
                    else{
                        foreach(Transform child in messageContainer){
                            Destroy(child.gameObject);
                        }
                        chatInput.text = "";
                    }
                }
            }
        }
    }

    public void AddMessage(int _senderId, string _message){
        GameObject _messageObject = Instantiate(messagePrefab, messageContainer);
        _messageObject.GetComponent<Text>().text = GameManager.players[_senderId].username + ": " + _message;
    }
}
