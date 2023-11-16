using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using UnityEditor;

public class SessionBrowserHandler : MonoBehaviour
{

    [SerializeField] NetWorkRunnerHandler _networkHandler;
    [SerializeField] Text _statusText;
    [SerializeField] SessionItem _sessionPrefab;
    [SerializeField] VerticalLayoutGroup _verticalLayoutGroup;

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
        ClearSessionList(); // limpiar previamente las sesiones creadas.

        if (allSessions.Count == 0) //chequeamos que la lista no este vacia.
        {
            NoSessionFound();
            return;
        }

        foreach (var session in allSessions)
        {
            AddNewSession(session);
       
        }

    }

    void ClearSessionList()
    {
        foreach (GameObject child in _verticalLayoutGroup.transform)
        {
            Destroy(child);

        }
        _statusText.gameObject.SetActive(false);

    }

    void NoSessionFound()
    {
        _statusText.text = "No sessions found";
        _statusText.gameObject.SetActive(true);

    }

    void AddNewSession(SessionInfo sessionInfo)
    {
        var currentItem = Instantiate(_sessionPrefab, _verticalLayoutGroup.transform);
        currentItem.SetInfo(sessionInfo, JoinSelectedSession);

    }

    void JoinSelectedSession(SessionInfo session)
    {
        _networkHandler.JoinGame(session);

    }


}
