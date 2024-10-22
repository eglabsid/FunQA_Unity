using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//레벨 데이터 정보를 저장하기 위한 ScriptableObject 클래스
public class LevelData : ScriptableObject
{
    public string LevelName; //레벨 데이터
    public List<TileData> baseTileDatas; //레벨의 기본 타일 데이터를 저장하는 리스트
    public List<TileData> obstacleTileDatas; //레벨의 장애물 타입 데이터를 저장하는 리스트
}

//개ㅂ려 타일에 대한 정보를 담기 위한 클래스
[System.Serializable]
public class TileData
{
    public Vector3Int Position; //타일 위치
    public int tileType; //타일의 타입
    public int tileNum; //특정 타일의 식별하기 위한 타일 번호
}
