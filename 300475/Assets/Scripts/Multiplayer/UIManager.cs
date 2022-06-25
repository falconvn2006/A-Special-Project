using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public Text statusText;

    public GameObject killfeed;
    public GameObject feedPrefab;
    public GameObject hitmarker;
    public GameObject pauseMenu;
    
    public string username;

    public bool isPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        
        DontDestroyOnLoad(this.gameObject);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape) && Client.instance.isConnected){
            if(isPaused){
                Resume();
            }
            else{
                Pause();
            }
        }
    }

    public void Resume(){
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
    }

    void Pause(){
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(true);
    }

    /// <summary>Attempts to connect to the server.</summary>
    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        username = usernameField.text;
        Client.instance.ConnectToServer();
    }

    public static void AddFeed(string killer, string killed){
        GameObject feed = Instantiate(instance.feedPrefab, instance.killfeed.transform);
        feed.GetComponentInChildren<Text>().text = killer + " killed " + killed;

        Destroy(feed, 5f);
    }

    public static void ReEnableStartMenu(){
        instance.startMenu.SetActive(true);
        instance.usernameField.interactable = true;
        instance.statusText.text = "Server Shutdown. Choose a new server.";
    }

    public void HitMarker(){
        StartCoroutine(instance.HitMarkerRoutine());
    }
    
    IEnumerator HitMarkerRoutine(){
        hitmarker.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        hitmarker.SetActive(false);
    }
    
    public void LeaveServer(){
        if(Client.instance.isConnected){
            Destroy(GameManager.instance.gameObject);
            Client.instance.Disconnect(0);
            pauseMenu.SetActive(false);
            startMenu.SetActive(true);
            Destroy(Client.instance.gameObject);
            Destroy(UIManager.instance.gameObject);
        }
    }
    
    public void CloseGame(){
        Application.Quit();
    }
}
