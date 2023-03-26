using System;
using System.Collections.Generic;
using UnityEngine;
using DesignerConfigs;

/// <summary>
/// 角色处理器类。角色包括了一切可以移动、旋转、拥有资源和属性的单位，如玩家和一般敌人。
/// </summary>
[RequireComponent(typeof(UnitMove),
    typeof(ChaRotate), typeof(UnitGravity))]
public class ChaHandler : MonoBehaviour
{
    /// <summary>
    /// 角色当前面向。
    /// </summary>
    public Vector3 faceDir;

    /// <summary>
    /// 角色各项bool状态。包含能够奔跑、能否自然回复体力等。
    /// </summary>
    public ChaState state => _stateFromTimeline + _stateFromBuff;
    private ChaState _stateFromTimeline = new ChaState(true,true,true);
    private ChaState _stateFromBuff = new ChaState(true,true,true);
    
    /// <summary>
    /// 角色接受到的移动命令。每个逻辑帧传递给UnitMove执行，随后清零。既有方向、又有大小。单位为m/s。
    /// </summary>
    private Vector3 _moveOrder;

    /// <summary>
    /// 角色接受到的转向命令。每个逻辑帧传递给UnitRotate执行，随后清零。
    /// </summary>
    private Vector3 _rotateOrder;

    /// <summary>
    /// 角色所处阵营。不同阵营间才可以互相伤害。
    /// </summary>
    public int side;

    /// <summary>
    /// 角色的tag。可以由此判断角色类型（如“Player”）、或触发一些效果。
    /// </summary>
    public string[] tags = Array.Empty<string>();

    /// <summary>
    /// 角色是否在奔跑。
    /// </summary>
    public bool isRunning;

    /// <summary>
    /// 角色是否已经死亡。
    /// </summary>
    public bool isDead;

    /// <summary>
    /// 角色所拥有的技能。
    /// </summary>
    public List<SkillObj> Skills = new List<SkillObj>();

    /// <summary>
    /// 角色所拥有的buff。
    /// </summary>
    public List<BuffObj> Buffs = new List<BuffObj>();

    /// <summary>
    /// 角色真实移速。由属性中的MoveSpeed计算所得（目前暂设定为相同）。
    /// </summary>
    public float moveSpeed => property.MoveSpeed * 1.00f * (isRunning ? Running.RunningSpeedUpRate : 1.00f);

    /// <summary>
    /// 角色资源（HP、体力）。
    /// </summary>
    public ChaResource resource { get; private set; } = new ChaResource(20, 20, 20);

    /// <summary>
    /// 角色属性（由基础属性和其他途径（如装备）获得的属性计算所得）。
    /// </summary>
    public ChaProperty property => (_baseProperty + _buffAddProp + _equipmentProp) * _buffMultiplyProp;

    //角色基础属性（玩家加点培养或读表获得，暂随意设定）。
    private ChaProperty _baseProperty = new ChaProperty(3, 20, 20, 20, 10, 5, 4.5f);

    //来自buff的加值属性修正。
    private ChaProperty _buffAddProp = ChaProperty.AllZero;

    //来自buff的乘值属性修正（暂定为最后处理）。
    private ChaProperty _buffMultiplyProp = ChaProperty.AllZero;

    //来自装备的加值属性修正。
    private ChaProperty _equipmentProp = ChaProperty.AllZero;

    //各个组件。
    private UnitMove _unitMove;
    private ChaRotate _chaRotate;

    private void Start()
    {
        _unitMove = this.GetComponent<UnitMove>();
        _chaRotate = this.GetComponent<ChaRotate>();

        faceDir = new Vector3(-1, 0, -1).normalized;
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;
        var timePassed = Time.fixedDeltaTime;

        //处理角色资源。
        HandleRes(timePassed);

        //削减技能剩余CD。
        for (var i = 0; i < this.Skills.Count; i++)
        {
            if (Skills[i].Cooldown > 0)
            {
                Skills[i].Cooldown -= timePassed;
            }
        }

        //处理buff。
        var toRemove = new List<BuffObj>();
        for (var i = 0; i < this.Buffs.Count; i++)
        {
            if (Buffs[i].Permanent == false) Buffs[i].Duration -= timePassed;
            Buffs[i].TimeElapsed += timePassed;

            if (Buffs[i].Model.TickTime > 0 && Buffs[i].Model.OnTick != null)
            {
                //float取模不精准，所以用乘1000后的整数来取模判断。
                if (Mathf.RoundToInt(Buffs[i].TimeElapsed * 1000) % Mathf.RoundToInt(Buffs[i].Model.TickTime * 1000) ==
                    0)
                {
                    Buffs[i].Model.OnTick(Buffs[i]);
                    Buffs[i].Ticked += 1;
                }
            }

            //只要duration <= 0，不管是否是permanent都移除掉。
            if (Buffs[i].Duration <= 0 || Buffs[i].Stack <= 0)
            {
                if (Buffs[i].Model.OnRemoved != null)
                {
                    Buffs[i].Model.OnRemoved(Buffs[i]);
                }

                toRemove.Add(Buffs[i]);
            }
        }

        if (toRemove.Count > 0)
        {
            for (var i = 0; i < toRemove.Count; i++)
            {
                this.Buffs.Remove(toRemove[i]);
            }

            ReCheckBuffProp();
        }

        //处理移动。
        HandleMove();
        //处理旋转。
        HandleRotate();
    }

