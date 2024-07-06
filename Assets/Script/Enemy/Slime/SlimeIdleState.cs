using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeIdleState : SlimeGroundState
{
    public SlimeIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
        {
         //   enemy.anim.SetFloat("MoveX", enemy.rb.velocity.x);
         //   enemy.anim.SetFloat("MoveY", enemy.rb.velocity.x);
            stateMachine.ChangeState(enemy.moveState);

        }

    }
}