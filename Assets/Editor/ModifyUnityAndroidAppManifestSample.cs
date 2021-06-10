using System.IO;
using System.Text;
using System.Xml;
using UnityEditor.Android;

public class NewBehaviourScript : IPostGenerateGradleAndroidProject
{
    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        var androidManifest = new AndroidManifest(GetManifestPath(basePath));

        // Add your XML manipulation routines
        androidManifest.SetStoragePermission();

        androidManifest.SetApplicationLegacyExternalStorage();

        androidManifest.Save();
    }

    public int callbackOrder { get { return 1; } }

    private string _manifestFilePath;

    private string GetManifestPath(string basePath)
    {
        if (string.IsNullOrEmpty(_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();
        }
        return _manifestFilePath;
    }
}


internal class AndroidXmlDocument : XmlDocument
{
    private string m_Path;
    protected XmlNamespaceManager nsMgr;
    public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
    public AndroidXmlDocument(string path)
    {
        m_Path = path;
        using (var reader = new XmlTextReader(m_Path))
        {
            reader.Read();
            Load(reader);
        }
        nsMgr = new XmlNamespaceManager(NameTable);
        nsMgr.AddNamespace("android", AndroidXmlNamespace);
    }

    public string Save()
    {
        return SaveAs(m_Path);
    }

    public string SaveAs(string path)
    {
        using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
        {
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
        return path;
    }
}


internal class AndroidManifest : AndroidXmlDocument
{
    private readonly XmlElement ApplicationElement;

    public AndroidManifest(string path) : base(path)
    {
        ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
    }

    private XmlAttribute CreateAndroidAttribute(string key, string value)
    {
        XmlAttribute attr = CreateAttribute("android", key, AndroidXmlNamespace);
        attr.Value = value;
        return attr;
    }

    internal XmlNode GetActivityWithLaunchIntent()
    {
        return SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and " +
                "intent-filter/category/@android:name='android.intent.category.LAUNCHER']", nsMgr);
    }

    internal void SetApplicationLegacyExternalStorage()
    {
        //<application ... android:requestLegacyExternalStorage="true" ... />
        ApplicationElement.Attributes.Append(CreateAndroidAttribute("requestLegacyExternalStorage", "true"));
    }

    internal void SetStartingActivityName(string activityName)
    {
        //<application android:name= $activityName ...>
        GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("name", activityName));
    }


    internal void SetHardwareAcceleration()
    {
        //<application android:hardwareAccelerated="true" ...>
        GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("hardwareAccelerated", "true"));
    }

    internal void SetStoragePermission()
    {
        //<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
        var manifest = SelectSingleNode("/manifest");
        XmlElement child = CreateElement("uses-permission");
        manifest.AppendChild(child);
        XmlAttribute newAttribute = CreateAndroidAttribute("name", "android.permission.READ_EXTERNAL_STORAGE");
        child.Attributes.Append(newAttribute);
    }
}
