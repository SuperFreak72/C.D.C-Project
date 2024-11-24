using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰�
using TMPro; // TextMeshPro

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // ���� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI itemInsertCountText; // ������ ���� Ƚ�� ǥ�� �ؽ�Ʈ
    public int currentRound; // ���� ����

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ����� ���� �ҷ�����
        int finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        scoreText.text = "Your Score: " + finalScore; // �ؽ�Ʈ�� ���� ǥ��

        // ����� ������ ���� Ƚ�� �ҷ�����
        int itemInsertCount = PlayerPrefs.GetInt("ItemInsertCount", 0);
        itemInsertCountText.text = "Items Inserted: " + itemInsertCount; // ������ ���� Ƚ�� ǥ��

        // ���� ���� �ҷ�����
        currentRound = PlayerPrefs.GetInt("CurrentRound", 1);
    }

    // �� �̵� ��ư�� ������ ȣ��� �Լ�
    public void LoadNextRound()
    {
        // ���� ���忡 ���� ���� ������ �̵�
        if (currentRound == 1)
        {
            SceneManager.LoadScene("ROUND2"); // ROUND2 ������ �̵�
        }
        else if (currentRound == 2)
        {
            SceneManager.LoadScene("ROUND3"); // ROUND3 ������ �̵�
        }
        else
        {
            // ��� ���带 �Ϸ����� ��� ���� �¸� ������ �̵�
            SceneManager.LoadScene("FINAL_WIN");
        }
    }

    // �� �̵� ��ư�� ������ ȣ��� �Լ�
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