using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasMatchController : NetworkBehaviour
{
    [SerializeField]
    private GameObject loseCanvas, winCanvas, tieCanvas, FightImage, _waitingPlayerCanvas;

    public PlayerModel _playerOne;
    public PlayerModel _playerTwo;

    [SerializeField]
    private TextMeshProUGUI _timerText;

    [SerializeField]
    private float _timer = 90;

    private bool _matchOn = false;

    private void Update()
    {
        if (!_matchOn) return;

        _timer -= Time.deltaTime;

        if (_timer <= 0)
            MatchFinishedByTime();
    }

    public override void FixedUpdateNetwork()
    {
        if (!_matchOn) return;

        _timerText.text = _timer.ToString("0");
    }


    private void MatchFinishedByTime()
    {
        _matchOn = false;

        if (_playerOne._life == _playerTwo._life)
            MatchTie();
        else if (_playerOne._life > _playerTwo._life)
            _playerTwo.RPC_Lose();
        else
            _playerOne.RPC_Lose();
    }

    private void MatchTie()
    {
        Time.timeScale = 0f;
        _matchOn = false;
        tieCanvas.SetActive(true);

        _playerOne.ChangeMatchState(false, false);
        _playerTwo.ChangeMatchState(false, false);
    }
}
