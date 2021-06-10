using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class AuthController : DefaultSceneController
{
    [Header("Поля ввода авторизации")]
    public InputField Auth_Email;
    public InputField Auth_Password;

    [Header("Поля ввода регистрации")]
    public InputField Reg_Name;
    public InputField Reg_Email;
    public InputField Reg_Password;
    public InputField Reg_C_Password;

    public void Auth()
    {
        //Параллельный запуск функции 
        StartCoroutine(AuthRequest());
    }

    public void Registration()
    {
        //Если пароли совпадает, параллельно запускаем функцию
        if (Reg_Password.text == Reg_C_Password.text)
            StartCoroutine(RegistRequest());
        //Иначе сбрасываем ввод паролей
        else
        {
            Reg_Password.text = "";
            Reg_C_Password.text = "";
        }

    }

    IEnumerator AuthRequest()
    {
        //Создаем форму запроса
        WWWForm form = new WWWForm();
        //Добавляем поля в форму запроса
        form.AddField("username", Auth_Email.text);
        form.AddField("password", Auth_Password.text);
        form.AddField("grant_type", "password");
        form.AddField("client_id", "2");
        form.AddField("client_secret", "9yl58Z6Pux9TnAHnvw77NDXDkj5cLJ7rtaRQOBGH");
        //Создаем Post запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "oauth/token", form))
        {
            //Отправляем форму
            yield return www.SendWebRequest();
            //Обработка результата
            if (www.result != UnityWebRequest.Result.Success)
            {
                //Если вход не успешен, сбрасываем поле пароля
                Auth_Password.text = "";
                Debug.Log(www.error);
            }
            else
            {
                //Конвертируем результат из JSON в класс Auth
                Auth auth = JsonConvert.DeserializeObject<Auth>(www.downloadHandler.text);
                //Получаем данные из результата
                DataStore.token = auth.access_token;
                DataStore.token_type = auth.token_type;
                //Загружаем главное меню
                LoadScene("MainScene");
            }
        }
    }

    IEnumerator RegistRequest()
    {
        //Создаем форму запроса
        WWWForm form = new WWWForm();
        //Добавляем поля в форму запроса
        form.AddField("name", Reg_Name.text);
        form.AddField("email", Reg_Email.text);
        form.AddField("password", Reg_Password.text);
        form.AddField("c_password", Reg_C_Password.text);
        //Создаем Post запрос с формой
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/register", form))
        {
            //Отправляем форму
            yield return www.SendWebRequest();
            //Обработка результата
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Конвертируем результат из JSON в класс RegistrRoot
                RegistrRoot registrRoot = JsonConvert.DeserializeObject<RegistrRoot>(www.downloadHandler.text);
                //Получаем данные из результата
                DataStore.token = registrRoot.data[0].token;
                //Загружаем главное меню
                LoadScene("MainScene");
            }
        }
    }
}
