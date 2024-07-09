using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheak_gate : MonoBehaviour
{
    private void Start()
    {

    }

    // 当有物体进入Collider时调用
    private void OnTriggerEnter(Collider other)
    {
        // 检查碰撞物体是否是"Player"
        if (other.CompareTag("Player"))
        {
            // 播放Open动画
            Debug.Log("Enter");
            GetComponent<Animation>().Play("Gate_Open");
        }
    }

    // 当有物体离开Collider时调用
    private void OnTriggerExit(Collider other)
    {
        // 检查碰撞物体是否是"Player"
        if (other.CompareTag("Player"))
        {
            // 播放Close动画
            Debug.Log("Exit");
            GetComponent<Animation>().Play("Gate_Close");
        }
    }
}

