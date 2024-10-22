using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//���� ������ ������ �����ϱ� ���� ScriptableObject Ŭ����
public class LevelData : ScriptableObject
{
    public string LevelName; //���� ������
    public List<TileData> baseTileDatas; //������ �⺻ Ÿ�� �����͸� �����ϴ� ����Ʈ
    public List<TileData> obstacleTileDatas; //������ ��ֹ� Ÿ�� �����͸� �����ϴ� ����Ʈ
}

//������ Ÿ�Ͽ� ���� ������ ��� ���� Ŭ����
[System.Serializable]
public class TileData
{
    public Vector3Int Position; //Ÿ�� ��ġ
    public int tileType; //Ÿ���� Ÿ��
    public int tileNum; //Ư�� Ÿ���� �ĺ��ϱ� ���� Ÿ�� ��ȣ
}
