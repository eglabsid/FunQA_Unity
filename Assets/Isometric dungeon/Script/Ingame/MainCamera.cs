using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    //���� ī�޶� �̱���
    public static MainCamera Instance { get; private set; }
    public Transform followTarget; //Ÿ��

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Update()
    {
        //����ٴ� Ÿ�� ����
        if(followTarget != null)
            MoveCamera(followTarget);
    }
    //ī�޶� Ư�� Ÿ�� ��ġ�� �̵� ��Ŵ
    public void MoveCamera(Transform _target)
    {
        transform.position = _target.transform.position - (Vector3.forward * 100f);
    }
}
