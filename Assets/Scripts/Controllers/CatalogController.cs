using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class CatalogController : DefaultSceneController
{
    #region Variables
    //public int size;
    [Header("Генерируемый объект для отображения данных")]
    public GameObject PrefabButton;
    [Header("Генерируемый объект для отображения книги")]
    public GameObject PrefabBook;
    [Header("Область для отображения жанров")]
    public ScrollRect GenresRect;
    [Header("Область для отображения тэгов")]
    public ScrollRect TagsRect;
    [Header("Область для отображения авторов")]
    public ScrollRect AuthorsRect;

    //Поле ввода поиска по имени
    private InputField InputQuary;
    //Поле отображения всех книг
    private ScrollRect AllBooks;

    [Header("Панель отображения всех книг")]
    public GameObject PanelAllBooks;
    [Header("Панель отображения фильтров")]
    public GameObject PanelAllFilters;
    [Header("Панель отображения атрибутов")]
    public GameObject PanelInfo;
    [Header("Панель обноления атрибута")]
    public GameObject PanelUpdate;
    [Header("Панель удаления атрибута")]
    public GameObject PanelDelete;
    [Header("Панель создания атрибута")]
    public GameObject PanelCreate;

    //Идентификатор фильтра
    private int Filter = 0;
    //Дополнение пути запроса
    private string path;
    //Идентификатор данных
    private int id;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        //Определяем объект ввода
        InputQuary = GameObject.Find("InputQuary").GetComponent<InputField>();
        //Если поиск включен
        if (DataStore.Search)
            //Записываем текст поиска
            InputQuary.text = DataStore.Query;
        //Отключаем отображение всех панелей
        CloseAllPanel();
        //Отобразить все книги
        ShowAllStories();
    }
    /// <summary>
    /// Отключаем отображение всех панелей 
    /// </summary>
    void CloseAllPanel()
    {
        //Отключаем панель отображения атрибутов 
        PanelInfo.SetActive(false);
        //Отключаем панель обновления атрибутов
        PanelUpdate.SetActive(false);
        //Отключаем панель удаления атрибутов
        PanelDelete.SetActive(false);
        //Отключаем панель создания атрибутов
        PanelCreate.SetActive(false);
    }
    /// <summary>
    /// Обновление отображений атрибутов
    /// </summary>
    public void UpdateDataButton()
    {
        //Находим объекты отображения атрибутов
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag("DataButton");
        //Проходимся по всем объектам
        foreach(GameObject btn in Buttons)
            //Уничтожаем их на сцене
            Destroy(btn.gameObject);
        //Отключаем отображение всех панелей 
        CloseAllPanel();
        //Параллельный запуск отображения авторов
        StartCoroutine(ShowAuthors());
        //Параллельный запуск отображения жанров
        StartCoroutine(ShowGenres());
        //Параллельный запуск отображения тэгов
        StartCoroutine(ShowTags());
    }

    /// <summary>
    /// Отображение жанров
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
                //Проходимся по всем данным из результата
                foreach (Data data in root.data)
                {
                    //Инициализируем объект для отображения атрибутов
                    GameObject newButton = Instantiate(PrefabButton, GenresRect.content.transform);
                    //Меняем текст в отображении
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //Добавляем обработчик события на нажатие
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //Включаем панель отображения книг
                        PanelAllBooks.SetActive(true);
                        //Отключаем панель отображения фильтров
                        PanelAllFilters.SetActive(false);
                        //Включаем фильтр по жанру
                        Filter = 2;
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                        //Включаем отображение всех книг
                        ShowAllStories();
                    });
                    //Добавляем обработчик события на нажатие кнопки удалить
                    newButton.GetComponent<DataButton>().ButtonDelete.onClick.AddListener(delegate {
                        //Включаем панель редактирования атрибутов
                        PanelInfo.SetActive(true);
                        //Включаем панель удаления атрибута
                        PanelDelete.SetActive(true);
                        //Меняем путь 
                        path = "genres";
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                    });
                    //Добавляем обработчик события на нажатие кнопки обновить
                    newButton.GetComponent<DataButton>().ButtonUpdate.onClick.AddListener(delegate {
                        //Включаем панель редактирования атрибутов
                        PanelInfo.SetActive(true);
                        //Включаем панель обновления атрибута
                        PanelUpdate.SetActive(true);
                        //Меняем путь 
                        path = "genres";
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                        //Прописываем наименование в поле редактирования
                        PanelUpdate.GetComponentInChildren<InputField>().text = data.name;
                    });
                }
            }
        }
    }
    /// <summary>
    /// Отображение тэгов
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
                //Проходимся по всем данным из результата
                foreach (Data data in root.data)
                {
                    //Инициализируем объект для отображения атрибутов
                    GameObject newButton = Instantiate(PrefabButton, TagsRect.content.transform);
                    //Меняем текст в отображении
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //Добавляем обработчик события на нажатие
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //Включаем панель отображения книг
                        PanelAllBooks.SetActive(true);
                        //Отключаем панель отображения фильтров
                        PanelAllFilters.SetActive(false);
                        //Включаем фильтр по жанру
                        Filter = 3;
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                        //Включаем отображение всех книг
                        ShowAllStories();
                    });
                    //Добавляем обработчик события на нажатие кнопки удалить
                    newButton.GetComponent<DataButton>().ButtonDelete.onClick.AddListener(delegate {
                        //Включаем панель редактирования атрибутов
                        PanelInfo.SetActive(true);
                        //Включаем панель удаления атрибута
                        PanelDelete.SetActive(true);
                        //Меняем путь 
                        path = "tags";
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                    });
                    //Добавляем обработчик события на нажатие кнопки обновить
                    newButton.GetComponent<DataButton>().ButtonUpdate.onClick.AddListener(delegate {
                        //Включаем панель редактирования атрибутов
                        PanelInfo.SetActive(true);
                        //Включаем панель обновления атрибута
                        PanelUpdate.SetActive(true);
                        //Меняем путь 
                        path = "tags";
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                        //Прописываем наименование в поле редактирования
                        PanelUpdate.GetComponentInChildren<InputField>().text = data.name;
                    });
                }
            }
        }
    }
    /// <summary>
    /// Отображения авторов
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
                //Проходимся по всем данным из результата
                foreach (Data data in root.data)
                {
                    //Инициализируем объект для отображения атрибутов
                    GameObject newButton = Instantiate(PrefabButton, AuthorsRect.content.transform);
                    //Меняем текст в отображении
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //Добавляем обработчик события на нажатие
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //Включаем панель отображения книг
                        PanelAllBooks.SetActive(true);
                        //Отключаем панель отображения фильтров
                        PanelAllFilters.SetActive(false);
                        //Включаем фильтр по жанру
                        Filter = 1;
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                        //Включаем отображение всех книг
                        ShowAllStories();
                    });
                    //Добавляем обработчик события на нажатие кнопки удалить
                    newButton.GetComponent<DataButton>().ButtonDelete.onClick.AddListener(delegate {
                        //Включаем панель редактирования атрибутов
                        PanelInfo.SetActive(true);
                        //Включаем панель удаления атрибута
                        PanelDelete.SetActive(true);
                        //Меняем путь 
                        path = "authors";
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                    });
                    //Добавляем обработчик события на нажатие кнопки обновить
                    newButton.GetComponent<DataButton>().ButtonUpdate.onClick.AddListener(delegate {
                        //Включаем панель редактирования атрибутов
                        PanelInfo.SetActive(true);
                        //Включаем панель обновления атрибута
                        PanelUpdate.SetActive(true);
                        //Меняем путь 
                        path = "authors";
                        //Присваиваем идентификатор атрибута
                        id = data.id;
                        //Прописываем наименование в поле редактирования
                        PanelUpdate.GetComponentInChildren<InputField>().text = data.name;
                    });
                }
            }
        }
    }
    /// <summary>
    /// Обработчик кнопки удаления
    /// </summary>
    public void ClickDeleteButton()
    {
        //Параллельный запуск функции
        StartCoroutine(DeleteData());
    }
    /// <summary>
    /// Удаление атрибута
    /// </summary>
    IEnumerator DeleteData()
    {
        //Создаем Delete запрос
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/" + path +"/" + id + "/destroy"))
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
                //Обновление отображений атрибутов
                UpdateDataButton();
            }
        }
    }
    /// <summary>
    /// Обработчик кнопки обновления
    /// </summary>
    public void ClickUpdateButton()
    {
        //Если текст не пустой
        if(PanelUpdate.GetComponentInChildren<InputField>().text != null)
            //Параллельный запуск функции
            StartCoroutine(UpdateData());
    }
    /// <summary>
    /// Обновление атрибута
    /// </summary>
    IEnumerator UpdateData()
    {
        //Создаем Put запрос
        using (UnityWebRequest www = UnityWebRequest.Put(DataStore.basePath + "api/" + path + "/" + id + 
            "/update?name=" + PanelUpdate.GetComponentInChildren<InputField>().text, "{ }"))
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
                //Обновление отображений атрибутов
                UpdateDataButton();
            }
        }
    }
    /// <summary>
    /// Создание нового атрибута
    /// </summary>
    /// <param name="text">дополнение к пути (тип атрибута)</param>
    public void ShowNewData(string text)
    {
        //Включаем панель редактирования атрибутов
        PanelInfo.SetActive(true);
        //Включаем панель создания атрибута
        PanelCreate.SetActive(true);
        //Очищаем поле ввода
        PanelUpdate.GetComponentInChildren<InputField>().text = "";
        //Меняем дополнение к пути
        path = text;
    }
    /// <summary>
    /// Обработчик кнопки создания атрибута
    /// </summary>
    public void ClickNewButton()
    {
        //Если поле ввода не пустое
        if (PanelCreate.GetComponentInChildren<InputField>().text != null)
            //Параллельный запуск функции
            StartCoroutine(NewData());
    }
    /// <summary>
    /// Создание атрибута
    /// </summary>
    IEnumerator NewData()
    {
        //Создаем форму запроса
        WWWForm form = new WWWForm();
        //Добавляем поля в форму запроса
        form.AddField("name", PanelCreate.GetComponentInChildren<InputField>().text);
        //Создаем Post запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/" + path, form))
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
                //Обновление отображений атрибутов
                UpdateDataButton();
            }
        }
    }
    /// <summary>
    /// Отображение всех книг
    /// </summary>
    public void ShowAllStories()
    {
        //Если включены фильтры
        if (Filter != 0)
            //Параллельный запуск функции отображений книг по фильтру
            StartCoroutine(ShowStoryWithFilter());
        else
            //Параллельный запуск функции отображений книг без фильтра
            StartCoroutine(ShowStory());
        //НАходим все отображаемые книги
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag("BookButton");
        //Проходим по всем объектам
        foreach (GameObject btn in Buttons)
            //Уничтожаем объект на сцене
            Destroy(btn.gameObject);
        //Находим область на сцене
        AllBooks = GameObject.Find("AllBooks").GetComponent<ScrollRect>();
        //Переопределяем количество столбцов
        AllBooks.GetComponentInChildren<GridLayoutGroup>().constraintCount =
            (int)Math.Ceiling((AllBooks.GetComponent<RectTransform>().rect.width - 10) / PrefabBook.GetComponent<RectTransform>().rect.width) - 1;
    }
    /// <summary>
    /// Отображение книг по фильтру
    /// </summary>
    IEnumerator ShowStoryWithFilter()
    {
        //Дополнение пути по фильтру
        if (Filter == 1) path = "api/authors/" + id;
        if (Filter == 2) path = "api/genres/" + id;
        if (Filter == 3) path = "api/tags/" + id;
        //Создаем Get запрос
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + path))
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
                //Конвертируем результат из JSON в класс MiddleStoryPivotRoot
                MiddleStoryPivotRoot storyRoot = JsonConvert.DeserializeObject<MiddleStoryPivotRoot>(www.downloadHandler.text);
                //Проходим по всем классам MiddleStoryPivot из data класса MiddleStoryPivotRoot
                foreach (MiddleStoryPivot story in storyRoot.data)
                {
                    //Инициализируем объект для отображения книги
                    GameObject newBook = Instantiate(PrefabBook, AllBooks.content.transform);
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
    /// <summary>
    /// Отображение книг без фильтра
    /// </summary>
    IEnumerator ShowStory()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories"))
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
                    //Если поиск включен
                    if (DataStore.Search)
                    {
                        //Если имя содержит поисковый запрос
                        if (story.name_rus.IndexOf(DataStore.Query, StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            //Инициализируем объект для отображения книги
                            GameObject newBook = Instantiate(PrefabBook, AllBooks.content.transform);
                            //Передаем путь изображения на сервере
                            newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                            //Записываем имя книги в подпись
                            newBook.GetComponentInChildren<Text>().text = story.name_rus;
                            //Добавляем обработчик события на нажатие
                            newBook.GetComponent<Button>().onClick.AddListener(delegate
                            {
                                //Присвоение id в хранилище значения id книги
                                DataStore.id = story.id;
                                //Загружаем сцену отображения книги
                                LoadScene("BookScene");
                            });
                        }
                    }
                    else
                    {
                        //Инициализируем объект для отображения книги
                        GameObject newBook = Instantiate(PrefabBook, AllBooks.content.transform);
                        //Передаем путь изображения на сервере
                        newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                        //Записываем имя книги в подпись
                        newBook.GetComponentInChildren<Text>().text = story.name_rus;
                        //Добавляем обработчик события на нажатие
                        newBook.GetComponent<Button>().onClick.AddListener(delegate
                        {
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
    /// <summary>
    /// Функция поиска книги по названию
    /// </summary>
    public void Search()
    {
        //Записываем запрос в хранилище
        DataStore.Query = InputQuary.text;
        //Включаем режим поиска в хранилище
        DataStore.Search = true;
        //Показываем все книги
        ShowAllStories();
    }
    /// <summary>
    /// Функция отмена поиска
    /// </summary>
    public void CanselSearch()
    {
        //Отключаем режим поиска в хранилище
        DataStore.Search = false;
        //Очищаем запрос на поиск
        InputQuary.text = "";
        //Очищаем фильтр
        Filter = 0;
        //Показываем все книги
        ShowAllStories();
    }
}