using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;

public class ChapterController : DefaultSceneController
{
    public GameObject text;
    public Button ReturnButton;
    private ScrollRect scrollRect;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        StartCoroutine(ShowDetail());
    }

    IEnumerator ShowDetail()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/chapters/"+DataStore.id))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                ChapterAllRoot root = JsonConvert.DeserializeObject<ChapterAllRoot>(www.downloadHandler.text);
                
                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Глава " + root.data.number + ": " + root.data.name;

                ReturnButton.onClick.AddListener(delegate {
                    DataStore.id = root.data.story_id;
                    LoadScene("BookScene");
                });

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
                label.GetComponentInChildren<Text>().text = root.data.text.ToString();
            }
        }
    }
}
