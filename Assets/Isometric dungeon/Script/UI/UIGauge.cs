using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGauge : MonoBehaviour
{
    public Image hpGauge; //체력 게이지 표시
    public Image staminaGauge; //스태미나 게이지를 표시

    //체력 게이지를 업데이트하는 메서드
    public void HPRefresh(float _hp, float _maxHp)
    {
        //체력 비율을 계산하여 hpGauge의 fillAmount 속성에 설정
        hpGauge.fillAmount = _hp / _maxHp; //_hp는 현재 체력, _maxHp는 최대 체력
    }

    //스태미나 게이지를 업데이트하는 메서드
    public void StaminaRefresh(float _stamina, float _maxStamina)
    {
        //스태미나 비율을 계산하여 staminaGauge의 fillAmount 속성에 설정
        staminaGauge.fillAmount = _stamina / _maxStamina; //_stamina는 현재 스태미나, _maxStamina는 최대 스태미나
    }
}
