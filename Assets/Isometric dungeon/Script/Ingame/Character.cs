using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //캐릭터의 상태를 나타내는 열거형
    public enum State { IDLE, MOVE, ATTACK, DEAD }
    
    //현재 캐릭터의 상태를 저장하는 변수
    public State state;
    
    public Rigidbody2D rigid;

    //캐릭터의 이동 속도를 설정하는 변수, protected로 선언되어 상속받은 클래스에서 접근 가능
    [field: SerializeField] protected float speed = 3f;

    public SpriteRenderer image;
    public Animator animator;

    //최대 체력을 나타내는 변수, SerializeField로 인스펙터에서 수정 가능, 상속받은 클래스에서 접근 가능
    [field: SerializeField] public int MaxHealth { get; protected set; } = 5;
    
    //현재 체력을 나타내는 변수, SerializeField로 인스펙터에서 수정 가능하며, 상속받은 클래스에서 접근 가능
    [field: SerializeField] public int Health { get; protected set; } = 5;

    // 최대 스태미너를 나타내는 변수, SerializeField로 인스펙터에서 수정 가능하며, 상속받은 클래스에서 접근 가능
    [field: SerializeField] public float MaxStamina { get; protected set; } = 100;

    //현재 스태미너를 나타내는 변수, SerializeField로 인스펙터에서 수정 가능하며, 상속받은 클래스에서 접근 가능
    [field: SerializeField] public float Stamina { get; protected set; } = 100f;

    //캐릭터의 공격력을 나타내는 변수, SerializeField로 인스펙터에서 수정 가능하며, 상속받은 클래스에서 접근 가능
    [field: SerializeField] public int Power { get; protected set; } = 1;

    //캐릭터 초기화하는 매서드, 가상 메서드로 선언되어 하위 클래스에서 재정의 가능
    public virtual void Init()
    {
        //스테미너와 체력을 최대값으로 초기화
        Stamina = MaxStamina;
        Health = MaxHealth;
        //캐릭터 대기 상태로 설정
        Idle();
    }

    // 캐릭터를 대기 상태로 설정하는 메서드, 가상 메서드로 선언
    public virtual void Idle() { rigid.velocity = Vector2.zero;  }

    //캐릭터의 이동 처리 가상 메서드, 하위 클래스에서 재정의 가능
    //이 메서드는 하위 클래스에 구현
    public virtual void Movement(Vector2 _val)    {    }
    public virtual void Dead() { } //캐릭터 죽음
    public virtual void Attack() { } //캐릭터 공격

    //캐릭터 피해 데미지
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
