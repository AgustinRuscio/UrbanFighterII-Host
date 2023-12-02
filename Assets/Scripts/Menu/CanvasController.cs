//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    public void BTN_Play(string Scene) => SceneManager.LoadScene(Scene);        

    public void BTN_Menu() => SceneManager.LoadScene("MainMenu");        

    public void BTN_Exit() => Application.Quit();


    private void Awake()
    {
        var x = FindObjectOfType<SpwnNetwrokPlayer>();
        
        if(x != null)
            Destroy(x.gameObject);
    }
}