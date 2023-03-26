using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    #region LayerMask
    
    /// <summary>
    /// 比较该layerMask是否含对应layer
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool CompareLayer(this LayerMask layerMask, int layer)
    {
        if((layerMask.value & (int)Mathf.Pow(2, layer)) == (int)Mathf.Pow(2, layer))
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Tranform

    /// <summary>
    /// 看向目标方向。默认初始方向为（0，1）。
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="dir"></param>
    public static void LookAt2D(this Transform transform, Vector2 dir)
    {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
        transform.rotation = rotation;
    }

    /// <summary>
    /// 看向目标方向
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="lookDir"></param>
    /// <param name="originFaceDir">初始正方向</param>
    public static void LookAt2D(this Transform transform, Vector2 lookDir, Vector2 originFaceDir)
    {
        Quaternion rotation = Quaternion.FromToRotation(originFaceDir, lookDir);
        transform.rotation = rotation;
    }

    #endregion

    #region GameObject

    /// <summary>
    /// 得到第一个标签为tag的子物体
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static GameObject FindChildWithTag(this GameObject gameObject, string tag)
    {
        var transf = gameObject.transform;
        if (transf.childCount == 0)
            return null;

        for (int i = 0; i < transf.childCount; i++)
        {
            if (transf.GetChild(i).CompareTag(tag))
                return transf.GetChild(i).gameObject;
            var target = transf.GetChild(i).gameObject.FindChildWithTag(tag);
            if (target != null)
                return target;
        }
        return null;
    }

    #endregion

}
