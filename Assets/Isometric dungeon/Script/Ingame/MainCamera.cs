using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    //메인 카메라 싱글톤
    public static MainCamera Instance { get; private set; }
    public Transform followTarget; //타겟

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Update()
    {
        //따라다닐 타켓 설정
        if(followTarget != null)
            MoveCamera(followTarget);
    }
    //카메라를 특정 타켓 위치로 이동 시킴
    public void MoveCamera(Transform _target)
    {
        transform.position = _target.transform.position - (Vector3.forward * 100f);
    }
}
