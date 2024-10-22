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

    //Player 및 AgentUI 컴포넌트 참조
    [field: SerializeField] Player player;
    [field: SerializeField] AgentUI agentUI;
    [field: SerializeField] Spawner spawner;
    int prevScore; //이전 점수 저장하는 변수

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

    //초기화 매서드
    public override void Initialize()
    {
        base.Initialize(); //부모 클래스 초기화 메서드 호출
        //UIManager를 통해 AgentUI를 가져옴
        agentUI = UIManager.Instance.GetComponentInChildren<AgentUI>();
        spawner = GameManager.Instance.GetComponentInChildren<Spawner>();
        //레벨을 생성함
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

            EndEpisode();  // 에피소드 종료
            IncrementEpisodeCount();  // 에피소드 카운트 증가
        }

        enemyCount = spawner.enemyAmount;

        //UI에 현재 누적 보상을 업데이트함
        agentUI.SetRewardValue(GetCumulativeReward());

        //에이전트가 살아있을 때 매 프레임마다 -1/MaxStep 보상을 줌
        AddReward(-1f / (float)MaxStep);

        if(Vector3.Distance(lastPosition, player.transform.position) < positionThreshold)
        {
            timeInCorner += Time.deltaTime;
            if (timeInCorner >= cornerThreshold)
            {
                autoMoveDirection = GetRandomDirection(); // 랜덤한 방향 선택
                timeInCorner = 0f; // 구석에서 머문 시간 초기화
            }
        }
        else
        {
            timeInCorner = 0f; // 구석에 있지 않으면 시간 초기화
            autoMoveDirection = Vector2.zero; // 움직이고 있으면 자동 이동 방향 초기화
        }

        // 자동 이동 방향이 설정되었을 때 해당 방향으로 플레이어를 이동시킴
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

        //플레이어의 체력이 0 이하이면 게임 종료 및 에피소드 종료
        if (player.Health <= 0)
        {
            GameManager.Instance.GameStop();
            EndEpisode();
            IncrementEpisodeCount();
        }
        //모든 적을 물리쳤을 때 보상을 주고 에피소드 종료
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
            // CSV 파일에 텐션 값 기록
            CSVManager.AppendToCSV(tension.ToString());
        }

        tensionValues.Clear();
    }
    */

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f); // 0도에서 360도 사이의 각도를 랜덤으로 생성
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized; // 랜덤한 방향의 단위 벡터를 반환
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

    //에이전트가 관찰을 수집하는 메서드
    public override void CollectObservations(VectorSensor sensor)
    {
        //플레이어의 위치를 타일맵 기준으로 관찰하고 정규화하여 센서에 추가
        var pos = LevelManager.Instance.baseTile.map.WorldToCell(player.transform.position);
        sensor.AddObservation((float)pos.x / (LevelManager.Instance.mapGrid.x * 0.5f));
        sensor.AddObservation((float)pos.y / (LevelManager.Instance.mapGrid.y * 0.5f));

        //플레이어의 체력과 스태미나를 정규화하여 센서에 추가
        sensor.AddObservation((float)player.Health / (float)player.MaxHealth);
        sensor.AddObservation((float)player.Stamina / (float)player.MaxStamina);

        //스폰된 적의 양을 정규화하여 센서에 추가
        var spawner = GameManager.Instance.Spawner;
        sensor.AddObservation(((float)spawner.enemyAmount / (float)spawner.createdEnemyAmount));

        //공격 가능한 적이 있는지 여부를 센서에 추가
        bool isCanAttack = player.attackEnemyList.Count > 0 ? true : false;
        sensor.AddObservation(isCanAttack);
    }

    //에이전트가 행동을 수행할 때 호출되는 메서드
    public override void OnActionReceived(ActionBuffers actions)
    {
        //에이전트의 행동을 처리하는 메서드 호출
        AgentAction(actions.DiscreteActions);
    }

    //에이전트의 행동을 처리하는 메서드
    public void AgentAction(ActionSegment<int> _act)
    {
        //첫번째 행동 인덱스를 가져옴
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

        //플레이어의 점수가 변경되었을 경우 보상을 추가
        if (prevScore != player.score)
        {
            AddReward( (player.score - prevScore) * 0.1f);
            prevScore = player.score;
        }

        //UI에 현재 스텝 카운트를 업데이트함
        agentUI.SetStepValue(StepCount);
    }

    //에피소드가 시작될 때 호출되는 메서드
    public override void OnEpisodeBegin()
    {
        //게임을 일시정지하고 초기 상태로 설정
        GameManager.Instance.GameStop();
        prevScore = 0;
        agentUI.SetStepValue(0);
        SetReward(0);
        agentUI.SetRewardValue(GetCumulativeReward());

        startTime = Time.time;

        //새로운 레벨을 생성하고 게임을 시작
        LevelManager.Instance.CreateLevel();
        GameManager.Instance.GameStart();

        lastCaptureTime = Time.time;

        tensionValues.Clear();

        initialEnemyCount = GameManager.Instance.Spawner.enemyAmount;
        currentEnemyCount = initialEnemyCount;

    }
}
