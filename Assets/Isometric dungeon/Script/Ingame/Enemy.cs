using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Character 클래스 상속
public class Enemy : Character
{
    //적이 플레이어를 감지하는 데 사용할 Collider들
    public Collider2D detectCollider; //공격
    public Collider2D attackCollider; //공격범위
    public Collider2D searchCollider; //탐색범위
    private int mask; //플레이어 레이어 마스크를 저장할 변수

    private Vector2 randomDirection; //램덤 이동 방향을 저장하는 변수

    private float randomMoveTime; //일정시간동안 랜덤 방향으로 이동하는 타이머
    private float randomMoveInterval = 2f; //랜덤 이동 사이의 간격
    private float randomMoveChance = 0.5f; //랜덤 이동할 확률

    private float lastAttackTime;
    private float attackCooldown = 2f;

    //적 AI를 시작하는 메서드, 타겟(플레이어) Transform을 받아옴
    public void AIStart(Transform _target)
    {
        //mask가 초기화되지 않았다면, 플레이어 레이어 마스크 설정
        if(mask == 0)
            mask = 1 << LayerMask.NameToLayer("Player");
        
        //적 AI 코루틴 시작
        StartCoroutine(AICoroutine(_target));
    }

    //적이 죽었을 때 실행되는 메서드, Charater클래스의 Dead 메서드를 재정의(오버라이드)
    public override void Dead()
    {
        //GameManager의 적 카운드를 줄임
        GameManager.Instance.Spawner.enemyAmount--;
        
        //적 삭제
        Destroy(gameObject);
    }
    //적이 죽기 시작할 때 실행되는 메서드
    public void DeadStart()
    {
        //적의 상태를 DEAD로 설정
        state = State.DEAD;
       
        //적의 속도를 0으로 설정하여 움직임을 멈춤
        rigid.velocity = Vector2.zero;
        
        //죽음 애니메이션 작동
        animator.Play(GameManager.Instance.DeadHash);
        
        //모든 코루틴 중지
        StopAllCoroutines();
    }
    //적이 공격을 시작할 때 실행되는 메서드
    public void AttackStart()
    {
        if(Time.time > lastAttackTime + attackCooldown)
        {
            //현재 애니메이션이 공격 아닐때, 공격 애니메이션을 재생
            if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != GameManager.Instance.AttackHash)
                animator.Play(GameManager.Instance.AttackHash);

            //상태를 ATTACK으로 설정
            state = State.ATTACK;

            //속도를 0으로 설정하여 움직임을 멈춤
            rigid.velocity = Vector2.zero;

            lastAttackTime = Time.time;
        }
        
    }
    //적의 공격 로직을 처리하는 메서드, Charater 클래스의 Attack 메서드를 재정의(오버라이드)
    public override void Attack()
    {
        //부모 클래스의 Attack 메서드를 호출 (기본 공격 로직 실행)
        base.Attack();
        
        //공격 범위에 플레이어가 있을 때, 플레이어에게 피해를 입힘
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

    //적이 플레이어를 향해 움직이는 메서드
    public void Move(Transform _target)
    {
        //현재 애니메이션이 이동 아닐때, 이동 애니메이션을 재생
        if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != GameManager.Instance.MoveHash)
            animator.Play(GameManager.Instance.MoveHash);
        
        //상태를 Move로 설정
        state = State.MOVE;
        
        if(_target != null)
        {
            //타겟의 위치에 따라 움직임의 방향을 결정
            Vector2 targetPos = Vector2.zero;
            if (transform.position.x < _target.position.x)
                targetPos = Vector2.left * 0.5f;
            else
                targetPos = Vector2.right * 0.5f;

            //타겟 방향으로의 벡터를 계산
            Vector2 dir = (Vector2)(_target.position - transform.position) + targetPos;

            //적이 타겟의 왼쪽에 있을 때 왼쪽으로 캐릭터를 돌림
            if (dir.x < 0)
            {
                transform.localScale = Vector3.one + (Vector3.left * 2f);
            }
            //적이 타겟의 오른쪽에 있을때 오른쪽으로 캐릭터를 돌림
            else if (dir.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            //계산된 방향으로 적을 이동시킴
            rigid.velocity = dir.normalized * speed;
        }
        //타켓이 없을 경우, 랜덤 방향으로 이동
        else
        {
            // 타겟이 없는 경우에도 타겟의 위치로 이동 (랜덤 이동 제거됨)
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

            rigid.velocity = direction * speed; // 타겟 위치로 이동
        }
    }
    //적의 AI로직을 실행하는 코루틴, 타켓(플레이어)을 추적하고 공격하는 로직
    public IEnumerator AICoroutine(Transform _target)
    {
        //적의 체력이 0 이상인 동안 루프를 계속함
        while (Health > 0)
        {        
            //공격 범위 내에 플레이어가 있으면 공격 시작
            if (detectCollider.IsTouchingLayers(mask))
            {
                AttackStart();
                yield return new WaitForSeconds(1f);
            }
            //탐색 범위 내에 플레이어가 있으면 타겟을 향해 이동
            else if (searchCollider.IsTouchingLayers(mask))
            {
                ChasePlayer(_target);
                yield return new WaitForEndOfFrame();
            }
            // 탐색 범위에서 벗어났을 경우에도 일정 시간동안 추적
            else
            {
                // 주기적으로 플레이어의 위치를 확인하고 그 방향으로 이동
                Vector2 playerPosition = GameManager.Instance.Player.transform.position;

                if (Vector2.Distance(transform.position, playerPosition) > searchCollider.bounds.extents.magnitude)
                {
                    MoveTowardsPlayer(playerPosition);
                }
                else
                {
                    // 탐색 범위에 플레이어가 없을 경우 랜덤 이동
                    Move(null);
                }
                yield return new WaitForSeconds(2f); // 2초마다 플레이어 위치 확인
            }
        }
    }

    //적이 피해를 입었을 때 실행되는 메서드, Charater 클래스의 Damaged 메서드를 재정의
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
