using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Column : MonoBehaviour
{

    float speed = 5;
    void Start()
    {
        //Reset();    //确保每次调用这个预制体都能重新生成初始位置
    }

    public void Reset(int k = 0)//重置物品位置的函数,有参数区分
    {
        float z = 100;
        if(k == 1)
        {
            z = Random.Range(-10, 80);
        }
        if(Random.Range(0, 100) < 50)   //利用分支结构实现中间偏移量
        {
            transform.position = new Vector3(Random.Range(2, 20), 0, z);
        }
        else
        {
            transform.position = new Vector3(Random.Range(-20, -2), 0, z);
        }
        transform.localScale = new Vector3(1, Random.Range(1, 6), 1);
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
        if(transform.position.z < Camera.main.transform.position.z - 1)
        {
            gameObject.SetActive(false);
        }
    }
}
