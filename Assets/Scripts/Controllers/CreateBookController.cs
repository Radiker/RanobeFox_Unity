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
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files;

        TextPathCover.text = Application.persistentDataPath;
        scrollViewImages.SetActive(true);

        //files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => dirInfo.GetFiles(ext, SearchOption.AllDirectories)).ToArray();
        dirInfo = new DirectoryInfo("/storage/emulated/0/Download");
        TextPathCover.text = dirInfo.ToString();
        files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => dirInfo.EnumerateFiles(ext, SearchOption.AllDirectories)).ToArray();
        //files = dirInfo.GetFiles();
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

    public static string GetAndroidInternalFilesDir()
    {
        string[] potentialDirectories = new string[]
        {
        //"/mnt/sdcard/Download",
        "/sdcard/Download",
        "/storage/sdcard0/Download",
        "/storage/sdcard1/Download",
        "/storage/emulated/0/Download"
        };

        if (Application.platform == RuntimePlatform.Android)
        {
            for (int i = 0; i < potentialDirectories.Length; i++)
            {
                if (Directory.Exists(potentialDirectories[i]))
                {
                    return potentialDirectories[i];
                }
            }
        }
        return "";
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

        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories", form))
        {
            www.SetRequestHeader("Authorization", DataStore.token_type + " " + DataStore.token);
            
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                MiddleStoryRoot middleStoryRoot = JsonConvert.DeserializeObject<MiddleStoryRoot>(www.downloadHandler.text);

                DataStore.id = middleStoryRoot.data.id;
                LoadScene("BookScene");
            }
        }
    }
}
