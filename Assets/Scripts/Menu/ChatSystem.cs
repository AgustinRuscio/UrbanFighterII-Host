using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class ChatSystem : MonoBehaviour
{


    [SerializeField] private Text _chatBox;
    [SerializeField] private Scrollbar _verticalScrollBar;
    [SerializeField] private InputField _inputMessage;

    private bool _newMessage;

   

    void Start()
    {
        _chatBox.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !_inputMessage.isFocused)
        {
            _inputMessage.ActivateInputField();
        }

        
    }

    public void TryEnterMessage()
    {
        if (string.IsNullOrEmpty(_inputMessage.text) || !Input.GetKeyDown(KeyCode.Return)) return;

        RecieveNewMessage(_inputMessage.text);
        _inputMessage.text = "";

    }


    void RecieveNewMessage(string newMsg)
    {
        _chatBox.text += '\r' + newMsg;

        _newMessage = true;

    }

    public void UpdateScrollbar()
    {
        if (!_newMessage) return;
       
        _verticalScrollBar.value = 0; //Esto es en scroll view, on value changed.

        _newMessage = false;

    }


}
