//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Fusion.Sockets;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(NetworkSceneManagerDefault))]
public class NetWorkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner _currentNetworkRunner;

    public event Action OnJoinLobby = delegate {  };
    
    public event Action<List<SessionInfo>> EventSessionListUpdate = delegate {};

    private void Awake()
    {
        //Viene del menu
        //if(_currentNetworkRunner.ActivePlayers.Count() <=1)
        //    CreateGame("As", SceneManager.GetActiveScene().name);
    }


    public void JoinLobby()
    {
        //if(_currentNetworkRunner) Destroy(_currentNetworkRunner);

        _currentNetworkRunner = Instantiate(_currentNetworkRunner);

        _currentNetworkRunner.AddCallbacks(this);

        var clientTask = JoinLobbyTask();
    }
    async Task JoinLobbyTask()
    {
        var result = await _currentNetworkRunner.JoinSessionLobby(SessionLobby.Custom, "Normal Lobby");
        Debug.Log(result);

        if (result.Ok)
        {
            OnJoinLobby();
        }
        else Debug.LogError("[Custom Error] Unable to join Lobby");
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        var client = InitializeGame(GameMode.Host, sessionName, SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}"));
    }
    
    public void JoinGame(SessionInfo info) //Desde esta variable se consigue el nombre de la sesion a cargar
    {
        var client = InitializeGame(GameMode.Client, info.Name);
    }
    
    async Task InitializeGame(GameMode gamemode, string sessionGame, SceneRef? sceneToLoad = null)
    {
        var sceneManage = _currentNetworkRunner.GetComponent<NetworkSceneManagerDefault>();
        _currentNetworkRunner.ProvideInput = true;

        Debug.Log("Initialize");
        
        var result = await _currentNetworkRunner.StartGame(new StartGameArgs()
        {
            GameMode =  gamemode,
            Scene = sceneToLoad,
            SessionName = sessionGame,
            CustomLobbyName = "Normal Lobby",
            SceneManager = sceneManage,
            PlayerCount = 2
        });
        
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        EventSessionListUpdate(sessionList);
    }
    
    
    
    //Nada
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }


    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}