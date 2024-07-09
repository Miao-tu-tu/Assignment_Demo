using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheakForGate : MonoBehaviour
{
    public GameObject gate1;
    public GameObject gate2;

    private Animator gate1Animator;
    private Animator gate2Animator;

    private void Start()
    {
        // 获取gate1和gate2的Animator组件
        gate1Animator = gate1.GetComponent<Animator>();
        gate2Animator = gate2.GetComponent<Animator>();
    }

    // 当碰撞开始时调用
    private void OnCollisionEnter(Collision collision)
    {
        // 检测碰撞的对象是否为Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 播放gate1和gate2的open动画
            PlayOpenAnimation();
        }
    }

    // 当碰撞结束时调用
    private void OnCollisionExit(Collision collision)
    {
        // 检测碰撞的对象是否为Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 播放gate1和gate2的close动画
            PlayCloseAnimation();
        }
    }

    private void PlayOpenAnimation()
    {
        // 播放gate1的LeftOpen动画
        if (gate1Animator != null)
        {
            gate1Animator.Play("LeftOpen");
        }

        // 播放gate2的RightOpen动画
        if (gate2Animator != null)
        {
            gate2Animator.Play("RightOpen");
        }
    }

    private void PlayCloseAnimation()
    {
        // 播放gate1的LeftClose动画
        if (gate1Animator != null)
        {
            gate1Animator.Play("LeftClose");
        }

        // 播放gate2的RightClose动画
        if (gate2Animator != null)
        {
            gate2Animator.Play("RightClose");
        }
    }
}


