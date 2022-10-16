using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsBackButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
