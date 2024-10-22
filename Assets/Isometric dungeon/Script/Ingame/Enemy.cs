using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Character Ŭ���� ���
public class Enemy : Character
{
    //���� �÷��̾ �����ϴ� �� ����� Collider��
    public Collider2D detectCollider; //����
    public Collider2D attackCollider; //���ݹ���
    public Collider2D searchCollider; //Ž������
    private int mask; //�÷��̾� ���̾� ����ũ�� ������ ����

    private Vector2 randomDirection; //���� �̵� ������ �����ϴ� ����

    private float randomMoveTime; //�����ð����� ���� �������� �̵��ϴ� Ÿ�̸�
    private float randomMoveInterval = 2f; //���� �̵� ������ ����
    private float randomMoveChance = 0.5f; //���� �̵��� Ȯ��

    private float lastAttackTime;
    private float attackCooldown = 2f;

    //�� AI�� �����ϴ� �޼���, Ÿ��(�÷��̾�) Transform�� �޾ƿ�
    public void AIStart(Transform _target)
    {
        //mask�� �ʱ�ȭ���� �ʾҴٸ�, �÷��̾� ���̾� ����ũ ����
        if(mask == 0)
            mask = 1 << LayerMask.NameToLayer("Player");
        
        //�� AI �ڷ�ƾ ����
        StartCoroutine(AICoroutine(_target));
    }

    //���� �׾��� �� ����Ǵ� �޼���, CharaterŬ������ Dead �޼��带 ������(�������̵�)
    public override void Dead()
    {
        //GameManager�� �� ī��带 ����
        GameManager.Instance.Spawner.enemyAmount--;
        
        //�� ����
        Destroy(gameObject);
    }
    //���� �ױ� ������ �� ����Ǵ� �޼���
    public void DeadStart()
    {
        //���� ���¸� DEAD�� ����
        state = State.DEAD;
       
        //���� �ӵ��� 0���� �����Ͽ� �������� ����
        rigid.velocity = Vector2.zero;
        
        //���� �ִϸ��̼� �۵�
        animator.Play(GameManager.Instance.DeadHash);
        
        //��� �ڷ�ƾ ����
        StopAllCoroutines();
    }
    //���� ������ ������ �� ����Ǵ� �޼���
    public void AttackStart()
    {
        if(Time.time > lastAttackTime + attackCooldown)
        {
            //���� �ִϸ��̼��� ���� �ƴҶ�, ���� �ִϸ��̼��� ���
            if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != GameManager.Instance.AttackHash)
                animator.Play(GameManager.Instance.AttackHash);

            //���¸� ATTACK���� ����
            state = State.ATTACK;

            //�ӵ��� 0���� �����Ͽ� �������� ����
            rigid.velocity = Vector2.zero;

            lastAttackTime = Time.time;
        }
        
    }
    //���� ���� ������ ó���ϴ� �޼���, Charater Ŭ������ Attack �޼��带 ������(�������̵�)
    public override void Attack()
    {
        //�θ� Ŭ������ Attack �޼��带 ȣ�� (�⺻ ���� ���� ����)
        base.Attack();
        
        //���� ������ �÷��̾ ���� ��, �÷��̾�� ���ظ� ����
        if (attackCollider.IsTouchingLayers(mask))
            GameManager.Instance.Player.GetComponent<Player>().Damaged(1);
    }

    public void ChasePlayer(Transform _target)
    {
        if(_target != null)
        {
            Vector2 direction = (_target.position - transform.position).normalized;
            rigid.velocity = direction * speed;

            if (direction.x < 0)
            {
                transform.localScale = Vector3.one + (Vector3.left * 2f);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
        else
        {
            Move(null);
        }
        
    }

    private void MoveTowardsPlayer(Vector2 playerPosition)
    {
        Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;
        rigid.velocity = direction*speed;

        if(direction.x < 0)
        {
            transform.localScale = Vector3.one + (Vector3.left * 2f);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    //���� �÷��̾ ���� �����̴� �޼���
    public void Move(Transform _target)
    {
        //���� �ִϸ��̼��� �̵� �ƴҶ�, �̵� �ִϸ��̼��� ���
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != GameManager.Instance.MoveHash)
            animator.Play(GameManager.Instance.MoveHash);
        
        //���¸� Move�� ����
        state = State.MOVE;
        
        if(_target != null)
        {
            //Ÿ���� ��ġ�� ���� �������� ������ ����
            Vector2 targetPos = Vector2.zero;
            if (transform.position.x < _target.position.x)
                targetPos = Vector2.left * 0.5f;
            else
                targetPos = Vector2.right * 0.5f;

            //Ÿ�� ���������� ���͸� ���
            Vector2 dir = (Vector2)(_target.position - transform.position) + targetPos;

            //���� Ÿ���� ���ʿ� ���� �� �������� ĳ���͸� ����
            if (dir.x < 0)
            {
                transform.localScale = Vector3.one + (Vector3.left * 2f);
            }
            //���� Ÿ���� �����ʿ� ������ ���������� ĳ���͸� ����
            else if (dir.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            //���� �������� ���� �̵���Ŵ
            rigid.velocity = dir.normalized * speed;
        }
        //Ÿ���� ���� ���, ���� �������� �̵�
        else
        {
            // Ÿ���� ���� ��쿡�� Ÿ���� ��ġ�� �̵� (���� �̵� ���ŵ�)
            Vector2 playerPosition = GameManager.Instance.Player.transform.position;
            Vector2 direction = (playerPosition - (Vector2)transform.position).normalized;

            if (direction.x < 0)
            {
                transform.localScale = Vector3.one + (Vector3.left * 2f);
            }
            else if (direction.x > 0)
            {
                transform.localScale = Vector3.one;
            }

            rigid.velocity = direction * speed; // Ÿ�� ��ġ�� �̵�
        }
    }
    //���� AI������ �����ϴ� �ڷ�ƾ, Ÿ��(�÷��̾�)�� �����ϰ� �����ϴ� ����
    public IEnumerator AICoroutine(Transform _target)
    {
        //���� ü���� 0 �̻��� ���� ������ �����
        while (Health > 0)
        {        
            //���� ���� ���� �÷��̾ ������ ���� ����
            if (detectCollider.IsTouchingLayers(mask))
            {
                AttackStart();
                yield return new WaitForSeconds(1f);
            }
            //Ž�� ���� ���� �÷��̾ ������ Ÿ���� ���� �̵�
            else if (searchCollider.IsTouchingLayers(mask))
            {
                ChasePlayer(_target);
                yield return new WaitForEndOfFrame();
            }
            // Ž�� �������� ����� ��쿡�� ���� �ð����� ����
            else
            {
                // �ֱ������� �÷��̾��� ��ġ�� Ȯ���ϰ� �� �������� �̵�
                Vector2 playerPosition = GameManager.Instance.Player.transform.position;

                if (Vector2.Distance(transform.position, playerPosition) > searchCollider.bounds.extents.magnitude)
                {
                    MoveTowardsPlayer(playerPosition);
                }
                else
                {
                    // Ž�� ������ �÷��̾ ���� ��� ���� �̵�
                    Move(null);
                }
                yield return new WaitForSeconds(2f); // 2�ʸ��� �÷��̾� ��ġ Ȯ��
            }
        }
    }

    //���� ���ظ� �Ծ��� �� ����Ǵ� �޼���, Charater Ŭ������ Damaged �޼��带 ������
    public override void Damaged(int _damage)
    {
        Health -= _damage;
        if (Health <= 0)
        {
            Health = 0;
            DeadStart();
        }
    }
}
