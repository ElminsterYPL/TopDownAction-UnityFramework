using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Customs
{
    //参考方向(旋转45度视角后)
    public static readonly Vector3 Up = new Vector3(1, 0, 1).normalized;
    public static readonly Vector3 Down = new Vector3(-1, 0, -1).normalized;
    public static readonly Vector3 Left = new Vector3(-1, 0, 1).normalized;
    public static readonly Vector3 Right = new Vector3(1, 0, -1).normalized;
    public static readonly Vector3 UpRight = (Up + Right).normalized;
    public static readonly Vector3 UpLeft = (Up + Left).normalized;
    public static readonly Vector3 DownRight = (Down + Right).normalized;
    public static readonly Vector3 DownLeft = (Down + Left).normalized;
    
    /// <summary>
    /// 将屏幕坐标转换为基于一个中心点的世界坐标面向（y始终为0）。
    /// </summary>
    /// <param name="center">判定的中心。</param>
    /// <param name="screenPos"></param>
    public static Vector3 ScreenPosToFaceDir(Vector3 center, Vector3 screenPos)
    {
        var screenDir = (screenPos - center).normalized;
        var dir = new Vector3(screenDir.x, 0, screenDir.y).normalized;
        return dir;
    }

    /// <summary>
    /// 求出y平面的二维向量(x,0,z)逆时针方向旋转angel度后的新向量。
    /// </summary>
    /// <param name="vector3">初始向量。</param>
    /// <param name="angel">想要逆时针旋转的角度。当顺时针旋转时，angel为负。</param>
    /// <returns></returns>
    public static Vector3 RotateVectorOnY(Vector3 vector3, float angel)
    {
        return new Vector3(vector3.x * Mathf.Cos(angel) - vector3.z * Mathf.Sin(angel),
            0,
            vector3.z * Mathf.Cos(angel) + vector3.x * Mathf.Sin(angel));
        
        //另一种通过矩阵变换实现（基于该游戏固定旋转45度）：
        //return (Up * vector3.z + Right * vector3.x).normalized;
    }
    
}
