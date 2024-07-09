using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door002_Cheak : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞物体是否是"Player"
        if (other.CompareTag("Player"))
        {
            // 播放Open动画
            GetComponent<Animation>().Play("Door002_Open");
        }
    }

    // 当有物体离开Collider时调用
    private void OnTriggerExit(Collider other)
    {
        // 检查碰撞物体是否是"Player"
        if (other.CompareTag("Player"))
        {
            // 播放Close动画
            GetComponent<Animation>().Play("Door002_Close");
        }
    }
}
