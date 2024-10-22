using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Text;
using System.IO;

// LevelManager 클래스에 대한 커스텀 에디터를 정의
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // LevelManager 인스터스 저장하기 위한 변수
    LevelManager levelManager;

    // LevelManager의 SarializedProperty를 저장하기 위한 변수
    SerializedProperty baseTileProp;
    SerializedProperty obstacleTileProp;

    //호출
    public void OnEnable()
    {
        // LevelManager의baseTile, obstacleTile 필드를 찾고 SerializedProperty에 저장
        baseTileProp = serializedObject.FindProperty("baseTile");
        obstacleTileProp = serializedObject.FindProperty("obstacleTile");

        // target으로 지정된 오브젝트를 LevelManager로 캐스팅하여 levelManager에 저장
        levelManager = (LevelManager)target;
    }

    // 인스펙터 GUI 커스터마이즈
    public override void OnInspectorGUI()
    {
        // Map Options
        // 그리드 최소/최대 크기를 설정하는 입력 필드
        levelManager.minGrid = EditorGUILayout.Vector2IntField("Min Grid", levelManager.minGrid);
        levelManager.maxGrid = EditorGUILayout.Vector2IntField("Max Grid", levelManager.maxGrid);

        // 구분선 추가
        GUILayout.Space(5f);

        // 장애물 생성 비율을 설정하는 슬라이더와 라벨
        GUILayout.Label("Obstarcle Creater Percentage");
        levelManager.obstarclePercentage = EditorGUILayout.IntSlider(levelManager.obstarclePercentage, 0, 100);

        // SerializedProperty가 변경되었는지 감지하기 위해 체크
        EditorGUI.BeginChangeCheck();

        // baseTile과 obstacleTile의 프로퍼티 필드를 인스펙터에 표시
        EditorGUILayout.PropertyField(baseTileProp, true);
        EditorGUILayout.PropertyField(obstacleTileProp, true);

        // 만약 프로퍼티 값이 변경 -> 그 변경 사항을 serializedObject에 반영
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        // "Create Level" 버튼 생성 -> 버튼 클릭시  levelManager의 CreateLevel 메서드 호출
        if (GUILayout.Button("Create Level", GUILayout.Width(200f)))
        {
            levelManager.CreateLevel();
        }
    }
}
