using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class ChapterController : DefaultSceneController
{
    [Header("���������� ������ ������ ������")]
    public GameObject text;
    [Header("������ �����������")]
    public Button ReturnButton;
    //������� ���������
    private ScrollRect scrollRect;
    //Id �����, ���������� �����
    private int scene_id;

    // Start is called before the first frame update
    void Start()
    {
        //���������� ������� ���������
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        //������������ ������ �������
        StartCoroutine(ShowDetail());
    }

    IEnumerator ShowDetail()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/chapters/"+DataStore.id))
        {
            //��������� ��������� ����������� ������������ � �������
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //���������� ������
            yield return www.SendWebRequest();
            //��������� �����������
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //������������ ��������� �� JSON � ����� FullChapterRoot
                FullChapterRoot root = JsonConvert.DeserializeObject<FullChapterRoot>(www.downloadHandler.text);
                //������� ������ ��� ������ ��������� �����
                GameObject label = Instantiate(text, scrollRect.content.transform);
                //������ ����� ������
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                //��������� �����
                label.GetComponentInChildren<Text>().text = "����� " + root.data.number + ": " + root.data.name;
                //��������� �������� id �����
                scene_id = root.data.story_id;
                //������� ������ ��� ������ ��������� �����
                label = Instantiate(text, scrollRect.content.transform);
                //��������� �����
                label.GetComponentInChildren<Text>().text = root.data.text.ToString();
            }
        }
    }

    //���������� ������� ������ �������� �����
    public void ClickDeleteButton()
    {
        //������������ ������ �������
        StartCoroutine(DeleteData());
    }
    IEnumerator DeleteData()
    {
        //������� Delete ������
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/chapters/" + DataStore.id + "/destroy"))
        {
            //��������� ��������� ����������� ������������ � �������
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //���������� ������
            yield return www.SendWebRequest();
            //��������� �����������
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

    //����������� �� ����� �����, ���������� �����
    public void Return()
    {
        //���������� id � ��������� �������� id �����
        DataStore.id = scene_id;
        //��������� ����� ����������� �����
        LoadScene("BookScene");
    }
}
