using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Singleton 패턴 사용하여 전역에서 접근할 수 있는 InputManager 인스턴스
    public static InputManager Instance { get; private set; }
    
    //현재 플레이어 캐릭터 참조하는 변수
    [HideInInspector] public Player player;
    
    //방향
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
        //플레이어가 설정되지 않았으면 입력을 처리하지 않음
        if (player == null)
            return;

        //입력 초기화
        dir = Vector2.zero;

        //키보드로 움직임
        if (Input.GetKey(KeyCode.W))
            dir += Vector2.up;
        if (Input.GetKey(KeyCode.S))
            dir += Vector2.down;
        if (Input.GetKey(KeyCode.A))
            dir += Vector2.left;
        if (Input.GetKey(KeyCode.D))
            dir += Vector2.right;

        //방향 입력을 Movement로 전달
        player.Movement(dir);
        
        //스페이스바를 누르면 공격 명력 실행
        if (Input.GetKey(KeyCode.Space))
            player.Attack();
    }
}
