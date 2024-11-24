using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가
using TMPro; // TextMeshPro

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // 점수 표시 텍스트
    public TextMeshProUGUI itemInsertCountText; // 아이템 삽입 횟수 표시 텍스트
    public int currentRound; // 현재 라운드

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 저장된 점수 불러오기
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = "Your Score: " + finalScore; // 텍스트에 점수 표시

        // 저장된 아이템 삽입 횟수 불러오기
        int itemInsertCount = PlayerPrefs.GetInt("ItemInsertCount", 0);
        itemInsertCountText.text = "Items Inserted: " + itemInsertCount; // 아이템 삽입 횟수 표시

        // 현재 라운드 불러오기
        currentRound = PlayerPrefs.GetInt("CurrentRound", 1);
    }

    // 씬 이동 버튼을 누르면 호출될 함수
    public void LoadNextRound()
    {
        // 현재 라운드에 따라 다음 씬으로 이동
        if (currentRound == 1)
        {
            SceneManager.LoadScene("ROUND2"); // ROUND2 씬으로 이동
        }
        else if (currentRound == 2)
        {
            SceneManager.LoadScene("ROUND3"); // ROUND3 씬으로 이동
        }
        else
        {
            // 모든 라운드를 완료했을 경우 최종 승리 씬으로 이동
            SceneManager.LoadScene("FINAL_WIN");
        }
    }

    // 씬 이동 버튼을 누르면 호출될 함수
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}