using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public enum AssetBundlePattern
{
    /// <summary>
    /// 编辑器加载模式
    /// </summary>
    EditorSimulation,
    /// <summary>
    /// 本地加载模式
    /// </summary>
    Local,
    /// <summary>
    /// 远端加载模式
    /// </summary>
    Remote
}



public class AssetLoadTest : MonoBehaviour
{
    // Start is called before the first frame update
    AssetBundle SampleAsset;
    AssetBundle mainAB;
    /// <summary>
    /// 打包模式
    /// 
    /// </summary>
    public AssetBundlePattern BuildingPattern;

    public Button LoadAssetBundleButton;
    public Button LoadAssetButton;
    public Button UnLoadFalseButton;
    public Button UnLoadTrueButton;

    //包名本应由Editor类管理
    public static string MainAssetBundleName = "SampleAssetBundle";
    public static string ObjectAssetBundleName = "resourcesBundle";

    //资源加载路径
    public string AssetBundleLoadPath;

    public string HTTPAddress = "http://26.103.245.3:8080/";

    public string HTTPAssetBundlePath;

    public string DownLoadPath;

    public string prefabPath = "Prefabs/SchoolScene";


    private GameObject instantiatedPrefab;
    void Start()
    {
        CheckAssetBundleLoadPath();
        LoadAssetBundleButton.onClick.AddListener(CheckAssetBundlePattern);
        LoadAssetButton.onClick.AddListener(LoadPrefab);
        UnLoadFalseButton.onClick.AddListener(UnloadPrefab);
        UnLoadTrueButton.onClick.AddListener(() => { UnLoadAsset(true); });


    }


   void CheckAssetBundleLoadPath()
    {
        switch (BuildingPattern)
        {
            case AssetBundlePattern.EditorSimulation:
                break;
            case AssetBundlePattern.Local:
                AssetBundleLoadPath = Path.Combine(Application.streamingAssetsPath, "1.0.1", MainAssetBundleName);
                break;
            case AssetBundlePattern.Remote:
                HTTPAssetBundlePath = Path.Combine(HTTPAddress, MainAssetBundleName);
                //Debug.Log(AssetBundleLoadPath);
                DownLoadPath = Path.Combine(Application.persistentDataPath, "DownLoadAssetBundle");
                //Debug.Log(DownLoadPath);
                AssetBundleLoadPath = Path.Combine(DownLoadPath, MainAssetBundleName);
                if (!Directory.Exists(AssetBundleLoadPath))
                {
                    Directory.CreateDirectory(AssetBundleLoadPath);
                }
                break;
        }

    }
    //void LoadAssetBundle1() //直接加载包体
    //{
    //    string assetBundlePath = Path.Combine(Application.dataPath, "Bundles", "prefab");
    //    SampleAsset = AssetBundle.LoadFromFile(assetBundlePath);

    //    if (SampleAsset == null)
    //    {
    //        Debug.Log("加载AB包失败");
    //        return;
    //    }
    //    else { 
    //     Debug.Log("加载完成");
    //    }
    //}

    //IEnumerator DownloadAssetBundle()
    //{
    //    string remotePath = Path.Combine(HTTPAddress, MainAssetBundleName);

    //    string mainBundleDownloadPath = Path.Combine(remotePath, MainAssetBundleName);

    //    //UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(mainBundlePath);
    //    UnityWebRequest webRequest = UnityWebRequest.Get(mainBundleDownloadPath);

    //    yield return webRequest.SendWebRequest();
    //    Debug.Log(webRequest.downloadHandler.data.Length);

    //    //AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(webRequest);

    //    //Debug.Log(assetBundle.name); 
    //    ////主包中加载Manifest
    //    //AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
    //    //foreach (string bundleName in manifest.GetAllAssetBundles())
    //    //{
    //    //    Debug.Log(bundleName);
    //    //}
    //    string mainBundleSavePath = Path.Combine(AssetBundleLoadPath, MainAssetBundleName);
    //    yield return SaveFile(mainBundleSavePath, webRequest.downloadHandler.data);
    //    yield return null;

