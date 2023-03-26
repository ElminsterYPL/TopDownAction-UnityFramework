using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
/// 用于修改buff的一条信息。
///</summary>
public struct ModifyBuffInfo
{
    ///<summary>
    /// buff的负责人（施加者）。可以为null。
    ///</summary>
    public GameObject Caster;

    ///<summary>
    /// buff添加的目标。不能为null。
    ///</summary>
    public GameObject Target;

    ///<summary>
    /// buff的model。
    ///</summary>
    public BuffModel BuffModel;

    ///<summary>
    /// 要修改的buff层数。正数为增加，负数则为减少。
    ///</summary>
    public int AddStack;

    ///<summary>
    /// duration是否为设置为逻辑。true代表设置为（直接改变为传入值），false代表正负修正值。
    ///</summary>
    public bool DurationIsSetTo;

    ///<summary>
    /// 是否是一个永久的buff。
    /// 如果为true，则在角色每帧管理时不会减少Duration。但buff即使是Permanent，当Duration小于等于0时也会被移除。
    ///</summary>
    public bool Permanent;

    ///<summary>
    /// buff持续时间。当该值小于等于0时，buff会被ChaHandler移除。
    ///</summary>
    public float Duration;

    ///<summary>
    /// buff的一些参数。例如一个护盾buff还能吸收多少伤害。
    ///</summary>
    public Dictionary<string, object> BuffParam;

    public ModifyBuffInfo(BuffModel model, GameObject caster, GameObject target,
        int stack, float duration, bool durationIsSetTo = true,bool permanent = false,
        Dictionary<string, object> buffParam = null)
    {
        this.BuffModel = model;
        this.Caster = caster;
        this.Target = target;
        this.AddStack = stack;
        this.Duration = duration;
        this.DurationIsSetTo = durationIsSetTo;
        this.BuffParam = buffParam;
        this.Permanent = permanent;
    }
}


///<summary>
/// 游戏中运行的、角色身上存在的buff。由ChaHandler自行管理。
///</summary>
public class BuffObj
{
    ///<summary>
    /// buff的model
    ///</summary>
    public BuffModel Model;

    ///<summary>
    ///buff持续时间
    ///</summary>
    public float Duration;

    ///<summary>
    /// 是否是一个永久的buff。
    ///</summary>
    public bool Permanent;

    ///<summary>
    /// 当前已叠加的层数。
    ///</summary>
    public int Stack;

    ///<summary>
    /// buff的释放者。可以为空。
    ///</summary>
    public GameObject Caster;

    ///<summary>
    /// buff的携带者。
    ///</summary>
    public GameObject Carrier;

    ///<summary>
    /// buff已经存在的时间。单位：秒。
    ///</summary>
    public float TimeElapsed = 0.00f;

    ///<summary>
    /// buff已经执行了多少次onTick。如果不执行onTick，则永远是0。
    ///</summary>
    public int Ticked = 0;

    ///<summary>
    /// buff的一些参数。例如一个护盾buff还能吸收多少伤害。
    ///</summary>
    public Dictionary<string, object> BuffParam = new Dictionary<string, object>();

    public BuffObj(BuffModel model, GameObject caster, GameObject carrier,  float duration, 
        int stack, bool permanent = false,
        Dictionary<string, object> buffParam = null)
    {
        this.Model = model;
        this.Caster = caster;
        this.Carrier = carrier;
        this.Duration = duration;
        this.Stack = stack;
        this.Permanent = permanent;
        if (buffParam != null) 
        {
            foreach(var kv in buffParam){
                this.BuffParam.Add(kv.Key, kv.Value);
            }
        }
    }
}

///<summary>
///buff的Model
///</summary>
public struct BuffModel
{
    ///<summary>
    /// buff的id
    ///</summary>
    public string ID;

    ///<summary>
    /// buff的名称
    ///</summary>
    public string Name;

    ///<summary>
    /// buff的优先级，优先级越低的buff越后执行。
    /// 必填，用于处理一些逻辑的矛盾。
    ///</summary>
    public int Priority;

    ///<summary>
    /// buff的最大堆叠层数。
    ///</summary>
    public int MaxStack;

    ///<summary>
    /// buff的tag。
    ///</summary>
    public string[] Tags;

    ///<summary>
    /// buff onTick的工作周期。单位：秒。
    /// 如果小于等于0则代表不会周期性工作，只要大于0，则最小值为Time.FixedDeltaTime。
    ///</summary>
    public float TickTime;

    ///<summary>
    /// buff对角色的属性修正。[0]为加减修正，[1]为乘法修正（不超过1的百分比）。
    ///</summary>
    public ChaProperty[] PropMod;
    
    /// <summary>
    /// buff对于角色状态的影响。
    /// </summary>
    public ChaState StateMod;

    ///<summary>
    /// buff被添加、层数改变时触发的事件。
    ///</summary>
    public BuffOnOccur OnOccur;
    ///<summary>
    /// buff被添加、层数改变时触发的事件的参数。
    ///</summary>
    public object[] OnOccurParams;

