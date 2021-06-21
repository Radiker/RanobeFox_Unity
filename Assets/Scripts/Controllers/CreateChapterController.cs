using Newtonsoft.Json;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class CreateChapterController : DefaultSceneController
{
    //Поля ввода информации о главе
    private InputField InputName;
    private InputField InputNumber;
    private InputField InputDescription;

    // Start is called before the first frame update
    void Start()
    {
        //Находим поля ввода на сцене
        InputName = GameObject.Find("InputName").GetComponent<InputField>();
        InputNumber = GameObject.Find("InputNumber").GetComponent<InputField>();
        InputDescription = GameObject.Find("InputDescription").GetComponent<InputField>();
    }

    //Обработчик нажатия кнопки создать
    public void OnButtonCreate()
    {
        //Параллельный запуск функции
        StartCoroutine(CreateBook());
    }

    IEnumerator CreateBook()
    {
        //Создаем форму 
        WWWForm form = new WWWForm();
        //Добавляем поля в форму запроса
        form.AddField("name", InputName.text);
        form.AddField("number", InputNumber.text);
        form.AddField("text", InputDescription.text);

        //Создаем Post запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/" + DataStore.id + "/chapters", form))
        {
            //Добавляем заголовок авторизации
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //Отправляем запрос
            yield return www.SendWebRequest();
            //Проверяем результат
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Конвертируем результат из JSON в класс FullChapterRoot
                FullChapterRoot middleStoryRoot = JsonConvert.DeserializeObject<FullChapterRoot>(www.downloadHandler.text);
                //Присвоение id в хранилище значения id книги
                DataStore.id = middleStoryRoot.data.story_id;
                //Загрузка сцены
                LoadScene("BookScene");
            }
        }
    }
}
