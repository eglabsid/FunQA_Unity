using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerAgent : Agent
{
    public const int MAX_Steps = 1000000;
    private const float Tmax = 20f;
    private float lastCaptureTime;

    private List<float> tensionValues;

    //Player �� AgentUI ������Ʈ ����
    [field: SerializeField] Player player;
    [field: SerializeField] AgentUI agentUI;
    [field: SerializeField] Spawner spawner;
    int prevScore; //���� ���� �����ϴ� ����

    private int episodeCount = 0;
    private float startTime;
    private float episodeTime;
    private int enemyCount;

    private float killTime;

    private int initialEnemyCount;
    private int currentEnemyCount;

    private Vector3 lastPosition;
    private float timeInCorner = 0f;
    private float cornerThreshold = 5.0f;
    private float positionThreshold = 0.1f;
    private Vector2 autoMoveDirection;

    //�ʱ�ȭ �ż���
    public override void Initialize()
    {
        base.Initialize(); //�θ� Ŭ���� �ʱ�ȭ �޼��� ȣ��
        //UIManager�� ���� AgentUI�� ������
        agentUI = UIManager.Instance.GetComponentInChildren<AgentUI>();
        spawner = GameManager.Instance.GetComponentInChildren<Spawner>();
        //������ ������
        LevelManager.Instance.CreateLevel();
        ResetTimer();
        lastPosition = player.transform.position;
        autoMoveDirection= Vector2.zero;

        tensionValues = new List<float>();
    }

    public void Update()
    {

        episodeTime = Time.time - startTime;
        agentUI.SetTimeValue(episodeTime);

        if(episodeTime >= 20f)
        {
            Debug.Log("40 seconds passed, ending episode");

            //SaveTensionValuesToCSV();

            EndEpisode();  // ���Ǽҵ� ����
            IncrementEpisodeCount();  // ���Ǽҵ� ī��Ʈ ����
        }

        enemyCount = spawner.enemyAmount;

        //UI�� ���� ���� ������ ������Ʈ��
        agentUI.SetRewardValue(GetCumulativeReward());

        //������Ʈ�� ������� �� �� �����Ӹ��� -1/MaxStep ������ ��
        AddReward(-1f / (float)MaxStep);

        if(Vector3.Distance(lastPosition, player.transform.position) < positionThreshold)
        {
            timeInCorner += Time.deltaTime;
            if (timeInCorner >= cornerThreshold)
            {
                autoMoveDirection = GetRandomDirection(); // ������ ���� ����
                timeInCorner = 0f; // �������� �ӹ� �ð� �ʱ�ȭ
            }
        }
        else
        {
            timeInCorner = 0f; // ������ ���� ������ �ð� �ʱ�ȭ
            autoMoveDirection = Vector2.zero; // �����̰� ������ �ڵ� �̵� ���� �ʱ�ȭ
        }

        // �ڵ� �̵� ������ �����Ǿ��� �� �ش� �������� �÷��̾ �̵���Ŵ
        if (autoMoveDirection != Vector2.zero)
        {
            player.Movement(autoMoveDirection);
        }

        lastPosition = player.transform.position;

        int updatedEnemyCount = GameManager.Instance.Spawner.enemyAmount;

        if (currentEnemyCount > updatedEnemyCount)
        {

            killTime = Time.time - startTime;

            // Calculate the time it took to kill the enemy
            float Tactual = Time.time - lastCaptureTime;
            lastCaptureTime = Time.time;

            // Calculate the tension based on the time taken
            float calculatedTension = CalculateTension(Tactual);

            // Store the calculated tension in a list
            tensionValues.Add(calculatedTension);

            // Save the episode number, kill time (Tactual), and tension value to the CSV file
            CSVManager.AppendToCSV(episodeCount + "," + killTime + "," + calculatedTension + "," + enemyCount);

            // Now update the currentEnemyCount after calculating tension
            currentEnemyCount = updatedEnemyCount;
        }

        //�÷��̾��� ü���� 0 �����̸� ���� ���� �� ���Ǽҵ� ����
        if (player.Health <= 0)
        {
            GameManager.Instance.GameStop();
            EndEpisode();
            IncrementEpisodeCount();
        }
        //��� ���� �������� �� ������ �ְ� ���Ǽҵ� ����
        else if (GameManager.Instance.Spawner.enemyAmount <= 0)
        {
            AddReward(1f);
            GameManager.Instance.GameStop();
            LevelManager.Instance.SaveLevel();

            //SaveTensionValuesToCSV();

            EndEpisode();
            IncrementEpisodeCount();
        }
        else if (StepCount >= 9950 && !(player.Health <= 0) && !(GameManager.Instance.Spawner.enemyAmount <= 0))
        {
            Debug.Log("OK");
            EndEpisode();
            IncrementEpisodeCount();
        }

    }

    public void OnEnemyKilled()
    {
        Debug.Log("Enemy killed! Calculating tension...");

        // Calculate the time it took to kill the enemy
        float Tactual = Time.time - lastCaptureTime;
        lastCaptureTime = Time.time;

        // Calculate the tension based on the time taken
        float calculatedTension = CalculateTension(Tactual);
        Debug.Log("Tension after enemy capture: " + calculatedTension);

        // Store the calculated tension in a list
        tensionValues.Add(calculatedTension);
    }

    private float CalculateTension(float Tactual)
    {
        Tactual = Mathf.Min(Tactual, Tmax);
        return (1f - (Tactual / Tmax)) * 100f;
    }

    /*
    private void SaveTensionValuesToCSV()
    {
        CSVManager.AppendToCSV("Episode " + episodeCount + ":");
        foreach (float tension in tensionValues)
        {
            // CSV ���Ͽ� �ټ� �� ���
            CSVManager.AppendToCSV(tension.ToString());
        }

        tensionValues.Clear();
    }
    */

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f); // 0������ 360�� ������ ������ �������� ����
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized; // ������ ������ ���� ���͸� ��ȯ
    }

    private void IncrementEpisodeCount()
    {
        episodeCount++;
        agentUI.SetEpisodeCount(episodeCount);
    }

    private void ResetTimer()
    {
        startTime = Time.time;
    }

    //������Ʈ�� ������ �����ϴ� �޼���
    public override void CollectObservations(VectorSensor sensor)
    {
        //�÷��̾��� ��ġ�� Ÿ�ϸ� �������� �����ϰ� ����ȭ�Ͽ� ������ �߰�
        var pos = LevelManager.Instance.baseTile.map.WorldToCell(player.transform.position);
        sensor.AddObservation((float)pos.x / (LevelManager.Instance.mapGrid.x * 0.5f));
        sensor.AddObservation((float)pos.y / (LevelManager.Instance.mapGrid.y * 0.5f));

        //�÷��̾��� ü�°� ���¹̳��� ����ȭ�Ͽ� ������ �߰�
        sensor.AddObservation((float)player.Health / (float)player.MaxHealth);
        sensor.AddObservation((float)player.Stamina / (float)player.MaxStamina);

        //������ ���� ���� ����ȭ�Ͽ� ������ �߰�
        var spawner = GameManager.Instance.Spawner;
        sensor.AddObservation(((float)spawner.enemyAmount / (float)spawner.createdEnemyAmount));

        //���� ������ ���� �ִ��� ���θ� ������ �߰�
        bool isCanAttack = player.attackEnemyList.Count > 0 ? true : false;
        sensor.AddObservation(isCanAttack);
    }

    //������Ʈ�� �ൿ�� ������ �� ȣ��Ǵ� �޼���
    public override void OnActionReceived(ActionBuffers actions)
    {
        //������Ʈ�� �ൿ�� ó���ϴ� �޼��� ȣ��
        AgentAction(actions.DiscreteActions);
    }

    //������Ʈ�� �ൿ�� ó���ϴ� �޼���
    public void AgentAction(ActionSegment<int> _act)
    {
        //ù��° �ൿ �ε����� ������
        var action = _act[0];
        switch (action)
        {
            case 0:
                player.Movement(Vector2.right);
                break;
            case 1:
                player.Movement(Vector2.left);
                break;
            case 2:
                player.Movement(Vector2.up);
                break;
            case 3:
                player.Movement(Vector2.down);
                break;
            case 4:
                player.Attack();
                break;
        }

        //�÷��̾��� ������ ����Ǿ��� ��� ������ �߰�
        if (prevScore != player.score)
        {
            AddReward( (player.score - prevScore) * 0.1f);
            prevScore = player.score;
        }

        //UI�� ���� ���� ī��Ʈ�� ������Ʈ��
        agentUI.SetStepValue(StepCount);
    }

    //���Ǽҵ尡 ���۵� �� ȣ��Ǵ� �޼���
    public override void OnEpisodeBegin()
    {
        //������ �Ͻ������ϰ� �ʱ� ���·� ����
        GameManager.Instance.GameStop();
        prevScore = 0;
        agentUI.SetStepValue(0);
        SetReward(0);
        agentUI.SetRewardValue(GetCumulativeReward());

        startTime = Time.time;

        //���ο� ������ �����ϰ� ������ ����
        LevelManager.Instance.CreateLevel();
        GameManager.Instance.GameStart();

        lastCaptureTime = Time.time;

        tensionValues.Clear();

        initialEnemyCount = GameManager.Instance.Spawner.enemyAmount;
        currentEnemyCount = initialEnemyCount;

    }
}
