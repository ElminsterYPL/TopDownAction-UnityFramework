///<summary>
///角色资源类。包含HP，体力等。
///</summary>
public class ChaResource
{
    /// <summary>
    /// 当前生命。
    /// </summary>
    public float Hp;

    /// <summary>
    /// 当前体力。
    /// </summary>
    public float Stamina;

    /// <summary>
    /// 当前技能值（魔力）。
    /// </summary>
    public float Mp;

    public ChaResource(float hp, float stamina = 0, float mp = 0)
    {
        Hp = hp;
        Stamina = stamina;
        Mp = mp;
    }

    /// <summary>
    /// 对于requirement当前资源是否足够。
    /// </summary>
    public bool Enough(ChaResource requirement)
    {
        return (
            Hp >= requirement.Hp &&
            Stamina >= requirement.Stamina &&
            Mp >= requirement.Mp
        );
    }

    public static ChaResource operator +(ChaResource a, ChaResource b)
    {
        return new ChaResource(
            a.Hp + b.Hp,
            a.Stamina + b.Stamina,
            a.Mp + b.Mp
        );
    }

    public static ChaResource operator *(ChaResource a, float b)
    {
        return new ChaResource(
            a.Hp * b,
            a.Stamina * b,
            a.Mp * b
        );
    }

    public static ChaResource operator *(float a, ChaResource b)
    {
        return new ChaResource(
            b.Hp * a,
            b.Stamina * a,
            b.Mp * a
        );
    }

    public static ChaResource operator -(ChaResource a, ChaResource b)
    {
        return a + b * (-1);
    }

    public static ChaResource Null = new ChaResource(0);
}