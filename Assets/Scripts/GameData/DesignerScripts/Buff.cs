using System.Collections.Generic;
using UnityEngine;

namespace DesignerScripts
{
    ///<summary>
    /// buff的各种回调事件
    ///</summary>
    public class Buff
    {
        public static Dictionary<string, BuffOnOccur> OnOccurFunc = new Dictionary<string, BuffOnOccur>()
        {
            
        };
        public static Dictionary<string, BuffOnRemoved> OnRemovedFunc = new Dictionary<string, BuffOnRemoved>(){
            
            
        };
        public static Dictionary<string, BuffOnTick> OnTickFunc = new Dictionary<string, BuffOnTick>()
        {
            {"MoveToFaceDir", MoveToFaceDir},
            {"Move", Move},
        };

        public static Dictionary<string, BuffOnCast> OnCastFunc = new Dictionary<string, BuffOnCast>()
        {
            
        };
        public static Dictionary<string, BuffOnHit> OnHitFunc = new Dictionary<string, BuffOnHit>()
        {
            
        };
        public static Dictionary<string, BuffOnBeHurt> BeHurtFunc = new Dictionary<string, BuffOnBeHurt>()
        {
            {"CantBeHurt", CantBeHurt},
        };

        public static Dictionary<string, BuffOnKill> OnKillFunc = new Dictionary<string, BuffOnKill>()
        {
            
        };
        public static Dictionary<string, BuffOnBeKilled> BeKilledFunc = new Dictionary<string, BuffOnBeKilled>()
        {
            
        };
        
        /// <summary>
        /// buff的携带者向其面向的方向移动。
        /// 共1个参数：
        /// [0]float：移动的距离。单位：m。
        /// </summary>
        /// <param name="buff"></param>
        private static void MoveToFaceDir(BuffObj buff)
        {
            var carrierChaCon = buff.Carrier.GetComponent<CharacterController>();
            var dir = buff.Carrier.GetComponent<ChaHandler>().faceDir;
            var dis = (float)buff.Model.OnTickParams[0];
            carrierChaCon.Move(dir * dis);
        }
        
        /// <summary>
        /// 无法受到伤害。
        /// 无参数。
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="attacker"></param>
        private static void CantBeHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
        {
            //暂时这样随意处理。（没有进行伤害tag、是否为治疗等判断）
            damageInfo.Damage.Normal = 0;
        }

        /// <summary>
        /// buff携带者向一个方向移动。
        /// 共2个参数：
        /// OnTickParams[0]：移动的距离。
        /// OnTickParams[1]：移动的方向。
        /// </summary>
        /// <param name="buff"></param>
        private static void Move(BuffObj buff)
        {
            var carrierChaCon = buff.Carrier.GetComponent<CharacterController>();
            var dir = (Vector3)buff.Model.OnTickParams[1];
            var dis = (float)buff.Model.OnTickParams[0];
            carrierChaCon.Move(dir * dis);
        }
        
    }
}