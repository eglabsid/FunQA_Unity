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
    public Vector2Int mapGrid; //현재 맵의 그리드 크기
    public Vector2Int minGrid = new Vector2Int(30, 30); //최소
    public Vector2Int maxGrid = new Vector2Int(50, 50); //최대
    public int obstarclePercentage = 1; //장애물 비율(0-100%) - 타일 맵에서 장애물 생성될 확률
    public string levelName; //현재 레벨의 이름을 저장

    //타일의 유형을 정의하는 열거형
    public enum TileType
    {
        Base = 1, //기본 타일
        Obstacle = 2, //장애물 타일
    }

    //타일맵과 그에 관련된 타일을 저장하는 구조체
    [System.Serializable]
    public struct TileMapValue
    {
        public Tilemap map; //타일맵 참조
        public List<Tile> tiles; //타일 목록
        public TileType type; //타일의 유형(기본/장애물)
    }

    //기본 타일맵과 관련된 데이터
    public TileMapValue baseTile;
    //장애물 타일맵과 관련된 데이터
    public TileMapValue obstacleTile;

    public void Awake()
    {
        Instance = this;
    }

    //레벨 생성
    public void CreateLevel()
    {
        obstarclePercentage = 0;

        //기존의 모든 타일을 지움
        baseTile.map.ClearAllTiles();
        obstacleTile.map.ClearAllTiles();

        //랜덤으로 맵 크기를 설정
        int xGrid = Random.Range(minGrid.x, maxGrid.x);
        int yGrid = Random.Range(minGrid.y, maxGrid.y);
        mapGrid = new Vector2Int(xGrid, yGrid);

        //그리드 범위 내에서 타일을 생성
        for (int i = -xGrid / 2; i < xGrid / 2; i++)
        {
            for (int j = -yGrid / 2; j < yGrid / 2; j++)
            {
                //랜덤한 기본 타일을 해당 위치에 배치
                baseTile.map.SetTile(new Vector3Int(i, j, 0), baseTile.tiles[UnityEngine.Random.Range(0, baseTile.tiles.Count)]);

                //설정된 확률에 따라 장애물 타일을 배치하거나 맵의 가장자리에 장애물 배치
                if (obstarclePercentage > Random.Range(0, 100) ||
                    ((i == -xGrid / 2 || i == (xGrid / 2) - 1) || (j == -yGrid / 2 || j == (yGrid / 2) - 1))) //set obstacle on edge)
                {
                    obstacleTile.map.SetTile(new Vector3Int(i, j, 0), obstacleTile.tiles[UnityEngine.Random.Range(0, obstacleTile.tiles.Count)]);
                }
            }
        }
    }

    //레벨을 저장하는 메소드
    public void SaveLevel()
    {
        //새로운 레벨 데이터를 ScriptableObject로 생성
        var newLevel = ScriptableObject.CreateInstance<LevelData>();
        newLevel.LevelName = levelName + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        //현재 타일맵에서 타일 데이터를 추출하여 레벨 데이터에 저장
        newLevel.baseTileDatas = GetTilesFromMap(baseTile).ToList();
        newLevel.obstacleTileDatas = GetTilesFromMap(obstacleTile).ToList();

        //json으로 변환 후 저장
        var json = JsonUtility.ToJson(newLevel, true);
        var sb = new StringBuilder();
        sb.Append(Application.dataPath);
        sb.Append("/");
        sb.Append(newLevel.LevelName);
        sb.Append(".json");
        var file = File.CreateText(sb.ToString());
        file.WriteLine(json);
        file.Close();

        //타일맵에서 타일 데이터 추출하는 내부 매서드
        IEnumerable<TileData> GetTilesFromMap(TileMapValue tileMap)
        {
            //타일맵의 모든 위치를 순회
            foreach (var pos in tileMap.map.cellBounds.allPositionsWithin)
            {
                //타일이 존재하는 위치만 추출
                if (tileMap.map.HasTile(pos))
                {
                    //타일 정보와 위치를 바탕으로 TileData 생성
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

    //셀 위치로부터 월드 좌표를 반환하는 메서드
    public Vector3 GetWorldPositionFromCellPosition(Vector3Int cellPosition)
    {
        return baseTile.map.layoutGrid.GetCellCenterWorld(cellPosition);
    }

    //셀 위치에 장애물이 있는지 확인하는 매서드
    public bool IsObstacleCell(Vector3Int cellPosition)
    {
        return obstacleTile.map.GetTile(cellPosition) != null;
    }
}
