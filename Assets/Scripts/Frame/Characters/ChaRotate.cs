using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 处理角色的旋转。包含万向旋转和四象限旋转。
/// </summary>
[RequireComponent(typeof(ChaHandler))]
public class ChaRotate : MonoBehaviour
{
    /// <summary>
    /// 该帧的目标朝向。因为无旋转速度，所以执行完毕后立即清零。
    /// </summary>
    private Vector3 _targetFaceDir;
    
    private ChaHandler _chaHandler;

    private EAniDir _aniDir = EAniDir.DownRight;
    
    [Header("配置")] 
    public Animator sheetAnimator;
    public SpriteRenderer sheetSpriteRenderer;
    public GameObject faceDirGo;
    public GameObject aniDirGo;
    
    private void Start()
    {
        _chaHandler = this.GetComponent<ChaHandler>();
        
        if(!faceDirGo)
            Debug.LogWarning("该角色无FaceDir物体，无法处理万向旋转！");
        
        if(!aniDirGo)
            Debug.LogWarning("该角色无AniDir物体，无法处理固定向旋转！");
    }

    private void FixedUpdate()
    {
        //处理万向旋转
        HandleFaceDir();

        //处理动画旋转
        HandleAniDir();
        
        //暂时处理动画
        DoWithFaceDirAni();
    }

    void HandleFaceDir()
    {
        var nowFaceDir = _chaHandler.faceDir;
        if(_targetFaceDir == Vector3.zero || _targetFaceDir == nowFaceDir)
            return;
        
        var angel = GetStandardAngel(_targetFaceDir, nowFaceDir);
        faceDirGo.transform.Rotate(0, angel, 0);
        _chaHandler.faceDir = _targetFaceDir;
        _targetFaceDir = Vector3.zero;
    }

    void HandleAniDir()
    {
        //得到固定向的动画面向
        var newAniDir = FaceDirToAniDir(_chaHandler.faceDir);
        if(newAniDir == _aniDir)
            return;
        
        //都转换为真正的万向面向（FaceDir）
        var oldDir = AniDirToFaceDir(_aniDir);
        var newDir = AniDirToFaceDir(newAniDir);

        //计算需要旋转的角度并旋转
        var angel = GetStandardAngel(newDir, oldDir);
        aniDirGo.transform.Rotate(0,angel, 0);

        _aniDir = newAniDir;
    }
    
    #region public

    public void DoRotate(Vector3 dir)
    {
        _targetFaceDir = dir;
    }

    /// <summary>
    /// 将万向的角色面向转变为固定面向的动画面向。
    /// </summary>
    /// <param name="faceDir">角色面向</param>
    /// <returns></returns>
    public EAniDir FaceDirToAniDir(Vector3 faceDir)
    {
        var angel = GetStandardAngel(faceDir);
        //目前设定为“上、右”优先（通过判断条件的“=”等号来控制）。
        if (angel > 0 && angel < 90)
        {
            return EAniDir.DownLeft;
        }
        else if(angel >= 90 && angel < 180)
        {
            return EAniDir.UpLeft;
        }
        else if(angel <= 0 && angel > -90)
        {
            return EAniDir.DownRight;
        }
        else if(angel <= -90 && angel >= -180)
        {
            return EAniDir.UpRight;
        }
        throw new ArgumentOutOfRangeException("faceDir","无法将该faceDir转换为正确的AniDir");
    }

    /// <summary>
    /// 将固定面向的动画面向转变为万向的角色面向。
    /// </summary>
    /// <param name="aniDir"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Vector3 AniDirToFaceDir(EAniDir aniDir)
    {
        switch (aniDir)
        {
            case EAniDir.UpRight:
                return Customs.UpRight;
            case EAniDir.DownRight:
                return Customs.DownRight;
            case EAniDir.DownLeft:
                return Customs.DownLeft;
            case EAniDir.UpLeft:
                return Customs.UpLeft;
            default:
                throw new ArgumentOutOfRangeException(nameof(aniDir), aniDir, null);
        }
    }

    #endregion

    #region static

    /// <summary>
    /// 输入一个角色方向，得到基于InputHandler.Down计算的y角度。
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static float GetStandardAngel(Vector3 dir)
    {
        float angel;
        angel = Vector3.SignedAngle(Customs.Down, dir, Vector3.up);
        if (angel >= 180)
            angel = -180f;
        angel = Mathf.Clamp(angel, -180f, 179.99f);
        return angel;
    }

    /// <summary>
    /// 输入一个角色方向，得到基于某个方向向量计算的y角度。
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public static float GetStandardAngel(Vector3 dir, Vector3 from)
    {
        float angel;
        angel = Vector3.SignedAngle(from, dir, Vector3.up);
        if (angel >= 180)
            angel = -180f;
        angel = Mathf.Clamp(angel, -180f, 179.99f);
        return angel;
    }

    #endregion
    
    //TODO 测试。先在这里进行动画的控制。
    void DoWithFaceDirAni()
    {
        bool faceLeft, faceUp;
        
        switch (_aniDir)
        {
            case EAniDir.UpRight:
                faceLeft = false;
                faceUp = true;
                break;
            case EAniDir.DownRight:
                faceLeft = false;
                faceUp = false;
                break;
            case EAniDir.DownLeft:
                faceLeft = true;
                faceUp = false;
                break;
            case EAniDir.UpLeft:
                faceLeft = true;
                faceUp = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        sheetSpriteRenderer.flipX = !faceLeft;
        sheetAnimator.SetBool("FaceUp", faceUp);
    }
}

public enum EAniDir
{
    UpRight,
    DownRight,
    DownLeft,
    UpLeft,
}
