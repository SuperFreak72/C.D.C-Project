using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Monster_S;

public class Monster_S : MonoBehaviour
{
    // 몬스터 구조체 정의
    [System.Serializable]
    public struct Monster
    {
        public string name;
        public int Health;
        public GameObject prefab;

        // 몬스터가 피해를 받는 메서드
        public void TakeDamage(int damage, Animator animator, GameObject monsterObject)
        {
            Health -= damage; // 피해를 받음
            if (Health <= 0)
            {
                Die(animator, monsterObject); // 사망 처리
            }
        }

        // 사망 처리 메서드
        public void Die(Animator animator, GameObject monsterObject)
        {
            // 몬스터 오브젝트 삭제
            if (monsterObject != null)
            {
                Destroy(monsterObject); // 오브젝트 삭제
            }

            // 애니메이션 멈추기
            if (animator != null)
            {
                animator.enabled = false; // Animator를 비활성화하여 애니메이션 중지
            }

            Debug.Log($"{name} has died.");
        }

       // 3. 플레이어에게 피해를 주는 매서드 (세부 코드는 각 몬스터에 부여)
        public void HandleCollision(GameObject playerObject, int damage)
        {
            // 플레이어에게 피해를 주는 로직
            PlayerMove playerScript = playerObject.GetComponent<PlayerMove>(); // 플레이어 스크립트를 가져옴
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage); // 플레이어에게 피해를 줌
                Debug.Log($"{name} attacked the player for {damage} damage.");
            }
        }
    }

    public Monster[] M_Struct;

    // 1. 몬스터 생성 메서드
    public GameObject SpawnMonster(int index, Vector3 position)
    {
        if (index < 0 || index >= M_Struct.Length)
        {
            Debug.LogError("Invalid monster index.");
            return null;
        }

        // 프리팹을 이용해 몬스터 생성
        GameObject monsterInstance = Instantiate(M_Struct[index].prefab, position, Quaternion.identity);
        if (monsterInstance != null)
        {
            Debug.Log($"Monster spawned: {M_Struct[index].name} at {position}");
        }
        else
        {
            Debug.LogError($"Failed to instantiate monster: {M_Struct[index].name}");
        }

        return monsterInstance;
    }

    // 2. 몬스터에게 피해를 주는 메서드
    public void DamageMonster(int index, int damage, Animator animator, GameObject monsterObject)
    {
        if (index < 0 || index >= M_Struct.Length)
        {
            Debug.LogError("Invalid monster index.");

            return;
        }

        M_Struct[index].TakeDamage(damage, animator, monsterObject);
    }
}