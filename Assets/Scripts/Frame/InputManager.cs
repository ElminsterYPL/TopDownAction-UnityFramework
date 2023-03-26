using UnityEngine;

/// <summary>
/// 玩家输入控制
/// </summary>
public class InputManager : SingletonMono<InputManager>
{
    public ChaHandler player;
    
    private void FixedUpdate()
    {
        var speed = player.moveSpeed;
        
        var inputW = Input.GetKey(KeyCode.W);
        var inputA = Input.GetKey(KeyCode.A);
        var inputS = Input.GetKey(KeyCode.S);
        var inputD = Input.GetKey(KeyCode.D);
        var inputShift = Input.GetKey(KeyCode.LeftShift);
        var inputMouseLeft = Input.GetKey(KeyCode.Mouse0);
        //var inputMouseRight = Input.GetKey(KeyCode.Mouse1);
        var inputSpace = Input.GetKey(KeyCode.Space);

        if (inputW && inputA)
        {
            player.OrderMove(Vector3.forward * speed);
        }
        else if (inputW && inputD)
        {
            player.OrderMove(Vector3.right * speed);
        }
        else if (inputS && inputA)
        {
            player.OrderMove(Vector3.left * speed);
        }
        else if (inputS && inputD)
        {
            player.OrderMove(Vector3.back * speed);
        }
        else if(inputW)
        {
            player.OrderMove(Customs.Up * speed);
        }
        else if (inputS)
        {
            player.OrderMove(Customs.Down * speed);
        }
        else if (inputD)
        {
            player.OrderMove(Customs.Right * speed);
        }
        else if (inputA)
        {
            player.OrderMove(Customs.Left * speed);
        }

        if (inputShift)
        {
            player.isRunning = true;
        }
        else
        {
            player.isRunning = false;
        }

        if (inputMouseLeft)
        {
            //进行近战攻击
            player.CastSkill("waveWeapon");
        }

        if (inputSpace)
        {
            player.CastSkill("flash");
        }

    }
}
