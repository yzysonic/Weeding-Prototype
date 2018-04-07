using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonReplay : MonoBehaviour {

    public void OnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