    #region private

    //处理角色资源。
    private void HandleRes(float timePassed)
    {
        var modSta = 0.00f;

        //体力相关
        //奔跑消耗体力
        if (isRunning)
        {
            modSta -= Running.RunningCostStaRate * timePassed;
        }

        //自然恢复体力（不在奔跑且能够自然恢复）
        if (!isRunning && state.CanRecoverSta)
        {
            modSta += property.StaRecoverRate * timePassed;
        }

        var modRes = new ChaResource(0, modSta);
        ModifyResources(modRes);
    }

    //处理移动。
    private void HandleMove()
    {
        if (!_unitMove || !state.CanMove)
            return;
        if (_moveOrder == Vector3.zero)
            return;
        _unitMove.DoMove(_moveOrder);
        //单摇杆。所以朝向与移动方向始终一致。
        OrderRotate(_moveOrder.normalized);
        _moveOrder = Vector3.zero;
    }

    //处理旋转。
    private void HandleRotate()
    {
        if (!_chaRotate)
            return;
        if (_rotateOrder == Vector3.zero)
            return;

        _chaRotate.DoRotate(_rotateOrder);
        _rotateOrder = Vector3.zero;
    }

    //重新计算buff对属性的修正。
    private void ReCheckBuffProp()
    {
        _buffAddProp.Zeroise();
        _buffMultiplyProp.Zeroise();
        _stateFromBuff.Initialize();
        for (var i = 0; i < Buffs.Count; i++)
        {
            _stateFromBuff += Buffs[i].Model.StateMod;
            
            var propMod = Buffs[i].Model.PropMod;
            if (propMod == null || propMod.Length == 0) continue;
            _buffAddProp += propMod[0] * Buffs[i].Stack;
            _buffMultiplyProp += propMod[1] * Buffs[i].Stack;
        }
    }

    #endregion

    #region public

    /// <summary>
    /// 命令角色移动。
    /// </summary>
    /// <param name="moveOrder">移动指令。该帧需要移动的距离（m/s）。</param>
    public void OrderMove(Vector3 moveOrder)
    {
        _moveOrder = moveOrder;
    }

    /// <summary>
    /// 命令角色旋转。
    /// </summary>
    /// <param name="rotateOrder">旋转指令。该帧的目标面向。以该角色为中心的单位向量。</param>
    /// <param name="isRawDir">是否是原始坐标（摄像机转动45度前）方向。默认为否。</param>
    public void OrderRotate(Vector3 rotateOrder, bool isRawDir = false)
    {
        //如果为原始坐标方向，则需要和摄像机一样，顺时针旋转45度。
        if (isRawDir)
            rotateOrder = Customs.RotateVectorOnY(rotateOrder, -45f);

        _rotateOrder = rotateOrder;
    }

    /// <summary>
    /// （从Timeline层面）总体设置角色状态。
    /// </summary>
    /// <param name="canMove"></param>
    /// <param name="canCastSkill"></param>
    /// <param name="canRecoverSta"></param>
    public void SetChaState(bool canMove, bool canCastSkill, bool canRecoverSta)
    {
        _stateFromTimeline.CanMove = canMove;
        _stateFromTimeline.CanCastSkill = canCastSkill;
        _stateFromTimeline.CanRecoverSta = canRecoverSta;
    }
    
    /// <summary>
    /// 修改角色资源。
    /// </summary>
    /// <param name="value">修正值。</param>
    public void ModifyResources(ChaResource value)
    {
        resource += value;
        resource.Hp = Mathf.Clamp(resource.Hp, 0, property.MaxHp);
        resource.Stamina = Mathf.Clamp(resource.Stamina, 0, property.MaxStamina);
        resource.Mp = Mathf.Clamp(resource.Mp, 0, property.MaxMp);
        if (resource.Hp <= 0)
        {
            Kill();
        }
    }

    /// <summary>
    /// 杀死这名角色。
    /// </summary>
    public void Kill()
    {
        isDead = true;

        //动画等的处理待做
    }

    /// <summary>
    /// 使角色学会一个技能。
    /// </summary>
    /// <param name="skillModel">技能Model</param>
    public void LearnSkill(SkillModel skillModel)
    {
        this.Skills.Add(new SkillObj(skillModel));

        if (skillModel.Buff != null)
        {
            for (var i = 0; i < skillModel.Buff.Length; i++)
            {
                var abi = skillModel.Buff[i];
                abi.Permanent = true;
                abi.Duration = 10;
                abi.DurationIsSetTo = true;
                this.ModifyBuff(abi);
            }
        }
    }

