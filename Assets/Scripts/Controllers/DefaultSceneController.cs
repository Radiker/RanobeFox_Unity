using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultSceneController : MonoBehaviour
{
    //Запуск сцены по имени
    public void LoadScene(string Name)
    {
        SceneManager.LoadScene(Name);
    }

    //Выход из приложения
    public void Exit()
    {
        Application.Quit();
    }
}
