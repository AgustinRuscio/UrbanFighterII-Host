//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System.Collections;
using UnityEngine;
using System;
using Fusion;

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

    [Networked(OnChanged = nameof(OnLifeChange))] [HideInInspector]
    public float _life { get; set; }

    [SerializeField]
    public LifeBar lifeBar;
    
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

    [Header("States")]
    private bool _canMove = true;
    private bool _crouching = false;
    private bool _blocking = false;

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
        _life = _maxLlife;  
        CameraMovement.instance.AddPlayer(transform);
        TargetSetter.Instance.AddPlayer(this);

        if (Object.HasInputAuthority)
        {
            _youIndicator = Instantiate(_youIndicatorPrefab, transform);
            _youIndicator.transform.Rotate(0, -90, 0);
        }

        StartCoroutine(wait());
    }
    public void SetPosition(Vector3 newPos) => RPC_Position(newPos);
    

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Position(Vector3 newPos) => transform.position = newPos;
    

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(.2f);

        GameManager.instance.AddPLayer(this);
        MatchOn = GameManager.instance.MatchState;
    }

    public void ChangeMatchState(bool newState) => MatchOn = newState;
    

    public void SetTarget(Transform newTarget) => target = newTarget;
    

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        TargetSetter.Instance.RemovePlayer(this);
        CameraMovement.instance.RemovePlayer(transform);
        GameManager.instance.RemovePlayer(this);
    }


    private void Update()
    {
        if(_youIndicator != null)
        _youIndicator.transform.position = transform.position + Vector3.up * 1.4f;
    }

    public override void FixedUpdateNetwork()
    {
        if (!MatchOn) return;

        //OnControllerUpdate();
        //OnControllerFixedUpdate();

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
        
        if(behaviour.lifeBar != null)
        behaviour.lifeBar.UpdateLifeBar(behaviour._life / behaviour._maxLlife);
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
        if (!_canMove || _crouching || !isLanded|| _blocking) return;
        OnJumpAnim();

        _rigidBody.Rigidbody.AddForce(_rigidBody.Rigidbody.velocity.x, _jumpForce, _rigidBody.Rigidbody.velocity.z);
    }

    public void TakeDamage(float dmg) => RPC_GetHit(dmg);

    [Rpc(RpcSources.Proxies, RpcTargets.StateAuthority)]
    public void RPC_GetHit(float dmg)
    {
        if (_blocking) return;

        OnGetHurtnim();

        _life -= dmg;      
        
        if (_life <= 0)
            Died();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.Proxies)]
    public void RPC_Win() => GameManager.instance.PlayerWin();
    

    public void Winning() => RPC_Win();
    

    public void Blocking(bool isBloking)
    {
        OnBlocking(_blocking);
        _blocking = isBloking;
    }

    public void Punch()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;
        OnPunchAnim();

        OnAttackiong(false);
        _midPunchZone.SetActive(true);
        StartCoroutine(Deactivate(_midPunchZone, .7f));
    }

    public void HighKick()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;
        OnHighKickAnim();

        OnAttackiong(false);
        _upPunchZone.SetActive(true);
        StartCoroutine(Deactivate(_upPunchZone, 1.5f));
    }

    public void LowKick()
    {
        if (!_canMove || _crouching || !isLanded || _blocking) return;
        OnLowKickAnim();

        OnAttackiong(false);
        _downPunchZone.SetActive(true);
        StartCoroutine(Deactivate(_downPunchZone, 2f));
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

    private bool isLanded{ get { return Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector3.down, 1.5f, _groundMask); } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 dir = Vector3.down * 1.5f;

        Gizmos.DrawRay(transform.position + new Vector3(0, 0.2f, 0), dir);
    }

    private void Died()
    {
        Winning();
        GameManager.instance.PlayerDeath();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Lose() => Died();
}