using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Windows;
using static UnityEditor.PlayerSettings;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using System.Xml.Serialization;

public enum AssetBundleCompresionPattern
{
    LZMA,
    LZ4,
    None
}
public class AssetBundleVersionDifference
{
    public List<string> AdditionAssetBundles;
    public List<string> ReducedAssetBundles;
}

public class AssetManager
{
    public static AssetBundleCompresionPattern CompresionPattern;
    public static string ManagerVersion = "1.0.0";
    /// <summary>
    /// 编辑器模拟下,不进行打包
    /// 本地模式,打包到StreamingAssets
    /// 远端模式,打包到任意远端路径,在该示例中为persistentDataPath
    /// </summary>
    public static AssetBundlePattern BuildingPattern;
    public static string ManagerTitle = "AssetManager";
    public static List<string> CurrentAllAssets = new List<string>();
    public static bool[] CurrenSelectAssets;
    public static int CurrentBuildVersion = 100;

    private static DefaultAsset _AssetBundleDirectory;
    public static DefaultAsset AssetBundleDirectory
    {
        get => _AssetBundleDirectory;
        set
        {
            if (_AssetBundleDirectory != value)
            {
                _AssetBundleDirectory = value;
                if (_AssetBundleDirectory != null)
                {
                    string directoryPath = AssetDatabase.GetAssetPath(AssetBundleDirectory);
                    CurrentAllAssets = FindAllAssetPathFromFolder(directoryPath);
                    CurrenSelectAssets = new bool[CurrentAllAssets.Count];
                }
                Debug.Log("属性已经变化");
            }
        }
    }

    //打包输出路径
    public static string AssetBundleOutputPath;
    //public static string AssetBundleOutputPath = Path.Combine(Application.persistentDataPath, "AssetBundleOutput");


    [MenuItem("AssetManager/BuildAssetBundle")]

    public static void BuildAssetBundle()
    {
        
        // 定义资源路径和输出目录
        string assetPath = Path.Combine(Application.dataPath, "School_S");
        string assetBundleDirectory = Path.Combine(Application.dataPath, "Bundles");

        // 创建输出目录
        if (!System.IO.Directory.Exists(assetBundleDirectory))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }

