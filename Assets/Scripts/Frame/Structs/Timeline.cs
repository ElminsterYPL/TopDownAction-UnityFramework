using UnityEngine;

///<summary>
/// 拥有多个效果节点的Timeline。暂仅用于Skill的释放处理。
///</summary>
public class TimelineObj
{
    ///<summary>
    /// Timeline的基础信息。
    ///</summary>
    public TimelineModel Model;
    
    ///<summary>
    /// Timeline的焦点对象。通常为技能Timeline的创造者（释放者）。
    ///</summary>
    public GameObject Caster;

    ///<summary>
    /// 倍速，1=100%，最小值为0.1f。
    ///</summary>
    public float timeScale
    {
        get => _timeScale;
        set => _timeScale = Mathf.Max(0.100f, value);
    }
    private float _timeScale;

    ///<summary>
    /// Timeline的创建参数。技能创建时默认为skillObj。
    ///</summary>
    public object Param;

    ///<summary>
    /// Timeline累计运行时间。
    ///</summary>
    public float TimeElapsed = 0;

    public TimelineObj(TimelineModel model, GameObject caster, object param)
    {
        this.Model = model;
        this.Caster = caster;
        this._timeScale = 1.00f;
        this.Param = param;
    }
}

///<summary>
/// Timeline的Model。由策划填表所得。
///</summary>
public struct TimelineModel
{
    /// <summary>
    /// Timeline的ID。
    /// </summary>
    public string ID;

    ///<summary>
    /// Timeline的效果节点。
    ///</summary>
    public TimelineNode[] Nodes;
    
    /// <summary>
    /// Timeline的总“寿命”。到达后被销毁。
    /// </summary>
    public float Duration;

    public TimelineModel(string id, TimelineNode[] nodes, float duration)
    {
        this.ID = id;
        this.Nodes = nodes;
        this.Duration = duration;
    }
}

/// <summary>
/// Timeline效果节点。
/// </summary>
public struct TimelineNode
{
    ///<summary>
    /// Timeline运行多久之后发生。单位：秒。
    ///</summary>
    public float TriggerTime;

    ///<summary>
    /// 要执行的脚本函数.
    ///</summary>
    public TimelineEvent DoEvent;

    ///<summary>
    /// 要执行的函数的参数.
    ///</summary>
    public object[] eveParams{get;}

    public TimelineNode(float triggerTime, string doEve, params object[] eveArgs)
    {
        this.TriggerTime = triggerTime;
        this.DoEvent = DesignerScripts.Timeline.Functions[doEve];
        this.eveParams = eveArgs;
    }
}

public delegate void TimelineEvent(TimelineObj timeline, params object[] args);