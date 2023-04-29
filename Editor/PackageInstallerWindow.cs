using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PackageInstallerWindow : EditorWindow
{
    #region Constants

    private const float k_WideButtonWidth = 200;
    private const float k_SimpleButtonWidth = 150;
    private const float k_SmallButtonWidth = 100;

    private const float k_VerticalPadding = 15;
    private const float k_HorizontalPadding = 10;

    #endregion

    private static ListRequest s_PackageListRequest;
    private static readonly List<PackageData> s_PackagesDatas = new List<PackageData>();

    private Vector2 m_ScrollPosition;

    [MenuItem("Tools/Extensions Installer Panel")]
    public static void ShowWindow()
    {
        s_PackagesDatas.Clear();
        s_PackagesDatas.AddRange(PackageData.GetAllPackageDatas());

        foreach (var package in s_PackagesDatas)
        {
            package.Status = PackageData.PackageInstallationState.NotInstalled;
        }

        GetPackageList();
        GetWindow<PackageInstallerWindow>("Extensions Installer Panel");
    }

    private void OnGUI()
    {
        var headerStyle =
            new GUIStyle(EditorStyles.whiteLargeLabel) { alignment = TextAnchor.MiddleCenter };

        GUILayout.Space(k_VerticalPadding);
        if (s_PackageListRequest != null && !s_PackageListRequest.IsCompleted)
        {
            GUILayout.Label("Searching for packages...", headerStyle);
            return;
        }
        GUILayout.Label("UnityExtensions packages", headerStyle);
        GUILayout.Space(k_VerticalPadding);
        var installingSomething = false;
        foreach (var packageData in s_PackagesDatas)
        {
            if(packageData.Status == PackageData.PackageInstallationState.CurrentlyInstalling)
            {
                installingSomething = true;
                break;
            }              
        }
        if (s_PackagesDatas.Count == 0) ReloadWindow();

        if (installingSomething)
        {
            GUI.enabled = false;
        }

        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);

        foreach (var packageData in s_PackagesDatas)
        {
            DrawPackageGUI(packageData);
        }

        GUILayout.EndScrollView();

        GUI.enabled = true;
    }

    private void DrawPackageGUI(PackageData packageData)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(k_HorizontalPadding);

        GUILayout.Label(packageData.PackageDisplayName);
        GUILayout.Space(k_HorizontalPadding);

        GUI.contentColor = Color.white;
        if (GUILayout.Button("Source", GUILayout.MaxWidth(k_SmallButtonWidth)))
        {
            Application.OpenURL(packageData.GitUrl);
        }
        GUILayout.Space(k_HorizontalPadding);

        switch (packageData.Status)
        {
            case PackageData.PackageInstallationState.CurrentlyInstalling:
                GUI.contentColor = Color.white;
                GUILayout.Button("Installing...", GUILayout.MaxWidth(k_SimpleButtonWidth));
                break;
            case PackageData.PackageInstallationState.Installed:
                GUI.contentColor = Color.red;
                if (GUILayout.Button("Remove", GUILayout.MaxWidth(k_SimpleButtonWidth)))
                {
                    packageData.Remove();
                }
                break;
            case PackageData.PackageInstallationState.NotInstalled:
                GUI.contentColor = Color.yellow;
                if (GUILayout.Button("Install", GUILayout.MaxWidth(k_SimpleButtonWidth)))
                {
                    packageData.Add();
                }
                break;
        }

        GUI.contentColor = Color.white;

        GUILayout.Space(k_HorizontalPadding);
        GUILayout.EndHorizontal();
        GUILayout.Space(k_VerticalPadding);
    }

    private void ReloadWindow()
    {
        Close();
        ShowWindow();
    }

    private static void GetPackageList()
    {
        s_PackageListRequest = Client.List(true, false);
        EditorApplication.update += Progress;
    }

    private static void Progress()
    {
        if (s_PackageListRequest.IsCompleted)
        {
            if (s_PackageListRequest.Status == StatusCode.Success)
            {
                foreach (var package in s_PackageListRequest.Result)
                {
                    foreach (var packageData in s_PackagesDatas)
                    {
                        if (package.name == packageData.PackageName)
                        {
                            packageData.Status = PackageData.PackageInstallationState.Installed;
                        }
                    }
                }
            }
            else if (s_PackageListRequest.Status >= StatusCode.Failure)
            {
                Debug.LogError(s_PackageListRequest.Error.message);
            }

            EditorApplication.update -= Progress;
        }
    }
}
