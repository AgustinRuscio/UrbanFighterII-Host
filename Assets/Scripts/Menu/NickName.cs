using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NickName : MonoBehaviour
{

    const float Y_OFFSET = 2.5f;

    Transform _owner;
    Text _myText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetOwner(Transform owner)
    {
        _owner = owner;
        _myText = GetComponent<Text>();

    }

    public void UpdateNickName(string newName)
    {
        _myText.text = newName;
    }

    public void UpdatePositionText()
    {
        transform.position = _owner.transform.position;
    }

}
