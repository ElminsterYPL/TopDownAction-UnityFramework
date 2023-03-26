/// <summary>
/// 角色状态。通常由Timeline调控。
/// </summary>
public class ChaState
{
    /// <summary>
    /// 角色是否能够移动。
    /// </summary>
    public bool CanMove;
    /// <summary>
    /// 角色是否可以释放技能。
    /// </summary>
    public bool CanCastSkill;
    /// <summary>
    /// 是否能够自然回复体力。通常在消耗体力时无法自然回复体力。
    /// </summary>
    public bool CanRecoverSta;

    public ChaState(bool canMove, bool canCastSkill, bool canRecoverSta)
    {
        CanMove = canMove;
        CanCastSkill = canCastSkill;
        CanRecoverSta = canRecoverSta;
    }

    /// <summary>
    /// 初始化角色状态。
    /// </summary>
    public void Initialize()
    {
        CanMove = true;
        CanCastSkill = true;
        CanRecoverSta = true;
    }

    public static ChaState operator +(ChaState cs1, ChaState cs2)
    {
        return new ChaState(
            cs1.CanMove && cs2.CanMove,
            cs1.CanCastSkill && cs2.CanCastSkill,
            cs1.CanRecoverSta && cs2.CanRecoverSta
        );
    }
}
