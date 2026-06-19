using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // appele par bouton Play
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // bouton "Quitter"
    public void QuitGame()
    {
        Debug.Log("Le joueur a quitté le jeu !");
        Application.Quit();
    }
}