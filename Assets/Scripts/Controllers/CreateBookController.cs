using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;
using TMPro;

public class CreateBookController : DefaultSceneController
{
    //Поля ввода информации
    private InputField InputNameOriginal;
    private InputField InputNameRus;
    private InputField InputDescription;
    //Путь до загружаемого файла
    private string CoverPath;

    [Header("Кнопка выбора файла")]
    public GameObject FileIcon;
    [Header("Поля вывода пути файла")]
    public TextMeshProUGUI TextPathCover;
    [Header("Область отображения списка иконок файлов")]
    public GameObject scrollViewImages;
    
    // Start is called before the first frame update
    void Start()
    {
        //Находим поля ввода на сцене
        InputNameOriginal = GameObject.Find("InputNameOriginal").GetComponent<InputField>();
        InputNameRus = GameObject.Find("InputNameRus").GetComponent<InputField>();
        InputDescription = GameObject.Find("InputDescription").GetComponent<InputField>();
    }

    public void LoadImageCover()
    {
        //Инициализируем класс для работы папками
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        //Инициализируем список классов работы с файлами
        FileInfo[] files;
        //Включаем отображение области отображения списка иконок файлов
        scrollViewImages.SetActive(true);
        //Ищем все существующие объекты иконок изображений
        //Делается для удаления дублирования при повторном нажатии
        GameObject[] FileIcons = GameObject.FindGameObjectsWithTag("DataButton");
        //Проходимся по всем объектам и удаляем их
        foreach (GameObject fileIcon in FileIcons)
            Destroy(fileIcon.gameObject);
        //Указываем папку для работы
        //В качестве пути вызываем функцию GetAndroidInternalFilesDir()
        dirInfo = new DirectoryInfo(GetAndroidInternalFilesDir());
        //Отбираем в папке файлы необходимых расширений
        files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => dirInfo.EnumerateFiles(ext, SearchOption.AllDirectories)).ToArray();
        //Проходимся по всем файлам
        foreach (FileInfo file in files)
        {
            //Создаем объект иконки изображения
            FileIconButton fileIcon = Instantiate(FileIcon, scrollViewImages.GetComponent<ScrollRect>().content.transform).GetComponent<FileIconButton>();
            //Меняем подпись объекта на имя файла
            fileIcon.fileNameText.text = file.Name;
            //Добавляем обработчик события по нажатию
            fileIcon.GetComponent<Button>().onClick.AddListener(delegate {
                //Сохраняем путь до файла
                CoverPath = file.FullName;
                //Отображаем полный путь до файла
                TextPathCover.text = CoverPath;
                //Скрываем область просмотр файлов
                scrollViewImages.SetActive(false);
            });
        }
    }

    //Функция получения пути
    public static string GetAndroidInternalFilesDir()
    {
        //Потенциальные пути до папки
        string[] potentialDirectories = new string[]
        {
        "/mnt/sdcard/Download",
        "/sdcard/Download",
        "/storage/sdcard0/Download",
        "/storage/sdcard1/Download",
        "/storage/emulated/0/Download"
        };
        //Если запущено на Android
        if (Application.platform == RuntimePlatform.Android)
        {
            //Проходим по массиву путей
            for (int i = 0; i < potentialDirectories.Length; i++)
            {
                //Возвращаем если путь существует
                if (Directory.Exists(potentialDirectories[i]))
                {
                    return potentialDirectories[i];
                }
            }
        }
        //Если запущено на Windows
        //Путь необходимо настраивать самостоятельно
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return "C:\\Users\\admin\\Downloads";
        }
        return "";
    }

    //Обработчик нажатия кнопки создать
    public void OnButtonCreate()
    {
        //Параллельно запускаем функцию
        StartCoroutine(CreateBook());
    }

    IEnumerator CreateBook()
    {
        //Записываем файл в качестве массива байтов
        byte[] imageBytes = File.ReadAllBytes(CoverPath);
        //Создаем форму запроса
        WWWForm form = new WWWForm();
        //Добавляем поля в форму запроса
        form.AddField("name_original", InputNameOriginal.text);
        form.AddField("name_rus", InputNameRus.text);
        form.AddField("description", InputDescription.text);
        //Добавляем массив байт в форму запроса
        form.AddBinaryData("cover", imageBytes);
        //Создаем Post запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories", form))
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
                //Конвертируем результат из JSON в класс MiddleStoryRoot
                MiddleStoryRoot middleStoryRoot = JsonConvert.DeserializeObject<MiddleStoryRoot>(www.downloadHandler.text);
                //Присвоение id в хранилище значения id книги
                DataStore.id = middleStoryRoot.data.id;
                //Загружаем сцену отображения книги
                LoadScene("BookScene");
            }
        }
    }
}