//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using Fusion;
using System;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer Local { get; private set; }

    NickName _myNickname;

    public event Action OnDespawned = delegate { };

    [Networked(OnChanged = nameof(OnNicknameChanged))]
    string Nickname { get; set; }




    public override void Spawned()
    {
        _myNickname = NickNameHandler.Instance.GetNewNickname(this);

        if (Object.HasInputAuthority)
        {

                Local = this;
            string newName = PlayerPrefs.GetString("nickname");
            RPC_SendNewNickname(newName);
        }


    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_SendNewNickname(string newName)
    {
        //Podemos pasar ese string por un filtro de palabras banneadas

        Nickname = newName;
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