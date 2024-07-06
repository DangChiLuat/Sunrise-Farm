using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundState
{

    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
     //   player.anim.SetFloat("MoveX", player.rb.velocity.x);
      //  player.anim.SetFloat("MoveY", player.rb.velocity.y);
        Player.instance.HandleMovement();


        if (player.rb.velocity.magnitude <=0.1 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);
    }


}
