using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;
using TMPro;
using System.Net.Http;

public class CreateBookController : DefaultSceneController
{
    private InputField InputNameOriginal;
    private InputField InputNameRus;
    private InputField InputDescription;
    private string CoverPath;

    public GameObject FileIcon, scrollViewImages;
    public TextMeshProUGUI TextPathCover;

    

    // Start is called before the first frame update
    void Start()
    {
        InputNameOriginal = GameObject.Find("InputNameOriginal").GetComponent<InputField>();
        InputNameRus = GameObject.Find("InputNameRus").GetComponent<InputField>();
        InputDescription = GameObject.Find("InputDescription").GetComponent<InputField>();
    }

    public void LoadImageCover()
    {
        DirectoryInfo dirInfo = new DirectoryInfo("/storage/emulated/0/Download/");
        FileInfo[] files;

        TextPathCover.text = Application.persistentDataPath;
        scrollViewImages.SetActive(true);

        files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => dirInfo.GetFiles(ext, SearchOption.AllDirectories)).ToArray();
        
        TextPathCover.text = files.Length.ToString();
        foreach (FileInfo file in files)
        {
            FileIconButton fileIcon = Instantiate(FileIcon, scrollViewImages.GetComponent<ScrollRect>().content.transform).GetComponent<FileIconButton>();
            fileIcon.fileNameText.text = file.Name;
            fileIcon.GetComponent<Button>().onClick.AddListener(delegate {
                CoverPath = file.FullName;
                TextPathCover.text = CoverPath;
                scrollViewImages.SetActive(false);
            });
        }
    }   
    


    // Update is called once per frame
    public void OnButtonCreate()
    {
        StartCoroutine(CreateBook());
    }

    IEnumerator CreateBook()
    {
        byte[] imageBytes = File.ReadAllBytes(CoverPath);


        WWWForm form = new WWWForm();
        form.AddField("name_original", InputNameOriginal.text);
        form.AddField("name_rus", InputNameRus.text);
        form.AddField("description", InputDescription.text);
        form.AddBinaryData("cover", imageBytes);

        //using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories/" + DataStore.id+ "/feedback", form))
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories", form))
        {
            www.SetRequestHeader("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiY2EyMmU1MzEwNjM3MDFjZmY4YTMwYjQyZjgzZDEyODVjZDI1NjFiNjMyMjFlY2E3ZDkyMjEwZGVlOGFhMmM1MDYxMTlhMDg3Yzk0NmM0ZjYiLCJpYXQiOjE2MjIwNDM2NzIuMjQ5MjA3LCJuYmYiOjE2MjIwNDM2NzIuMjQ5MjE0LCJleHAiOjE2NTM1Nzk2NzIuMjM1MTQ3LCJzdWIiOiIxIiwic2NvcGVzIjpbXX0.DsolNt_LeIE9n8kL63rrXoTpWULDX5xhzNt4ZODQRWpDGgY3OHcyfji-Qo1opxByvqrCnFqoWpL-30_hwkQXmDZS9TPPuzZbgG6y-oHGbQUC_nwI54rVBGZ77UAjyX12PIc9BQdk1Ka1v-ChHNbAuI-ei1MhLYpvnPiqrp4Eon8Lo0Fzz30EcI64mqe80mp_8chwXTOcWbm7K0-OhyUDOMylJhiEuWF6Zp4JPoDKg8QjSLnyfhGZeRDDgTXRHzLHeZiaTm4rmo4TknrJhY2ySbzXZGTLBCkcPhr_XOhRjBBDQc1OzsqZQdC6lnd99SWdnAt9YKEtp1MXlRt9tcH9DzeqiM2c2m2UihkKpuAWoGSM3I4hzuopv3mauQxDi4-K-eZ7oqtF-EnIxDzVx3abwqOYUgR3Lv2KLX-YdoJa0hgdijM0TOkJdMkz2rwvbcfb8lkL0RJlZA3OySv_fGJt1aLbsmXtl_XD6ii-DbC52llVpbe5fYjl6plGGifuL5ux_n4uaHpUP6MjecIsmaJ1RASTwixBlf8KHl5YZLGREmFz2lRNd9tMV4h4FYdbVZJrra46lb2B2xkDbEchQXrlN7DhS1ogOLXIZ17yYL-JZ3OcMT7ri7BVZk2WYFyOnJz895nz_jR569wKEd1NYYomPKXr8lh7PfQUbNTON3PrvfU");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                LoadScene("CatalogScene");
            }
        }
    }
}
