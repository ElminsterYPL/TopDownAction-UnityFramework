using System.Collections.Generic;
using UnityEngine;

namespace DesignerScripts
{
    public class Timeline
    {
        public static Dictionary<string, TimelineEvent> Functions = new Dictionary<string, TimelineEvent>()
        {
            { "WaveWeapon", WaveWeapon },
            { "SetWeaponRay", SetWeaponRay },
            { "SetFaceDirFromMouse", SetFaceDirFromMouse },
            { "ChangeCasterState", ChangeCasterState },
            { "MoveCasterFromFaceDir", MoveCasterFromFaceDir },
            { "SetCasterTrail", SetCasterTrail },
            { "ModifyBuffOnCaster", ModifyBuffOnCaster}
        };

        /// <summary>
        /// 开关与Caster的ChaHandler同级的TrailRenderer组件。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">共1个参数：
        /// [0]：开(true)或关(false)。
        /// </param>
        private static void SetCasterTrail(TimelineObj timeline, object[] args)
        {
            var trailRenderer = timeline.Caster.GetComponent<TrailRenderer>();
            if (trailRenderer)
            {
                trailRenderer.enabled = (bool)args[0];
            }
        }

        /// <summary>
        /// 使释放者瞬移其面向方向一小段距离。直接获取CharacterController调用Move()，无视角色state中canMove的状态。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">共1个参数：
        /// [0]：本次瞬移移动的距离。
        /// </param>
        private static void MoveCasterFromFaceDir(TimelineObj timeline, object[] args)
        {
            var casterController = timeline.Caster.GetComponent<CharacterController>();
            var dis = (float)args[0];
            var dir = timeline.Caster.GetComponent<ChaHandler>().faceDir;
            casterController.Move(dir * dis);
        }

        /// <summary>
        /// 改变Timeline释放者的可控状态。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">共2个参数：
        /// [0]：是否可以移动。
        /// [1]：是否可以释放技能。
        /// [2]：是否可以自然恢复体力。
        /// </param>
        private static void ChangeCasterState(TimelineObj timeline, object[] args)
        {
            timeline.Caster.GetComponent<ChaHandler>().SetChaState((bool)args[0], (bool)args[1], (bool)args[2]);
        }

        /// <summary>
        /// 将该timeline的caster角色的面向设定为当前鼠标的位置。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">无参数。</param>
        private static void SetFaceDirFromMouse(TimelineObj timeline, object[] args)
        {
            var caster = timeline.Caster.GetComponent<ChaHandler>();

            var dir = Customs.ScreenPosToFaceDir(new Vector3(Screen.width / 2, Screen.height / 2, 0),
                Input.mousePosition);
            caster.OrderRotate(dir, true);
        }

        /// <summary>
        /// 获取MeleeWeapon并调用其挥动武器。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">无参数。</param>
        private static void WaveWeapon(TimelineObj timeline, object[] args)
        {
            var caster = timeline.Caster;
            var faceDir = caster.GetComponent<ChaHandler>().faceDir;
            caster.GetComponentInChildren<MeleeWeapon>().WaveWeapon(faceDir, caster.transform.position);
        }

        /// <summary>
        /// 设置武器检测碰撞射线开或关。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">共1个参数：
        /// [0]bool：目标状态。true为开，false为关。
        /// </param>
        private static void SetWeaponRay(TimelineObj timeline, object[] args)
        {
            if (args.Length <= 0)
                return;

            var caster = timeline.Caster;
            caster.GetComponentInChildren<MeleeWeapon>().turnOnRay = (bool)args[0];
        }
        
        /// <summary>
        /// 修改timeline的caster上的buff。
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="args">共1个参数：
        /// [0]ModifyBuffInfo：修改buff信息。
        /// </param>
        private static void ModifyBuffOnCaster(TimelineObj timeline, params object[] args)
        {
            if (timeline.Caster && args.Length > 0)
            {
                var mbi = (ModifyBuffInfo)args[0];
                mbi.Caster = timeline.Caster;
                mbi.Target = timeline.Caster;
                var cs = timeline.Caster.GetComponent<ChaHandler>();
                if (cs)
                {
                    cs.ModifyBuff(mbi);
                }
            }
        }
    }
}