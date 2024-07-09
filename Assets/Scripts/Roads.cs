using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roads : MonoBehaviour
{
    float speed = 5;
    public void Reset(Vector3 position)
    {
        transform.position = position;
        if((int)position.z == 0)
        {
            PlayAnim();
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * 1 * 0.01f);  //Vector3.back * speed * Time.deltaTime
        if (transform.position.z < Camera.main.transform.position.z - 1)
        {
            //gameObject.SetActive(false);
            Reset(Vector3.forward * (Camera.main.transform.position.z + 11));
            PlayAnim();
        }

    }

    void PlayAnim()
    {
        int tmp = Random.Range(0, 100);
        if (tmp < 25)
        {
            GetComponent<Animation>().Play("Road");
        }else if(25 <= tmp &&  tmp < 50)
        {
            GetComponent<Animation>().Play("Road 1");
        }
        else if (50 <= tmp && tmp < 75)
        {
            GetComponent<Animation>().Play("Road 2");
        }
        else
        {
            GetComponent<Animation>().Play("Road 3");
        }

    }
}
