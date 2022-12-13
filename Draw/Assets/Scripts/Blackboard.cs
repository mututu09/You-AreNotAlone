using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    //画板尺寸
    private int m_TextureWidth;
    private int m_TextureHeight;

    //当前画板图片
    private Texture2D m_CurrentTexture;

    //当前编辑的颜色数组
    private Color32[] m_CurrentColors;

    //重置画板的颜色数组
    private Color[] m_CleanColorsArray;

    //当前画板的绘制颜色
    private Color m_CurrentBrushColor;

    //用于检测笔位置的Plane
    private Plane m_BoardPlane;

    public Plane BoardPlane
    {
        get
        {
            return m_BoardPlane;
        }
            
    }

    //先前拖拽的位置
    private Vector2 previous_drag_position;

    //用于获取HDRP Lit Shader主帖图的字段
    private static readonly int m_BaseColorMap = Shader.PropertyToID("_BaseColorMap");

    private void Awake()
    {
        m_BoardPlane = new Plane(transform.forward, transform.position);
        //获取画板尺寸
        var l_originTexture = GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
        m_TextureWidth = l_originTexture.width;
        m_TextureHeight = l_originTexture.height;
        var l_originColor = l_originTexture.GetPixels32();
        //新建一个纹理赋予材质
        m_CurrentTexture = new Texture2D(l_originTexture.width, l_originTexture.height);
        m_CurrentTexture.SetPixels32(l_originColor);
        m_CurrentTexture.Apply();
        //HDRP使用这种方式给材质设置纹理，普通项目使用注释掉的方法赋值
        //GetComponent<MeshRenderer>().material.mainTexture = m_CurrentTexture;
        GetComponent<MeshRenderer>().material.SetTexture(m_BaseColorMap, m_CurrentTexture);
    }

    public void DrawStop()
    {
        previous_drag_position = Vector2.zero;
    }

    public void UpdateDisplay(Vector3 _worldPoint, Vector2 _uvPoint, int _brushWidth, Color32 _brushColor)
    {
        Vector2 pixel_pos = UVToPixelCoordinates(_uvPoint);

        m_CurrentColors = m_CurrentTexture.GetPixels32();

        if (previous_drag_position == Vector2.zero)
        {
            // 如果这是我们第一次在该图像上拖动，只需在鼠标位置上为像素着色
            MarkPixelsToColour(pixel_pos, _brushWidth, _brushColor);
        }
        else
        {
            // 在上次更新呼叫所在的行中显示颜色
            ColourBetween(previous_drag_position, pixel_pos, _brushWidth, _brushColor);
        }

        ApplyMarkedPixelChanges();

        previous_drag_position = pixel_pos;
    }

    /// <summary>
    /// UV坐标转像素坐标
    /// </summary>
    private Vector2 UVToPixelCoordinates(Vector2 _vector2)
    {
        // 需要以我们的坐标为中心
        float centered_x = _vector2.x * m_TextureWidth;
        float centered_y = _vector2.y * m_TextureHeight;

        // 将当前鼠标位置四舍五入到最近的像素
        Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

        return pixel_pos;
    }
    /// <summary>
    /// 计算需要绘制的像素数量
    /// </summary>
    public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
    {
        //找出每个方向（x和y）需要着色的像素数量
        int center_x = (int)center_pixel.x;
        int center_y = (int)center_pixel.y;
        //int extra_radius = Mathf.Min(0, pen_thickness - 2);

        for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
        {
            // 检查X是否环绕图像，因此我们不在图像的另一侧绘制像素
            if (x >= m_TextureWidth || x < 0)
                continue;

            for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
            {
                MarkPixelToChange(x, y, color_of_pen);
            }
        }
    }

    /// <summary>
    /// 俩点之间插入过渡点
    /// </summary>
    private void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
    {
        // 获取从头到尾的距离
        float distance = Vector2.Distance(start_point, end_point);
        Vector2 direction = (start_point - end_point).normalized;

        Vector2 cur_position = start_point;

        // 根据自上次更新以来经过的时间，计算在start_point和end_point之间进行插值的次数
        float lerp_steps = 1 / distance;

        for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
        {
            cur_position = Vector2.Lerp(start_point, end_point, lerp);
            MarkPixelsToColour(cur_position, width, color);
        }
    }

    /// <summary>
    /// 修改像素数组信息
    /// </summary>
    public void MarkPixelToChange(int x, int y, Color color)
    {
        // 需要将x和y坐标转换为数组的平面坐标
        int array_pos = y * m_TextureHeight + x;

        // 检查这是一个有效的位置
        if (array_pos > m_CurrentColors.Length || array_pos < 0)
            return;

        m_CurrentColors[array_pos] = color;
    }

    /// <summary>
    /// 将新的像素数组赋值给纹理
    /// </summary>
    public void ApplyMarkedPixelChanges()
    {
        m_CurrentTexture.SetPixels32(m_CurrentColors);
        m_CurrentTexture.Apply();
    }

    /// <summary>
    /// 笔尖在画板正面还是背面
    /// </summary>
    /// <param name="_point">笔尖的位置</param>
    /// <returns>>当在正面的时候返回正值，当在背面的时候返回负值</returns>
    public bool GetSideOfBoardPlane(Vector3 _point)
    {
        return m_BoardPlane.GetSide(_point);
    }

    /// <summary>
    /// 笔尖距画板的距离
    /// </summary>
    /// <param name="_point"></param>
    /// <returns></returns>
    public float GetDistanceFromBoardPlane(Vector3 _point)
    {
        return m_BoardPlane.GetDistanceToPoint(_point);
    }

    /// <summary>
    /// 矫正后的笔尖应该在的位置
    /// </summary>
    /// <param name="point">笔尖的位置</param>
    /// <returns>矫正后的笔尖位置</returns>
    public Vector3 ProjectPointOnBoardPlane(Vector3 point)
    {
        float d = -Vector3.Dot(m_BoardPlane.normal, point - transform.position);
        return point + m_BoardPlane.normal * d;
    }
}

