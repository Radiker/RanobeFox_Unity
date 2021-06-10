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

    public Sprite[] Bookmarks;
    public GameObject PrefabButton;
    public ScrollRect scrollDataRect;

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
        //using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/1"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");
            
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.url);
                Debug.Log(www.uri);
                FullStoryRoot root = JsonConvert.DeserializeObject<FullStoryRoot>(www.downloadHandler.text);

                GameObject CoverImage = Instantiate(cover, scrollRect.content.transform);
                CoverImage.GetComponentInChildren<ImageLoader>().url = DataStore.basePath + root.data[0].cover_link;

                GameObject label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Оригинальное название:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].name_original;

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Русское название:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].name_rus;

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Описание:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = root.data[0].description;

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Авторы:";

                foreach (Author authors in root.data[0].authors)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = authors.name;
                }

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Жанры:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = "";
                foreach (AdditionData genres in root.data[0].genres)
                {
                    label.GetComponentInChildren<Text>().text += genres.name+"; ";
                }

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
                label.GetComponentInChildren<Text>().text = "Тэги:";

                label = Instantiate(text, scrollRect.content.transform);
                label.GetComponentInChildren<Text>().text = "";
                foreach (AdditionData tags in root.data[0].tags)
                {
                    label.GetComponentInChildren<Text>().text += tags.name + "; ";
                }

                StartCoroutine(ShowChapters());
            }
        }
    }

    IEnumerator ShowChapters()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/"+DataStore.id+"/chapters"))
        //using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/1/chapters"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");

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
                label.GetComponentInChildren<Text>().fontSize = 12;
                label.GetComponentInChildren<Text>().text = "\nГлавы:";

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
        //using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/1/feedback"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");

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
                label.GetComponentInChildren<Text>().fontSize = 12;
                label.GetComponentInChildren<Text>().text = "\nОтзывы:";

                foreach (Mark data in root.data)
                {
                    label = Instantiate(text, scrollRect.content.transform);
                    label.GetComponentInChildren<Text>().text = data.name + " " + data.created_at + "\n";
                    label.GetComponentInChildren<Text>().text += "<b>Оценка:</b> " + data.value + "\n";
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
        //using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/1/feedback", form))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");
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
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");

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
                    ButtonAdd.GetComponent<Image>().sprite = Bookmarks[0];                    
                    ButtonAdd.onClick.AddListener(delegate {
                        StartCoroutine(DestroyDownload());
                    });
                }
                else
                {
                    ButtonAdd.GetComponent<Image>().sprite = Bookmarks[1];
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
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                ButtonAdd.GetComponent<Image>().sprite = Bookmarks[0];
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
            //www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                ButtonAdd.GetComponent<Image>().sprite = Bookmarks[1];
                ButtonAdd.onClick.RemoveAllListeners();
                ButtonAdd.onClick.AddListener(delegate
                {
                    StartCoroutine(AppendDownload());
                });
            }
        }
    }

    public void ClickDeleteButton()
    {
        StartCoroutine(DeleteStory());
    }
    IEnumerator DeleteStory()
    {
        using (UnityWebRequest www = UnityWebRequest.Delete(DataStore.basePath + "api/stories/" + DataStore.id + "/destroy"))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                LoadScene("MainScene");
            }
        }
    }

    public void ClickButtonCreateChapter()
    {
        LoadScene("CreateChapterScene");
    }
    public void ClickAddAuthor()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        foreach(GameObject prefab in prefabs)
        {
            Destroy(prefab.gameObject);
        }
        StartCoroutine(ShowAuthors());
    }
    public void ClickAddGenre()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab.gameObject);
        }
        StartCoroutine(ShowGenres());
    }
    public void ClickAddTag()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("DataButton");
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab.gameObject);
        }
        StartCoroutine(ShowTags());
    }

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
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    newButton.GetComponentInChildren<Text>().text = data.name;

                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        StartCoroutine(AddGenres(data.id));
                    });
                }
            }
        }
    }
    IEnumerator AddGenres(int id)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/genres/" + id))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                LoadScene("BookScene");
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
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    newButton.GetComponentInChildren<Text>().text = data.name;

                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        StartCoroutine(AddTags(data.id));
                    });
                }
            }
        }
    }
    IEnumerator AddTags(int id)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/tags/" + id))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                LoadScene("BookScene");
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
                    GameObject newButton = Instantiate(PrefabButton, scrollDataRect.content.transform);
                    newButton.GetComponentInChildren<Text>().text = data.name;

                    newButton.GetComponent<DataButton>().ButtonInfo.onClick.AddListener(delegate {
                        StartCoroutine(AddAuthors(data.id));
                    });
                }
            }
        }
    }

    IEnumerator AddAuthors(int id)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(DataStore.basePath + "api/stories/" + DataStore.id + "/authors/" + id))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                LoadScene("BookScene");
            }
        }
    }
}
