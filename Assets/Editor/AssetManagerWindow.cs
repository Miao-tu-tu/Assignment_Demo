using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetManagerWindow : EditorWindow
{
    public static GUIStyle TitleLabelStyle;
    public static GUIStyle VersionlabelStyle;
    public static Texture2D LogoTexture;
    public static GUIStyle LogoLabelStyle;

    private void Awake()
    {
        TitleLabelStyle = new GUIStyle();
        TitleLabelStyle.alignment = TextAnchor.MiddleCenter;
        TitleLabelStyle.fontSize = 24;
        TitleLabelStyle.normal.textColor = Color.white;

        VersionlabelStyle = new GUIStyle();
        VersionlabelStyle.alignment = TextAnchor.LowerRight;
        VersionlabelStyle.fontSize = 14;
        VersionlabelStyle.normal.textColor = Color.green;

        LogoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Rabbit.png");
        LogoLabelStyle = new GUIStyle();

        LogoLabelStyle.alignment = TextAnchor.UpperCenter;


    }       
    private void OnGUI()
    {
        #region 标题图
        GUILayout.Space(20);
        if (LogoTexture != null )
        {
            GUILayout.Label(LogoTexture, LogoLabelStyle);
        }
        #endregion

        #region Title文字内容
        GUILayout.Space(20);
        GUILayout.Label(AssetManager.ManagerTitle, TitleLabelStyle);
        #endregion

        #region 版本号
        GUILayout.Space(20);
        GUILayout.Label(AssetManager.ManagerVersion,  VersionlabelStyle);
        #endregion

        #region 打包路径
        GUILayout.Space(10);
        AssetManager.BuildingPattern = (AssetBundlePattern)EditorGUILayout.EnumPopup("打包模式",AssetManager.BuildingPattern);
        #endregion
        
        #region 压缩方式
        GUILayout.Space(10);
        AssetManager.CompresionPattern = (AssetBundleCompresionPattern)EditorGUILayout.EnumPopup("压缩方式",AssetManager.CompresionPattern);
        #endregion

        #region 打包文件夹选择
        GUILayout.Space(20);
        AssetManager.AssetBundleDirectory = EditorGUILayout.ObjectField(AssetManager.AssetBundleDirectory, typeof(DefaultAsset), true) as DefaultAsset;
        if (AssetManager.CurrentAllAssets != null)
        {
            for (int i = 0; i < AssetManager.CurrentAllAssets.Count; i++)
            {
                AssetManager.CurrenSelectAssets[i] = EditorGUILayout.ToggleLeft(AssetManager.CurrentAllAssets[i], AssetManager.CurrenSelectAssets[i]);
            }
        }       
        #endregion




        if (GUILayout.Button("打包AssetBundle"))
        {
            string directoryPath = AssetDatabase.GetAssetPath(AssetManager.AssetBundleDirectory);
            //AssetManager.BuildAssetBundleFromEditorWindow();
            //AssetManager.GetSelectedAssetsDependencies();
            //AssetManager.BuildAssetBundleFromDirectory();
            //AssetManager.BuildAssetBundle();
            AssetManager.BuildAssetBundlesFromSets();
            Debug.Log("按钮被按下");
        }
    }
}
