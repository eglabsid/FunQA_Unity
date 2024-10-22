using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AgentUI : MonoBehaviour
{
    public TextMeshProUGUI stepValue; //에이전트의 현재 스탭 수를 표시
    public TextMeshProUGUI rewardValue; //에이전트의 누적 보상을 표시
    public TextMeshProUGUI timeValue;
    public TextMeshProUGUI episodeCountValue;
    private int episodeCount = 0;

    public void SetEpisodeCount(int _count)
    {
        episodeCountValue.text = _count.ToString();
        Debug.Log(_count);
    }

    //에이전트의 현재 스텝 수를 UI에 설정
    public void SetStepValue(int _value)
    {
        stepValue.text = _value.ToString(); //정수 값을 문자열로 변환하여 stepValue 텍스트에 설정
    }

    //에이전트의 누적 보상 값을 UI 설정
    public void SetRewardValue(float _value)
    {
        rewardValue.text = _value.ToString("N2"); //실수 값을 소수점 두 자리까지 문자열로 변환하여 rewardValue 텍스트에 설정
    }

    public void SetTimeValue(float _time)
    {
        timeValue.text = _time.ToString("N2") + " s";
    }
}
