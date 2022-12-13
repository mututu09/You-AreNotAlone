using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //线索和提示UI
    /*[SerializeField] GameObject ClueObject01;
    [SerializeField] GameObject ClueObject02;
    [SerializeField] GameObject ClueObject03;*/
    [SerializeField] GameObject ClueCanvas01;
    [SerializeField] GameObject ClueCanvas02;
    [SerializeField] GameObject ClueCanvas03;
    [SerializeField] GameObject Drawboard;

    //用于判断是否第一次点击线索
    private bool isclue01;
    private bool isclue02;
    private bool isclue03;
    private bool isfind;//找到线索

    [SerializeField] private int cluecount;//当前索取的线索总数


    //开始文本
    [SerializeField] GameObject startPanel;//开始的UI；
    [SerializeField] GameObject DrawPanel;//可以作画的UI；

    //可以开始作画的UI
    [SerializeField] GameObject NoThinkUI;
    // Start is called before the first frame update
    void Start()
    {
        ClueCanvas01.SetActive(false);
        ClueCanvas02.SetActive(false);
        ClueCanvas03.SetActive(false);
        Drawboard.SetActive(false);
        DrawPanel.SetActive(false);
        NoThinkUI.SetActive(false);
        isclue01 = false;
        isclue02 = false;
        isclue03 = false;
        isfind = false;
        cluecount = 0;

        //显示开始UI
        startPanel.SetActive(true);//显示开始界面对话UI
        StartCoroutine(CountTime(10.0f,startPanel));//调用协程5秒后隐藏物体
    }

    // Update is called once per frame
    void Update()
    {
        /*time -= Time.deltaTime;
        Debug.Log("时间：" + time);
        if (time <= 0)
        {
            InvisibleObject.SetActive(false);//隐藏当前物体
        }*/
        //如果三个线索都找到了，则进入可以开始作画
        if (cluecount==3&&isfind==false)
        {
            StartCoroutine(later());//显示找齐线索的面板
            NoThinkUI.SetActive(true);
            isfind = true;
            Drawboard.SetActive(true);//显示画板
            StartCoroutine(CountTime(5.0f,DrawPanel));//5秒后关闭开始界面的对话UI
           // StartCoroutine(CountTime(10.0f, NoThinkUI));//10秒后关闭开始界面的对话UI

        }
    }
    /*public void Clue01()
    {
        nowClue = ClueCanvas01;
        ClueCanvas01.SetActive(true);//显示线索一文本
        time = 2.0f;
        if(isclue01==false)
        {
            cluecount++;
            isclue01 = true;
        }
    }
    public void Clue02()
    {
        nowClue = ClueCanvas02;
        ClueCanvas02.SetActive(true);//显示线索二文本
        time = 2.0f;
        if (isclue02 == false)
        {
            cluecount++;
            isclue02 = true;
        }
    }
    public void Clue03()
    {
        nowClue = ClueCanvas03;
        ClueCanvas03.SetActive(true);//显示线索三文本
        time = 2.0f;
        if (isclue03 == false)
        {
            cluecount++;
            isclue03 = true;
        }
    }*/
    public void Clue(int num)
    {
        switch(num)
        {
            case 1:
                ClueCanvas01.SetActive(true);//显示线索一文本
                if (isclue01 == false)
                {
                    cluecount++;
                    isclue01 = true;
                }
                StartCoroutine(CountTime(2.0f,ClueCanvas01));
                break;
            case 2:
                ClueCanvas02.SetActive(true);//显示线索二文本
                if (isclue02 == false)
                {
                    cluecount++;
                    isclue02 = true;
                }
                StartCoroutine(CountTime(2.0f,ClueCanvas02));
                break;
            case 3:
                ClueCanvas03.SetActive(true);//显示线索三文本
                if (isclue03 == false)
                {
                    cluecount++;
                    isclue03 = true;
                }
                StartCoroutine(CountTime(2.0f,ClueCanvas03));
                break;
            default:break;
        }
    }
    IEnumerator CountTime(float times,GameObject gameObject)
    {
        yield return new WaitForSeconds(times);
        gameObject.SetActive(false);
        
    }
    //延时两秒显示找齐线索画布
    IEnumerator later()
    {
        yield return new WaitForSeconds(3.0f);
        DrawPanel.SetActive(true);
    }

}
