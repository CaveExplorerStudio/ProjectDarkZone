using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public Canvas quitMenu;
    public Button play, exit;
    
	void Start () {
        quitMenu.enabled = false;
	}
	
	public void ExitPress()
    {
        quitMenu.enabled = true;
        play.enabled = false;
        exit.enabled = false;
    }

    public void NoPress()
    {
        quitMenu.enabled = false;
        play.enabled = true;
        exit.enabled = true;
    }

    public void PlayGame()
    {
        Application.LoadLevel("Overworld");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}