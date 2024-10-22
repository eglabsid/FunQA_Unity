using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //������ ȿ�� �� (HP�� Stamina�� ȸ���ϴ� ����)
    public float value = 0;

    //�������� ������ ��Ÿ���� ������
    public enum Type { HP, Stamina}
    public Type type;
    
    int mask;

    public void Awake()
    {
        //"Player"���̾��� �ε����� �����ͼ� mask ������ ����
        mask = LayerMask.NameToLayer("Player");
    }
    public void OnTriggerEnter2D(Collider2D _other)
    {
        //�浹�� ������Ʈ�� �÷��̾� ���̾ ���ϴ��� Ȯ��
        if (_other.gameObject.layer == mask)
        {
            //�������� Stamina Ÿ���̸� �÷��̾��� ���¹̳ʸ� ȸ��
            if(type == Type.Stamina)
                _other.gameObject.GetComponentInParent<Player>().AddStamina((int)value);
            //�������� HP Ÿ���̸� �÷��̾��� ü���� ȸ��
            else
                _other.gameObject.GetComponentInParent<Player>().AddHp((int)value);
            //������ ����� �ı�
            Destroy(gameObject);
        }
    }
}
