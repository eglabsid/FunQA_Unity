using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public Vector2Int mapGrid; //���� ���� �׸��� ũ��
    public Vector2Int minGrid = new Vector2Int(30, 30); //�ּ�
    public Vector2Int maxGrid = new Vector2Int(50, 50); //�ִ�
    public int obstarclePercentage = 1; //��ֹ� ����(0-100%) - Ÿ�� �ʿ��� ��ֹ� ������ Ȯ��
    public string levelName; //���� ������ �̸��� ����

    //Ÿ���� ������ �����ϴ� ������
    public enum TileType
    {
        Base = 1, //�⺻ Ÿ��
        Obstacle = 2, //��ֹ� Ÿ��
    }

    //Ÿ�ϸʰ� �׿� ���õ� Ÿ���� �����ϴ� ����ü
    [System.Serializable]
    public struct TileMapValue
    {
        public Tilemap map; //Ÿ�ϸ� ����
        public List<Tile> tiles; //Ÿ�� ���
        public TileType type; //Ÿ���� ����(�⺻/��ֹ�)
    }

    //�⺻ Ÿ�ϸʰ� ���õ� ������
    public TileMapValue baseTile;
    //��ֹ� Ÿ�ϸʰ� ���õ� ������
    public TileMapValue obstacleTile;

    public void Awake()
    {
        Instance = this;
    }

    //���� ����
    public void CreateLevel()
    {
        obstarclePercentage = 0;

        //������ ��� Ÿ���� ����
        baseTile.map.ClearAllTiles();
        obstacleTile.map.ClearAllTiles();

        //�������� �� ũ�⸦ ����
        int xGrid = Random.Range(minGrid.x, maxGrid.x);
        int yGrid = Random.Range(minGrid.y, maxGrid.y);
        mapGrid = new Vector2Int(xGrid, yGrid);

        //�׸��� ���� ������ Ÿ���� ����
        for (int i = -xGrid / 2; i < xGrid / 2; i++)
        {
            for (int j = -yGrid / 2; j < yGrid / 2; j++)
            {
                //������ �⺻ Ÿ���� �ش� ��ġ�� ��ġ
                baseTile.map.SetTile(new Vector3Int(i, j, 0), baseTile.tiles[UnityEngine.Random.Range(0, baseTile.tiles.Count)]);

                //������ Ȯ���� ���� ��ֹ� Ÿ���� ��ġ�ϰų� ���� �����ڸ��� ��ֹ� ��ġ
                if (obstarclePercentage > Random.Range(0, 100) ||
                    ((i == -xGrid / 2 || i == (xGrid / 2) - 1) || (j == -yGrid / 2 || j == (yGrid / 2) - 1))) //set obstacle on edge)
                {
                    obstacleTile.map.SetTile(new Vector3Int(i, j, 0), obstacleTile.tiles[UnityEngine.Random.Range(0, obstacleTile.tiles.Count)]);
                }
            }
        }
    }

    //������ �����ϴ� �޼ҵ�
    public void SaveLevel()
    {
        //���ο� ���� �����͸� ScriptableObject�� ����
        var newLevel = ScriptableObject.CreateInstance<LevelData>();
        newLevel.LevelName = levelName + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        //���� Ÿ�ϸʿ��� Ÿ�� �����͸� �����Ͽ� ���� �����Ϳ� ����
        newLevel.baseTileDatas = GetTilesFromMap(baseTile).ToList();
        newLevel.obstacleTileDatas = GetTilesFromMap(obstacleTile).ToList();

        //json���� ��ȯ �� ����
        var json = JsonUtility.ToJson(newLevel, true);
        var sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append("/");
        sb.Append(newLevel.LevelName);
        sb.Append(".json");
        var file = File.CreateText(sb.ToString());
        file.WriteLine(json);
        file.Close();

        //Ÿ�ϸʿ��� Ÿ�� ������ �����ϴ� ���� �ż���
        IEnumerable<TileData> GetTilesFromMap(TileMapValue tileMap)
        {
            //Ÿ�ϸ��� ��� ��ġ�� ��ȸ
            foreach (var pos in tileMap.map.cellBounds.allPositionsWithin)
            {
                //Ÿ���� �����ϴ� ��ġ�� ����
                if (tileMap.map.HasTile(pos))
                {
                    //Ÿ�� ������ ��ġ�� �������� TileData ����
                    var tile = tileMap.tiles.Find(_x => _x.sprite.Equals(tileMap.map.GetSprite(pos)));

                    yield return new TileData()
                    {
                        Position = pos,
                        tileType = (int)tileMap.type,
                        tileNum = tileMap.tiles.IndexOf(tile),
                    };
                };

            }
        }
    }

    //�� ��ġ�κ��� ���� ��ǥ�� ��ȯ�ϴ� �޼���
    public Vector3 GetWorldPositionFromCellPosition(Vector3Int cellPosition)
    {
        return baseTile.map.layoutGrid.GetCellCenterWorld(cellPosition);
    }

    //�� ��ġ�� ��ֹ��� �ִ��� Ȯ���ϴ� �ż���
    public bool IsObstacleCell(Vector3Int cellPosition)
    {
        return obstacleTile.map.GetTile(cellPosition) != null;
    }
}
