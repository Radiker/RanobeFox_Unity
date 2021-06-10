using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Response;
using UnityEngine.UI;
using System;

public class MyLibController : DefaultSceneController
{
    [Header("Генерируемый объект для отображения книг")]
    public GameObject PrefabButtonBook;
    //Область отображения книг
    private ScrollRect MyBook;

    // Start is called before the first frame update
    void Start()
    {
        //Параллельный запуск функции
        StartCoroutine(ShowMyStory());
        //Определяем область на сцене
        MyBook = GameObject.Find("My Book").GetComponent<ScrollRect>();
        //задаем количество столбцов для отображения книг
        MyBook.GetComponentInChildren<GridLayoutGroup>().constraintCount =
            (int)Math.Ceiling((MyBook.GetComponent<RectTransform>().rect.width - 10) / PrefabButtonBook.GetComponent<RectTransform>().rect.width) - 1;
    }

    IEnumerator ShowMyStory()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/downloads/my"))
        {
            //Добавляем заголовок авторизации пользователя к запросу
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //Отправляем форму
            yield return www.SendWebRequest();
            //Обработка результата
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Конвертируем результат из JSON в класс StoryRoot
                StoryRoot storyRoot = JsonConvert.DeserializeObject<StoryRoot>(www.downloadHandler.text);
                //Проходим по всем классам Story из data класса StoryRoot
                foreach (Story story in storyRoot.data)
                {
                    //Инициализируем объект для отображения книги
                    GameObject newBook = Instantiate(PrefabButtonBook, MyBook.content.transform);
                    //Передаем путь изображения на сервере
                    newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                    //Записываем имя книги в подпись
                    newBook.GetComponentInChildren<Text>().text = story.name_rus;
                    //Добавляем обработчик события на нажатие
                    newBook.GetComponent<Button>().onClick.AddListener(delegate {
                        //Присвоение id в хранилище значения id книги
                        DataStore.id = story.id;
                        //Загружаем сцену отображения книги
                        LoadScene("BookScene");
                    });
                }
            }
        }
    }
}
