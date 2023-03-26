using System.Collections.Generic;
using UnityEngine;

///<summary>
/// 每帧处理游戏中所有的DamageInfo。
///</summary>
public class DamageManager : SingletonMono<DamageManager>
{
    private List<DamageInfo> _damageInfos = new List<DamageInfo>();

    private void FixedUpdate() 
    {
        var i = 0;
        while( i < _damageInfos.Count)
        {
            DealWithDamage(_damageInfos[i]);
            //处理后即刻移除
            _damageInfos.RemoveAt(0);
        }
    }
    
    /// <summary>
    /// 每条DamageInfo（游戏伤害信息）的处理流程。
    /// </summary>
    /// <param name="dInfo"></param>
    private void DealWithDamage(DamageInfo dInfo)
    {
        if (!dInfo.Defender) return;
        var defenderCha = dInfo.Defender.GetComponent<ChaHandler>();
        if (!defenderCha) return;
        if (defenderCha.isDead) 
            return;
        
        ChaHandler attackerChaState = null;
        //触发攻击者OnHit。
        if (dInfo.Attacker)
        {
            attackerChaState = dInfo.Attacker.GetComponent<ChaHandler>();
            for (var i = 0; i < attackerChaState.Buffs.Count; i++)
            {
                if (attackerChaState.Buffs[i].Model.OnHit != null)
                {
                    attackerChaState.Buffs[i].Model.OnHit(attackerChaState.Buffs[i], ref dInfo, dInfo.Defender);
                }
            }
        }
        
        //触发防御者OnBeHurt。
        for (var i = 0; i < defenderCha.Buffs.Count; i++)
        {
            if (defenderCha.Buffs[i].Model.OnBeHurt != null)
            {
               defenderCha.Buffs[i].Model.OnBeHurt(defenderCha.Buffs[i], ref dInfo, dInfo.Attacker);
            }
        }
        
        if (defenderCha.CanBeKilledByDamageInfo(dInfo))
        {
            //如果受击者可能被杀死，则分别触发攻击者OnKill和受击者OnBeKilled。
            if (attackerChaState)
            {
                for (var i = 0; i < attackerChaState.Buffs.Count; i++)
                {
                    if (attackerChaState.Buffs[i].Model.OnKill != null)
                    {
                        attackerChaState.Buffs[i].Model.OnKill(attackerChaState.Buffs[i], dInfo, dInfo.Defender);
                    }
                }
            }
            for (var i = 0; i < defenderCha.Buffs.Count; i++)
            {
                if (defenderCha.Buffs[i].Model.OnBeKilled != null)
                {
                    defenderCha.Buffs[i].Model.OnBeKilled(defenderCha.Buffs[i], dInfo, dInfo.Attacker);
                }
            }
        }
        
        //处理最终的DamageInfo。
        var isHeal = dInfo.IsHeal();
        var dVal = dInfo.DamageValue(isHeal);
        defenderCha.ModifyResources(new ChaResource(-dVal));

        //伤害流程结束，添加本次流程需要附加的buff(包含攻击者和受击者)。
        for (var i = 0; i < dInfo.AddBuffs.Count; i++)
        {
            var toCha = dInfo.AddBuffs[i].Target;
            var toChaState = toCha.Equals(dInfo.Attacker) ? attackerChaState : defenderCha;

            if (toChaState && toChaState.isDead == false)
            {
                toChaState.ModifyBuff(dInfo.AddBuffs[i]);
            }
        }
        
    }

    /// <summary>
    /// 添加一个DamageInfo。
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="damage"></param>
    /// <param name="criticalRate"></param>
    /// <param name="tags"></param>
    public void DoDamage(GameObject attacker, GameObject target, Damage damage, float criticalRate, DamageInfoTag[] tags)
    {
        this._damageInfos.Add(new DamageInfo(attacker, target, damage, criticalRate, tags));
    }
}
