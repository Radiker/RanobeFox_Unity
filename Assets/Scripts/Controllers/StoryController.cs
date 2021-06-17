using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class StoryController : DefaultSceneController
{
    [Header("Объект обложки")]
    public GameObject cover;
    [Header("Объект текста")]
    public GameObject text;
    [Header("Объект главы")]
    public GameObject chapter;
    [Header("Объект написания отзыва")]
    public GameObject feedback;

    //Область вывода данных
    private ScrollRect scrollRect;
    //Кнопка добавления в коллекцию
    private Button ButtonAdd;

    [Header("Изображения закладок")]
    public Sprite[] Bookmarks;
    [Header("Объект кнопки добавления атрибута")]
    public GameObject PrefabButton;
    [Header("Область кнопок добавления атрибутов")]
    public ScrollRect scrollDataRect;

    // Start is called before the first frame update
    void Start()
    {
        //Определяем объект вывода данных
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        //Определяем кнопку добавления в коллекцию
        ButtonAdd = GameObject.Find("ButtonAdd").GetComponent<Button>();
        //Параллельный запуск функций
        StartCoroutine(ShowStories());
        StartCoroutine(CheckDownloads());
    }
    /// <summary>
    /// Отображение основных данных книги
    /// </summary>
    IEnumerator ShowStories()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id))
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
                //Конвертируем результат из JSON в класс FullStoryRoot
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);
                //Инициализируем объект для обложки
                GameObject CoverImage = Instantiate(cover, scrollRect.content.transform);
                //Задаем ссылку на обложку
                CoverImage.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + root.data[0].cover_link;
                //Инициализируем объект для заголовка
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Оригинальное название:";
                //Инициализируем объект для Оригинального названия
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].name_original;
                //Инициализируем объект для заголовка
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Русское название:";
                //Инициализируем объект для Русского названия
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].name_rus;
                //Инициализируем объект для заголовка
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Описание:";
                //Инициализируем объект для Описания
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].description;
                //Инициализируем объект для заголовка
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Авторы:";
                //Инициализируем объектов для авторов
                foreach (Author authors in root.data[0].authors)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = authors.name;
                }
                //Инициализируем объект для заголовка
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Жанры:";
                //Инициализируем объектов для жанров
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = "";
                foreach (AdditionData genres in root.data[0].genres)
                {
                    label.GetComponentInChildren<Text>().text += genres.name+"; ";
                }
                //Инициализируем объект для заголовка
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Тэги:";
                //Инициализируем объектов для тэгов
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = "";
                foreach (AdditionData tags in root.data[0].tags)
                {
                    label.GetComponentInChildren<Text>().text += tags.name + "; ";
                }
                //Параллельный запуск отображения глав
                StartCoroutine(ShowChapters());
            }
        }
    }
    /// <summary>
    /// Отображение глав книги
    /// </summary>
    IEnumerator ShowChapters()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/chapters"))
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
                //Конвертируем результат из JSON в класс ChapterRoot
                ChapterRoot root = JsonConvert.DeserializeObject<ChapterRoot>(www.downloadHandler.text);
                //Инициализируем объект заголовка
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().fontSize = 12;
                label.GetComponentInChildren<Text>().text = "\nГлавы:";
                //Проходимся по всем данным из результата
                foreach (Chapter data in root.data)
                {
                    //Инициализируем объект для отображения глав
                    GameObject button = Instantiate(chapter, scrollRect.content.transform);
                    //Меняем текст в отображении
                    button.GetComponentInChildren<Text>().text = "Глава "+data.number+": "+data.name;
                    //Добавляем обработчик события на нажатие
                    button.GetComponent<Button>().onClick.AddListener(delegate {
                        //Присвоение id в хранилище значения id главы
                        DataStore.id = data.id;
                        //Загружаем сцену отображения главы
                        LoadScene("ChapterScene");
                    });
                }
            }
            //Параллельный запуск отображения отзывов
            StartCoroutine(ShowMarks());
        }
    }
    /// <summary>
    /// Отображение отзывов о книге
    /// </summary>
    IEnumerator ShowMarks()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/feedback"))
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
                //Конвертируем результат из JSON в класс MarkRoot
                MarkRoot root = JsonConvert.DeserializeObject<MarkRoot>(www.downloadHandler.text);
                //Инициализируем объект для заголовка
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().fontSize = 12;
                label.GetComponentInChildren<Text>().text = "\nОтзывы:";
                //Проходим по всем классам Mark из data класса MarkRoot
                foreach (Mark data in root.data)
                {
                    //Инициализируем объект для отзыва
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = data.name + " " + data.created_at + "\n";
                    label.GetComponentInChildren<Text>().text += "<b>Оценка:</b> " + data.value + "\n";
                    label.GetComponentInChildren<Text>().text += data.description;
                    label.GetComponentInChildren<Text>().tag = "Feedback";
                }
            }
            //Инициализируем объект для написания загловка
            GameObject feedbackElement = Instantiate(feedback, scrollRect.content.transform);
            //Добавляем обработчик события на нажатие кнопки добавления
            feedbackElement.GetComponent<FeedbackButton>().button.onClick.AddListener(delegate {
                StartCoroutine(GiveFeedback(feedbackElement.GetComponent<FeedbackButton>().slider.value, feedbackElement.GetComponent<FeedbackButton>().inputField.text));
            });
        }
    }
    /// <summary>
    /// Написание отзыва
    /// </summary>
    /// <param name="value">Целочисленое значение оценки</param>
    /// <param name="desc">Описание отзыва</param>
    /// <returns></returns>
    IEnumerator GiveFeedback(float value, string desc)
    {
        //Создаем форму запроса
        WWWForm form = new WWWForm();
        //Добавляем поля в форму запроса
        form.AddField("value", value.ToString());
        form.AddField("description", desc);
        //Создаем Post запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/" + DataStore.id+ "/feedback", form))
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
                //Находим все объекты отображения отзывов
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Feedback");
                //Проходимся по всем элементам
                foreach (GameObject btn in gameObjects)
                    //Уничтожаем все элемнты
                    Destroy(btn.gameObject);
                //Загружаем все отзывы
                StartCoroutine(ShowMarks());
            }
        }
    }
    /// <summary>
    /// Проверка нахождения книги в избранном
    /// </summary>
    IEnumerator CheckDownloads()
    {
        //Создаем Get запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/downloads/"+DataStore.id))
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
                //Конвертируем результат из JSON в класс FullStoryRoot
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);
                //Удаляем все обработчики нажатий на кнопке
                ButtonAdd.onClick.RemoveAllListeners();
                //Если книга есть в избранном
                if (root.data.Count != 0)
                {
                    //Меняем иконку кнопки
                    ButtonAdd.GetComponent<Image>().sprite = Bookmarks[0];                    
                    //Добавляем новый обработчик нажатия
                    ButtonAdd.onClick.AddListener(delegate {
                        //Вызов функции удаления из избранного
                        StartCoroutine(DestroyDownload());
                    });
                }
                else
                {
                    //Меняем иконку кнопки
                    ButtonAdd.GetComponent<Image>().sprite = Bookmarks[1];
                    //Добавляем новый обработчик нажатия
                    ButtonAdd.onClick.AddListener(delegate {
                        //Вызов функции добавления в избранное
                        StartCoroutine(AppendDownload());
                    });
                }
            }
        }
    }
    /// <summary>
    /// Добавление в избранное
    /// </summary>
    IEnumerator AppendDownload()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id + "/add"))
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
                //Меняем иконку кнопки
                ButtonAdd.GetComponent<Image>().sprite = Bookmarks[0];
                //Удаляем все обработчики нажатий на кнопке
                ButtonAdd.onClick.RemoveAllListeners();
                //Добавляем новый обработчик нажатия
                ButtonAdd.onClick.AddListener(delegate {
                    //Вызов функции удаления из избранного
                    StartCoroutine(DestroyDownload());
                });
            }
        }
    }
    /// <summary>
    /// Удаление из избранного
    /// </summary>
    IEnumerator DestroyDownload()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/downloads/" + DataStore.id + "/destroy"))
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
                //Меняем иконку кнопки
                ButtonAdd.GetComponent<Image>().sprite = Bookmarks[1];
                //Удаляем все обработчики нажатий на кнопке
                ButtonAdd.onClick.RemoveAllListeners();
                //Добавляем новый обработчик нажатия
                ButtonAdd.onClick.AddListener(delegate
                {
                    //Вызов функции добавления в избранное
                    StartCoroutine(AppendDownload());
                });
            }
        }
    }
    /// <summary>
    /// Обработчик нажатия на кнопку удаления книги
    /// </summary>
    public void ClickDeleteButton()
    {
        //Параллельный запуск функции
        StartCoroutine(DeleteStory());
    }
    /// <summary>
    /// Удаление книги
    /// </summary>
    IEnumerator DeleteStory()
    {
        //Создаем Delete запрос
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/stories/" + DataStore.id + "/destroy"))
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
                //Загрузка главной сцены
                LoadScene("MainScene");
            }
        }
    }
    /// <summary>
    /// Обработчик нажатия на кнопку создания главы
    /// </summary>
    public void ClickButtonCreateChapter()
    {
        //Загрузка сцены создания главы
        LoadScene("CreateChapterScene");
    }
    /// <summary>
    /// Обработчик нажатия на кнопку добавления автора
    /// </summary>
    public void ClickAddAuthor()
    {
        //Находим все элементы атрибутов
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        //Проходимся по всем элементам
        foreach(GameObject prefab in prefabs)
            //Удаляем объект на сцене
            Destroy(prefab.gameObject);
        //Параллельный запуск функции отображений авторов
        StartCoroutine(ShowAuthors());
    }
    /// <summary>
    /// Обработчик нажатия на кнопку добавления жанра
    /// </summary>
    public void ClickAddGenre()
    {
        //Находим все элементы атрибутов
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        //Проходимся по всем элементам
        foreach (GameObject prefab in prefabs)
            //Удаляем объект на сцене
            Destroy(prefab.gameObject);
        //Параллельный запуск функции отображений жанров
        StartCoroutine(ShowGenres());
    }
    /// <summary>
    /// Обработчик нажатия на кнопку добавления тэга
    /// </summary>
    public void ClickAddTag()
    {
        //Находим все элементы атрибутов
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        //Проходимся по всем элементам
        foreach (GameObject prefab in prefabs)
            //Удаляем объект на сцене
            Destroy(prefab.gameObject);
        //Параллельный запуск функции отображений тэгов
        StartCoroutine(ShowTags());
    }
    /// <summary>
    /// Показ жанров
    /// </summary>
    IEnumerator ShowGenres()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/genres"))
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
                //Конвертируем результат из JSON в класс DataRoot
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);
                //Проходим по всем классам Data из data класса DataRoot
                foreach (Data data in root.data)
                {
                    //Инициализируем объект для отображения жанра
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    //Выводим название жанра
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //Добавляем обработчик события на нажатие
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //Добавление жанра книге
                        StartCoroutine(AddGenres(data.id));
                    });
                }
            }
        }
    }
    /// <summary>
    /// Добавление жанра книге
    /// </summary>
    /// <param name="id">Идентификатор жанра</param>
    /// <returns></returns>
    IEnumerator AddGenres(int id)
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/genres/" + id))
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
                //Перезагрузка сцены
                LoadScene("BookScene");
            }
        }
    }
    /// <summary>
    /// Показ тэгов
    /// </summary>
    IEnumerator ShowTags()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/tags"))
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
                //Конвертируем результат из JSON в класс DataRoot
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);
                //Проходим по всем классам Data из data класса DataRoot
                foreach (Data data in root.data)
                {
                    //Инициализируем объект для отображения тэга
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    //Выводим название тэга
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //Добавляем обработчик события на нажатие
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //Добавление тэга книге
                        StartCoroutine(AddTags(data.id));
                    });
                }
            }
        }
    }
    /// <summary>
    /// Добавление тэга книге
    /// </summary>
    /// <param name="id">Идентификатор тэга</param>
    /// <returns></returns>
    IEnumerator AddTags(int id)
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/tags/" + id))
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
                //Перезагрузка сцены
                LoadScene("BookScene");
            }
        }
    }
    /// <summary>
    /// Показ авторов
    /// </summary>
    IEnumerator ShowAuthors()
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/authors"))
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
                //Конвертируем результат из JSON в класс DataRoot
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);
                //Проходим по всем классам Data из data класса DataRoot
                foreach (Data data in root.data)
                {
                    //Инициализируем объект для отображения автора
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    //Выводим название автора
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //Добавляем обработчик события на нажатие
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //Добавление автора книге
                        StartCoroutine(AddAuthors(data.id));
                    });
                }
            }
        }
    }
    /// <summary>
    /// Добавление автора книге
    /// </summary>
    /// <param name="id">Идентификатор автора</param>
    /// <returns></returns>
    IEnumerator AddAuthors(int id)
    {
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/authors/" + id))
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
                //Перезагрузка сцены
                LoadScene("BookScene");
            }
        }
    }
}