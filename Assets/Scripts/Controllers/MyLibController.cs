using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Response;
using UnityEngine.UI;
using System;

public class MyLibController : DefaultSceneController
{
    public GameObject PrefabButtonBook;
    private ScrollRect MyBook;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShowMyStory());

        MyBook = GameObject.Find("My Book").GetComponent<ScrollRect>();
        MyBook.GetComponentInChildren<GridLayoutGroup>().constraintCount =
            (int)Math.Ceiling((MyBook.GetComponent<RectTransform>().rect.width - 10) / PrefabButtonBook.GetComponent<RectTransform>().rect.width) - 1;
    }

    IEnumerator ShowMyStory()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/downloads/my"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

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
                    GameObject newBook = Instantiate(PrefabButtonBook, MyBook.content.transform);
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
