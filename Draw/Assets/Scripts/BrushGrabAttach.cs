using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;
using VRTK.GrabAttachMechanics;

public class BrushGrabAttach : VRTK_BaseGrabAttach
{

    [Header("Painter Options")]

    [SerializeField]
    private Transform tips;//笔尖
    [SerializeField] protected Blackboard board;//画板

    public event UnityAction OnStartGrad;
    public event UnityAction OnStopGrad;

    #region 重写的父类方法
    protected override void Initialise()
    {
        //初始化父类的一些字段，这些字段只是标识这个抓附机制的作用
        tracked = false;
        kinematic = false;
        climbable = false;

        //初始化自定义的属性
        if (precisionGrab)//最好不要用精确抓取，因为这样很有可能会让笔处于一个不合理的位置，这样使用的时候，会很变扭（比如必须手腕旋转一个角度，笔才是正的） 
        {
            Debug.LogError("PrecisionGrab cant't be true in case of PainterGrabAttach Mechanic");
        }
    }

    public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
    {
        if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
        {
            SnapObjectToGrabToController(givenGrabbedObject);
            grabbedObjectScript.isKinematic = true;
            OnStartGrad.Invoke();
            return true;
        }
        return false;
    }

    public override void StopGrab(bool applyGrabbingObjectVelocity)
    {
        ReleaseObject(applyGrabbingObjectVelocity);
        OnStopGrad.Invoke();
        base.StopGrab(applyGrabbingObjectVelocity);
    }

    public override void ProcessFixedUpdate()
    {
        if (grabbedObject)//只有抓住物体后，grabbedObject才不会
        {
            grabbedObject.transform.rotation = controllerAttachPoint.transform.rotation * Quaternion.Euler(grabbedSnapHandle.transform.localEulerAngles);
            grabbedObject.transform.position = controllerAttachPoint.transform.position - (grabbedSnapHandle.transform.position - grabbedObject.transform.position);
            float distance = board.GetDistanceFromBoardPlane(tips.position);//笔尖距离平面的距离
            bool isPositiveOfBoardPlane = board.GetSideOfBoardPlane(tips.position);//笔尖是不是在笔尖的正面
            Vector3 direction = grabbedObject.transform.position - tips.position;//笔尖位置指向笔的位置的差向量
            //当笔尖穿透的时候，需要矫正笔的位置 
            if (isPositiveOfBoardPlane || distance > 0.0001f)
            {
                Vector3 pos = board.ProjectPointOnBoardPlane(tips.position);
                grabbedObject.transform.position = pos - board.BoardPlane.normal * 0.001f + direction;//pos是笔尖的位置，而不是笔的位置，加上direction后才是笔的位置 
            }
        }
    }

    #endregion

    //让手柄抓住物体
    private void SnapObjectToGrabToController(GameObject obj)
    {
        if (!precisionGrab)
        {
            SetSnappedObjectPosition(obj);
        }
    }

    //设置物体和手柄连接的位置 
    private void SetSnappedObjectPosition(GameObject obj)
    {
        if (grabbedSnapHandle == null)
        {
            obj.transform.position = controllerAttachPoint.transform.position;
        }
        else
        {
            //设置旋转,controllerAttachPoint是手柄上的一个与物体的连接点
            obj.transform.rotation = controllerAttachPoint.transform.rotation * Quaternion.Euler(grabbedSnapHandle.transform.localEulerAngles);
            //因为grabbedSnapHandle和obj.transform之间可能不是同一个点，所以为了让手柄抓的位置是grabbedSnapHandle,需要减去括号中代表的向量
            obj.transform.position = controllerAttachPoint.transform.position - (grabbedSnapHandle.transform.position - obj.transform.position);
        }
    }
}

