using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class StoryController : DefaultSceneController
{
    public GameObject cover;
    public GameObject text;
    public GameObject chapter;
    public GameObject feedback;
    private ScrollRect scrollRect;
    private Button ButtonAdd;



    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        ButtonAdd = GameObject.Find("ButtonAdd").GetComponent<Button>();
        StartCoroutine(ShowStories());
        StartCoroutine(CheckDownloads());
    }

    IEnumerator ShowStories()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);

                GameObject CoverImage = Instantiate(cover, scrollRect.content.transform);
                CoverImage.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + root.data[0].cover_link;

                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Оригинальное название:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                label.GetComponentInChildren<Text>().text = root.data[0].name_original;

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Русское название:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                label.GetComponentInChildren<Text>().text = root.data[0].name_rus;

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Описание:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                label.GetComponentInChildren<Text>().text = root.data[0].description;

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Авторы:";

                foreach (Author authors in root.data[0].authors)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                    label.GetComponentInChildren<Text>().text = authors.name;
                }

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Жанры:";

                foreach (Genre genres in root.data[0].genres)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                    label.GetComponentInChildren<Text>().text = genres.name;
                }

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Тэги:";

                foreach (Tag tags in root.data[0].tags)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                    label.GetComponentInChildren<Text>().text = tags.name;
                }

                StartCoroutine(ShowChapters());
                //StartCoroutine(ShowMarks());
            }
        }
    }

    IEnumerator ShowChapters()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/chapters"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                ChapterRoot root = JsonConvert.DeserializeObject<ChapterRoot>(www.downloadHandler.text);

                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Главы:";

                foreach (Chapter data in root.data)
                {
                    GameObject button = Instantiate(chapter, scrollRect.content.transform);
                    button.GetComponentInChildren<Text>().text = "Глава "+data.number+": "+data.name;
                    button.GetComponent<Button>().onClick.AddListener(delegate {
                        DataStore.id = data.id;
                        LoadScene("ChapterScene");
                    });
                }
            }
            StartCoroutine(ShowMarks());
        }
    }

    IEnumerator ShowMarks()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/feedback"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                MarkRoot root = JsonConvert.DeserializeObject<MarkRoot>(www.downloadHandler.text);

                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Отзывы:";

                foreach (Mark data in root.data)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = data.name + " " + data.created_at + "\n";
                    label.GetComponentInChildren<Text>().text += data.value + "\n";
                    label.GetComponentInChildren<Text>().text += data.description;
                    label.GetComponentInChildren<Text>().tag = "Feedback";
                }
            }
            GameObject feedbackElement = Instantiate(feedback, scrollRect.content.transform);
            feedbackElement.GetComponent<FeedbackButton>().button.onClick.AddListener(delegate {
                StartCoroutine(GiveFeedback(feedbackElement.GetComponent<FeedbackButton>().slider.value, feedbackElement.GetComponent<FeedbackButton>().inputField.text));
            });
        }
    }

    IEnumerator GiveFeedback(float value, string desc)
    {
        WWWForm form = new WWWForm();
        form.AddField("value", value.ToString());
        form.AddField("description", desc);

        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/" + DataStore.id+ "/feedback", form))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Feedback");
                foreach (GameObject btn in gameObjects)
                    Destroy(btn.gameObject);
                StartCoroutine(ShowMarks());
            }
        }
    }

    IEnumerator CheckDownloads()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/downloads/"+DataStore.id))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);

                ButtonAdd.onClick.RemoveAllListeners();
                if (root.data.Count != 0)
                {
                    ButtonAdd.GetComponentInChildren<Text>().text = "-";
                    ButtonAdd.onClick.AddListener(delegate {
                        StartCoroutine(DestroyDownload());
                    });
                }
                else
                {
                    ButtonAdd.GetComponentInChildren<Text>().text = "+";
                    ButtonAdd.onClick.AddListener(delegate {
                        StartCoroutine(AppendDownload());
                    });
                }
            }
        }
    }

    IEnumerator AppendDownload()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id + "/add"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                ButtonAdd.GetComponentInChildren<Text>().text = "-";
                ButtonAdd.onClick.RemoveAllListeners();
                ButtonAdd.onClick.AddListener(delegate {
                    StartCoroutine(DestroyDownload());
                });
            }
        }
    }

    IEnumerator DestroyDownload()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/downloads/" + DataStore.id + "/destroy"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                ButtonAdd.GetComponentInChildren<Text>().text = "+";
                ButtonAdd.onClick.RemoveAllListeners();
                ButtonAdd.onClick.AddListener(delegate
                {
                    StartCoroutine(AppendDownload());
                });
            }
        }
    }
}
