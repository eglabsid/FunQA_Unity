using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //아이템과 적의 스폰 정보를 저장하는 배열
    [SerializeField] public SpawnInfo[] itemSpawnInfos;
    [SerializeField] public SpawnInfo[] enemySpawnInfos;

    //현재 맵에 존재하는 적의 수를 추적하는 변수들
    [HideInInspector]
    public int enemyAmount; //현재 살이있는 적의 수
    [HideInInspector]
    public int createdEnemyAmount; //생성된 적의 총 수

    //스폰 정보 클래스, 각각의 스폰 오브젝트와 그 양을 정의
    [System.Serializable]
    public class SpawnInfo
    {
        public GameObject go; //스폰할 오브젝트
        public int minAmount = 5; //최소
        public int maxAmount = 20; //최대
    }

    //스폰 초기화 메서드, 자식 오브젝트를 제거하여 초기 상태로 복원
    public void Initialize()
    {
        Transform[] childList = GetComponentsInChildren<Transform>();
        
        if (childList != null)
        {
            //현재 Spawner의 자식 오브젝트들을 모두 제거
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }

    //아이템을 스폰하는 메서드
    public void SpawnObject()
    {
        //맵 정보를 관리하는 LeveManager 인스턴스 참조
        var levelManager = LevelManager.Instance;

        //각 아이템 스폰 정보를 순회하며 아이템 스폰
        foreach (var itemInfo in itemSpawnInfos)
        {
            //아이템 스폰 수 램덤
            int amount = Random.Range(itemInfo.minAmount, itemInfo.maxAmount);
            
            for (int i = 0; i < amount; i++)
            {
                //장애물 위치를 피해 아이템을 스폰할 위치를 찾음
                while (true)
                {
                    int xPos = Random.Range(-levelManager.mapGrid.x/2, levelManager.mapGrid.x/2);
                    int yPos = Random.Range(-levelManager.mapGrid.y/2, levelManager.mapGrid.y/2);
                    var cellPos = new Vector3Int(xPos, yPos);

                    //선택된 위치가 장애물이 아니면 아이템을 스폰
                    if (!levelManager.IsObstacleCell(cellPos))
                    {
                        var worldPos = levelManager.GetWorldPositionFromCellPosition(cellPos);
                        Instantiate(itemInfo.go, worldPos, Quaternion.identity, transform);
                        break;
                    }
                }
            }
        }
    }

    //적을 스폰하는 메서드
    public void SpawnEnemy()
    {
        var levelManager = LevelManager.Instance;
        createdEnemyAmount = enemyAmount = 0; //초기화
        
        //각 적 스폰 정보를 순회하며 적 스폰
        foreach (var enemyInfo in enemySpawnInfos)
        {
            //적 스폰 수 랜덤
            int amount = Random.Range(enemyInfo.minAmount, enemyInfo.maxAmount);
            
            for (int i = 0; i < amount; i++)
            {
                //장애물 위치를 피해 적을 스폰할 위치를 찾음
                while (true)
                {
                    int xPos = Random.Range(-levelManager.mapGrid.x / 2, levelManager.mapGrid.x / 2);
                    int yPos = Random.Range(-levelManager.mapGrid.y / 2, levelManager.mapGrid.y / 2);
                    var cellPos = new Vector3Int(xPos, yPos);

                    //선택된 위치가 장애물이 아니면 적을 스폰
                    if (!levelManager.IsObstacleCell(cellPos))
                    {
                        var worldPos = levelManager.GetWorldPositionFromCellPosition(cellPos);
                        var enemy = Instantiate(enemyInfo.go, worldPos, Quaternion.identity, transform);
                        
                        //적 AI 시작, 적 수 증가
                        enemy.GetComponent<Enemy>().AIStart(GameManager.Instance.Player.transform);
                        enemyAmount++;
                        break;
                    }
                }
            }
        }
        createdEnemyAmount = enemyAmount; //생성된 적 수 기록
    }
}
