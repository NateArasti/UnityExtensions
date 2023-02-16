using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

[CreateAssetMenu(fileName = "ExtensionPackageData", menuName = "ExtensionsInstaller/ExtensionPackageData")]
public class PackageData : ScriptableObject
{
    public enum PackageInstallationState
    {
        NotInstalled,
        Installed,
        CurrentlyInstalling
    }

    [SerializeField] private string m_PackageDisplayName;
    [SerializeField] private string m_PackageName;
    [SerializeField] private string m_GitUrl;
    private Request m_CurrentRequest;

    public string PackageDisplayName => m_PackageDisplayName;
    public string PackageName => m_PackageName;
    public string GitUrl => m_GitUrl;
    public PackageInstallationState Status { get; set; }

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

    public static PackageData[] GetAllPackageDatas()
    {
        var guids = AssetDatabase.FindAssets($"t:{nameof(PackageData)}");
        var packageDatas = new PackageData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            packageDatas[i] = AssetDatabase.LoadAssetAtPath<PackageData>(path);
        }

        return packageDatas;

    }
}
