using UnityEngine;

public class SlimeBattleState : EnemyState
{

    public Transform player;
    private Enemy_Slime enemy;
    public int moveDir;
    public SlimeBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Slime _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;


    }

    public override void Update()
    {
        base.Update();
        enemy.anim.SetFloat("MoveX", enemy.rb.velocity.x);
        enemy.anim.SetFloat("MoveY", enemy.rb.velocity.y);


        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().bounciness < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(enemy.attackState);
            }

        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > enemy.agroDistance)
                stateMachine.ChangeState(enemy.idleState);
        }

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;
        MoveTowardsPlayer();
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }
    private void MoveTowardsPlayer()
    {
        // Tính toán hướng di chuyển
        Vector2 direction = (player.position - enemy.transform.position).normalized;

        // Đặt vận tốc của kẻ địch
        enemy.rb.velocity = direction * enemy.moveSpeed;

        // Cập nhật hướng của kẻ địch
        if (enemy.rb.velocity.x < 0)
        {
            enemy.transform.localScale = new Vector3(-Mathf.Abs(enemy.transform.localScale.x), enemy.transform.localScale.y, enemy.transform.localScale.z);
        }
        else if (enemy.rb.velocity.x > 0)
        {
            enemy.transform.localScale = new Vector3(Mathf.Abs(enemy.transform.localScale.x), enemy.transform.localScale.y, enemy.transform.localScale.z);
        }
    }
}
