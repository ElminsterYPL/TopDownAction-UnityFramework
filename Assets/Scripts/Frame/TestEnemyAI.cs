using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAI : MonoBehaviour
{
    private ChaHandler _player;
    private ChaHandler _chaHandler;

    private void Start()
    {
        _chaHandler = this.GetComponent<ChaHandler>();

        _player = GameObject.Find("Player").GetComponent<ChaHandler>();
        if (!_player)
        {
            Debug.LogError("未找到玩家");
        }
        
        _chaHandler.LearnSkill(DesignerTables.Skill.Data["waveWeaponAI"]);
    }

    private void FixedUpdate()
    {
        var moveDir = (_player.transform.position - this.transform.position).normalized;
        moveDir.y = 0; //避免出现y轴上的偏差导致指向箭头角度出错
        var moveSpeed = _chaHandler.moveSpeed;
        
        _chaHandler.OrderMove(moveSpeed * moveDir);
    }
    
    private void OnTriggerStay(Collider other)
    {
        ChaHandler chaHandler;
        if (other.TryGetComponent<ChaHandler>(out chaHandler) && chaHandler == _player)
        {
            //该Skill有CD的限制，所以检测到的每帧都尝试释放是无影响的。
            _chaHandler.CastSkill("waveWeaponAI");
        }
    }
}
