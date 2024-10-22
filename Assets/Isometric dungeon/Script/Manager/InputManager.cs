using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Singleton ���� ����Ͽ� �������� ������ �� �ִ� InputManager �ν��Ͻ�
    public static InputManager Instance { get; private set; }
    
    //���� �÷��̾� ĳ���� �����ϴ� ����
    [HideInInspector] public Player player;
    
    //����
    Vector2 dir;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        //�÷��̾ �������� �ʾ����� �Է��� ó������ ����
        if (player == null)
            return;

        //�Է� �ʱ�ȭ
        dir = Vector2.zero;

        //Ű����� ������
        if (Input.GetKey(KeyCode.W))
            dir += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            dir += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            dir += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            dir += Vector2.right;

        //���� �Է��� Movement�� ����
        player.Movement(dir);
        
        //�����̽��ٸ� ������ ���� ��� ����
        if (Input.GetKey(KeyCode.Space))
            player.Attack();
    }
}
