using System.Collections.Generic;

namespace DesignerTables
{
    
    public class Skill{
        public static Dictionary<string, SkillModel> Data = new Dictionary<string, SkillModel>()
        {
            //挥动武器
            {"waveWeapon", new SkillModel("waveWeapon", new ChaResource(0, 3), 
                new ChaResource(0, 3), "skill_WaveWeapon", null, 0)},
            //角色瞬身
            {"flash", new SkillModel("flash", new ChaResource(0, 5), 
                new ChaResource(0, 5), "skill_Flash", null, 0.5f)},
            //挥动武器，不带跟随鼠标、而是攻击面向方向。无任何消耗但有CD。用于测试敌人AI。
            {"waveWeaponAI", new SkillModel("waveWeaponAI", ChaResource.Null, 
                ChaResource.Null, "skill_WaveWeaponAI", null, 2f)},
        };
    }
}