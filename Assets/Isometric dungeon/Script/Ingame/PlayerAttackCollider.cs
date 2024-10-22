using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    //�÷��̾� ��ü�� �����ϴ� ����
    [field: SerializeField] Player player;
    int mask; //�� ĳ���͸� �ν��ϱ� ���� ���̾� ����ũ
    public void Awake()
    {
        mask = LayerMask.NameToLayer("Enemy");
    }
    public void OnTriggerEnter2D(Collider2D _other)
    {
        //�浹�� ������Ʈ�� Enemy ���̾ ���ϴ��� Ȯ��
        if (_other.gameObject.layer == mask)
            //�÷��̾��� ������ �� ����Ʈ�� �߰�
            player.attackEnemyList.Add(_other.GetComponentInParent<Enemy>());
    }
    public void OnTriggerExit2D(Collider2D _other)
    {
        //�浹�� ������Ʈ�� Enemy ���̾ ���ϴ��� Ȯ��
        if (_other.gameObject.layer == mask)
            //�÷��̾��� ������ �� ����Ʈ���� ����
            player.attackEnemyList.Remove(_other.GetComponentInParent<Enemy>());
    }
}
