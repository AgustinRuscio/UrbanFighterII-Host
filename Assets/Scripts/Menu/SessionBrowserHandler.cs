using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SessionBrowser : MonoBehaviour
{

    [SerializeField] NetWorkRunnerHandler _networkHandler;


    void OnEnabled()
    {

        _networkHandler.EventSessionListUpdate += ReceiveSessionList;

    }

    void OnDisabled()
    {

        _networkHandler.EventSessionListUpdate -= ReceiveSessionList;


    }

    void ReceiveSessionList(List<SessionInfo> allSessions)
    {



    }

}
