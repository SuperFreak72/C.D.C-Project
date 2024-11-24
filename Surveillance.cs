using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surveillance : MonoBehaviour
{
    public GameObject monsterAI; // MonsterAI_Crown ������Ʈ
    private MonsterAI_Crown aiScript; // MonsterAI_Crown ��ũ��Ʈ ����

    private void Awake()
    {
        // MonsterAI_Crown ��ũ��Ʈ�� ����
        if (monsterAI != null)
        {
            aiScript = monsterAI.GetComponent<MonsterAI_Crown>();
            if (aiScript == null)
            {
                Debug.LogError("MonsterAI_Crown script not found on the assigned object.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Target �±װ� �ִ� �������� ������ ��
        if (other.CompareTag("Target"))
        {        
            // MonsterAI_Crown�� AI �̵��� ����
            aiScript.StopWandering();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Target �±װ� �޸� �������� Ʈ���ſ��� ������ ��
        if (other.CompareTag("Target"))
        {           
            // MonsterAI_Crown�� AI�� ���� ���ƴٴϵ��� ��
            aiScript.StartWandering();
        }
    }
}
