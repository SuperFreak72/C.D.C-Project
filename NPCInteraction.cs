using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionUI; // UI 오브젝트를 여기에 할당
    public float interactionRange = 5f; // 상호작용이 가능한 거리

    private Transform player;

    private void Start()
    {
        // Tag가 "Player"인 오브젝트를 찾습니다.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 처음에는 UI를 비활성화
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // NPC와 Player 사이의 거리를 계산합니다.
            float distance = Vector3.Distance(transform.position, player.position);

            // 거리 내에 있으면 UI를 활성화하고, 그렇지 않으면 비활성화합니다.
            if (distance <= interactionRange)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }
    }

    private void ShowUI()
    {
        if (interactionUI != null && !interactionUI.activeSelf)
        {
            interactionUI.SetActive(true);
        }
    }

    private void HideUI()
    {
        if (interactionUI != null && interactionUI.activeSelf)
        {
            interactionUI.SetActive(false);
        }
    }
}
