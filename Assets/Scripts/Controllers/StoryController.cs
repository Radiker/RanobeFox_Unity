using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class StoryController : DefaultSceneController
{
    [Header("������ �������")]
    public GameObject cover;
    [Header("������ ������")]
    public GameObject text;
    [Header("������ �����")]
    public GameObject chapter;
    [Header("������ ��������� ������")]
    public GameObject feedback;

    //������� ������ ������
    private ScrollRect scrollRect;
    //������ ���������� � ���������
    private Button ButtonAdd;

    [Header("����������� ��������")]
    public Sprite[] Bookmarks;
    [Header("������ ������ ���������� ��������")]
    public GameObject PrefabButton;
    [Header("������� ������ ���������� ���������")]
    public ScrollRect scrollDataRect;

    // Start is called before the first frame update
    void Start()
    {
        //���������� ������ ������ ������
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        //���������� ������ ���������� � ���������
        ButtonAdd = GameObject.Find("ButtonAdd").GetComponent<Button>();
        //������������ ������ �������
        StartCoroutine(ShowStories());
        StartCoroutine(CheckDownloads());
    }
    /// <summary>
    /// ����������� �������� ������ �����
    /// </summary>
    IEnumerator ShowStories()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id))
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
                //������������ ��������� �� JSON � ����� FullStoryRoot
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);
                //�������������� ������ ��� �������
                GameObject CoverImage = Instantiate(cover, scrollRect.content.transform);
                //������ ������ �� �������
                CoverImage.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + root.data[0].cover_link;
                //�������������� ������ ��� ���������
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "������������ ��������:";
                //�������������� ������ ��� ������������� ��������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].name_original;
                //�������������� ������ ��� ���������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "������� ��������:";
                //�������������� ������ ��� �������� ��������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].name_rus;
                //�������������� ������ ��� ���������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "��������:";
                //�������������� ������ ��� ��������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].description;
                //�������������� ������ ��� ���������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "������:";
                //�������������� �������� ��� �������
                foreach (Author authors in root.data[0].authors)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = authors.name;
                }
                //�������������� ������ ��� ���������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "�����:";
                //�������������� �������� ��� ������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = "";
                foreach (AdditionData genres in root.data[0].genres)
                {
                    label.GetComponentInChildren<Text>().text += genres.name+"; ";
                }
                //�������������� ������ ��� ���������
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "����:";
                //�������������� �������� ��� �����
                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = "";
                foreach (AdditionData tags in root.data[0].tags)
                {
                    label.GetComponentInChildren<Text>().text += tags.name + "; ";
                }
                //������������ ������ ����������� ����
                StartCoroutine(ShowChapters());
            }
        }
    }
    /// <summary>
    /// ����������� ���� �����
    /// </summary>
    IEnumerator ShowChapters()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/chapters"))
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
                //������������ ��������� �� JSON � ����� ChapterRoot
                ChapterRoot root = JsonConvert.DeserializeObject<ChapterRoot>(www.downloadHandler.text);
                //�������������� ������ ���������
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().fontSize = 12;
                label.GetComponentInChildren<Text>().text = "\n�����:";
                //���������� �� ���� ������ �� ����������
                foreach (Chapter data in root.data)
                {
                    //�������������� ������ ��� ����������� ����
                    GameObject button = Instantiate(chapter, scrollRect.content.transform);
                    //������ ����� � �����������
                    button.GetComponentInChildren<Text>().text = "����� "+data.number+": "+data.name;
                    //��������� ���������� ������� �� �������
                    button.GetComponent<Button>().onClick.AddListener(delegate {
                        //���������� id � ��������� �������� id �����
                        DataStore.id = data.id;
                        //��������� ����� ����������� �����
                        LoadScene("ChapterScene");
                    });
                }
            }
            //������������ ������ ����������� �������
            StartCoroutine(ShowMarks());
        }
    }
    /// <summary>
    /// ����������� ������� � �����
    /// </summary>
    IEnumerator ShowMarks()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/feedback"))
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
                //������������ ��������� �� JSON � ����� MarkRoot
                MarkRoot root = JsonConvert.DeserializeObject<MarkRoot>(www.downloadHandler.text);
                //�������������� ������ ��� ���������
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().fontSize = 12;
                label.GetComponentInChildren<Text>().text = "\n������:";
                //�������� �� ���� ������� Mark �� data ������ MarkRoot
                foreach (Mark data in root.data)
                {
                    //�������������� ������ ��� ������
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = data.name + " " + data.created_at + "\n";
                    label.GetComponentInChildren<Text>().text += "<b>������:</b> " + data.value + "\n";
                    label.GetComponentInChildren<Text>().text += data.description;
                    label.GetComponentInChildren<Text>().tag = "Feedback";
                }
            }
            //�������������� ������ ��� ��������� ��������
            GameObject feedbackElement = Instantiate(feedback, scrollRect.content.transform);
            //��������� ���������� ������� �� ������� ������ ����������
            feedbackElement.GetComponent<FeedbackButton>().button.onClick.AddListener(delegate {
                StartCoroutine(GiveFeedback(feedbackElement.GetComponent<FeedbackButton>().slider.value, feedbackElement.GetComponent<FeedbackButton>().inputField.text));
            });
        }
    }
    /// <summary>
    /// ��������� ������
    /// </summary>
    /// <param name="value">������������ �������� ������</param>
    /// <param name="desc">�������� ������</param>
    /// <returns></returns>
    IEnumerator GiveFeedback(float value, string desc)
    {
        //������� ����� �������
        WWWForm form = new WWWForm();
        //��������� ���� � ����� �������
        form.AddField("value", value.ToString());
        form.AddField("description", desc);
        //������� Post ������ � ������
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/" + DataStore.id+ "/feedback", form))
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
                //������� ��� ������� ����������� �������
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Feedback");
                //���������� �� ���� ���������
                foreach (GameObject btn in gameObjects)
                    //���������� ��� �������
                    Destroy(btn.gameObject);
                //��������� ��� ������
                StartCoroutine(ShowMarks());
            }
        }
    }
    /// <summary>
    /// �������� ���������� ����� � ���������
    /// </summary>
    IEnumerator CheckDownloads()
    {
        //������� Get ������ � ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/downloads/"+DataStore.id))
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
                //������������ ��������� �� JSON � ����� FullStoryRoot
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);
                //������� ��� ����������� ������� �� ������
                ButtonAdd.onClick.RemoveAllListeners();
                //���� ����� ���� � ���������
                if (root.data.Count != 0)
                {
                    //������ ������ ������
                    ButtonAdd.GetComponent<Image>().sprite = Bookmarks[0];                    
                    //��������� ����� ���������� �������
                    ButtonAdd.onClick.AddListener(delegate {
                        //����� ������� �������� �� ����������
                        StartCoroutine(DestroyDownload());
                    });
                }
                else
                {
                    //������ ������ ������
                    ButtonAdd.GetComponent<Image>().sprite = Bookmarks[1];
                    //��������� ����� ���������� �������
                    ButtonAdd.onClick.AddListener(delegate {
                        //����� ������� ���������� � ���������
                        StartCoroutine(AppendDownload());
                    });
                }
            }
        }
    }
    /// <summary>
    /// ���������� � ���������
    /// </summary>
    IEnumerator AppendDownload()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id + "/add"))
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
                //������ ������ ������
                ButtonAdd.GetComponent<Image>().sprite = Bookmarks[0];
                //������� ��� ����������� ������� �� ������
                ButtonAdd.onClick.RemoveAllListeners();
                //��������� ����� ���������� �������
                ButtonAdd.onClick.AddListener(delegate {
                    //����� ������� �������� �� ����������
                    StartCoroutine(DestroyDownload());
                });
            }
        }
    }
    /// <summary>
    /// �������� �� ����������
    /// </summary>
    IEnumerator DestroyDownload()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/downloads/" + DataStore.id + "/destroy"))
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
                //������ ������ ������
                ButtonAdd.GetComponent<Image>().sprite = Bookmarks[1];
                //������� ��� ����������� ������� �� ������
                ButtonAdd.onClick.RemoveAllListeners();
                //��������� ����� ���������� �������
                ButtonAdd.onClick.AddListener(delegate
                {
                    //����� ������� ���������� � ���������
                    StartCoroutine(AppendDownload());
                });
            }
        }
    }
    /// <summary>
    /// ���������� ������� �� ������ �������� �����
    /// </summary>
    public void ClickDeleteButton()
    {
        //������������ ������ �������
        StartCoroutine(DeleteStory());
    }
    /// <summary>
    /// �������� �����
    /// </summary>
    IEnumerator DeleteStory()
    {
        //������� Delete ������
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/stories/" + DataStore.id + "/destroy"))
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
                //�������� ������� �����
                LoadScene("MainScene");
            }
        }
    }
    /// <summary>
    /// ���������� ������� �� ������ �������� �����
    /// </summary>
    public void ClickButtonCreateChapter()
    {
        //�������� ����� �������� �����
        LoadScene("CreateChapterScene");
    }
    /// <summary>
    /// ���������� ������� �� ������ ���������� ������
    /// </summary>
    public void ClickAddAuthor()
    {
        //������� ��� �������� ���������
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        //���������� �� ���� ���������
        foreach(GameObject prefab in prefabs)
            //������� ������ �� �����
            Destroy(prefab.gameObject);
        //������������ ������ ������� ����������� �������
        StartCoroutine(ShowAuthors());
    }
    /// <summary>
    /// ���������� ������� �� ������ ���������� �����
    /// </summary>
    public void ClickAddGenre()
    {
        //������� ��� �������� ���������
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        //���������� �� ���� ���������
        foreach (GameObject prefab in prefabs)
            //������� ������ �� �����
            Destroy(prefab.gameObject);
        //������������ ������ ������� ����������� ������
        StartCoroutine(ShowGenres());
    }
    /// <summary>
    /// ���������� ������� �� ������ ���������� ����
    /// </summary>
    public void ClickAddTag()
    {
        //������� ��� �������� ���������
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        //���������� �� ���� ���������
        foreach (GameObject prefab in prefabs)
            //������� ������ �� �����
            Destroy(prefab.gameObject);
        //������������ ������ ������� ����������� �����
        StartCoroutine(ShowTags());
    }
    /// <summary>
    /// ����� ������
    /// </summary>
    IEnumerator ShowGenres()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/genres"))
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
                //������������ ��������� �� JSON � ����� DataRoot
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);
                //�������� �� ���� ������� Data �� data ������ DataRoot
                foreach (Data data in root.data)
                {
                    //�������������� ������ ��� ����������� �����
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    //������� �������� �����
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //��������� ���������� ������� �� �������
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //���������� ����� �����
                        StartCoroutine(AddGenres(data.id));
                    });
                }
            }
        }
    }
    /// <summary>
    /// ���������� ����� �����
    /// </summary>
    /// <param name="id">������������� �����</param>
    /// <returns></returns>
    IEnumerator AddGenres(int id)
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/genres/" + id))
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
                //������������ �����
                LoadScene("BookScene");
            }
        }
    }
    /// <summary>
    /// ����� �����
    /// </summary>
    IEnumerator ShowTags()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/tags"))
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
                //������������ ��������� �� JSON � ����� DataRoot
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);
                //�������� �� ���� ������� Data �� data ������ DataRoot
                foreach (Data data in root.data)
                {
                    //�������������� ������ ��� ����������� ����
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    //������� �������� ����
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //��������� ���������� ������� �� �������
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //���������� ���� �����
                        StartCoroutine(AddTags(data.id));
                    });
                }
            }
        }
    }
    /// <summary>
    /// ���������� ���� �����
    /// </summary>
    /// <param name="id">������������� ����</param>
    /// <returns></returns>
    IEnumerator AddTags(int id)
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/tags/" + id))
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
                //������������ �����
                LoadScene("BookScene");
            }
        }
    }
    /// <summary>
    /// ����� �������
    /// </summary>
    IEnumerator ShowAuthors()
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/authors"))
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
                //������������ ��������� �� JSON � ����� DataRoot
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);
                //�������� �� ���� ������� Data �� data ������ DataRoot
                foreach (Data data in root.data)
                {
                    //�������������� ������ ��� ����������� ������
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    //������� �������� ������
                    newButton.GetComponentInChildren<Text>().text = data.name;
                    //��������� ���������� ������� �� �������
                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        //���������� ������ �����
                        StartCoroutine(AddAuthors(data.id));
                    });
                }
            }
        }
    }
    /// <summary>
    /// ���������� ������ �����
    /// </summary>
    /// <param name="id">������������� ������</param>
    /// <returns></returns>
    IEnumerator AddAuthors(int id)
    {
        //������� Get ������
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/authors/" + id))
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
                //������������ �����
                LoadScene("BookScene");
            }
        }
    }
}