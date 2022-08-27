using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PackageData", menuName = "PackageInstaller/PackageData")]
public class PackageData : ScriptableObject
{
    public enum PackageInstallationState
    {
        NotInstalled,
        Installed,
        CurrentlyInstalling
    }

    [SerializeField] private string _packageDisplayName;
    [SerializeField] private string _packageName;
    [SerializeField] private string _gitUrl;
    private Request _request;

    public string PackageDisplayName => _packageDisplayName;
    public string PackageName => _packageName;
    public string GitUrl => _gitUrl;
    public PackageInstallationState Status { get; set; }

    public bool Shown { get; set; }

    public void Add()
    {
        _request = Client.Add(_gitUrl);
        EditorApplication.update += InstallProgress;
        Status = PackageInstallationState.CurrentlyInstalling;
    }

    public void Remove()
    {
        _request = Client.Remove(_packageName);
        EditorApplication.update += RemoveProgress;
    }

    private void InstallProgress()
    {
        if (_request.IsCompleted)
        {
            if (_request.Status == StatusCode.Success)
            {
                Debug.Log("Installed: " + _packageDisplayName);
                Status = PackageInstallationState.Installed;
            }
            else if (_request.Status >= StatusCode.Failure)
            {
                Debug.Log(_request.Error.message);
            }

            EditorApplication.update -= InstallProgress;
        }
    }

    private void RemoveProgress()
    {
        if (_request.IsCompleted)
        {
            if (_request.Status == StatusCode.Success)
            {
                Debug.Log("Removed: " + _packageDisplayName);
                Status = PackageInstallationState.NotInstalled;
            }
            else if (_request.Status >= StatusCode.Failure)
            {
                Debug.Log(_request.Error.message);
            }

            EditorApplication.update -= RemoveProgress;
        }
    }
}
