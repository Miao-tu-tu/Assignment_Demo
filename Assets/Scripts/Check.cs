using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TriggerEvent : UnityEvent { }

/// <summary>
/// 触发检测
/// </summary>
public class Check : MonoBehaviour
{
    public TriggerEvent onenter, onexit;    //进入，离开事件
    //public GameObject gate1;

    //private Animator gate1Animator;

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
        if(other.tag == "Player")
        {
            if(onenter != null)
            {
                onenter.Invoke();
                Debug.Log("show things");
                //PlayOpenAnimation();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if(onexit != null)
            {
                onexit.Invoke();
                //PlayCloseAnimation();
            }
        }
    }

    //private void PlayOpenAnimation()
    //{
    //    // 播放gate1的LeftOpen动画
    //    Debug.Log("Animation Begin");
    //    gate1.GetComponent<Animation>().Play("Gate_Open");
    //    Debug.Log("Animation End");
    //}
    //private void PlayCloseAnimation()
    //{
    //    // 播放gate1的LeftClose动画
    //    if (gate1Animator != null)
    //    {
    //        gate1Animator.Play("Gate_Close");
    //    }
    //}
}
