using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PointInformation : MonoBehaviour
{
    private VRTK_Pointer vrtk_point;//获取射线点的信息
    public Color32 yellow;
    public Color32 blue;
    public Color32 white;
    public Color32 red;
    void Start()
    {

        vrtk_point = this.gameObject.GetComponent<VRTK_Pointer>();
        vrtk_point.DestinationMarkerEnter += enter;
    }
    private void enter(object sender, DestinationMarkerEventArgs e)
    {
        Debug.Log("当前碰触：" + e.raycastHit.collider.gameObject.name);
        switch(e.raycastHit.collider.gameObject.name)
        {
            case "yellow":
                PenBrush.penbrush.m_BrushColor =yellow;
                break;
            case "blue":
                PenBrush.penbrush.m_BrushColor =blue;
                break;
            case "white":
                PenBrush.penbrush.m_BrushColor =white;
                break;
            case "red":
                PenBrush.penbrush.m_BrushColor = red;
                break;


        }
    }
    void DisEnble()
    {
        vrtk_point.DestinationMarkerEnter -= enter;
    }
}
