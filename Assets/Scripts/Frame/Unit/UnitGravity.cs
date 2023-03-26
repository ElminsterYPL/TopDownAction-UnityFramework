using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有会受重力影响的单位需要挂载该组件。暂不受其他组件（如ChaHandler）的调控。
/// </summary>
public class UnitGravity : MonoBehaviour
{
    /// <summary>
    /// 该逻辑帧的通过重力获得的速度。每帧结算，结算后清零。
    /// </summary>
    private Vector3 _velocity;
    
    /// <summary>
    /// 该逻辑帧的重力加速度。仅在单位没有接触地面时生效。
    /// </summary>
    private Vector3 _acceleration;

    private const float GravityValue = -9.81f;
    
    private CharacterController _controller;

    private void Start()
    {
        _controller = this.GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        _velocity += _acceleration;
        if(_controller && _controller.isGrounded != true)
        {
            _acceleration.y += GravityValue * Time.fixedDeltaTime;
        }
        else
        {
            _acceleration = Vector3.zero;
        }

        //实际移动并归零速度。
        _controller.Move(_velocity * Time.fixedDeltaTime);
        _velocity = Vector3.zero;
    }
}
