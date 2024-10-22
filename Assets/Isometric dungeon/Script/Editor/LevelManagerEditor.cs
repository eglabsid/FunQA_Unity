using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Text;
using System.IO;

// LevelManager Ŭ������ ���� Ŀ���� �����͸� ����
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    // LevelManager �ν��ͽ� �����ϱ� ���� ����
    LevelManager levelManager;

    // LevelManager�� SarializedProperty�� �����ϱ� ���� ����
    SerializedProperty baseTileProp;
    SerializedProperty obstacleTileProp;

    //ȣ��
    public void OnEnable()
    {
        // LevelManager��baseTile, obstacleTile �ʵ带 ã�� SerializedProperty�� ����
        baseTileProp = serializedObject.FindProperty("baseTile");
        obstacleTileProp = serializedObject.FindProperty("obstacleTile");

        // target���� ������ ������Ʈ�� LevelManager�� ĳ�����Ͽ� levelManager�� ����
        levelManager = (LevelManager)target;
    }

    // �ν����� GUI Ŀ���͸�����
    public override void OnInspectorGUI()
    {
        // Map Options
        // �׸��� �ּ�/�ִ� ũ�⸦ �����ϴ� �Է� �ʵ�
        levelManager.minGrid = EditorGUILayout.Vector2IntField("Min Grid", levelManager.minGrid);
        levelManager.maxGrid = EditorGUILayout.Vector2IntField("Max Grid", levelManager.maxGrid);

        // ���м� �߰�
        GUILayout.Space(5f);

        // ��ֹ� ���� ������ �����ϴ� �����̴��� ��
        GUILayout.Label("Obstarcle Creater Percentage");
        levelManager.obstarclePercentage = EditorGUILayout.IntSlider(levelManager.obstarclePercentage, 0, 100);

        // SerializedProperty�� ����Ǿ����� �����ϱ� ���� üũ
        EditorGUI.BeginChangeCheck();

        // baseTile�� obstacleTile�� ������Ƽ �ʵ带 �ν����Ϳ� ǥ��
        EditorGUILayout.PropertyField(baseTileProp, true);
        EditorGUILayout.PropertyField(obstacleTileProp, true);

        // ���� ������Ƽ ���� ���� -> �� ���� ������ serializedObject�� �ݿ�
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

        // "Create Level" ��ư ���� -> ��ư Ŭ����  levelManager�� CreateLevel �޼��� ȣ��
        if (GUILayout.Button("Create Level", GUILayout.Width(200f)))
        {
            levelManager.CreateLevel();
        }
    }
}
