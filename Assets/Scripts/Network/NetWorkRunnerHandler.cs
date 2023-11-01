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

[RequireComponent(typeof(NetworkRunner))]
[RequireComponent(typeof(NetworkSceneManagerDefault))]
public class NetWorkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkRunner _networkRunner;
    
    private NetworkRunner _currentNetworkRunner;

    public event Action OnJoinLobby = delegate {  };
    
    public event Action<List<SessionInfo>> EventSessionListUpdate = delegate {};

    public void JoinLobby()
    {
        if(_currentNetworkRunner) Destroy(_currentNetworkRunner);

        _currentNetworkRunner = Instantiate(_networkRunner);
        
        _currentNetworkRunner.AddCallbacks(this);

        var clientTask = JoinLobbyTask();
    }
    async Task JoinLobbyTask()
    {
        var result = await _currentNetworkRunner.JoinSessionLobby(SessionLobby.Custom, "Normal Lobby");

        if (result.Ok)
        {
            OnJoinLobby();
        }
        else
        {
            Debug.LogError("[Custom Error] Unable to join Lobby");
        }
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
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }


    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}