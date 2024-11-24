using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionUI; // UI ������Ʈ�� ���⿡ �Ҵ�
    public float interactionRange = 5f; // ��ȣ�ۿ��� ������ �Ÿ�

    private Transform player;

    private void Start()
    {
        // Tag�� "Player"�� ������Ʈ�� ã���ϴ�.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // ó������ UI�� ��Ȱ��ȭ
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // NPC�� Player ������ �Ÿ��� ����մϴ�.
            float distance = Vector3.Distance(transform.position, player.position);

            // �Ÿ� ���� ������ UI�� Ȱ��ȭ�ϰ�, �׷��� ������ ��Ȱ��ȭ�մϴ�.
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
