using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어 캐릭터를 정의하는 클래스
public class Player : Character
{
    //플레이어가 공격 중인지
    public bool isAttack;

    //UI 게이지
    public UIGauge uiGauge;

    //플레이어가 공격할 적 리스트
    public List<Enemy> attackEnemyList;
    
    //플레이어 점수
    public int score;

    public void Start()
    {
        //uiGauge가 설정되지 않는 경우, UIManager에서 UIGauge 컴포넌트를 찾아 설정
        if (uiGauge == null)
        {
            uiGauge = UIManager.Instance.transform.GetComponentInChildren<UIGauge>();
            //초기 Hp와 Stamina 값을 UI에 반영
            uiGauge.HPRefresh(Health, MaxHealth);
            uiGauge.StaminaRefresh(Stamina, MaxStamina);
        }
        //게임시작
        GameManager.Instance.GameStart();
    }

    public void Update()
    {
        //매 프레임마다 스태미나 회복(시간 비례)
        AddStamina(Time.deltaTime * 2.5f);
    }

    //캐릭터 초기화
    public override void Init()
    {
        base.Init(); //부모 클래스(Character)의 Init 메서드 호출
        transform.position = Vector2.zero;
        score = 0;

        //uiGauge가 설정되어 있으면 HP와 Stamina 값을 UI에 적용
        if (uiGauge != null)
        {
            uiGauge.HPRefresh(Health, MaxHealth);
            uiGauge.StaminaRefresh(Stamina, MaxStamina);
        }
    }

    //플레이어를 대기 상태로 전환
    public override void Idle()
    {
        base.Idle();

        //상태가 IDLE이 아니면 상태 전환 -> 애니메이션 재생
        if (state != State.IDLE)
        {
            state = State.IDLE;
            animator.Play(GameManager.Instance.IdleHash);
        }
    }

    //플레이어 이동 처리
    public override void Movement(Vector2 _val)
    {
        //상태가 ATTACK이나 DEAD이면 이동 처리 않함
        if (state == State.ATTACK || state == State.DEAD)
            return;

        base.Movement(_val); //부모 클래스의 Movement 메서드 호출

        //플레이어 방향 설정
        if (_val.x < 0f)
            transform.localScale = Vector3.one + Vector3.left * 2f;
        else if (_val.x > 0f)
            transform.localScale = Vector3.one;

        //이동속도 설정
        rigid.velocity = _val.normalized * speed;

        //상태가 MOVE가 아니면 상태 전환 -> 이동 애니메이션 재생
        if (state != State.MOVE)
        {
            state = State.MOVE;
            animator.Play(GameManager.Instance.MoveHash);
        }
    }

    //플레이어 공격 처리
    public override void Attack()
    {
        //이미 공격 애니메이션 재생 중이거나 스태미나가 부족하면 공격하지 않음
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == GameManager.Instance.AttackHash 
            || Stamina < 10)
        {
            return;
        }
        
        base.Attack(); //부모 클래스의 Attack 메서드를 호출
        rigid.velocity = Vector2.zero; //공격중에는 속도 0

        if (state != State.ATTACK)
        {
            state = State.ATTACK;
            animator.Play(GameManager.Instance.AttackHash);
        }

        //공격할 적 리스트를 배열로 변환
        Enemy[] attackArray = attackEnemyList.ToArray();

        //리스트에 있는 모든 적에게 데미지를 가함
        for (int i = 0; i < attackArray.Length; i++)
        {
            attackArray[i].Damaged(Power);
            //적의 체력이 0이하가 되면 리스트에서 제거하고 점수를 증가시킴
            if (attackArray[i].Health <= 0)
            {
                attackEnemyList.Remove(attackArray[i]);
                score++;
            }
        }
        //공격 후 스태미나 소모
        AddStamina(-10);        
    }

    //플레이어가 데미지를 받았을 떄 호출되는 메서드
    public override void Damaged(int _damage)
    {
        base.Damaged(_damage);
        uiGauge.HPRefresh(Health, MaxHealth);
    }

    //플레이어가 죽었을 때 호출되는 메서드
    public override void Dead()
    {
        if (state != State.DEAD)
        {
            state = State.DEAD;
            animator.Play(GameManager.Instance.DeadHash);
        }
    }

    //스태미나를 추가하는 메서드
    public void AddStamina(float _stamina)
    {
        Stamina += _stamina;

        //스태미나 최대치 초과 금지
        if (Stamina > MaxStamina)
            Stamina = MaxStamina;

        //스태미나 게이지 갱신
        uiGauge.StaminaRefresh(Stamina, MaxStamina);
    }

    //체력을 추가하는 메서드
    public void AddHp(int _hp)
    {
        Health += _hp;

        if (Health > MaxHealth)
            Health = MaxHealth;

        uiGauge.HPRefresh(Health, MaxHealth);
    }
}
