using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static Monster_S;

public class Monster_S : MonoBehaviour
{
    // ���� ����ü ����
    [System.Serializable]
    public struct Monster
    {
        public string name;
        public int Health;
        public GameObject prefab;

        // ���Ͱ� ���ظ� �޴� �޼���
        public void TakeDamage(int damage, Animator animator, GameObject monsterObject)
        {
            Health -= damage; // ���ظ� ����
            if (Health <= 0)
            {
                Die(animator, monsterObject); // ��� ó��
            }
        }

        // ��� ó�� �޼���
        public void Die(Animator animator, GameObject monsterObject)
        {
            // ���� ������Ʈ ����
            if (monsterObject != null)
            {
                Destroy(monsterObject); // ������Ʈ ����
            }

            // �ִϸ��̼� ���߱�
            if (animator != null)
            {
                animator.enabled = false; // Animator�� ��Ȱ��ȭ�Ͽ� �ִϸ��̼� ����
            }

            Debug.Log($"{name} has died.");
        }

       // 3. �÷��̾�� ���ظ� �ִ� �ż��� (���� �ڵ�� �� ���Ϳ� �ο�)
        public void HandleCollision(GameObject playerObject, int damage)
        {
            // �÷��̾�� ���ظ� �ִ� ����
            PlayerMove playerScript = playerObject.GetComponent<PlayerMove>(); // �÷��̾� ��ũ��Ʈ�� ������
            if (playerScript != null)
            {
                playerScript.TakeDamage(damage); // �÷��̾�� ���ظ� ��
                Debug.Log($"{name} attacked the player for {damage} damage.");
            }
        }
    }

    public Monster[] M_Struct;

    // 1. ���� ���� �޼���
    public GameObject SpawnMonster(int index, Vector3 position)
    {
        if (index < 0 || index >= M_Struct.Length)
        {
            Debug.LogError("Invalid monster index.");
            return null;
        }

        // �������� �̿��� ���� ����
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

    // 2. ���Ϳ��� ���ظ� �ִ� �޼���
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