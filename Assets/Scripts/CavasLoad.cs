using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CavasLoad : MonoBehaviour
{
    public Image Avatar;
    public Text Name;
    // Start is called before the first frame update
    void Start()
    {
        Avatar.sprite = LoginCheak.avatar;
        Name.text = LoginCheak.userName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
