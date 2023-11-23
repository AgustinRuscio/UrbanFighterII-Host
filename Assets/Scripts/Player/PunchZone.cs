//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;
using Fusion;


public class PunchZone : NetworkBehaviour
{
    [SerializeField]
    private float _damage;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Input De ataque");


        if (!Object || !Object.HasStateAuthority) return;

        Debug.Log("Tire una piña");

        var damageable = other.GetComponent<IDamageable>();

        if(damageable == null) return;

        damageable.TakeDamage(_damage); 
    }
}