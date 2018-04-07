using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour {

    public void OnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
