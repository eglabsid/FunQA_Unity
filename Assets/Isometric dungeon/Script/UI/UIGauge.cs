using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGauge : MonoBehaviour
{
    public Image hpGauge; //ü�� ������ ǥ��
    public Image staminaGauge; //���¹̳� �������� ǥ��

    //ü�� �������� ������Ʈ�ϴ� �޼���
    public void HPRefresh(float _hp, float _maxHp)
    {
        //ü�� ������ ����Ͽ� hpGauge�� fillAmount �Ӽ��� ����
        hpGauge.fillAmount = _hp / _maxHp; //_hp�� ���� ü��, _maxHp�� �ִ� ü��
    }

    //���¹̳� �������� ������Ʈ�ϴ� �޼���
    public void StaminaRefresh(float _stamina, float _maxStamina)
    {
        //���¹̳� ������ ����Ͽ� staminaGauge�� fillAmount �Ӽ��� ����
        staminaGauge.fillAmount = _stamina / _maxStamina; //_stamina�� ���� ���¹̳�, _maxStamina�� �ִ� ���¹̳�
    }
}
