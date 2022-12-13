using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlane : MonoBehaviour
{
    private bool isShow = false;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
    public void changeColorPlane()
    {
        if(isShow )
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        isShow = !isShow;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
