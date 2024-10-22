using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //�����۰� ���� ���� ������ �����ϴ� �迭
    [SerializeField] public SpawnInfo[] itemSpawnInfos;
    [SerializeField] public SpawnInfo[] enemySpawnInfos;

    //���� �ʿ� �����ϴ� ���� ���� �����ϴ� ������
    [HideInInspector]
    public int enemyAmount; //���� �����ִ� ���� ��
    [HideInInspector]
    public int createdEnemyAmount; //������ ���� �� ��

    //���� ���� Ŭ����, ������ ���� ������Ʈ�� �� ���� ����
    [System.Serializable]
    public class SpawnInfo
    {
        public GameObject go; //������ ������Ʈ
        public int minAmount = 5; //�ּ�
        public int maxAmount = 20; //�ִ�
    }

    //���� �ʱ�ȭ �޼���, �ڽ� ������Ʈ�� �����Ͽ� �ʱ� ���·� ����
    public void Initialize()
    {
        Transform[] childList = GetComponentsInChildren<Transform>();
        
        if (childList != null)
        {
            //���� Spawner�� �ڽ� ������Ʈ���� ��� ����
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }

    //�������� �����ϴ� �޼���
    public void SpawnObject()
    {
        //�� ������ �����ϴ� LeveManager �ν��Ͻ� ����
        var levelManager = LevelManager.Instance;

        //�� ������ ���� ������ ��ȸ�ϸ� ������ ����
        foreach (var itemInfo in itemSpawnInfos)
        {
            //������ ���� �� ����
            int amount = Random.Range(itemInfo.minAmount, itemInfo.maxAmount);
            
            for (int i = 0; i < amount; i++)
            {
                //��ֹ� ��ġ�� ���� �������� ������ ��ġ�� ã��
                while (true)
                {
                    int xPos = Random.Range(-levelManager.mapGrid.x/2, levelManager.mapGrid.x/2);
                    int yPos = Random.Range(-levelManager.mapGrid.y/2, levelManager.mapGrid.y/2);
                    var cellPos = new Vector3Int(xPos, yPos);

                    //���õ� ��ġ�� ��ֹ��� �ƴϸ� �������� ����
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

    //���� �����ϴ� �޼���
    public void SpawnEnemy()
    {
        var levelManager = LevelManager.Instance;
        createdEnemyAmount = enemyAmount = 0; //�ʱ�ȭ
        
        //�� �� ���� ������ ��ȸ�ϸ� �� ����
        foreach (var enemyInfo in enemySpawnInfos)
        {
            //�� ���� �� ����
            int amount = Random.Range(enemyInfo.minAmount, enemyInfo.maxAmount);
            
            for (int i = 0; i < amount; i++)
            {
                //��ֹ� ��ġ�� ���� ���� ������ ��ġ�� ã��
                while (true)
                {
                    int xPos = Random.Range(-levelManager.mapGrid.x / 2, levelManager.mapGrid.x / 2);
                    int yPos = Random.Range(-levelManager.mapGrid.y / 2, levelManager.mapGrid.y / 2);
                    var cellPos = new Vector3Int(xPos, yPos);

                    //���õ� ��ġ�� ��ֹ��� �ƴϸ� ���� ����
                    if (!levelManager.IsObstacleCell(cellPos))
                    {
                        var worldPos = levelManager.GetWorldPositionFromCellPosition(cellPos);
                        var enemy = Instantiate(enemyInfo.go, worldPos, Quaternion.identity, transform);
                        
                        //�� AI ����, �� �� ����
                        enemy.GetComponent<Enemy>().AIStart(GameManager.Instance.Player.transform);
                        enemyAmount++;
                        break;
                    }
                }
            }
        }
        createdEnemyAmount = enemyAmount; //������ �� �� ���
    }
}