    //}
    //下载主包方法拓展为下载文件方法
    IEnumerator DownloadFile(string fileName, Action callBack, bool isSaveFile = true)
    {
        string mainBundleDownloadPath = Path.Combine(HTTPAssetBundlePath, fileName);
        Debug.Log(mainBundleDownloadPath);

        UnityWebRequest webRequest = UnityWebRequest.Get(mainBundleDownloadPath);
        webRequest.SendWebRequest();

        while (!webRequest.isDone)
        {
            //下载总字节数
            Debug.Log(webRequest.downloadedBytes);
            //下载进度
            Debug.Log(webRequest.downloadProgress);
            yield return new WaitForEndOfFrame();
        }

        Debug.Log(webRequest.downloadHandler.data.Length);

        if (isSaveFile)
        {
            string assetBundleDownloadPath = Path.Combine(AssetBundleLoadPath, fileName);
            yield return StartCoroutine(SaveFile(assetBundleDownloadPath, webRequest.downloadHandler.data,callBack));
        }
        else
        {
            //判断对象是否为空
            callBack?.Invoke();
        }
    }


    //异步保存文件
    IEnumerator SaveFile(string filePath, byte[] bytes,Action callBack)
    {
        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        yield return fileStream.WriteAsync(bytes, 0, bytes.Length); //调用异步写入文件的函数
        fileStream.Flush();
        fileStream.Close();
        fileStream.Dispose();

        callBack?.Invoke();
        Debug.Log($"{filePath}保存成功");
        yield return null;
    }

    void CheckAssetBundlePattern()
    {
        if (BuildingPattern == AssetBundlePattern.Remote)
        {
            Debug.Log("进入携程");
            StartCoroutine(DownloadFile(ObjectAssetBundleName, LoadAssetBundle));
        }
        else
        {
            Debug.Log("正在加载...");
            Debug.Log("加载完成...");
        }
    }

    void LoadAssetBundle() //加载主包及其依赖包
    {
        string assetBundlePath = Path.Combine(AssetBundleLoadPath, MainAssetBundleName);
        //加载主包
        AssetBundle mainAB = AssetBundle.LoadFromFile(assetBundlePath);
        AssetBundleManifest assetBundleManifest = mainAB.LoadAsset<AssetBundleManifest>(nameof(AssetBundleManifest));
        foreach (string depAssetBundleName in assetBundleManifest.GetAllDependencies(ObjectAssetBundleName))
        {
            Debug.Log(depAssetBundleName);
            assetBundlePath = Path.Combine(AssetBundleLoadPath, depAssetBundleName);
            AssetBundle.LoadFromFile(assetBundlePath);
        }

        assetBundlePath = Path.Combine(AssetBundleLoadPath, ObjectAssetBundleName);
        SampleAsset = AssetBundle.LoadFromFile(assetBundlePath);
        if (SampleAsset == null)
        {
            Debug.Log("加载AB包失败");
            return;
        }
        else
        {
            Debug.Log("加载完成");
        }

    }

    void LoadAsset()
    {

        string assetBundlePath = Path.Combine(AssetBundleLoadPath, MainAssetBundleName);
        SampleAsset = AssetBundle.LoadFromFile(assetBundlePath);
        var prefab = SampleAsset.LoadAsset<GameObject>("Cube");
        Instantiate(prefab);
        Debug.Log("加载资源完成");
    }

    // 加载预制体的方法
    void LoadPrefab()
    {
        if (instantiatedPrefab == null)
        {
            // 从资源路径中加载预制体
            GameObject prefab = Resources.Load<GameObject>(prefabPath);

            if (prefab != null)
            {
                // 实例化预制体并存储引用
                instantiatedPrefab = Instantiate(prefab);
                Debug.Log("Prefab loaded successfully.");
            }
            else
            {
                Debug.LogError($"Failed to load prefab at path: {prefabPath}");
            }
        }
        else
        {
            Debug.Log("Prefab is already loaded.");
        }
    }

    // 卸载预制体的方法
    void UnloadPrefab()
    {
        if (instantiatedPrefab != null)
        {
            // 销毁实例化的预制体
            Destroy(instantiatedPrefab);
            instantiatedPrefab = null;
            Debug.Log("Prefab unloaded successfully.");
        }
        else
        {
            Debug.Log("No prefab is currently loaded.");
        }
    }

    void UnLoadAsset(bool isTrue)
    {
        Debug.Log(isTrue);
        SampleAsset.Unload(isTrue);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
