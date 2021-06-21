using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Response;
using TMPro;

public class CreateBookController : DefaultSceneController
{
    //���� ����� ����������
    private InputField InputNameOriginal;
    private InputField InputNameRus;
    private InputField InputDescription;
    //���� �� ������������ �����
    private string CoverPath;
    FileInfo[] files;

    [Header("������ ������ �����")]
    public GameObject FileIcon;
    [Header("���� ������ ���� �����")]
    public TextMeshProUGUI TextPathCover;
    [Header("������� ����������� ������ ������ ������")]
    public GameObject scrollViewImages;
    
    // Start is called before the first frame update
    void Start()
    {
        //������� ���� ����� �� �����
        InputNameOriginal = GameObject.Find("InputNameOriginal").GetComponent<InputField>();
        InputNameRus = GameObject.Find("InputNameRus").GetComponent<InputField>();
        InputDescription = GameObject.Find("InputDescription").GetComponent<InputField>();
    }

    public void LoadImageCover()
    {
        //�������������� ����� ��� ������ �������
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        //�������������� ������ ������� ������ � �������
        files = null;
        //�������� ����������� ������� ����������� ������ ������ ������
        scrollViewImages.SetActive(true);
        //���� ��� ������������ ������� ������ �����������
        //�������� ��� �������� ������������ ��� ��������� �������
        GameObject[] FileIcons = GameObject.FindGameObjectsWithTag("DataButton");
        //���������� �� ���� �������� � ������� ��
        foreach (GameObject fileIcon in FileIcons)
            Destroy(fileIcon.gameObject);
        //��������� ����� ��� ������
        //� �������� ���� �������� ������� GetAndroidInternalFilesDir()
        dirInfo = new DirectoryInfo(GetAndroidInternalFilesDir());
        //�������� � ����� ����� ����������� ����������
        files = new string[] { "*.jpeg", "*.jpg", "*.png" }.SelectMany(ext => dirInfo.EnumerateFiles(ext, SearchOption.AllDirectories)).ToArray();
        //��������� �������� ������
        if(files.Length != 0)
            StartCoroutine(LoadFromFile(0));
    }

    IEnumerator LoadFromFile(int i)
    {
        //������� ������ ������ �����������
        FileIconButton fileIcon = Instantiate(FileIcon, scrollViewImages.GetComponent<ScrollRect>().content.transform).GetComponent<FileIconButton>();
        //������ ������� ������� �� ��� �����
        fileIcon.fileNameText.text = files[i].Name;

        //��������� ���������� ������� �� �������
        fileIcon.GetComponent<Button>().onClick.AddListener(delegate {
            //��������� ���� �� �����
            CoverPath = files[i].FullName;
            //���������� ������ ���� �� �����
            TextPathCover.text = CoverPath;
            //�������� ������� �������� ������
            scrollViewImages.SetActive(false);
        });

        UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + files[i].FullName);
        //���������� ������
        yield return www.SendWebRequest();
        //��������� ��������� �������
        if (www.result != UnityWebRequest.Result.Success)
            Debug.Log(www.error);
        else
        {
            //�������� �������� �� ����������
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture as Texture2D;
            fileIcon.GetComponentInChildren<AspectRatioFitter>().aspectRatio = (float)texture.width / (float)texture.height;
            //������� ������ �� ������ ��������
            Sprite webSprite =
                Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            //���������� ������� �����������
            fileIcon.GetComponentInChildren<Image>().sprite = webSprite;
        }
        if (i + 1 < files.Length)
        {
            i++;
            StartCoroutine(LoadFromFile(i));
        }
    }

    //������� ��������� ����
    public static string GetAndroidInternalFilesDir()
    {
        //������������� ���� �� �����
        string[] potentialDirectories = new string[]
        {
        "/mnt/sdcard/Download",
        "/sdcard/Download",
        "/storage/sdcard0/Download",
        "/storage/sdcard1/Download",
        "/storage/emulated/0/Download"
        };
        //���� �������� �� Android
        if (Application.platform == RuntimePlatform.Android)
        {
            //�������� �� ������� �����
            for (int i = 0; i < potentialDirectories.Length; i++)
            {
                //���������� ���� ���� ����������
                if (Directory.Exists(potentialDirectories[i]))
                {
                    return potentialDirectories[i];
                }
            }
        }
        //���� �������� �� Windows
        //���� ���������� ����������� ��������������
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return "C:\\Users\\admin\\Downloads";
        }
        return "";
    }

    //���������� ������� ������ �������
    public void OnButtonCreate()
    {
        //����������� ��������� �������
        StartCoroutine(CreateBook());
    }

    IEnumerator CreateBook()
    {
        //���������� ���� � �������� ������� ������
        byte[] imageBytes = File.ReadAllBytes(CoverPath);
        //������� ����� �������
        WWWForm form = new WWWForm();
        //��������� ���� � ����� �������
        form.AddField("name_original", InputNameOriginal.text);
        form.AddField("name_rus", InputNameRus.text);
        form.AddField("description", InputDescription.text);
        //��������� ������ ���� � ����� �������
        form.AddBinaryData("cover", imageBytes);
        //������� Post ������ � ������
        using (UnityWebRequest www = UnityWebRequest.Post(DataStore.basePath + "api/stories", form))
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
                //������������ ��������� �� JSON � ����� MiddleStoryRoot
                MiddleStoryRoot middleStoryRoot = JsonConvert.DeserializeObject<MiddleStoryRoot>(www.downloadHandler.text);
                //���������� id � ��������� �������� id �����
                DataStore.id = middleStoryRoot.data.id;
                //��������� ����� ����������� �����
                LoadScene("BookScene");
            }
        }
    }
}