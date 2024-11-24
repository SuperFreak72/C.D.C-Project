using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterManager : MonoBehaviour
{
    public Monster_S monsterSpawner; // Monster_S ��ũ��Ʈ ����
    private List<GameObject> activeMonsters = new List<GameObject>(); // ������ ���� ����

    // ���庰�� ���� ���� ��ġ�� �ٸ��� ����
    public Transform[] spawnPointsRound1; // ���� 1 ��ġ
    public Transform[] spawnPointsRound2; // ���� 2 ��ġ
    public Transform[] spawnPointsRound3; // ���� 3 ��ġ

    private void Start()
    {
        // "Monster_Stc" ���� ������Ʈ�� ã�Ƽ� Monster_S ��ũ��Ʈ�� ������
        GameObject monsterSTC = GameObject.Find("Monster_Stc");
        if (monsterSTC != null)
        {
            monsterSpawner = monsterSTC.GetComponent<Monster_S>();
            if (monsterSpawner != null)
            {
                foreach (var monster in monsterSpawner.M_Struct)
                {
                    Debug.Log($"Monster Name: {monster.name}, Health: {monster.Health}");
                }
            }
            else
            {
                Debug.LogError("Monster_S component not found on Monster_Stc.");
            }
        }
        else
        {
            Debug.LogError("Monster_Stc GameObject not found.");
        }
    }

    public void SpawnMonstersForRound(int round)
    {
        // ������ Ȱ��ȭ�� ���� ����
        foreach (GameObject monster in activeMonsters)
        {
            Destroy(monster);
        }
        activeMonsters.Clear(); // ����Ʈ �ʱ�ȭ

        // ���忡 �´� ���� Ÿ�԰� ���� ��������
        List<(int monsterType, int count)> monstersToSpawn = GetMonstersToSpawnForRound(round);
        Transform[] spawnPoints = GetSpawnPointsForRound(round);

        Debug.Log($"Round {round} - Monsters to spawn: {monstersToSpawn.Count}, Spawn Points: {spawnPoints.Length}");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn points for this round are not set.");
            return;
        }

        // ���� ����
        for (int i = 0; i < monstersToSpawn.Count; i++)
        {
            int monsterType = monstersToSpawn[i].monsterType;
            int monsterCount = monstersToSpawn[i].count;

            Debug.Log($"Spawning {monsterCount} of monster type {monsterType}");

            for (int j = 0; j < monsterCount; j++)
            {
                int spawnIndex = (i * monsterCount + j) % spawnPoints.Length;
                Vector3 spawnPosition = spawnPoints[spawnIndex].position;

                GameObject monsterInstance = monsterSpawner.SpawnMonster(monsterType, spawnPosition);
                if (monsterInstance != null)
                {
                    activeMonsters.Add(monsterInstance);
                    Debug.Log($"Monster spawned: {monsterSpawner.M_Struct[monsterType].name} at {spawnPosition}");
                }
                else
                {
                    Debug.LogError($"Failed to spawn monster type {monsterType}.");
                }
            }
        }
    }

    // ���庰 ���� Ÿ�԰� ���� ����
    private List<(int monsterType, int count)> GetMonstersToSpawnForRound(int round)
    {
        switch (round)
        {
            case 1:
                return new List<(int, int)>
            {
                (0, 1), // ������, 2����
                (1, 6), // ����, 3����
                (2, 0), // �׷�, 2����
                (3, 4)  // �̹�, 3����
            };
            case 2:
                return new List<(int, int)>
            {
                (0, 2), // ������, 3����
                (1, 8), // ����, 4����
                (2, 3), // �׷�, 3����
                (3, 4)  // �̹�, 2����
            };
            case 3:
                return new List<(int, int)>
            {
                (0, 3), // ������, 4����
                (1, 10), // ����, 5����
                (2, 4), // �׷�, 4����
                (3, 7)  // �̹�, 4����
            };
            default:
                return new List<(int, int)>();
        }
    }

    // ���忡 �´� ���� ��ġ �迭 ��ȯ
    private Transform[] GetSpawnPointsForRound(int round)
    {
        switch (round)
        {
            case 1:
                return spawnPointsRound1;
            case 2:
                return spawnPointsRound2;
            case 3:
                return spawnPointsRound3;
            default:
                return new Transform[0];
        }
    }
}
