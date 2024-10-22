using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾� ĳ���͸� �����ϴ� Ŭ����
public class Player : Character
{
    //�÷��̾ ���� ������
    public bool isAttack;

    //UI ������
    public UIGauge uiGauge;

    //�÷��̾ ������ �� ����Ʈ
    public List<Enemy> attackEnemyList;
    
    //�÷��̾� ����
    public int score;

    public void Start()
    {
        //uiGauge�� �������� �ʴ� ���, UIManager���� UIGauge ������Ʈ�� ã�� ����
        if (uiGauge == null)
        {
            uiGauge = UIManager.Instance.transform.GetComponentInChildren<UIGauge>();
            //�ʱ� Hp�� Stamina ���� UI�� �ݿ�
            uiGauge.HPRefresh(Health, MaxHealth);
            uiGauge.StaminaRefresh(Stamina, MaxStamina);
        }
        //���ӽ���
        GameManager.Instance.GameStart();
    }

    public void Update()
    {
        //�� �����Ӹ��� ���¹̳� ȸ��(�ð� ���)
        AddStamina(Time.deltaTime * 2.5f);
    }

    //ĳ���� �ʱ�ȭ
    public override void Init()
    {
        base.Init(); //�θ� Ŭ����(Character)�� Init �޼��� ȣ��
        transform.position = Vector2.zero;
        score = 0;

        //uiGauge�� �����Ǿ� ������ HP�� Stamina ���� UI�� ����
        if (uiGauge != null)
        {
            uiGauge.HPRefresh(Health, MaxHealth);
            uiGauge.StaminaRefresh(Stamina, MaxStamina);
        }
    }

    //�÷��̾ ��� ���·� ��ȯ
    public override void Idle()
    {
        base.Idle();

        //���°� IDLE�� �ƴϸ� ���� ��ȯ -> �ִϸ��̼� ���
        if (state != State.IDLE)
        {
            state = State.IDLE;
            animator.Play(GameManager.Instance.IdleHash);
        }
    }

    //�÷��̾� �̵� ó��
    public override void Movement(Vector2 _val)
    {
        //���°� ATTACK�̳� DEAD�̸� �̵� ó�� ����
        if (state == State.ATTACK || state == State.DEAD)
            return;

        base.Movement(_val); //�θ� Ŭ������ Movement �޼��� ȣ��

        //�÷��̾� ���� ����
        if (_val.x < 0f)
            transform.localScale = Vector3.one + Vector3.left * 2f;
        else if (_val.x > 0f)
            transform.localScale = Vector3.one;

        //�̵��ӵ� ����
        rigid.velocity = _val.normalized * speed;

        //���°� MOVE�� �ƴϸ� ���� ��ȯ -> �̵� �ִϸ��̼� ���
        if (state != State.MOVE)
        {
            state = State.MOVE;
            animator.Play(GameManager.Instance.MoveHash);
        }
    }

    //�÷��̾� ���� ó��
    public override void Attack()
    {
        //�̹� ���� �ִϸ��̼� ��� ���̰ų� ���¹̳��� �����ϸ� �������� ����
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == GameManager.Instance.AttackHash 
            || Stamina < 10)
        {
            return;
        }
        
        base.Attack(); //�θ� Ŭ������ Attack �޼��带 ȣ��
        rigid.velocity = Vector2.zero; //�����߿��� �ӵ� 0

        if (state != State.ATTACK)
        {
            state = State.ATTACK;
            animator.Play(GameManager.Instance.AttackHash);
        }

        //������ �� ����Ʈ�� �迭�� ��ȯ
        Enemy[] attackArray = attackEnemyList.ToArray();

        //����Ʈ�� �ִ� ��� ������ �������� ����
        for (int i = 0; i < attackArray.Length; i++)
        {
            attackArray[i].Damaged(Power);
            //���� ü���� 0���ϰ� �Ǹ� ����Ʈ���� �����ϰ� ������ ������Ŵ
            if (attackArray[i].Health <= 0)
            {
                attackEnemyList.Remove(attackArray[i]);
                score++;
            }
        }
        //���� �� ���¹̳� �Ҹ�
        AddStamina(-10);        
    }

    //�÷��̾ �������� �޾��� �� ȣ��Ǵ� �޼���
    public override void Damaged(int _damage)
    {
        base.Damaged(_damage);
        uiGauge.HPRefresh(Health, MaxHealth);
    }

    //�÷��̾ �׾��� �� ȣ��Ǵ� �޼���
    public override void Dead()
    {
        if (state != State.DEAD)
        {
            state = State.DEAD;
            animator.Play(GameManager.Instance.DeadHash);
        }
    }

    //���¹̳��� �߰��ϴ� �޼���
    public void AddStamina(float _stamina)
    {
        Stamina += _stamina;

        //���¹̳� �ִ�ġ �ʰ� ����
        if (Stamina > MaxStamina)
            Stamina = MaxStamina;

        //���¹̳� ������ ����
        uiGauge.StaminaRefresh(Stamina, MaxStamina);
    }

    //ü���� �߰��ϴ� �޼���
    public void AddHp(int _hp)
    {
        Health += _hp;

        if (Health > MaxHealth)
            Health = MaxHealth;

        uiGauge.HPRefresh(Health, MaxHealth);
    }
}
