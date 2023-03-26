using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    public ChaHandler player;

    private void Start()
    {
        player.LearnSkill(DesignerTables.Skill.Data["waveWeapon"]);
        player.LearnSkill(DesignerTables.Skill.Data["flash"]);
    }

    private void OnGUI()
    {
        var fontStyle = new GUIStyle
        {
            normal =
            {
                textColor = Color.black
            },
            fontSize = 30
        };
        GUI.Label(new Rect(0, 0, 200, 200), $"体力：{player.resource.Stamina}", fontStyle);
    }
}
