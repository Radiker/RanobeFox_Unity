using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class CatalogController : DefaultSceneController
{
    public int size;
    public GameObject PrefabButton;
    public GameObject PrefabBook;
    public ScrollRect GenresRect;
    public ScrollRect TagsRect;
    public ScrollRect AuthorsRect;
    private InputField InputQuary;
    private ScrollRect AllBooks;

    public GameObject PanelAllBooks;
    public GameObject PanelAllFilters;

    public GameObject PanelInfo;
    public GameObject PanelUpdate;
    public GameObject PanelDelete;
    public GameObject PanelCreate;

    private int Filter = 0;
    private string path;
    private int id;

    // Start is called before the first frame update
    void Start()
    {
        InputQuary = GameObject.Find("InputQuary").GetComponent<InputField>();
        if (DataStore.Search)
            InputQuary.text = DataStore.Query;
        CloseAllPanel();
        ShowAllStories();
    }

    public void UpdateDataButton()
    {
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag("DataButton");
        foreach(GameObject btn in Buttons)
            Destroy(btn.gameObject);
        StartCoroutine(ShowAuthors());
        StartCoroutine(ShowGenres());
        StartCoroutine(ShowTags());
    }

    void CloseAllPanel()
    {
        PanelInfo.SetActive(false);
        PanelUpdate.SetActive(false);
        PanelDelete.SetActive(false);
        PanelCreate.SetActive(false);
}

    // Update is called once per frame
    IEnumerator ShowGenres()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/genres"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);

                foreach (Data data in root.data)
                {
                    GameObject newButton = Instantiate(PrefabButton, GenresRect.content.transform);
                    newButton.GetComponentInChildren<Text>().text = data.name;

                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        PanelAllBooks.SetActive(true);
                        PanelAllFilters.SetActive(false);
                        Filter = 2;
                        id = data.id;
                        ShowAllStories();
                        Debug.Log(data.id);
                    });
                    newButton.GetComponent<DataButton>().ButtonDelete.onClick.AddListener(delegate {
                        PanelInfo.SetActive(true);
                        PanelDelete.SetActive(true);
                        path = "genres";
                        id = data.id;
                    });
                    newButton.GetComponent<DataButton>().ButtonUpdate.onClick.AddListener(delegate {
                        PanelInfo.SetActive(true);
                        PanelUpdate.SetActive(true);
                        path = "genres";
                        id = data.id;
                        PanelUpdate.GetComponentInChildren<InputField>().text = data.name;
                    });
                }
            }
        }
    }

    IEnumerator ShowTags()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/tags"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);

                foreach (Data data in root.data)
                {
                    GameObject newButton = Instantiate(PrefabButton, TagsRect.content.transform);
                    newButton.GetComponentInChildren<Text>().text = data.name;

                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        PanelAllBooks.SetActive(true);
                        PanelAllFilters.SetActive(false); 
                        Filter = 3;
                        id = data.id;
                        ShowAllStories();
                        Debug.Log(data.id);
                    });
                    newButton.GetComponent<DataButton>().ButtonDelete.onClick.AddListener(delegate {
                        PanelInfo.SetActive(true);
                        PanelDelete.SetActive(true);
                        path = "tags";
                        id = data.id;
                    });
                    newButton.GetComponent<DataButton>().ButtonUpdate.onClick.AddListener(delegate {
                        PanelInfo.SetActive(true);
                        PanelUpdate.SetActive(true);
                        path = "tags";
                        id = data.id;
                        PanelUpdate.GetComponentInChildren<InputField>().text = data.name;
                    });
                }
            }
        }
    }

    IEnumerator ShowAuthors()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/authors"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                DataRoot root = JsonConvert.DeserializeObject<DataRoot>(www.downloadHandler.text);

                foreach (Data data in root.data)
                {
                    GameObject newButton = Instantiate(PrefabButton, AuthorsRect.content.transform);
                    newButton.GetComponentInChildren<Text>().text = data.name;

                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        PanelAllBooks.SetActive(true);
                        PanelAllFilters.SetActive(false); 
                        Filter = 1;
                        id = data.id;
                        ShowAllStories();
                        Debug.Log(data.id); 
                    });
                    newButton.GetComponent<DataButton>().ButtonDelete.onClick.AddListener(delegate {
                        PanelInfo.SetActive(true);
                        PanelDelete.SetActive(true);
                        path = "authors";
                        id = data.id;
                    });
                    newButton.GetComponent<DataButton>().ButtonUpdate.onClick.AddListener(delegate {
                        PanelInfo.SetActive(true);
                        PanelUpdate.SetActive(true);
                        path = "authors";
                        id = data.id;
                        PanelUpdate.GetComponentInChildren<InputField>().text = data.name;
                    });
                }
            }
        }
    }

    public void ClickDeleteButton()
    {
        StartCoroutine(DeleteData());
    }
    IEnumerator DeleteData()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/" + path +"/" + id + "/destroy"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                UpdateDataButton();
                CloseAllPanel();
            }
        }
    }

    public void ClickUpdateButton()
    {
        if(PanelUpdate.GetComponentInChildren<InputField>().text != null)
            StartCoroutine(UpdateData());
    }
    IEnumerator UpdateData()
    {
        using (UnityWebRequest www = UnityWebRequest.Put(DataStore.basePath + "api/" + path + "/" + id + 
            "/update?name=" + PanelUpdate.GetComponentInChildren<InputField>().text, "{ }"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                UpdateDataButton();
                CloseAllPanel();
            }
        }
    }

    public void ShowNewData(string text)
    {
        PanelInfo.SetActive(true);
        PanelCreate.SetActive(true);
        PanelUpdate.GetComponentInChildren<InputField>().text = "";
        path = text;
    }
    public void ClickNewButton()
    {
        if (PanelCreate.GetComponentInChildren<InputField>().text != null)
            StartCoroutine(NewData());
    }
    IEnumerator NewData()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", PanelCreate.GetComponentInChildren<InputField>().text);

        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/" + path, form))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                UpdateDataButton();
                CloseAllPanel();
            }
        }
    }

    public void ShowAllStories()
    {
        if (Filter != 0)
            StartCoroutine(ShowStoryWithFilter());
        else 
            StartCoroutine(ShowStory());
        GameObject[] Buttons = GameObject.FindGameObjectsWithTag("BookButton");
        foreach (GameObject btn in Buttons)
            Destroy(btn.gameObject);
        AllBooks = GameObject.Find("AllBooks").GetComponent<ScrollRect>();
        AllBooks.GetComponentInChildren<GridLayoutGroup>().constraintCount =
            (int)Math.Ceiling((AllBooks.GetComponent<RectTransform>().rect.width - 10) / PrefabBook.GetComponent<RectTransform>().rect.width) - 1;
    }
    IEnumerator ShowStoryWithFilter()
    {
        string path = "";
        if (Filter == 1) path = "api/authors/" + id;
        if (Filter == 2) path = "api/genres/" + id;
        if (Filter == 3) path = "api/tags/" + id;
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + path))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                MiddleStoryPivotRoot storyRoot = JsonConvert.DeserializeObject<MiddleStoryPivotRoot>(www.downloadHandler.text);


                foreach (MiddleStoryPivot story in storyRoot.data)
                {
                    GameObject newBook = Instantiate(PrefabBook, AllBooks.content.transform);
                    newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                    newBook.GetComponentInChildren<Text>().text = story.name_rus;
                    newBook.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        DataStore.id = story.id;
                        LoadScene("BookScene");
                    });
                }
            }
        }
    }
    IEnumerator ShowStory()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                StoryRoot storyRoot = JsonConvert.DeserializeObject<StoryRoot>(www.downloadHandler.text);


                foreach (Story story in storyRoot.data)
                {
                    if (DataStore.Search)
                    {
                        if (story.name_rus.IndexOf(DataStore.Query, StringComparison.CurrentCultureIgnoreCase) != -1)
                        {
                            GameObject newBook = Instantiate(PrefabBook, AllBooks.content.transform);
                            newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                            newBook.GetComponentInChildren<Text>().text = story.name_rus;
                            newBook.GetComponent<Button>().onClick.AddListener(delegate
                            {
                                DataStore.id = story.id;
                                LoadScene("BookScene");
                            });
                        }
                    }
                    else
                    {
                        GameObject newBook = Instantiate(PrefabBook, AllBooks.content.transform);
                        newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                        newBook.GetComponentInChildren<Text>().text = story.name_rus;
                        newBook.GetComponent<Button>().onClick.AddListener(delegate
                        {
                            DataStore.id = story.id;
                            LoadScene("BookScene");
                        });
                    }
                }
            }
        }
    }

    public void Search()
    {
        DataStore.Query = InputQuary.text;
        DataStore.Search = true;
        ShowAllStories();
    }

    public void CanselSearch()
    {
        DataStore.Search = false;
        InputQuary.text = "";
        Filter = 0;
        ShowAllStories();
    }

}
