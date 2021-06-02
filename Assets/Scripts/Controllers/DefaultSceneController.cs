using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultSceneController : MonoBehaviour
{
    public void LoadScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
