using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public float rotationSpeed = 5f; // ȸ�� �ӵ�
    public AudioClip detectionSound; // ���� ����� Ŭ��
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private bool hasPlayedSound = false; // ������� ����Ǿ����� ����
    private AudioSource audioSource;

    private void Start()
    {
        playerTransform = null;
        audioSource = GetComponent<AudioSource>();

        // AudioSource�� ���ٸ� �ڵ� �߰�
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // �÷��̾ ������ ���¿��� �ٶ󺸴� ����
        if (isPlayerInRange && playerTransform != null)
        {
            N_LookAtPlayer();
        }
    }

    private void N_LookAtPlayer()
    {
        // �÷��̾� ���� ���
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // õõ�� ȸ���ϱ�
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹 ��ü�� �÷��̾� �±׸� ���� ���
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isPlayerInRange = true;

            // ����� ��� ���� Ȯ�� �� ���
            if (!hasPlayedSound && detectionSound != null)
            {
                audioSource.PlayOneShot(detectionSound);
                hasPlayedSound = true; // �ѹ��� ����ǵ��� ����
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ ������ ����� ��
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }
    }
}