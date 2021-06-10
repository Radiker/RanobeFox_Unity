using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Response;
using UnityEngine.UI;

public class MainController : DefaultSceneController
{
    [Header("Генерируемый объект для отображения книг")]
    public GameObject PrefabButtonBook;
    //Поле ввода поискового запроса
    private InputField InputQuary;

    // Start is called before the first frame update
    void Start()
    {
        //Определяем обънет на сцене
        InputQuary = GameObject.Find("InputQuary").GetComponent<InputField>();
        //Параллельный запуск функций
        StartCoroutine(ShowLatestStory());
        StartCoroutine(ShowTopStory());
    }

    IEnumerator ShowTopStory()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/top"))
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
                //Определяем объект для заполнения
                ScrollRect TopBook = GameObject.Find("Top Book").GetComponent<ScrollRect>();
                //Проходим по всем классам Story из data класса StoryRoot
                foreach (Story story in storyRoot.data)
                {
                    //Инициализируем объект для отображения книги
                    GameObject newBook = Instantiate(PrefabButtonBook, TopBook.content.transform);
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

    IEnumerator ShowLatestStory()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/latest"))
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
                //Определяем объект для заполнения
                ScrollRect LatestBook = GameObject.Find("Latest Book").GetComponent<ScrollRect>();
                //Проходим по всем классам Story из data класса StoryRoot
                foreach (Story story in storyRoot.data)
                {
                    //Инициализируем объект для отображения книги
                    GameObject newBook = Instantiate(PrefabButtonBook, LatestBook.content.transform);
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

    //Обработчик события нажатия кнопки поиска
    public void Search()
    {
        //Сохраняем запрос в хранилище
        DataStore.Query = InputQuary.text;
        //Записываем истиность поиска в хранилище
        DataStore.Search = true;
        //Загружаем сцену каталога книг
        LoadScene("CatalogScene");
    }
}
