
/// <summary>
/// 技能Obj。角色一切非Unit组件能提供的功能都称为Skill。Skill会产生一个Timeline。
/// </summary>
public class SkillObj
{
    ///<summary>
    /// 技能的模板。创建于skillModel。运行中允许改变。
    ///</summary>
    public SkillModel Model;

    ///<summary>
    /// 冷却时间。
    ///</summary>
    public float Cooldown;

    public SkillObj(SkillModel model)
    {
        this.Model = model;
        this.Cooldown = 0;
    }
}

///<summary>
/// 技能Model。策划填表所得。SkillObj的一部分。
///</summary>
public struct SkillModel
{
    ///<summary>
    /// 技能的id。
    ///</summary>
    public string ID;

    ///<summary>
    /// 技能使用的条件。暂设定为消耗角色资源。
    ///</summary>
    public ChaResource Condition;

    ///<summary>
    /// 技能的消耗。成功之后会扣除这些资源。
    ///</summary>
    public ChaResource Cost;

    ///<summary>
    /// 技能的效果，即其创建的Timeline。
    ///</summary>
    public TimelineModel Effect;

    //<summary>
    // 学会技能的时赋予的被动buff。
    //</summary>
    public ModifyBuffInfo[] Buff;

    /// <summary>
    /// 设定的CD。每次释放技能后设置。如果小于GCD，则每次释放技能后SkillObj的Cooldown设定为GCD。
    /// </summary>
    public float CdSet;
    
    public SkillModel(string id, ChaResource cost, ChaResource condition, string effectTimeline, ModifyBuffInfo[] buff, float cdSet)
    {
        ID = id;
        Cost = cost;
        Condition = condition;
        Effect = DesignerTables.Timeline.Data[effectTimeline];
        Buff = buff;
        CdSet = cdSet;
    }
}
