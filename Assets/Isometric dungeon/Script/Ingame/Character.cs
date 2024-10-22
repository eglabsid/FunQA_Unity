using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //ĳ������ ���¸� ��Ÿ���� ������
    public enum State { IDLE, MOVE, ATTACK, DEAD }
    
    //���� ĳ������ ���¸� �����ϴ� ����
    public State state;
    
    public Rigidbody2D rigid;

    //ĳ������ �̵� �ӵ��� �����ϴ� ����, protected�� ����Ǿ� ��ӹ��� Ŭ�������� ���� ����
    [field: SerializeField] protected float speed = 3f;

    public SpriteRenderer image;
    public Animator animator;

    //�ִ� ü���� ��Ÿ���� ����, SerializeField�� �ν����Ϳ��� ���� ����, ��ӹ��� Ŭ�������� ���� ����
    [field: SerializeField] public int MaxHealth { get; protected set; } = 5;
    
    //���� ü���� ��Ÿ���� ����, SerializeField�� �ν����Ϳ��� ���� �����ϸ�, ��ӹ��� Ŭ�������� ���� ����
    [field: SerializeField] public int Health { get; protected set; } = 5;

    // �ִ� ���¹̳ʸ� ��Ÿ���� ����, SerializeField�� �ν����Ϳ��� ���� �����ϸ�, ��ӹ��� Ŭ�������� ���� ����
    [field: SerializeField] public float MaxStamina { get; protected set; } = 100;

    //���� ���¹̳ʸ� ��Ÿ���� ����, SerializeField�� �ν����Ϳ��� ���� �����ϸ�, ��ӹ��� Ŭ�������� ���� ����
    [field: SerializeField] public float Stamina { get; protected set; } = 100f;

    //ĳ������ ���ݷ��� ��Ÿ���� ����, SerializeField�� �ν����Ϳ��� ���� �����ϸ�, ��ӹ��� Ŭ�������� ���� ����
    [field: SerializeField] public int Power { get; protected set; } = 1;

    //ĳ���� �ʱ�ȭ�ϴ� �ż���, ���� �޼���� ����Ǿ� ���� Ŭ�������� ������ ����
    public virtual void Init()
    {
        //���׹̳ʿ� ü���� �ִ밪���� �ʱ�ȭ
        Stamina = MaxStamina;
        Health = MaxHealth;
        //ĳ���� ��� ���·� ����
        Idle();
    }

    // ĳ���͸� ��� ���·� �����ϴ� �޼���, ���� �޼���� ����
    public virtual void Idle() { rigid.velocity = Vector2.zero;  }

    //ĳ������ �̵� ó�� ���� �޼���, ���� Ŭ�������� ������ ����
    //�� �޼���� ���� Ŭ������ ����
    public virtual void Movement(Vector2 _val)    {    }
    public virtual void Dead() { } //ĳ���� ����
    public virtual void Attack() { } //ĳ���� ����

    //ĳ���� ���� ������
    public virtual void Damaged(int _damage)
    {
        Health -= _damage;
        if (Health <= 0)
        {
            Health = 0;
            Dead();
        }
    }
    
}
