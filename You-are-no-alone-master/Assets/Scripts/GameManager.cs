using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    [SerializeField] GameObject StartUI;

    [SerializeField] Animator teacherAni;//玩家动画状态机
    [SerializeField] Animator cameraAni;//相机的动画状态机
    [SerializeField] Camera cameras;//相机

    public Animator LightAni;
    public int LightSpeed;

    //控制PostProcessVolume的值
    public Animator PostProcessAni;
    public int PostProcessSpeed;

    //呼吸和心跳的Audio Source
    public AudioSource BreathSource;
    public AudioSource HeartSource;

    //三种不同程度的呼吸和心跳
    public AudioClip[] BreathClip;
    public AudioClip[] HeartClip;

    //结束的画面和文本
    public GameObject EndPanel;
    public Text End_Text;
    public float needTime;//打字机显示完文字需要的时间
    public AudioSource typewriter;//打字机音效

    // Start is called before the first frame update
    void Start()
    {
        if(Instance==null)
        {
            Instance = this;
        }
        StartUI.SetActive(true);//显示开始UI
        teacherAni.SetBool("isSpeeding", true);//切换玩家状态
        StartCoroutine(StartScene());

        LightSpeed = 1;
        PostProcessSpeed = 1;

        EndPanel.SetActive(false); //隐藏结束的画布
        
        //InvokeRepeating("CameraInterval", 2.0f,0.0f);//每间隔两秒调用一次眨眼效果
    }

    // Update is called once per frame
    void Update()
    {
        LightAni.speed = LightSpeed;
        PostProcessAni.speed = PostProcessSpeed;
    }
    IEnumerator StartScene()
    {
        yield return new WaitForSeconds(5.0f);
        
        StartUI.SetActive(false);
        teacherAni.SetBool("isSpeeding", false);//关闭教师动画
        teacherAni.gameObject.SetActive(false);//隐藏教师模型
    }
    IEnumerator CountTime(float time,GameObject gameObject)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    //相机眨眼效果
    public void  CameraInterval()
    {
        cameraAni.SetTrigger("isBlink");//开始眨眼效果
    }

    //改变呼吸和心跳的函数
    public void BreathAndHeart(int index)
    {
        BreathSource.clip = BreathClip[index];
        HeartSource.clip = HeartClip[index];

        //改变灯光闪烁速度和场景曝光速度
        LightSpeed = index;
        PostProcessSpeed = index;
    }
    public void Ends()
    {
        EndPanel.SetActive(true);
        typewriter.Play();//播放打字机音效
        End_Text.DOText("刚才的场景是否让你感觉到不安？这，就是孤独症孩子们平时的生活。当孤独症儿童处在陌生的的环境中或面对让他感觉很困难的任务时，他们会表现出各种各样的逃避性行为。他们的这种焦虑或恐惧感越强，从而视觉与听觉变得更加敏感，他们的问题行为就会表现得越突出。孤独症儿童就像遥远的星星一样被孤立在人群之外，然而，他们依然渴望得到我们的了解、关注、尊重和接纳，他们应该跟正常的孩子一样受到这个世界的温柔相待。请各位平等对待他们，尊重他们，关爱而不同情，照顾他们敏感而自尊的心。", needTime);
        Invoke("StopAudio", needTime);
    }
    public void StopAudio()
    {
        typewriter.Stop();//停止打字机音效
    }
}
