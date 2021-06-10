using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class AuthController : DefaultSceneController
{
    [Header("���� ����� �����������")]
    public InputField Auth_Email;
    public InputField Auth_Password;

    [Header("���� ����� �����������")]
    public InputField Reg_Name;
    public InputField Reg_Email;
    public InputField Reg_Password;
    public InputField Reg_C_Password;

    public void Auth()
    {
        //������������ ������ ������� 
        StartCoroutine(AuthRequest());
    }

    public void Registration()
    {
        //���� ������ ���������, ����������� ��������� �������
        if (Reg_Password.text == Reg_C_Password.text)
            StartCoroutine(RegistRequest());
        //����� ���������� ���� �������
        else
        {
            Reg_Password.text = "";
            Reg_C_Password.text = "";
        }

    }

    IEnumerator AuthRequest()
    {
        //������� ����� �������
        WWWForm form = new WWWForm();
        //��������� ���� � ����� �������
        form.AddField("username", Auth_Email.text);
        form.AddField("password", Auth_Password.text);
        form.AddField("grant_type", "password");
        form.AddField("client_id", "2");
        form.AddField("client_secret", "9yl58Z6Pux9TnAHnvw77NDXDkj5cLJ7rtaRQOBGH");
        //������� Post ������ � ������
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "oauth/token", form))
        {
            //���������� �����
            yield return www.SendWebRequest();
            //��������� ����������
            if (www.result != UnityWebRequest.Result.Success)
            {
                //���� ���� �� �������, ���������� ���� ������
                Auth_Password.text = "";
                Debug.Log(www.error);
            }
            else
            {
                //������������ ��������� �� JSON � ����� Auth
                Auth auth = JsonConvert.DeserializeObject<Auth>(www.downloadHandler.text);
                //�������� ������ �� ����������
                DataStore.token = auth.access_token;
                DataStore.token_type = auth.token_type;
                //��������� ������� ����
                LoadScene("MainScene");
            }
        }
    }

    IEnumerator RegistRequest()
    {
        //������� ����� �������
        WWWForm form = new WWWForm();
        //��������� ���� � ����� �������
        form.AddField("name", Reg_Name.text);
        form.AddField("email", Reg_Email.text);
        form.AddField("password", Reg_Password.text);
        form.AddField("c_password", Reg_C_Password.text);
        //������� Post ������ � ������
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/register", form))
        {
            //���������� �����
            yield return www.SendWebRequest();
            //��������� ����������
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //������������ ��������� �� JSON � ����� RegistrRoot
                RegistrRoot registrRoot = JsonConvert.DeserializeObject<RegistrRoot>(www.downloadHandler.text);
                //�������� ������ �� ����������
                DataStore.token = registrRoot.data[0].token;
                //��������� ������� ����
                LoadScene("MainScene");
            }
        }
    }
}