        // 获取指定目录下的所有资源
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetPath, "");

        if (assetPaths.Length == 0)
        {
            Debug.LogError($"没有资源: {assetPath}");
            return;
        }

        // 为所有资源设置 AssetBundle 名称
        AssetImporter assetImporter;
        foreach (string path in assetPaths)
        {
            assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter != null)
            {
                assetImporter.assetBundleName = "schoolscene";
            }
        }


        // 构建 AssetBundle
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows);

        // 刷新项目窗口
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 获取当前资源名
    /// </summary>
    /// <returns></returns>
    public static List<string>GetALLSelectAssetNames()
    {
        List<string>selectedAssetNames = new List<string>();
        if(CurrenSelectAssets == null || CurrenSelectAssets.Length == 0)
        {
            return null;
        }
        for(int i = 0; i< CurrenSelectAssets.Length; i++)
        {
            if (CurrenSelectAssets[i])
            {
                selectedAssetNames.Add(CurrentAllAssets[i]);
            }
        }
        return selectedAssetNames;
    }
    /// <summary>
    /// 从当前资源名获取依赖资源名
    /// </summary>
    public static List<string> GetSelectedAssetsDependencies()
    {
        List<string> dependencies = new List<string>();
        List<string>selectedAsset = GetALLSelectAssetNames();

        for(int i = 0; i < selectedAsset.Count; i++)
        {
            //集合列表L中的一个元素
            string[] deps = AssetDatabase.GetDependencies(selectedAsset[i],true);
            foreach(string depName in deps)
            {
                Debug.Log(depName);
            }
        }

        return dependencies;
    }

    static BuildAssetBundleOptions CheckCompressionPattern()
    {

        BuildAssetBundleOptions option = new BuildAssetBundleOptions();
        switch (CompresionPattern)
        {
            case AssetBundleCompresionPattern.LZMA:
                option = BuildAssetBundleOptions.None;
                break;
            case AssetBundleCompresionPattern.LZ4:
                option = BuildAssetBundleOptions.ChunkBasedCompression;
                break;
            case AssetBundleCompresionPattern.None:
                option = BuildAssetBundleOptions.UncompressedAssetBundle;
                break;
        }
        return option;
    }

    [MenuItem("AssetManager/AssetManagerWindow")]

    static void OpenAssetManagerWindow()
    {
        AssetManagerWindow window = (AssetManagerWindow)EditorWindow.GetWindow(typeof(AssetManagerWindow), true, "AssetManager");
        window.Show();
    }

    static void CheckOutputPath()
    {
        string versionString = CurrentBuildVersion.ToString();
        for(int i = versionString.Length - 1;i >= 1;i--)
        {
            versionString = versionString.Insert(i, ".");
        }

        switch (BuildingPattern)
        {
            case AssetBundlePattern.EditorSimulation:
                break;
            case AssetBundlePattern.Local:
                AssetBundleOutputPath = Path.Combine(Application.streamingAssetsPath, versionString, AssetLoadTest.MainAssetBundleName);
                break;
            case AssetBundlePattern.Remote:
                AssetBundleOutputPath = Path.Combine(Application.persistentDataPath, versionString, AssetLoadTest.MainAssetBundleName);
                break;
        }
    }
    public static List<string> FindAllAssetPathFromFolder(string directoryPath)
    {
        List<string> objectPaths = new List<string>();
        //检查文件夹是否存在
        if (!System.IO.Directory.Exists(directoryPath) || string.IsNullOrEmpty(directoryPath))
        {
            Debug.Log("文件夹路径不存在");
            return null;
        }
        //System.IO空间下命名的类是Windows平台自带的对文件夹操作的类.
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        FileInfo[] fileInfos = directoryInfo.GetFiles();
        
        //所有非元数据文件路径都添加到列表中用于打包这些文件
        for (int i = 0; i < fileInfos.Length; i++)
        {
            var file = fileInfos[i];
            //跳过Unity的meta文件（后缀名为.meta）
            if (file.Extension.Contains("meta"))
            {
                continue;
            }
            //根据路径生成对应对象的完整路径
            string path = $"{directoryPath}/{file.Name}";
            objectPaths.Add(path);
            Debug.Log(path);
        }
        return objectPaths;
    }

    //public static void BuildAssetBundlesSets()
    //{
    //    CheckOutputPath();
    //    if (AssetBundleDirectory == null)
    //    {
    //        Debug.Log("打包路径不存在");
    //        return;
    //    }
    //    //所有输入到Editor窗口的Asset,就称为SourceAsset
    //    List<string> selectedAssetNames = GetALLSelectAssetNames();

    //    //所有由SourceAsset所衍生的,就称为DerivedAsset
    //    List<List<GUID>> selectedAssetDependencies = new List<List<GUID>>();

    //    //遍历每个SourceAsset的依赖
    //    foreach (string selectedAssetName in selectedAssetNames)
    //    {
    //        string[] assetDeps = AssetDatabase.GetDependencies(selectedAssetName, true);

    //        List<GUID> assetGUIDs = new List<GUID>();
    //        //因为构建打包时按照路径来识别资源的打包方式,所以这里需要将获取依赖资产之前,将SourceAsset的当前对象下依赖的每个Path转换为GUID
    //        GUID assetGUID = AssetDatabase.GUIDFromAssetPath(selectedAssetName);
    //        assetGUIDs.Add(assetGUID);

    //        //遍历所有选择
    //        foreach (string assetPath in assetDeps)
    //        {
    //            assetGUID = AssetDatabase.GUIDFromAssetPath(assetPath);
    //            assetGUIDs.Add(assetGUID);
    //            Debug.Log(assetGUID);
    //        }

    //        selectedAssetDependencies.Add(assetGUIDs);
    //    }
    //}
    /// <summary>
    /// 
    /// </summary>
    public static void BuildAssetBundlesFromSets()
    {

        CheckOutputPath();
        Debug.Log($"打包路径为:{ AssetBundleOutputPath}");

        if (AssetBundleDirectory == null)
        {

            Debug.Log("打包路径不存在");
            return;
        }
        //所有输入到Editor窗口的Asset,就称为SourceAsset
        List<string> selectedAssetNames = GetALLSelectAssetNames();

        //所有由SourceAsset所衍生的,就称为DerivedAsset
        //由DerivedAsset的信息所构成的列表,也就是要出力2的的信息列表
        List<List<string>> selectedAssetDependencies = new List<List<string>>();

        //遍历每个SourceAsset的依赖
        foreach (string selectedAssetName in selectedAssetNames)
        {
            string[] assetDeps = AssetDatabase.GetDependencies(selectedAssetName, true);

            List<string> assetGUIDs = new List<string>();

            foreach (string assetPath in assetDeps)
            {
                string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                assetGUIDs.Add(assetGUID);
            }

            selectedAssetDependencies.Add(assetGUIDs);
        }
        for(int i = 0; i < selectedAssetDependencies.Count; i++)
        {
            int nextIndex = i + 1;
            if(nextIndex >= selectedAssetDependencies.Count)
            {
                break;
            }

            Debug.Log($"对比之前{selectedAssetDependencies[i].Count}");
            Debug.Log($"对比之前{selectedAssetDependencies[nextIndex].Count}");

            for(int j = 0; j <= i; j++)
            {
                List<string> newDependencies = ContrastDependenceFromGUID(selectedAssetDependencies[i], selectedAssetDependencies[nextIndex]);
                //添加集合L中
                if(newDependencies != null && newDependencies.Count > 0)
                {
                    selectedAssetDependencies.Add(newDependencies);
                }
            }
        }
        AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[selectedAssetDependencies.Count];

        for (int i = 0; i < assetBundleBuilds.Length; i++)
        {
            string[] assetNames = new string[selectedAssetDependencies[i].Count];

            ///
            List<string> assetGUIDs = selectedAssetDependencies[i];

            for (int j = 0; j < assetNames.Length; j++)
            {
                string assetName = AssetDatabase.GUIDToAssetPath(selectedAssetDependencies[i][j].ToString());
                if (assetName.Contains(".cs"))
                {
                    continue;
                }
                assetNames[j] = assetName;
            }

            string[] assetNameArray = assetNames.ToArray();
            assetBundleBuilds[i].assetBundleName = ComputeAssetSetSignature(assetNameArray);
            assetBundleBuilds[i].assetNames = assetNameArray;
        }

        if (string.IsNullOrEmpty(AssetBundleOutputPath))
        {
            Debug.LogError("输出路径为空");
            return;
        }
        else if (!System.IO.Directory.Exists(AssetBundleOutputPath))
        {
            System.IO.Directory.CreateDirectory(AssetBundleOutputPath);
        }

        BuildPipeline.BuildAssetBundles(AssetBundleOutputPath, assetBundleBuilds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);
        Debug.Log(AssetBundleOutputPath);


        ///
        string[] currentVersionHashTable = BuildAssetBundleHashTable(assetBundleBuilds);
        if (CurrentBuildVersion >= 101)
        {
            Debug.Log("进入if判断");
            CurrentBuildVersion--;
            CheckOutputPath();

            string lastVersionHashTablePath = Path.Combine(AssetBundleOutputPath, "AssetBundleHashs");
            string lastVersionHashString = System.IO.File.ReadAllText(lastVersionHashTablePath);
            string[] lastVersionHashTable = JsonConvert.DeserializeObject<string[]>(lastVersionHashString);

            AssetBundleVersionDifference difference = ContrastAssetBundleHashTable(lastVersionHashTable, currentVersionHashTable);

            if (difference.AdditionAssetBundles.Count > 0)
            {
                foreach (var additionAssetBundle in difference.AdditionAssetBundles)
                {
                    Debug.Log($"当前版本新增资源{additionAssetBundle}");
                }
            }

            if (difference.ReducedAssetBundles.Count > 0)
            {
                foreach (var reducedAssetBundle in difference.ReducedAssetBundles)
                {
                    Debug.Log($"当前版本减少资源{reducedAssetBundle}");
                }
            }
        }

        CurrentBuildVersion++;
        Debug.Log(assetBundleBuilds);

        BuildAssetBundleHashTable(assetBundleBuilds);
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 对比两个资源版本的Hash
    /// </summary>
    /// <param name="currentAssetHashs"></param>
    static void GetVersionDiffrence(string[] currentAssetHashs)
    {
        if (CurrentBuildVersion >= 101)
        {
            Debug.Log("进入if判断");
            CurrentBuildVersion--;
            CheckOutputPath();

            string lastVersionHashTablePath = Path.Combine(AssetBundleOutputPath, "AssetBundleHashs");
            string lastVersionHashString = System.IO.File.ReadAllText(lastVersionHashTablePath);
            string[] lastVersionHashTable = JsonConvert.DeserializeObject<string[]>(lastVersionHashString);

            AssetBundleVersionDifference difference = ContrastAssetBundleHashTable(lastVersionHashTable, currentAssetHashs);

            if (difference.AdditionAssetBundles.Count > 0)
            {
                foreach (var additionAssetBundle in difference.AdditionAssetBundles)
                {
                    Debug.Log($"当前版本新增资源{additionAssetBundle}");
                }
            }

            if (difference.ReducedAssetBundles.Count > 0)
            {
                foreach (var reducedAssetBundle in difference.ReducedAssetBundles)
                {
                    Debug.Log($"当前版本减少资源{reducedAssetBundle}");
                }
            }
        }

    }
    /// <summary>
    /// 对比两个不同版本之间的新增和减少的部分
    /// </summary>
    /// <param name="oldHashTable"></param>
    /// <param name="newHashTable"></param>
    /// <returns></returns>
    public static AssetBundleVersionDifference ContrastAssetBundleHashTable(string[] oldHashTable, string[] newHashTable)
    {
        AssetBundleVersionDifference difference = new AssetBundleVersionDifference();
        difference.AdditionAssetBundles = new List<string>();
        difference.ReducedAssetBundles = new List<string>();

        // 如果旧的哈希表中含有新哈希表中不包含的哈希, 说明是需要移除的资产
        foreach (string assetHash in oldHashTable)
        {
            if (!newHashTable.Contains(assetHash))
            {
                difference.ReducedAssetBundles.Add(assetHash);

            }
        }

        // 如果新的哈希表中, 旧的哈希表中不包含的哈希, 说明是新增的资产
        foreach (string assetHash in newHashTable)
        {
            if (!oldHashTable.Contains(assetHash))
            {
                difference.AdditionAssetBundles.Add(assetHash);
            }
        }

        return difference;
    }

    /// <summary>
    /// 接收一组 asset 的名字，并通过 AssetDatabase.AssetPathToGUID 获取这些 asset 的 GUID。
    /// </summary>
    /// <param name="assetNames"></param>
    /// <returns></returns>
    static string ComputeAssetSetSignature(IEnumerable<string> assetNames)
    {
        var assetGuids = assetNames.Select(AssetDatabase.AssetPathToGUID);

        MD5 md5 = MD5.Create();

        foreach (string assetGuid in assetGuids.OrderBy(x => x))
        {
            byte[] buffer = Encoding.ASCII.GetBytes(assetGuid);
            md5.TransformBlock(buffer, 0, buffer.Length, null, 0);
        }

        md5.TransformFinalBlock(new byte[0], 0, 0);

        return BytesToHexString(md5.Hash);
    }
    /// <summary>
    /// 将 MD5 Hash 值转换为十六进制字符串并返回。
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    static string BytesToHexString(byte[] bytes)
    {
        StringBuilder byteString = new StringBuilder();
        foreach (byte aByte in bytes)
        {
            byteString.Append(aByte.ToString("x2"));
        }
        return byteString.ToString();
    }
    /// <summary>
    /// 将AB包的大小和AB包中文件列表所代表的Hash值写入到表中
    /// </summary>
    /// <param name="assetBundleBuilds"></param>
    /// <returns></returns>
    public static string[] BuildAssetBundleHashTable(AssetBundleBuild[] assetBundleBuilds)
    {
        string[] assetBundleHashes = new string[assetBundleBuilds.Length];
        for (int i = 0; i < assetBundleBuilds.Length; i++)
        {
            string assetBundlePath = Path.Combine(AssetBundleOutputPath, assetBundleBuilds[i].assetBundleName);
            FileInfo fileInfo = new FileInfo(assetBundlePath);
            //表中记录的是一个AssetBundle文件的长度及其内容
            assetBundleHashes[i] = $"{fileInfo.Length}_{assetBundleBuilds[i].assetBundleName}";
        }

        string hashString = JsonConvert.SerializeObject(assetBundleHashes);
        string hashFilePath = Path.Combine(AssetBundleOutputPath, "AssetBundleHashs");
        System.IO.File.WriteAllText(hashFilePath, hashString);

        return assetBundleHashes;
    }


    public static void BuildAssetBundleFromEditorWindow()
    {
        CheckOutputPath();
        List<string> buildAssets = new List<string>();
        for (int i = 0; i < CurrenSelectAssets.Length; i++)
        {
            if (CurrenSelectAssets[i])
            {
                buildAssets.Add(CurrentAllAssets[i]);
            }
        }

        AssetBundleBuild[] builds = new AssetBundleBuild[1];
        //将多个Asset打包进一个AssetBundle
        //assetBundleName是要打包的包名
        builds[0].assetBundleName = AssetLoadTest.ObjectAssetBundleName;
        //assetNames是要打包资源的路径
        builds[0].assetNames = buildAssets.ToArray();

        BuildPipeline.BuildAssetBundles(AssetBundleOutputPath, builds, CheckCompressionPattern(), BuildTarget.StandaloneWindows);

        AssetDatabase.Refresh();
    }


    public static void BuildAssetBundleFromDirectory()
    {
        CheckOutputPath();
        if (AssetBundleDirectory == null)
        {
            Debug.Log("打包路径不存在");
            return;
        }
        string directoryPath = AssetDatabase.GetAssetPath(AssetBundleDirectory);

        //将多个Asset打包进一个AssetBundle
        AssetBundleBuild[] builds = new AssetBundleBuild[1];

        //assetNames表示目录的资源的路径数组
        builds[0].assetNames = FindAllAssetPathFromFolder(directoryPath).ToArray();
        //assetBundleName是要包的包名
        builds[0].assetBundleName = AssetLoadTest.ObjectAssetBundleName;

        if (string.IsNullOrEmpty(AssetBundleOutputPath))
        {
            Debug.LogError("输出路径为空");
            return;
        }else if (!System.IO.Directory.Exists(AssetBundleOutputPath))
        {
            System.IO.Directory.CreateDirectory(AssetBundleOutputPath);
        }


        BuildPipeline.BuildAssetBundles(AssetBundleOutputPath, builds, CheckCompressionPattern(),
                                        BuildTarget.StandaloneWindows);

        Debug.Log(AssetBundleOutputPath);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// List是引用类型,注意参数形式
    /// </summary>
    /// <param name="depsA"></param>
    /// <param name="depsB"></param>
    /// <returns></returns>
    public static List<string> ContrastDependenceFromGUID(List<string> depsA, List<string> depsB)
    {
        List<string> newDerivedAssets = new List<string>();
        
        //取交集
        foreach (string assetGUID in depsA)
        {
            if (depsB.Contains(assetGUID))
            {
                newDerivedAssets.Add(assetGUID);
            }
        }

        //取差集
        foreach (string derivedAssetGUID in newDerivedAssets)
        {
            if (depsA.Contains(derivedAssetGUID))
            {
                depsA.Remove(derivedAssetGUID);
            }
            if (depsB.Contains(derivedAssetGUID))
            {
                depsB.Remove(derivedAssetGUID);
            }
        }

        Debug.Log(newDerivedAssets.Count);
        return newDerivedAssets;
    }


}