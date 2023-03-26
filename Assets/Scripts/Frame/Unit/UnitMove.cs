using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有需要移动的单位都需要挂载该组件。
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class UnitMove : MonoBehaviour
{
    /// <summary>
    /// 该单位是否可以移动。
    /// </summary>
    private bool _canMove;
    
    /// <summary>
    /// 该逻辑帧的速度。每帧结算，结算后清零。
    /// </summary>
    private Vector3 _velocity;

    private CharacterController _controller;
    
    private void Start()
    {
        _controller = this.GetComponent<CharacterController>();
        
        _canMove = true;
    }

    private void FixedUpdate()
    {
        if(_velocity == Vector3.zero || !_controller)
            return;
        
        //实际移动并归零速度。
        _controller.Move(_velocity * Time.fixedDeltaTime);
        _velocity = Vector3.zero;
    }

    #region public

    public void DoMove(Vector3 moveOrder)
    {
        if(_canMove == false)
            return;

        _velocity = moveOrder;
    }

    #endregion
}
