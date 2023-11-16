using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;

public class SessionItem : MonoBehaviour
{

    [SerializeField] Text _sessionNameText;
    [SerializeField] Text _playerCountText;
    [SerializeField] Button _joinButton;

    SessionInfo _currentSession;

    public void SetInfo(SessionInfo session, Action <SessionInfo> onClick)
    {
        _currentSession = session;

        _sessionNameText.text = session.Name;
        _playerCountText.text = $"{session.PlayerCount} / {session.MaxPlayers}";
        _joinButton.enabled = session.PlayerCount < session.MaxPlayers;
        _joinButton.onClick.AddListener(() => onClick(session));
       
    }





}
