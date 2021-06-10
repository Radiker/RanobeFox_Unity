using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Response;
using UnityEngine.UI;

public class MainController : DefaultSceneController
{
    [Header("������������ ������ ��� ����������� ����")]
    public GameObject PrefabButtonBook;
    //���� ����� ���������� �������
    private InputField InputQuary;

    // Start is called before the first frame update
    void Start()
    {
        //���������� ������ �� �����
        InputQuary = GameObject.Find("InputQuary").GetComponent<InputField>();
        //������������ ������ �������
        StartCoroutine(ShowLatestStory());
        StartCoroutine(ShowTopStory());
    }

    IEnumerator ShowTopStory()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/top"))
        {
            //��������� ��������� ����������� ������������ � �������
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //���������� �����
            yield return www.SendWebRequest();
            //��������� ����������
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //������������ ��������� �� JSON � ����� StoryRoot
                StoryRoot storyRoot = JsonConvert.DeserializeObject<StoryRoot>(www.downloadHandler.text);
                //���������� ������ ��� ����������
                ScrollRect TopBook = GameObject.Find("Top Book").GetComponent<ScrollRect>();
                //�������� �� ���� ������� Story �� data ������ StoryRoot
                foreach (Story story in storyRoot.data)
                {
                    //�������������� ������ ��� ����������� �����
                    GameObject newBook = Instantiate(PrefabButtonBook, TopBook.content.transform);
                    //�������� ���� ����������� �� �������
                    newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                    //���������� ��� ����� � �������
                    newBook.GetComponentInChildren<Text>().text = story.name_rus;
                    //��������� ���������� ������� �� �������
                    newBook.GetComponent<Button>().onClick.AddListener(delegate {
                        //���������� id � ��������� �������� id �����
                        DataStore.id = story.id;
                        //��������� ����� ����������� �����
                        LoadScene("BookScene");
                    });
                }
            }
        }
    }

    IEnumerator ShowLatestStory()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/latest"))
        {
            //��������� ��������� ����������� ������������ � �������
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //���������� �����
            yield return www.SendWebRequest();
            //��������� ����������
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //������������ ��������� �� JSON � ����� StoryRoot
                StoryRoot storyRoot = JsonConvert.DeserializeObject<StoryRoot>(www.downloadHandler.text);
                //���������� ������ ��� ����������
                ScrollRect LatestBook = GameObject.Find("Latest Book").GetComponent<ScrollRect>();
                //�������� �� ���� ������� Story �� data ������ StoryRoot
                foreach (Story story in storyRoot.data)
                {
                    //�������������� ������ ��� ����������� �����
                    GameObject newBook = Instantiate(PrefabButtonBook, LatestBook.content.transform);
                    //�������� ���� ����������� �� �������
                    newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                    //���������� ��� ����� � �������
                    newBook.GetComponentInChildren<Text>().text = story.name_rus;
                    //��������� ���������� ������� �� �������
                    newBook.GetComponent<Button>().onClick.AddListener(delegate {
                        //���������� id � ��������� �������� id �����
                        DataStore.id = story.id;
                        //��������� ����� ����������� �����
                        LoadScene("BookScene");
                    });
                }
            }
        }
    }

    //���������� ������� ������� ������ ������
    public void Search()
    {
        //��������� ������ � ���������
        DataStore.Query = InputQuary.text;
        //���������� ��������� ������ � ���������
        DataStore.Search = true;
        //��������� ����� �������� ����
        LoadScene("CatalogScene");
    }
}
