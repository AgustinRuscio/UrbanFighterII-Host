//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using Fusion;

public class PlayerView 
{
    private NetworkMecanimAnimator _animator;

    public PlayerView(NetworkMecanimAnimator animator)
    {
        _animator = animator;
    }

    public void Move(float x) => _animator.Animator.SetFloat("Move", x);
    public void Jump() => _animator.Animator.SetTrigger("Jump");
    //public void Jump() => _animator.SetTrigger("Jump", true);

    public void Crouch (bool isCrouching) => _animator.Animator.SetBool("Crouch", isCrouching);

    public void Punch() => _animator.Animator.SetTrigger("Punch");
    public void GetHurt() => _animator.Animator.SetTrigger("GetHurt");
    public void LowKick() => _animator.Animator.SetTrigger("LowKick");
    public void HighKick() => _animator.Animator.SetTrigger("HighKick");
    public void Blocking(bool isBlocking) => _animator.Animator.SetBool("Blocking", isBlocking);
}