    ///<summary>
    /// buff在每个工作周期会执行的函数。
    ///</summary>
    public BuffOnTick OnTick;
    ///<summary>
    /// buff在每个工作周期会执行的函数的参数。
    ///</summary>
    public object[] OnTickParams;

    ///<summary>
    /// buffObj被移除前触发的事件。
    ///</summary>
    public BuffOnRemoved OnRemoved;
    ///<summary>
    /// buffObj被移除前触发的事件的参数。
    ///</summary>
    public object[] OnRemovedParams;

    ///<summary>
    /// 释放技能时buff触发的事件。
    ///</summary>
    public BuffOnCast OnCast;
    ///<summary>
    /// 释放技能时buff触发的事件的参数。
    ///</summary>
    public object[] OnCastParams;

    ///<summary>
    /// 伤害到其他角色时会触发的事件。
    ///</summary>
    public BuffOnHit OnHit;
    ///<summary>
    /// 伤害到其他角色时会触发的事件的参数。
    ///</summary>
    public object[] OnHitParams;

    ///<summary>
    /// 被其他角色伤害时会触发的事件。
    ///</summary>
    public BuffOnBeHurt OnBeHurt;
    ///<summary>
    /// 被其他角色伤害时会触发的事件的参数。
    ///</summary>
    public object[] OnBeHurtParams;

    ///<summary>
    /// 击杀其他角色时会触发的事件。
    ///</summary>
    public BuffOnKill OnKill;
    ///<summary>
    /// 击杀其他角色时会触发的事件的参数。
    ///</summary>
    public object[] OnKillParams;

    ///<summary>
    /// 被其他角色击杀时会触发的事件。
    ///</summary>
    public BuffOnBeKilled OnBeKilled;
    ///<summary>
    /// 被其他角色击杀时会触发的事件的参数。
    ///</summary>
    public object[] OnBeKilledParams;
    
    public BuffModel(
        string id, string name, string[] tags, int priority, int maxStack, float tickTime,
        string onOccur, object[] occurParam,
        string onRemoved, object[] removedParam,
        string onTick, object[] tickParam,
        string onCast, object[] castParam,
        string onHit, object[] hitParam,
        string beHurt, object[] hurtParam,
        string onKill, object[] killParam,
        string beKilled, object[] beKilledParam, 
        ChaProperty[] propMod = null,
        ChaState stateMod = null)
    {
        this.ID = id;
        this.Name = name;
        this.Tags = tags;
        this.Priority = priority;
        this.MaxStack = maxStack;
        this.TickTime = tickTime;
        
        this.PropMod = new ChaProperty[2]
        {
            ChaProperty.AllZero,
            ChaProperty.AllZero
        };
        if (propMod != null)
        {
            for (var i = 0; i < Mathf.Min(2, propMod.Length); i++)
            {
                PropMod[i] = propMod[i];
            }
        }

        if (stateMod != null)
        {
            StateMod = stateMod;
        }
        else
        {
            StateMod = new ChaState(true, true, true);
        }

        this.OnOccur = (onOccur == "") ? null : DesignerScripts.Buff.OnOccurFunc[onOccur];
        this.OnOccurParams = occurParam;
        this.OnRemoved = (onRemoved == "") ? null : DesignerScripts.Buff.OnRemovedFunc[onRemoved];
        this.OnRemovedParams = removedParam;
        this.OnTick = (onTick == "") ? null : DesignerScripts.Buff.OnTickFunc[onTick];
        this.OnTickParams = tickParam;
        this.OnCast = (onCast == "") ? null : DesignerScripts.Buff.OnCastFunc[onCast];
        this.OnCastParams = castParam;
        this.OnHit = (onHit == "") ? null : DesignerScripts.Buff.OnHitFunc[onHit];
        this.OnHitParams = hitParam;
        this.OnBeHurt = (beHurt == "") ? null: DesignerScripts.Buff.BeHurtFunc[beHurt];
        this.OnBeHurtParams = hurtParam;        
        this.OnKill = (onKill == "") ? null : DesignerScripts.Buff.OnKillFunc[onKill];
        this.OnKillParams = killParam;
        this.OnBeKilled = (beKilled == "") ? null : DesignerScripts.Buff.BeKilledFunc[beKilled];
        this.OnBeKilledParams = beKilledParam;
    }
}

public delegate void BuffOnOccur(BuffObj buff, int modifyStack);
public delegate void BuffOnRemoved(BuffObj buff);
public delegate void BuffOnTick(BuffObj buff);
public delegate void BuffOnHit(BuffObj buff, ref DamageInfo damageInfo, GameObject target);
public delegate void BuffOnBeHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker);
public delegate void BuffOnKill(BuffObj buff, DamageInfo damageInfo, GameObject target);
public delegate void BuffOnBeKilled(BuffObj buff, DamageInfo damageInfo, GameObject attacker);
public delegate TimelineObj BuffOnCast(BuffObj buff, SkillObj skill, TimelineObj timeline);
