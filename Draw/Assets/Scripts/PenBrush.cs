using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PenBrush : MonoBehaviour
{
    [SerializeField] private Color32 m_BrushColor;
    [SerializeField] private int m_BrushWidth;
    [SerializeField] private Transform m_RayOrigin;
    [SerializeField] private float m_RayLength;
    [SerializeField] private Blackboard m_Blackboard;

    private bool m_IsGrab;
    private RaycastHit m_HitInfo;

    private void Awake()
    {
        //扩展的物体抓取脚本 
        GetComponent<BrushGrabAttach>().OnStartGrad += OnStartGrab;
        GetComponent<BrushGrabAttach>().OnStopGrad += OnStopGrad;
    }

    private void OnStartGrab()
    {
        m_IsGrab = true;
    }
    private void OnStopGrad()
    {
        m_IsGrab = false;
        m_Blackboard.DrawStop();
    }

    private bool m_PreviousHaveHit;

    private void Update()
    {
        if (!m_IsGrab) return;
        var l_ray = new Ray(m_RayOrigin.position, m_RayOrigin.forward);

        Vector3 forward = m_RayOrigin.transform.TransformDirection(Vector3.forward) * m_RayLength;
        //绘制射线，调试用的。需要打开Gizmos开关，不然不显示。
        Debug.DrawRay(m_RayOrigin.position, forward, Color.magenta);

        if (Physics.Raycast(l_ray, out m_HitInfo, m_RayLength))
        {
            if (m_HitInfo.collider.CompareTag("Blackboard"))
            {
                m_Blackboard.UpdateDisplay(m_HitInfo.point, m_HitInfo.textureCoord, m_BrushWidth, m_BrushColor);
                m_PreviousHaveHit = true;
            }
        }
        else if (m_PreviousHaveHit)
        {
            m_Blackboard.DrawStop();
            m_PreviousHaveHit = false;
        }
    }

    private void OnDestroy()
    {
        GetComponent<BrushGrabAttach>().OnStartGrad -= OnStartGrab;
        GetComponent<BrushGrabAttach>().OnStopGrad -= OnStopGrad;
    }
}

