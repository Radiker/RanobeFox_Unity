using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    //Переменная для хранения пути
    public string url = "";

    void Start()
    {
        //Параллельный запуск функции
        StartCoroutine(LoadFromLikeCoroutine());
    }

    // this section will be run independently
    private IEnumerator LoadFromLikeCoroutine()
    {
        //Создаем запрос на получение текстуры
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        //Отправляем запрос
        yield return www.SendWebRequest();
        //Проверяем результат запроса
        if (www.result != UnityWebRequest.Result.Success)
            Debug.Log(www.error);
        else
        {
            //Получаем текстуру из результата
            Texture2D webTexture = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
            //Создаем спрайт на основе текстуры
            Sprite webSprite = SpriteFromTexture2D(webTexture);
            //Присвоение спрайта изображению
            gameObject.GetComponent<Image>().sprite = webSprite;
        }
    }

    Sprite SpriteFromTexture2D(Texture2D texture)
    {
        //Создаем спрайт из текстуры с необходимыми параметрами
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
}