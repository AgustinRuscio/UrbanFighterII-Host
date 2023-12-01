//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using UnityEngine;
using System;
using Fusion;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerModel : NetworkBehaviour, IDamageable
{
    //NetWork

    NetworkInputData inputData;

    [SerializeField]
    private GameObject _youIndicatorPrefab;
    private GameObject _youIndicator;

    [Header("Components")]
    private NetworkRigidbody _rigidBody;

    [SerializeField]
    private NetworkMecanimAnimator _animator;

    private CapsuleCollider _capsuleCollider;

    [SerializeField]
    private LayerMask _groundMask;

    [Header("Atributes")]
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    public float _maxLlife;

    [Networked(OnChanged = nameof(OnLifeChange))][HideInInspector]
    public float _life { get; set; }

    [SerializeField]
    public LifeBar _lifeBar;

    public event Action<float> OnDamage = delegate { };

    [SerializeField]
    private Transform target;

    private Vector3 _direction;

    #region Delegates MVC

    //-----Controller
    private Action OnControllerUpdate;
    private Action OnControllerFixedUpdate;


    //-----View
    public event Action OnPunchAnim;
    public event Action OnLowKickAnim;
    public event Action OnHighKickAnim;
    public event Action OnJumpAnim;
    public event Action OnGetHurtnim;

    public event Action<bool> OnCrouchAnim;
    public event Action<bool> OnBlocking;
    public event Action<float> OnMoveAnim;

    #endregion


    [Header("Attack Zones")]
    [SerializeField]
    private GameObject _midPunchZone;

    [SerializeField]
    private GameObject _downPunchZone;

    [SerializeField]
    private GameObject _upPunchZone;

    private Vector3 initialPos;
    

    [Header("States")]
    private bool _canMove = true;
    private bool _crouching = false;
    private bool _blocking = false;
    private bool _onWarmUp =true;
    
    private bool MatchOn = true;

    void Awake()
    {
        _rigidBody = GetComponent<NetworkRigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        //Setting controller Methods in Actions
        var controller = new PlayerController(this);

        OnControllerUpdate += controller.OnUpdate;
        OnControllerFixedUpdate += controller.OnFixedUpdate;

        var view = new PlayerView(_animator);
        
        OnMoveAnim += view.Move;
        OnCrouchAnim += view.Crouch;
        OnJumpAnim += view.Jump;
        OnPunchAnim += view.Punch;
        OnLowKickAnim += view.LowKick;
        OnHighKickAnim += view.HighKick;
        OnGetHurtnim += view.GetHurt;
        OnBlocking += view.Blocking;
    }


    public override void Spawned()
    {
        initialPos = transform.position;
        _life = _maxLlife;
        CameraMovement.instance.AddPlayer(transform);
        TargetSetter.Instance.AddPlayer(this);

        if (Object.HasStateAuthority) //Soy el host?
        {

            if (Object.HasInputAuthority) //P1
            {
                GameManager.instance.AddPlayer(this, true);

                _lifeBar = GameObject.FindGameObjectWithTag("PlayerOneLifeBar").GetComponent<LifeBar>();
            }
            else //P2
            {
                GameManager.instance.AddPlayer(this, false);
                _lifeBar = GameObject.FindGameObjectWithTag("PlayerTwoLifeBar").GetComponent<LifeBar>();

            }
        }
        else //No soy host
        {
            if (Object.HasInputAuthority) //P2
            {
                GameManager.instance.AddPlayer(this, false);
                _lifeBar = GameObject.FindGameObjectWithTag("PlayerTwoLifeBar").GetComponent<LifeBar>();
            }
            else //P1
            {

                GameManager.instance.AddPlayer(this, true);
                _lifeBar = GameObject.FindGameObjectWithTag("PlayerOneLifeBar").GetComponent<LifeBar>();
            }
        }


        MatchOn = GameManager.instance.MatchState;
        _lifeBar.UpdateLifeBar(_life / _maxLlife);

        if (Object.HasInputAuthority)
        {
            _youIndicator = Instantiate(_youIndicatorPrefab, transform);
            _youIndicator.transform.Rotate(0, -90, 0);
        }

    }

    public void SetPosition(Vector3 newPos) => RPC_Position(newPos);

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Position(Vector3 newPos) => transform.position = newPos;

    public void ChangeMatchState(bool newState, bool warmUp)
    {
        MatchOn = newState;
        _onWarmUp = warmUp;
    }

    public void SetTarget(Transform newTarget) => target = newTarget;


    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        TargetSetter.Instance.RemovePlayer(this);
        CameraMovement.instance.RemovePlayer(transform);
        GameManager.instance.RemovePlayer(this);
        SceneManager.LoadScene("StartMenu");
    }


    private void Update()
    {
        if (_youIndicator != null)
            _youIndicator.transform.position = transform.position + Vector3.up * 1.4f;
    }

    public void BackToLbby()
    {
        Runner.Shutdown();
        
    }
    
    public override void FixedUpdateNetwork()
    {
        if (!MatchOn) return;

        if (GetInput(out inputData))
        {
            Move(inputData.xMovement);

            if (inputData.isJump)
                Jump();

            if (inputData._punch)
                Punch();

            if (inputData._highKick)
                HighKick();

            if (inputData._lowKick)
                LowKick();

            Crouch(inputData.isCrouching);
            Blocking(inputData.isBlocking);
        }

        if (target != null)
            transform.LookAt(target.position);
    }

    private static void OnLifeChange(Changed<PlayerModel> changed)
    {
        var behaviour = changed.Behaviour;

        behaviour.OnDamage(behaviour._life / behaviour._maxLlife);

        if (behaviour._lifeBar != null)
            behaviour._lifeBar.UpdateLifeBar(behaviour._life / behaviour._maxLlife);

        behaviour.OnGetHurtnim();
    }

    public void Move(float xMovement)
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;
        OnMoveAnim(_direction.x);

        _direction = new Vector3(xMovement, 0, 0);

        _direction *= Time.deltaTime;
        _direction.Normalize();
        _direction *= _speed;

        _rigidBody.Rigidbody.AddForce(_direction);
    }

    public void Jump()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;

        RPC_Jump();

        OnJumpAnim();

        _rigidBody.Rigidbody.AddForce(_rigidBody.Rigidbody.velocity.x, _jumpForce, _rigidBody.Rigidbody.velocity.z);
    }

    [Rpc(RpcSources.All, RpcTargets.All)] //De All a All tampoco funciona, de hecho duplica la ccion
    public void RPC_Jump()
    {
        //El de input auth / proxi (Segun yo son lo mismo) no ven la animacion
        OnJumpAnim();
        _onWarmUp = false;
    }

    public void ReFillLife()
    {
        _life = _maxLlife;
        RPC_Reposition();
        
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Reposition()
    {
        transform.position = initialPos;
    }
    
    
    public void TakeDamage(float dmg)
    {
        if (_blocking) return;

        Debug.Log("Auch");

        _life -= dmg;

        if (_life <= 0)
        {
            if(!_onWarmUp)
                Died();
            else
                ReFillLife();
        }
    }

    public void Blocking(bool isBloking)
    {
        OnBlocking(_blocking);
        _blocking = isBloking;
    }

    public void Punch()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;

        RPC_Punch();
        //OnPunchAnim();

        Debug.Log("Punch");

        OnAttackiong(false);
        _midPunchZone.SetActive(true);
        StartCoroutine(Deactivate(_midPunchZone, .7f));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Punch()
    {
        OnPunchAnim();
    }


    public void HighKick()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;

        //OnHighKickAnim();
        RPC_HighKick();

        OnAttackiong(false);
        _upPunchZone.SetActive(true);
        StartCoroutine(Deactivate(_upPunchZone, 1.5f));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_HighKick()
    {
        OnHighKickAnim();
    }

    public void LowKick()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;

        //OnLowKickAnim();
        RPC_LowKick();

        OnAttackiong(false);
        _downPunchZone.SetActive(true);
        StartCoroutine(Deactivate(_downPunchZone, 2f));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LowKick()
    {
        OnLowKickAnim();
    }

    private void OnAttackiong(bool state) => _canMove = state;

    public void Crouch(bool isBool)
    {
        if (!isLanded || _blocking) return;
        OnCrouchAnim(_crouching);

        _crouching = isBool;


        if (_crouching)
        {
            _capsuleCollider.center = new Vector3(0, -0.33f, 0.33f);
            _capsuleCollider.height = 1.33f;
        }
        else
        {
            _capsuleCollider.center = new Vector3(0, -0.1f, 0);
            _capsuleCollider.height = 1.79f;
        }
    }


    IEnumerator Deactivate(GameObject obj, float cd)
    {
        yield return new WaitForSeconds(cd);
        obj.SetActive(false);
        OnAttackiong(true);
    }

    private bool isLanded { get { return Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector3.down, 1.5f, _groundMask); } }

    private void Died()
    {
        Losing();
    }

    [Rpc(RpcSources.All, RpcTargets.InputAuthority)]
    public void RPC_Lose() => GameManager.instance.PlayerDeath(this);

    public void Losing() => RPC_Lose();

    //-------

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Win()
    {
        Debug.Log("A ver si funcionas capo");
        GameManager.instance.PlayerWin();
    }

    public void Winning()
    {
        RPC_Win();
    }
}