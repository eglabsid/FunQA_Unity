using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //아이템 효과 값 (HP나 Stamina를 회복하는 정도)
    public float value = 0;

    //아이템의 종류를 나타내는 열거형
    public enum Type { HP, Stamina}
    public Type type;
    
    int mask;

    public void Awake()
    {
        //"Player"레이어의 인덱스를 가져와서 mask 변수에 저장
        mask = LayerMask.NameToLayer("Player");
    }
    public void OnTriggerEnter2D(Collider2D _other)
    {
        //충돌한 오브젝트가 플레이어 레이어에 속하는지 확인
        if (_other.gameObject.layer == mask)
        {
            //아이템이 Stamina 타입이면 플레이어의 스태미너를 회복
            if(type == Type.Stamina)
                _other.gameObject.GetComponentInParent<Player>().AddStamina((int)value);
            //아이템이 HP 타입이면 플레이어의 체력을 회복
            else
                _other.gameObject.GetComponentInParent<Player>().AddHp((int)value);
            //아이템 사용후 파괴
            Destroy(gameObject);
        }
    }
}
