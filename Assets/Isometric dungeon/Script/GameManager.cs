using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [field: SerializeField] public GameObject PlayerPrefab { get; private set; }

    [HideInInspector] public Player Player { get; private set; }
    [field: SerializeField] public Spawner Spawner { get; private set; }
    public int IdleHash { get; private set; }
    public int MoveHash { get; private set; }
    public int AttackHash { get; private set; }
    public int DeadHash { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        IdleHash = Animator.StringToHash("Idle");
        MoveHash = Animator.StringToHash("Move");
        AttackHash = Animator.StringToHash("Attack");
        DeadHash = Animator.StringToHash("Dead");
    }
    public void Start()
    {
        Player = Instantiate(PlayerPrefab).GetComponent<Player>();
        InputManager.Instance.player = Player;
        MainCamera.Instance.followTarget = Player.transform;
    }

    public void GameStart()
    {
        GameStop();
        Player.Init();
        Spawner.Initialize();
        Spawner.SpawnObject();
        Spawner.SpawnEnemy();
    }

    public void GameStop()
    {
    }
}
