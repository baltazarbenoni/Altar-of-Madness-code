using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//C 2024 Daniel Snapir alias Baltazar Benoni

public class IntroSceneManager : MonoBehaviour
{
    
    void Start()
    {
        
    }
    void Update()
    {
        CheckInput();
    }
    private void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("ChangeScene");
            SceneChanger();
        }
    }
    private void SceneChanger()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
}
