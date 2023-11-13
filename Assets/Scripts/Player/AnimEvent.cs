using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    [SerializeField]
    private AudioClip _hurtClip, _attackClip, _jumpClip;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void GettingHurt()
    {
        _source.clip = _hurtClip;
        _source.Play();
    }

    public void Attack()
    {
        _source.clip = _attackClip;
        _source.Play();
    }

    public void Jumping()
    {
        _source.clip = _jumpClip;
        _source.Play();
    }

}
