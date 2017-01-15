using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {

    public void StartButtonClick()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }
}