    /// <summary>
    /// 通过ID获得角色已学习的技能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SkillObj GetSkillById(string id)
    {
        for (var i = 0; i < Skills.Count; i++)
        {
            if (Skills[i].Model.ID == id)
            {
                return Skills[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 释放技能。
    /// </summary>
    /// <param name="id">需要释放的技能的ID</param>
    /// <returns></returns>
    public bool CastSkill(string id)
    {
        if (!state.CanCastSkill) return false;

        var skillObj = GetSkillById(id);
        if (skillObj == null || skillObj.Cooldown > 0)
        {
            return false;
        }

        var castSuccess = false;
        if (resource.Enough(skillObj.Model.Condition))
        {
            var timeline = new TimelineObj(skillObj.Model.Effect, this.gameObject, skillObj);

            //触发buff的onCast回调。
            for (var i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Model.OnCast != null)
                {
                    timeline = Buffs[i].Model.OnCast(Buffs[i], skillObj, timeline);
                }
            }

            //有时onCast回调会使timeline变为空（比如buff“沉默”），所以需要进行一次判空。
            if (timeline != null)
            {
                //消耗资源并创建Timeline实体。
                this.ModifyResources(-1 * skillObj.Model.Cost);
                TimelineManager.GetInstance().AddTimeline(timeline);
                castSuccess = true;
            }
        }

        //无论释放成功与否，都设定CD。如果无CD（CD为0），则设定为最小GCD。
        skillObj.Cooldown = Mathf.Max(skillObj.Model.CdSet, Miscs.Gcd);
        
        return castSuccess;
    }

    /// <summary>
    /// 判断该角色是否会被某次伤害杀死（以方便进行buff的触发）。
    /// </summary>
    /// <param name="dInfo"></param>
    /// <returns></returns>
    public bool CanBeKilledByDamageInfo(DamageInfo dInfo)
    {
        if (dInfo.IsHeal()) return false;
        var dValue = dInfo.DamageValue(false);
        return dValue >= this.resource.Hp;
    }

    /// <summary>
    /// 添加或删除buff（修改buff层数）。
    /// </summary>
    public void ModifyBuff(ModifyBuffInfo buff)
    {
        var bCaster = new List<GameObject>();
        if (buff.Caster) bCaster.Add(buff.Caster);
        var hasOnes = GetBuffById(buff.BuffModel.ID, bCaster);
        var modStack = Mathf.Min(buff.AddStack, buff.BuffModel.MaxStack);
        var toRemove = false;
        BuffObj toAddBuff;
        if (hasOnes.Count > 0)
        {
            //已经存在的情况。
            //因为限定caster只传入了buff.Caster，所以hasOnes数组最多只有一个对应的BuffObj。
            hasOnes[0].BuffParam = new Dictionary<string, object>();
            if (buff.BuffParam != null)
            {
                //通过传入的Buff进行参数更新。
                foreach (var kv in buff.BuffParam)
                {
                    hasOnes[0].BuffParam[kv.Key] = kv.Value;
                }
            }

            //重置或修正Duration。
            hasOnes[0].Duration = buff.DurationIsSetTo ? buff.Duration : (buff.Duration + hasOnes[0].Duration);

            //修改Buff层数。
            var afterAdd = hasOnes[0].Stack + modStack;
            modStack = afterAdd >= hasOnes[0].Model.MaxStack
                ? (hasOnes[0].Model.MaxStack - hasOnes[0].Stack)
                : (afterAdd <= 0 ? (0 - hasOnes[0].Stack) : modStack);
            hasOnes[0].Stack += modStack;
            hasOnes[0].Permanent = buff.Permanent;
            toAddBuff = hasOnes[0];
            toRemove = hasOnes[0].Stack <= 0;
        }
        else
        {
            //新建。
            toAddBuff = new BuffObj(
                buff.BuffModel,
                buff.Caster,
                this.gameObject,
                buff.Duration,
                buff.AddStack,
                buff.Permanent,
                buff.BuffParam
            );
            Buffs.Add(toAddBuff);
            //根据Priority进行排序。
            Buffs.Sort((a, b) => { return a.Model.Priority.CompareTo(b.Model.Priority); });
        }

        if (toRemove == false && buff.BuffModel.OnOccur != null)
        {
            buff.BuffModel.OnOccur(toAddBuff, modStack);
        }

        ReCheckBuffProp();
    }

    ///<summary>
    /// 获取角色对应的BuffObj。同一个BuffModel、施加者（caster）不同时，视为不同的BuffObj。
    ///</summary>
    public List<BuffObj> GetBuffById(string id, List<GameObject> caster = null)
    {
        var res = new List<BuffObj>();
        for (var i = 0; i < Buffs.Count; i++)
        {
            //如果给出了限定caster，则除了id相同外还需要caster至少符合限定caster中的一个。
            if (Buffs[i].Model.ID == id && (caster == null || caster.Count <= 0 || caster.Contains(Buffs[i].Caster)))
            {
                res.Add(Buffs[i]);
            }
        }

        return res;
    }

    #endregion
}