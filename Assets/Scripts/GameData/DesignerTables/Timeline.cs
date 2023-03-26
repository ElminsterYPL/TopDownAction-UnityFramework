using System.Collections.Generic;

namespace DesignerTables
{
    public class Timeline
    {
        public static Dictionary<string, TimelineModel> Data = new Dictionary<string, TimelineModel>()
        {
            //挥动近战武器。
            {
                "skill_WaveWeapon", new TimelineModel("skill_WaveWeapon", new[]
                {
                    new TimelineNode(0.00f, "ChangeCasterState", new object[] { false, false, false }),
                    new TimelineNode(0.00f, "WaveWeapon", null),
                    new TimelineNode(0.00f, "SetFaceDirFromMouse", null),
                    new TimelineNode(0.00f, "SetWeaponRay", new object[] { true }),
                    new TimelineNode(0.50f, "SetWeaponRay", new object[] { false }),
                    new TimelineNode(0.55f, "ChangeCasterState", new object[] { true, true, true }),
                }, 0.55f)
            },

            //玩家角色瞬身。
            {
                "skill_Flash", new TimelineModel("skill_Flash", new[]
                {
                    new TimelineNode(0.00f, "ChangeCasterState", new object[] { false, false, false }),
                    new TimelineNode(0.00f, "SetCasterTrail", new object[] { true }),
                    new TimelineNode(0.00f, "ModifyBuffOnCaster", new object[]
                    {
                        new ModifyBuffInfo(Buff.Data["ConstantMoveToFaceDir"], null, null, 1, 0.20f, true, false)
                    }),
                    new TimelineNode(0.00f, "ModifyBuffOnCaster", new object[]
                    {
                        new ModifyBuffInfo(Buff.Data["CantBeHurt"], null, null, 1, 0.20f, true, false)
                    }),
                    new TimelineNode(0.20f, "SetCasterTrail", new object[] { false }),
                    new TimelineNode(0.20f, "ChangeCasterState", new object[] { true, true, true }),
                }, 0.20f)
            },
            
            //AI挥动近战武器。(无面向指向鼠标的调整)
            {
                "skill_WaveWeaponAI", new TimelineModel("skill_WaveWeaponAI", new[]
                {
                    new TimelineNode(0.00f, "ChangeCasterState", new object[] { false, false, false }),
                    new TimelineNode(0.00f, "WaveWeapon", null),
                    new TimelineNode(0.00f, "SetWeaponRay", new object[] { true }),
                    new TimelineNode(0.50f, "SetWeaponRay", new object[] { false }),
                    new TimelineNode(0.55f, "ChangeCasterState", new object[] { true, true, true }),
                }, 0.55f)
            },
        };
    }
}