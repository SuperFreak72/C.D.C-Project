using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterManager : MonoBehaviour
{
    public Monster_S monsterSpawner; // Monster_S 스크립트 참조
    private List<GameObject> activeMonsters = new List<GameObject>(); // 생성된 몬스터 저장

    // 라운드별로 몬스터 생성 위치를 다르게 설정
    public Transform[] spawnPointsRound1; // 라운드 1 위치
    public Transform[] spawnPointsRound2; // 라운드 2 위치
    public Transform[] spawnPointsRound3; // 라운드 3 위치

    private void Start()
    {
        // "Monster_Stc" 게임 오브젝트를 찾아서 Monster_S 스크립트를 가져옴
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
        // 기존의 활성화된 몬스터 삭제
        foreach (GameObject monster in activeMonsters)
        {
            Destroy(monster);
        }
        activeMonsters.Clear(); // 리스트 초기화

        // 라운드에 맞는 몬스터 타입과 수량 가져오기
        List<(int monsterType, int count)> monstersToSpawn = GetMonstersToSpawnForRound(round);
        Transform[] spawnPoints = GetSpawnPointsForRound(round);

        Debug.Log($"Round {round} - Monsters to spawn: {monstersToSpawn.Count}, Spawn Points: {spawnPoints.Length}");

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Spawn points for this round are not set.");
            return;
        }

        // 몬스터 생성
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

    // 라운드별 몬스터 타입과 수량 설정
    private List<(int monsterType, int count)> GetMonstersToSpawnForRound(int round)
    {
        switch (round)
        {
            case 1:
                return new List<(int, int)>
            {
                (0, 1), // 스누퍼, 2마리
                (1, 6), // 봄버, 3마리
                (2, 0), // 그랩, 2마리
                (3, 4)  // 미믹, 3마리
            };
            case 2:
                return new List<(int, int)>
            {
                (0, 2), // 스누퍼, 3마리
                (1, 8), // 봄버, 4마리
                (2, 3), // 그랩, 3마리
                (3, 4)  // 미믹, 2마리
            };
            case 3:
                return new List<(int, int)>
            {
                (0, 3), // 스누퍼, 4마리
                (1, 10), // 봄버, 5마리
                (2, 4), // 그랩, 4마리
                (3, 7)  // 미믹, 4마리
            };
            default:
                return new List<(int, int)>();
        }
    }

    // 라운드에 맞는 생성 위치 배열 반환
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
