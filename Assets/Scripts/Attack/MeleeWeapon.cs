using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 近战武器类。由持有者释放Skill、被Timeline调用。
/// TODO：抽象出单次攻击除动画外的数据，包含：施加击退与否及其数值、施加硬直与否及其数值、命中特效、造成伤害（可能不是基于攻击者而是基于武器）等等，可以配置或调用。
/// </summary>
public class MeleeWeapon : MonoBehaviour
{
    [Header("绑点和配置")] 
    public Transform pointA; //武器端点1，绘制武器示意红线起点。
    public Transform pointB; //武器端点2，绘制武器示意红线终点。
    public LayerMask targetLayer; //攻击目标层级。
    public Transform[] rayPoints; //射线发射点。
    public GameObject particle;
    
    [Header("监视")] 
    public Dictionary<int, Vector3> DicLastPoints = new(); //存放上个位置信息。
    public List<Collider> hasCollide = new();   //存放已经命中过的碰撞体（避免二次结算）。
    public bool turnOnRay;
    
    [Header("数据配置")] 
    public float waveWeaponDuration = 0.5f; //挥剑持续时间。
    public Vector3 originPos, originRota;   //每次挥剑前的初始位置和旋转信息（规范化动画后不再需要，也不再需要tempFather）。
    public Vector3 endPos, endRota; //每次挥剑结束后（背在背上时）的位置和旋转信息。
    public GameObject weaponOnBack;

    private Animator _animator;
    private static readonly int Wave = Animator.StringToHash("Wave");

    private ChaHandler _caster;
    
    private void Start()
    {
        _animator = this.GetComponentInChildren<Animator>();
        _caster = this.GetComponentInParent<ChaHandler>();
        
        if (DicLastPoints.Count == 0)
        {
            for (var i = 0; i < rayPoints.Length; i++)
            {
                DicLastPoints.Add(rayPoints[i].GetHashCode(), rayPoints[i].position);
            }
        }

        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (transform.GetChild(0).gameObject.activeSelf == false)
            return;

        if (turnOnRay == false)
            return;

        var newA = pointA.position;
        var newB = pointB.position;
        Debug.DrawLine(newA, newB, Color.red, 1f);

        //发出检测射线并处理。
        CastHitRays(rayPoints);
    }

    #region public

    /// <summary>
    /// 挥动武器攻击。
    /// </summary>
    /// <param name="centerDir">角色武器攻击的中心位置（通常为角色当前面向）。</param>
    /// <param name="chaPos">角色位置。</param>
    public void WaveWeapon(Vector3 centerDir, Vector3 chaPos)
    {
        //背上与手中两把武器的显隐控制。
        var weapon = transform.GetChild(0).gameObject;
        weapon.SetActive(true);
        weaponOnBack.SetActive(false);
        
        // TODO 通过tempFather修正不是默认z轴朝前的动画。后续需要z轴为默认正面。
        weapon.transform.LookAt(chaPos + centerDir);

        // TODO 未规范，每次挥动武器前需要将武器位置归正到默认位置。
        weapon.transform.localPosition = originPos;
        weapon.transform.localEulerAngles = originRota;
        
        _animator.SetTrigger(Wave);

        ResetWeaponInfo();

        StartCoroutine(TurnOffWeapon(weapon));
    }

    #endregion

    #region private

    //挥动武器结束后，重置位置、处理背上与手上武器的显隐。
    IEnumerator TurnOffWeapon(GameObject weapon)
    {
        yield return new WaitForSeconds(waveWeaponDuration);
        
        //挥动完毕后激活背上的武器、失活真正的武器
        weapon.transform.localPosition = endPos;
        weapon.transform.localEulerAngles = endRota;
        weaponOnBack.SetActive(true);
        weapon.SetActive(false);
    }
    
    //发出检测射线并处理。
    private void CastHitRays(Transform[] points)
    {
        var raycastHits = new RaycastHit[5]; //基于可能命中的最大目标数量设置。测试暂定为5。
        for (var i = 0; i < points.Length; i++)
        {
            var nowPos = points[i];
            DicLastPoints.TryGetValue(nowPos.GetHashCode(), out var lastPos);
            
            //如果为挥动动画的第一帧，则返回默认值Vector3.zero。
            if (lastPos == Vector3.zero)
            {
                //初始化DicLastPoints中的对应点。
                DicLastPoints.Add(points[i].GetHashCode(), points[i].position);
                
            }
            else
            {
                //绘制预计命中蓝线。
                Debug.DrawRay(lastPos, nowPos.position - lastPos, Color.blue, 1f);

                //基于上一帧所在位置打出命中射线。
                var ray = new Ray(lastPos, nowPos.position - lastPos);
                Physics.RaycastNonAlloc(ray, raycastHits, Vector3.Distance(lastPos, nowPos.position), targetLayer,
                    QueryTriggerInteraction.Ignore);
                foreach (var hit in raycastHits)
                {
                    if (!hit.collider) continue;
                    if(hasCollide.Contains(hit.collider))  continue;
                
                    //命中处理。
                    hasCollide.Add(hit.collider);

                    //TODO 暂且这样简单地处理伤害。
                    var atk = _caster.property.Atk;
                    var def = hit.collider.GetComponent<ChaHandler>().property.Def;
                    var damage = atk - def;
                    DamageManager.GetInstance().DoDamage(_caster.gameObject, hit.collider.gameObject, 
                        new Damage(damage), 0, new DamageInfoTag[0]);

                    //击退（为受击者附加“持续向一个方向移动”的buff）。
                    var fightOffDir = (hit.collider.transform.position - _caster.transform.position).normalized;
                    var fightOffBuff = DesignerTables.Buff.Data["ConstantMove"];
                    fightOffBuff.OnTickParams = new object[2] { 0.3f, fightOffDir };
                    hit.collider.GetComponent<ChaHandler>().ModifyBuff( new ModifyBuffInfo(
                        fightOffBuff, null, null, 1, 0.1f, true, false, null));

                    //硬直（为受击者附加“一段时间内无法移动、攻击、自然恢复体力”的buff）。
                    
                    
                    //TODO 暂且这样简单地生成特效。
                    if(particle && damage > 0)
                    {
                        var go = Instantiate(particle, hit.point, Quaternion.identity);
                        Destroy(go, 3f);
                    }
                
                    break;
                }
            }

            if (nowPos.position != lastPos)
            {
                DicLastPoints[nowPos.GetHashCode()] = nowPos.position; //存入上个位置信息。
            }
        }
    }

    //重置武器记录点、命中记录等信息。
    private void ResetWeaponInfo()
    {
        //清除上次攻击记录的点位
        DicLastPoints.Clear();
        //清除已命中记录
        hasCollide = new();
    }
    
    #endregion
    
}