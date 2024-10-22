using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AgentUI : MonoBehaviour
{
    public TextMeshProUGUI stepValue; //������Ʈ�� ���� ���� ���� ǥ��
    public TextMeshProUGUI rewardValue; //������Ʈ�� ���� ������ ǥ��
    public TextMeshProUGUI timeValue;
    public TextMeshProUGUI episodeCountValue;
    private int episodeCount = 0;

    public void SetEpisodeCount(int _count)
    {
        episodeCountValue.text = _count.ToString();
        Debug.Log(_count);
    }

    //������Ʈ�� ���� ���� ���� UI�� ����
    public void SetStepValue(int _value)
    {
        stepValue.text = _value.ToString(); //���� ���� ���ڿ��� ��ȯ�Ͽ� stepValue �ؽ�Ʈ�� ����
    }

    //������Ʈ�� ���� ���� ���� UI ����
    public void SetRewardValue(float _value)
    {
        rewardValue.text = _value.ToString("N2"); //�Ǽ� ���� �Ҽ��� �� �ڸ����� ���ڿ��� ��ȯ�Ͽ� rewardValue �ؽ�Ʈ�� ����
    }

    public void SetTimeValue(float _time)
    {
        timeValue.text = _time.ToString("N2") + " s";
    }
}
