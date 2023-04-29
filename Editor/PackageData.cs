using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class PackageData
{
    public enum PackageInstallationState
    {
        None,
        NotInstalled,
        Installed,
        CurrentlyInstalling
    }

    public static string RootPath
    {
        get
        {
            var g = AssetDatabase.FindAssets($"t:Script {nameof(PackageData)}");
            return AssetDatabase.GUIDToAssetPath(g[0]);
        }
    }

    [SerializeField] private string m_PackageDisplayName;
    [SerializeField] private string m_PackageName;
    [SerializeField] private string m_GitUrl;
    private Request m_CurrentRequest;

    public string PackageDisplayName => m_PackageDisplayName;
    public string PackageName => m_PackageName;
    public string GitUrl => m_GitUrl;
    public PackageInstallationState Status { get; set; }

    public PackageData(string displayName, string packageName, string gitUrl)
    {
        m_PackageDisplayName = displayName;
        m_PackageName = packageName;
        m_GitUrl = gitUrl;
    }

    private void InstallProgress()
    {
        if (m_CurrentRequest.IsCompleted)
        {
            if (m_CurrentRequest.Status == StatusCode.Success)
            {
                Debug.Log("Installed: " + m_PackageDisplayName);
                Status = PackageInstallationState.Installed;
            }
            else if (m_CurrentRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(m_CurrentRequest.Error.message);
            }

            EditorApplication.update -= InstallProgress;
        }
    }

    private void RemoveProgress()
    {
        if (m_CurrentRequest.IsCompleted)
        {
            if (m_CurrentRequest.Status == StatusCode.Success)
            {
                Debug.Log("Removed: " + m_PackageDisplayName);
                Status = PackageInstallationState.NotInstalled;
            }
            else if (m_CurrentRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(m_CurrentRequest.Error.message);
            }

            EditorApplication.update -= RemoveProgress;
        }
    }

    public void Add()
    {
        m_CurrentRequest = Client.Add(m_GitUrl);
        EditorApplication.update += InstallProgress;
        Status = PackageInstallationState.CurrentlyInstalling;
    }

    public void Remove()
    {
        m_CurrentRequest = Client.Remove(m_PackageName);
        EditorApplication.update += RemoveProgress;
    }

    public static List<PackageData> GetAllPackageDatas()
    {
        FileInfo file = new FileInfo(RootPath);
        var path = file.Directory.ToString();
        path.Replace('\\', '/');

        path = Path.Combine(path, "packagesData.json");
        var packageListWrapper = JsonUtility.FromJson(File.ReadAllText(path), typeof(PackageDataListWrapper)) as PackageDataListWrapper;

        return packageListWrapper.packageDatas;
    }

    [System.Serializable]
    private class PackageDataListWrapper
    {
        public List<PackageData> packageDatas;

        public PackageDataListWrapper(List<PackageData> packageDatas)
        {
            this.packageDatas = packageDatas;
        }
    }
}
