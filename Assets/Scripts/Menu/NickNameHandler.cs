using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NickNameHandler : MonoBehaviour
{
    public static NickNameHandler Instance { get; private set; }

    [SerializeField] NickName _nicknameItemPrefab;

    List<NickName> _allNicknames;

    void Awake()
    {
        Instance = this;

        _allNicknames = new List<NickName>();
    }

    public NickName GetNewNickname(NetworkPlayer owner)
    {
        var newNickname = Instantiate(_nicknameItemPrefab, transform);
        _allNicknames.Add(newNickname);

        newNickname.SetOwner(owner.transform);

        owner.OnDespawned += () =>
        {
            _allNicknames.Remove(newNickname);
            Destroy(newNickname.gameObject);
        };

        return newNickname;
    }

    void LateUpdate()
    {
        foreach (var nickname in _allNicknames)
        {
            nickname.UpdatePositionText();
        }
    }
}
