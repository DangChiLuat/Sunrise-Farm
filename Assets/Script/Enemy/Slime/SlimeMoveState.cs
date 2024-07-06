using UnityEngine;

public class SlimeMoveState : SlimeGroundState
{
    private Vector2 targetPosition;
    public SlimeMoveState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
    }
    public override void Enter()
    {
        base.Enter();
        SetRandomTargetPosition();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        MoveTowardsTarget();
        if (Vector2.Distance(enemy.transform.position, targetPosition) < 0.1f)
        {
            SetRandomTargetPosition();
        }

    }
    private void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)enemy.transform.position).normalized;
        rb.velocity = direction * enemy.moveSpeed;
        enemy.anim.SetFloat("MoveX", rb.velocity.x);
        enemy.anim.SetFloat("MoveY", rb.velocity.y);

        // lat mat khi thay doi huong 
        if (rb.velocity.x < 0)
        {
            enemy.transform.localScale = new Vector3(-Mathf.Abs(enemy.transform.localScale.x), enemy.transform.localScale.y, enemy.transform.localScale.z);
        }
        else if (rb.velocity.x > 0)
        {
            enemy.transform.localScale = new Vector3(Mathf.Abs(enemy.transform.localScale.x), enemy.transform.localScale.y, enemy.transform.localScale.z);
        }
    }


    private void SetRandomTargetPosition()
    {
        Vector2 randomDirection = Random.insideUnitCircle * 2f;
        targetPosition = (Vector2)enemy.transform.position + randomDirection;
    }
}
