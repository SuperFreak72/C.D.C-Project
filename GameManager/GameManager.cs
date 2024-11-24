using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public MonsterManager monsterManager; // MonsterManager 참조 추가
    public static GameManager instance;


    public int roundItemValue; // 라운드에서 필요한 아이템 수
    public int roundTime; // 라운드 시간
    public int countdownTime;

    public int itemValue = 0; // 현재 아이템 점수
    public bool isGameover = false;

    public int currentRound = 1; // 현재 라운드 번호
    public const int maxRounds = 3; // 최대 라운드 수

    public int itemInsertCount = 0; // 아이템을 넣은 횟수 추적 변수

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복되는 GameManager 오브젝트가 생기지 않도록 기존 오브젝트 파괴
        }
    }

    void Start()
    {
        itemInsertCount = PlayerPrefs.GetInt("ItemInsertCount", 0);
        monsterManager.SpawnMonstersForRound(currentRound);
        StartRound();
    }

    void Update()
    {
        if (isGameover && Input.GetMouseButtonDown(0))
        {
            RestartGame();
        }

        // 승리 씬에서 돌아올 때
        if (SceneManager.GetActiveScene().name == "Cults" && Input.GetMouseButtonDown(0))
        {
            ReturnToGame();
        }
    }

    private void StartRound()
    {
        roundItemValue = 100 * currentRound; // 라운드별 필요한 아이템 수 증가
        roundTime = 180 * currentRound; // 라운드 시간 증가
        countdownTime = roundTime;
        itemValue = 0; // 점수 초기화
        itemInsertCount = 0; // 아이템 삽입 횟수 초기화
        isGameover = false;

        Debug.Log($"Round {currentRound} 시작: 아이템 목표 {roundItemValue}, 제한 시간 {roundTime}초");

        // 몬스터 소환 함수 호출
        if (monsterManager != null)
        {
            monsterManager.SpawnMonstersForRound(currentRound);
        }

        CountDown(); // 카운트다운 시작
    }

    public void OnPlayerDead()
    {
        isGameover = true;
    }

    public void CountDown()
    {
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        if (itemValue >= roundItemValue)
        {
            Debug.Log($"Round {currentRound} Win");
            if (currentRound < maxRounds)
            {
                currentRound++;
                SaveScoreAndLoadWinScene();
            }
            else
            {
                SaveScoreAndLoadFinalWinScene();
            }
        }
        else
        {
            Debug.Log("Round Lose");
            DestroyAndLoadFailScene(); // 실패 시 GameManager 파괴 후 패배 씬 이동
        }
    }

    // 아이템을 넣을 때마다 호출되는 함수
    public void InsertItem()
    {
        itemValue++; // 아이템 점수 증가
        itemInsertCount++; // 아이템 삽입 횟수 증가
        Debug.Log($"아이템 삽입 {itemInsertCount}회, 현재 점수: {itemValue}");
    }

    void SaveScoreAndLoadWinScene()
    {
        PlayerPrefs.SetInt("FinalScore", itemValue); // 점수 저장
        PlayerPrefs.SetInt("ItemInsertCount", itemInsertCount); // 아이템 삽입 횟수 저장
        PlayerPrefs.SetInt("CurrentRound", currentRound); // 현재 라운드 저장
        PlayerPrefs.Save(); // PlayerPrefs 저장
        SceneManager.LoadScene("Cults"); // 승리 씬
    }

    public void DestroyAndLoadFailScene()
    {
        PlayerPrefs.SetInt("FinalScore", itemValue); // 점수 저장
        PlayerPrefs.SetInt("ItemInsertCount", itemInsertCount); // 아이템 삽입 횟수 저장
        PlayerPrefs.Save(); // PlayerPrefs 저장
        Destroy(gameObject); // GameManager 오브젝트 파괴
        SceneManager.LoadScene("Fail"); // 패배 씬
    }

    void SaveScoreAndLoadFinalWinScene()
    {
        PlayerPrefs.SetInt("FinalScore", itemValue); // 최종 점수 저장
        PlayerPrefs.SetInt("ItemInsertCount", itemInsertCount); // 아이템 삽입 횟수 저장
        PlayerPrefs.Save(); // PlayerPrefs 저장
        SceneManager.LoadScene("Cults"); // 최종 승리 씬
    }

    void RestartGame()
    {
        currentRound = 1; // 라운드 초기화
        itemInsertCount = 0; // 게임 재시작 시 아이템 삽입 횟수 초기화
        PlayerPrefs.SetInt("ItemInsertCount", itemInsertCount); // PlayerPrefs에도 초기화된 값 저장
        PlayerPrefs.Save(); // 변경 사항을 저장

        SceneManager.LoadScene("ROUND1"); // 첫 번째 라운드 씬으로 이동
    }

    // 승리 씬에서 돌아올 때 호출되는 메서드
    public void ReturnToGame()
    {
        itemValue = 0; // 아이템 점수 초기화
        itemInsertCount = 0; // 아이템 삽입 횟수 초기화
        countdownTime = roundTime; // 카운트다운 초기화

        // 현재 라운드에 따라 다음 라운드로 이동
        if (currentRound == 2)
        {
            SceneManager.LoadScene("ROUND2"); // ROUND2 씬으로 이동
            StartRound(); // ROUND2의 StartRound 호출

            if (monsterManager != null)
            {
                monsterManager.SpawnMonstersForRound(currentRound);
            }

        }
        else if (currentRound == 3)
        {
            SceneManager.LoadScene("ROUND3"); // ROUND3 씬으로 이동
            StartRound(); // ROUND3의 StartRound 호출

            if (monsterManager != null)
            {
                monsterManager.SpawnMonstersForRound(currentRound);
            }
        }
        else
        {
            SaveScoreAndLoadFinalWinScene(); // 마지막 승리 씬으로 이동
        }
    }
}