using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Column : MonoBehaviour
{
    public GameObject PrefabColumn;
    public GameObject PrefabRoad;
    public AudioClip audioClip;

    private AudioSource audioSource;
    List<GameObject> listColumn = new List<GameObject>();
    List<GameObject> listRoad = new List<GameObject>();

    public float freq = 2;
    float freqTime = 0;
    int RoadCount = 0;
    void Start()
    {
        freqTime = 0;
        for(int i = 0; i < 20; i++)
        {
            CreateColumn(1);
        }

        Vector3 vec = Vector3.zero;
        for (int i = 0; i < 12; i++)
        {
            CreateRoad(vec);
            vec.z -= 1;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (freqTime <= 0)
        {
            CreateColumn();
            freqTime = freq;
        }
        else
        {
            freqTime -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaySound();
        }
    }

    void CreateColumn(int k = 0)
    {
        GameObject obj = null;  //遍历循环访问列表中的柱子
        for(int i = 0; i < listColumn.Count; i++)
        {
            if (listColumn[i].activeInHierarchy == false)   //找到列表，将列表包含在obj中，循环终止
            {
                obj = listColumn[i];
                break;
            }
        }
        if(obj == null) //循环结束，未找到物品，创建新物品
        {
            obj = Instantiate(PrefabColumn);
            listColumn.Add(obj);    //将物品添加到列表中

        }
        else
        {
            obj.SetActive(true);
        }
        obj.GetComponent<Column>().Reset(k);
    }

    void CreateRoad(Vector3 position)
    {
        if(RoadCount >= 12)
        {
            return;
        }
        GameObject obj = null;
        for (int i = 0; i < listRoad.Count; i++)
        {
            if (listRoad[i].activeInHierarchy == false)
            {
                obj = listRoad[i];
                break;
            }
        }
        if (obj == null)
        {
            obj = Instantiate(PrefabRoad);
            listRoad.Add(obj);

        }
        else
        {
            obj.SetActive(true);
        }
        obj.GetComponent<Roads>().Reset(position);
        RoadCount++;
    }

    public void SubRoadCount()
    {
        RoadCount--;
    }

    // 播放声音片段的方法
    void PlaySound()
    {
        // 设置AudioSource的clip属性为指定的声音片段
        audioSource.clip = audioClip;

        // 播放声音片段

        Debug.Log("music~");
        audioSource.Play();
    }

    public void OnRegister()
    {
        Application.OpenURL("https://www.yuanshen.com/#/");
    }
}
