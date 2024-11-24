using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surveillance : MonoBehaviour
{
    public GameObject monsterAI; // MonsterAI_Crown 오브젝트
    private MonsterAI_Crown aiScript; // MonsterAI_Crown 스크립트 참조

    private void Awake()
    {
        // MonsterAI_Crown 스크립트를 참조
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
        // Target 태그가 있는 아이템이 들어왔을 때
        if (other.CompareTag("Target"))
        {        
            // MonsterAI_Crown의 AI 이동을 멈춤
            aiScript.StopWandering();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Target 태그가 달린 아이템이 트리거에서 나갔을 때
        if (other.CompareTag("Target"))
        {           
            // MonsterAI_Crown의 AI가 맵을 돌아다니도록 함
            aiScript.StartWandering();
        }
    }
}
