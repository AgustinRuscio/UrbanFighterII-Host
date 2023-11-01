//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class SpwnNetwrokPlayer : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField]
    private NetworkPlayer _playerPrefab;

    [SerializeField]
    private Transform _playerOneSpawnPoint, _playerTwoSpawnPoint;

    NetworkPlayerInput _playerInputs;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!NetworkPlayer.Local) return;
       
        if (_playerInputs == null)
            _playerInputs = NetworkPlayer.Local.GetComponent<NetworkPlayerInput>();
        else
            input.Set(_playerInputs.GetInputData());
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
       // if (runner.Topology == SimulationConfig.Topologies.Shared)
       // {
       //     Debug.Log("[Custom msg] On Connected to Server - Spawning local player");
       //
       //     runner.Spawn(_playerPrefab, _playerOneSpawnPoint.position, _playerOneSpawnPoint.rotation, runner.LocalPlayer);
       // }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
            runner.Spawn(_playerPrefab, _playerOneSpawnPoint.position, _playerOneSpawnPoint.rotation, player);
    }

    #region cosa


    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    #endregion
}