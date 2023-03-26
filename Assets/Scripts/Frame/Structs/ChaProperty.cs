using UnityEngine;

///<summary>
///角色属性类。包含最大HP、体力上限、攻击力等。
///</summary>
public struct ChaProperty
{
    ///<summary>
    ///移动速度数值（可培养的纯数值值。需要通过公式转换为真正的移动速度（m/s））。
    ///</summary>
    public float MoveSpeed;

    ///<summary>
    ///最大生命。
    ///</summary>
    public float MaxHp;

    /// <summary>
    /// 最大体力。
    /// </summary>
    public float MaxStamina;

    /// <summary>
    /// 最大技能值（魔力）。
    /// </summary>
    public float MaxMp;

    ///<summary>
    ///攻击力。
    ///</summary>
    public float Atk;

    /// <summary>
    /// 防御力。
    /// </summary>
    public float Def;

    /// <summary>
    /// 每秒体力恢复量。
    /// </summary>
    public float StaRecoverRate;

    public ChaProperty(float moveSpeed, float maxHp = 0, float maxStamina = 0, float maxMp = 0, 
        float atk = 0, float def = 0, float staRecoverRate = 0)
    {
        MoveSpeed = moveSpeed;
        MaxHp = maxHp;
        MaxStamina = maxStamina;
        MaxMp = maxMp;
        Atk = atk;
        Def = def;
        StaRecoverRate = staRecoverRate;
    }

    public static ChaProperty AllZero = new ChaProperty(0);

    ///<summary>
    ///将所有值清0。
    ///</summary>
    public void Zeroise()
    {
        MoveSpeed = 0;
        MaxHp = 0;
        MaxStamina = 0;
        MaxMp = 0;
        Atk = 0;
        Def = 0;
        StaRecoverRate = 0;
    }

    //定义加法和乘法（乘法用于某些buff的乘法型增益，如“攻击力增加50%”，则直接与0.5相乘即可，内部会进行150%的乘值计算）。
    public static ChaProperty operator +(ChaProperty a, ChaProperty b)
    {
        return new ChaProperty(
            a.MoveSpeed + b.MoveSpeed,
            a.MaxHp + b.MaxHp,
            a.MaxStamina + b.MaxStamina,
            a.MaxMp + b.MaxMp,
            a.Atk + b.Atk,
            a.Def + b.Def,
            a.StaRecoverRate + b.StaRecoverRate
        );
    }

    public static ChaProperty operator *(ChaProperty a, ChaProperty b)
    {
        return new ChaProperty(
            a.MoveSpeed * (1.0000f + Mathf.Max(b.MoveSpeed, -0.9999f)),
            a.MaxHp * (1.0000f + Mathf.Max(b.MaxHp, -0.9999f)),
            a.MaxStamina * (1.0000f + Mathf.Max(b.MaxStamina, -0.9999f)),
            a.MaxMp * (1.0000f + Mathf.Max(b.MaxMp, -0.9999f)),
            a.Atk * (1.0000f + Mathf.Max(b.Atk, -0.9999f)),
            a.Def * (1.0000f + Mathf.Max(b.Def, -0.9999f)),
            a.StaRecoverRate * (1.0000f + Mathf.Max(b.StaRecoverRate, -0.9999f))
        );
    }

    public static ChaProperty operator *(ChaProperty a, float b)
    {
        return new ChaProperty(
            a.MoveSpeed * b,
            a.MaxHp * b,
            a.MaxStamina * b,
            a.MaxMp * b,
            a.Atk * b,
            a.Def * b,
            a.StaRecoverRate * b
        );
    }
}