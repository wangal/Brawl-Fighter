using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public void GameStart () {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

}
