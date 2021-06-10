using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class ChapterController : DefaultSceneController
{
    [Header("Вызываемый объект вывода текста")]
    public GameObject text;
    [Header("Кнопка возвращения")]
    public Button ReturnButton;
    //Область прокрутки
    private ScrollRect scrollRect;
    //Id книги, содержащей главу
    private int scene_id;

    // Start is called before the first frame update
    void Start()
    {
        //Определяем область прокрутки
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        //Параллельный запуск функции
        StartCoroutine(ShowDetail());
    }

    IEnumerator ShowDetail()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/chapters/"+DataStore.id))
        {
            //Добавляем заголовок авторизации пользователя к запросу
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //Отправляем запрос
            yield return www.SendWebRequest();
            //Обработка результатов
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Конвертируем результат из JSON в класс FullChapterRoot
                FullChapterRoot root = JsonConvert.DeserializeObject<FullChapterRoot>(www.downloadHandler.text);
                //Создаем объект для текста заголовка главы
                GameObject label = Instantiate(text, scrollRect.content.transform);
                //Задаем стиль тексту
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                //Добавляем текст
                label.GetComponentInChildren<Text>().text = "Глава " + root.data.number + ": " + root.data.name;
                //Сохраняем значения id книги
                scene_id = root.data.story_id;
                //Создаем объект для текста заголовка главы
                label = Instantiate(text, scrollRect.content.transform);
                //Добавляем текст
                label.GetComponentInChildren<Text>().text = root.data.text.ToString();
            }
        }
    }

    //Обработчик события кнопки удаления главы
    public void ClickDeleteButton()
    {
        //Параллельный запуск функции
        StartCoroutine(DeleteData());
    }
    IEnumerator DeleteData()
    {
        //Создаем Delete запрос
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/chapters/" + DataStore.id + "/destroy"))
        {
            //Добавляем заголовок авторизации пользователя к запросу
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //Отправляем запрос
            yield return www.SendWebRequest();
            //Обработка результатов
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Return();
            }
        }
    }

    //Возвращение на сцену книги, содержащей главу
    public void Return()
    {
        //Присвоение id в хранилище значения id книги
        DataStore.id = scene_id;
        //Загружаем сцену отображения книги
        LoadScene("BookScene");
    }
}
