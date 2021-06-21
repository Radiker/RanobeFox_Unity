using Newtonsoft.Json;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class CreateChapterController : DefaultSceneController
{
    //���� ����� ���������� � �����
    private InputField InputName;
    private InputField InputNumber;
    private InputField InputDescription;

    // Start is called before the first frame update
    void Start()
    {
        //������� ���� ����� �� �����
        InputName = GameObject.Find("InputName").GetComponent<InputField>();
        InputNumber = GameObject.Find("InputNumber").GetComponent<InputField>();
        InputDescription = GameObject.Find("InputDescription").GetComponent<InputField>();
    }

    //���������� ������� ������ �������
    public void OnButtonCreate()
    {
        //������������ ������ �������
        StartCoroutine(CreateBook());
    }

    IEnumerator CreateBook()
    {
        //������� ����� 
        WWWForm form = new WWWForm();
        //��������� ���� � ����� �������
        form.AddField("name", InputName.text);
        form.AddField("number", InputNumber.text);
        form.AddField("text", InputDescription.text);

        //������� Post ������ � ������
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/" + DataStore.id + "/chapters", form))
        {
            //��������� ��������� �����������
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //���������� ������
            yield return www.SendWebRequest();
            //��������� ���������
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //������������ ��������� �� JSON � ����� FullChapterRoot
                FullChapterRoot middleStoryRoot = JsonConvert.DeserializeObject<FullChapterRoot>(www.downloadHandler.text);
                //���������� id � ��������� �������� id �����
                DataStore.id = middleStoryRoot.data.story_id;
                //�������� �����
                LoadScene("BookScene");
            }
        }
    }
}
