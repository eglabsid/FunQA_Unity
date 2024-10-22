using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    //플레이어 객체를 참조하는 변수
    [field: SerializeField] Player player;
    int mask; //적 캐릭터를 인식하기 위한 레이어 마스크
    public void Awake()
    {
        mask = LayerMask.NameToLayer("Enemy");
    }
    public void OnTriggerEnter2D(Collider2D _other)
    {
        //충돌한 오브젝트가 Enemy 레이어에 속하는지 확인
        if (_other.gameObject.layer == mask)
            //플레이어의 공격할 적 리스트에 추가
            player.attackEnemyList.Add(_other.GetComponentInParent<Enemy>());
    }
    public void OnTriggerExit2D(Collider2D _other)
    {
        //충돌한 오브젝트가 Enemy 레이어에 속하는지 확인
        if (_other.gameObject.layer == mask)
            //플레이어의 공격할 적 리스트에서 제거
            player.attackEnemyList.Remove(_other.GetComponentInParent<Enemy>());
    }
}
