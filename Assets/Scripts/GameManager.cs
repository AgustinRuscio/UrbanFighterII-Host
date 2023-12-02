//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    [SerializeField]
    private Transform[] _spawnPoints;

    [HideInInspector]
    public List<PlayerModel> _players;

    [HideInInspector]
    public PlayerModel _playerOne, _playerTwo;

    [SerializeField]
    private GameObject loseCanvas, winCanvas, tieCanvas, FightImage, _waitingPlayerCanvas, _warmUpCanvas;

    public int PlayerCounting { get; private set; }

    [SerializeField]
    private TextMeshProUGUI _timerText;

    [SerializeField]
    private float _timer = 90;

    private bool _matchOn = false;

    public bool MatchState => _matchOn;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 1;
    }

    IEnumerator Desapear()
    {
        yield return new WaitForSeconds(2f);
        _matchOn = true;
        FightImage.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (!_matchOn) return;

        _timer -= Time.deltaTime;

        if (_timer <= 0)
            MatchFinishedByTime();
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
    
    int counting = 0;
    public void AddPlayer(PlayerModel model, bool host)
    {
        if (!_players.Contains(model))
        {
            _players.Add(model);

            if (host)
            {
                _playerOne = model;

                Debug.Log("Player One In");
                Verification();

                StartCoroutine(LetPlayerAdd());
            }
            else
            {
                _playerTwo = model;
                
                Debug.Log("Player two In");

                counting++;
                StartCoroutine(LetPlayerAdd());
            }
        }
    }

    IEnumerator LetPlayerAdd()
    {
        yield return new WaitForSeconds (.5f);

        CheckPlayerIn();
    }

    private void CheckPlayerIn()
    {
        Debug.Log("Check!!");
        if(_playerOne)
        Debug.Log(_playerOne.gameObject.name + "¨: Player One");
        if(_playerTwo)
        Debug.Log(_playerTwo.gameObject .name + "¨: Player One");

        if (counting > 0)
        {
            Debug.Log("Both players in");

            Verification();
            
            _playerOne.ChangeMatchState(true, false);  
            _playerTwo.ChangeMatchState(true, false);

            StartCoroutine(WarmUp());
        }
    }

    IEnumerator WarmUp()
    {
        _warmUpCanvas.SetActive(true);
        
        yield return new WaitForSeconds(6.5f);
        
        _warmUpCanvas.SetActive(false);
        
        _timerText.gameObject.SetActive(true);
        
        _playerOne.ReFillLife();
        _playerTwo.ReFillLife();
        
        FightImage.gameObject.SetActive(true);
        StartCoroutine(Desapear());
    }
    
    
    public void RemovePlayer(PlayerModel model)
    {
        if(_players.Contains(model))
            _players.Remove(model);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RESET()
    {
        Runner.Shutdown();
        
        var x = FindObjectOfType<SpwnNetwrokPlayer>();
        Destroy(x.gameObject);
        
        SceneManager.LoadScene("StartMenu");
        
    }


    public void BTN_GoTomenu() => RPC_RESET();
    

    public void Verification()
    {
        if (_players.Count == 1)
        {
            _timerText.gameObject.SetActive(false);
            _waitingPlayerCanvas.SetActive(true);
        }
        else
        {
            _waitingPlayerCanvas.SetActive(false);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
       if (!_matchOn) return;
        
        _timerText.text = _timer.ToString("0");
    }

    public void PlayerDeath(PlayerModel p)
    {
        Time.timeScale = 0f;
        _matchOn = false;
        loseCanvas.SetActive(true);

        if (p == _playerOne)
            _playerTwo.Winning();
        else
        {
            PlayerWin();
            //_playerOne.Winning();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlayerWin()
    {
        Time.timeScale = 0f;
        _matchOn = false;
        winCanvas.SetActive(true);
        
        Debug.Log("Despues dle RPC");
    }
    
    public void PlayerWin()
    {
        Debug.Log("Antes dle RPC");
        RPC_PlayerWin();
    }
}