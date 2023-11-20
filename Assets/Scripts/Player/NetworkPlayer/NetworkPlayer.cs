//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using Fusion;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }

    NickName _myNickname;

    public event Action OnDespawned = delegate { };

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    string Nickname { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
            Local = this;
    }
    static void OnNicknameChanged(Changed<NetworkPlayer> changed)
    {
        changed.Behaviour._myNickname.UpdateNickName(changed.Behaviour.Nickname);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        OnDespawned();
    }
}