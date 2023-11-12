using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] NetWorkRunnerHandler _networkHandler;

    [Header("Panels")]

    [SerializeField] GameObject _joinLobbyPanel;
    [SerializeField] GameObject _statusPanel;
    [SerializeField] GameObject _sessionBrowserPanel;
    [SerializeField] GameObject _hostGamePanel;

    [Header("Buttons")]

    [SerializeField] Button _joinLobbyBTN;
    [SerializeField] Button _hostPanelBTN;
    [SerializeField] Button _hostGameBTN;

    [Header("InputFields")]

    [SerializeField] InputField _sessionNameField;

    [Header("Texts")]

    [SerializeField] Text _statusText;


    private void Start()
    {
        _joinLobbyBTN.onClick.AddListener(BTN_JoinLobby);
        _hostPanelBTN.onClick.AddListener(BTN_ShowHostPanel);
        _hostGameBTN.onClick.AddListener(BTN_HostGame);

        _networkHandler.OnJoinLobby += () =>
        {
            _statusPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);

        };
    }


    void BTN_JoinLobby()
    {
        _networkHandler.JoinLobby();
        _joinLobbyPanel.SetActive(false);
        _statusPanel.SetActive(true);
        _statusText.text = "Joining Lobby... ";
        //corrutina para que pase a ser un punto, dos, tres puntos..


    }

    void BTN_ShowHostPanel()
    {
        _sessionBrowserPanel.SetActive(false);
        _hostGamePanel.SetActive(true);

    }


    void BTN_HostGame()
    {

        _networkHandler.CreateGame(_sessionNameField.text, "Level1");
    }


}
