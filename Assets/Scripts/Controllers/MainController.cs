using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Response;
using UnityEngine.UI;

public class MainController : DefaultSceneController
{
    public GameObject PrefabButtonBook;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowLatestStory());
    }

    IEnumerator ShowLatestStory()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/latest"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", DataStore.token_type +  " "+ DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                StoryRoot storyRoot = JsonConvert.DeserializeObject<StoryRoot>(www.downloadHandler.text);

                ScrollRect LatestBook = GameObject.Find("Latest Book").GetComponent<ScrollRect>();

                foreach (Story story in storyRoot.data)
                {
                    GameObject newBook = Instantiate(PrefabButtonBook, LatestBook.content.transform);
                    newBook.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + story.cover_link;
                    newBook.GetComponentInChildren<Text>().text = story.name_rus;
                    newBook.GetComponent<Button>().onClick.AddListener(delegate {
                        DataStore.id = story.id;
                        LoadScene("BookScene");
                    });
                }
            }
        }
    }
}
