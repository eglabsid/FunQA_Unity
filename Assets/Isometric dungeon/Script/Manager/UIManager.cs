using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Singleton 패턴을 사용하기 위한 정적 인스턴스 변수
    public static UIManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
