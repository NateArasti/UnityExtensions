using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PackageInstallerWindow : EditorWindow
{
    private const string PackagesDatasPath = "Packages/com.arastigames.unityextensions-installer/Editor/PackagesDatas";

    private static readonly List<PackageData> _packagesDatas = new List<PackageData>();

    private static ListRequest PackageListRequest;

    private Vector2 _scrollPosition;

    [MenuItem("Tools/UnityExtensions Installer/Install Panel")]
    public static void ShowWindow()
    {
        _packagesDatas.Clear();
        foreach (var file in Directory.GetFiles(PackagesDatasPath))
        {
            var data = AssetDatabase.LoadAssetAtPath<PackageData>(file);
            if (data != null)
            {
                _packagesDatas.Add(data);
                data.Status = PackageData.PackageInstallationState.NotInstalled;
            }
        }
        GetPackageList();
        GetWindow<PackageInstallerWindow>();
    }

    private void OnGUI()
    {
        if (PackageListRequest != null && !PackageListRequest.IsCompleted)
        {
            GUILayout.Label("Searching for packages...", EditorStyles.boldLabel);
            return;
        }
        GUILayout.Label("Separate UnityExtension packages installer", EditorStyles.boldLabel);
        var installingSomething = false;
        foreach (var packageData in _packagesDatas)
        {
            if(packageData.Status == PackageData.PackageInstallationState.CurrentlyInstalling)
            {
                installingSomething = true;
                break;
            }              
        }
        if (_packagesDatas.Count == 0) ReloadWindow();
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        foreach (var packageData in _packagesDatas)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(packageData.PackageDisplayName);
            if (installingSomething)
            {
                GUI.enabled = false;
            }
            switch (packageData.Status)
            {
                case PackageData.PackageInstallationState.CurrentlyInstalling:
                    GUILayout.Button("Installing...", GUILayout.MaxWidth(200));
                    break;
                case PackageData.PackageInstallationState.Installed:
                    if (GUILayout.Button("Remove", GUILayout.MaxWidth(200)))
                    {
                        packageData.Remove();
                    }
                    break;
                case PackageData.PackageInstallationState.NotInstalled:
                    if (GUILayout.Button("Install", GUILayout.MaxWidth(200)))
                    {
                        packageData.Add();
                    }
                    break;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            var normalColor = GUI.contentColor;
            GUI.contentColor = Color.cyan;
            packageData.Shown = EditorGUILayout.Foldout(packageData.Shown, "Source");
            if (packageData.Shown && GUILayout.Button("Open git", GUILayout.MaxWidth(200)))
            {
                Application.OpenURL(packageData.GitUrl);
            }
            GUI.contentColor = normalColor;
            GUILayout.Space(10);
        }
        GUILayout.EndScrollView();
    }

    private void ReloadWindow()
    {
        Close();
        ShowWindow();
    }

    private static void GetPackageList()
    {
        PackageListRequest = Client.List(true, false);
        EditorApplication.update += Progress;
    }

    static void Progress()
    {
        if (PackageListRequest.IsCompleted)
        {
            if (PackageListRequest.Status == StatusCode.Success)
                foreach (var package in PackageListRequest.Result)
                {
                    foreach (var packageData in _packagesDatas)
                    {
                        if (package.name == packageData.PackageName)
                        {
                            packageData.Status = PackageData.PackageInstallationState.Installed;
                        }
                    }
                }
            else if (PackageListRequest.Status >= StatusCode.Failure)
                Debug.Log(PackageListRequest.Error.message);

            EditorApplication.update -= Progress;
        }
    }
}
