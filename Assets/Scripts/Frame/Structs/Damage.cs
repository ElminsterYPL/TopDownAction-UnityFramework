using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// 伤害、治疗等必须要走的Info逻辑。
///</summary>
public class DamageInfo
{
    ///<summary>
    /// 造成伤害的攻击者。可以为null。
    ///</summary>
    public GameObject Attacker;

    ///<summary>
    /// 本次伤害/治疗的受击者。不能为null。
    ///</summary>
    public GameObject Defender;

    ///<summary>
    /// 伤害Info类型Tag。
    ///</summary>
    public DamageInfoTag[] Tags;

    ///<summary>
    /// 伤害值（治疗值）。
    ///</summary>
    public Damage Damage;

    ///<summary>
    /// 暴击率。
    ///</summary>
    public float CriticalRate;

    ///<summary>
    /// 命中率。
    ///</summary>
    public float HitRate = 2.00f;

    ///<summary>
    /// 伤害/治疗完毕后，给受击角色添加的buff。
    ///</summary>
    public List<ModifyBuffInfo> AddBuffs = new List<ModifyBuffInfo>();

    public DamageInfo(GameObject attacker, GameObject defender, Damage damage, float baseCriticalRate, DamageInfoTag[] tags)
    {
        this.Attacker = attacker;
        this.Defender = defender;
        this.Damage = damage;
        this.CriticalRate = baseCriticalRate;
        this.Tags = new DamageInfoTag[tags.Length];
        for (var i = 0; i < tags.Length; i++)
        {
            this.Tags[i] = tags[i];
        }
    }

    ///<summary>
    /// 根据策划脚本计算最终伤害
    ///</summary>
    public float DamageValue(bool asHeal)
    {
        //return DesignerScripts.CommonScripts.DamageValue(this, asHeal);
        return this.Damage.Overall(asHeal);
    }

    ///<summary>
    ///根据tag判断本次“伤害”是否是一次治疗。具体由策划定义。
    ///</summary>
    public bool IsHeal()
    {
        for (var i = 0; i < this.Tags.Length; i++)
        {
            if (Tags[i] == DamageInfoTag.DirectHeal || Tags[i] == DamageInfoTag.PeriodHeal)
            {
                return true;
            }
        }
        return false;
    }
    
}

///<summary>
/// 伤害Struct
///</summary>
public struct Damage
{
    public float Normal;

    public Damage(float normal)
    {
        this.Normal = normal;
    }

    ///<summary>
    /// 统计规则。返回int，但具体数值还需要经过策划公式。暂定为一致。
    ///</summary>
    public float Overall(bool asHeal = false)
    {
        return (asHeal == false) ? Mathf.Max(0, Normal) : Mathf.Min(0, Normal);
    }

    public static Damage operator +(Damage a, Damage b)
    {
        return new Damage(
            a.Normal + b.Normal
        );
    }
    public static Damage operator *(Damage a, float b)
    {
        return new Damage(
            a.Normal * b
        );
    }
}

///<summary>
/// 伤害类型Tag元素。
///</summary>
public enum DamageInfoTag
{
    //暂定测试
    DirectDamage = 0,   //直接伤害
    PeriodDamage = 1,   //间歇性伤害
    ReflectDamage = 2,  //反噬伤害
    DirectHeal = 10,    //直接治疗
    PeriodHeal = 11,    //间歇性治疗
}