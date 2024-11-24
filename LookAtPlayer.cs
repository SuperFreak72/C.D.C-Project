using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public float rotationSpeed = 5f; // 회전 속도
    public AudioClip detectionSound; // 감지 오디오 클립
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private bool hasPlayedSound = false; // 오디오가 재생되었는지 여부
    private AudioSource audioSource;

    private void Start()
    {
        playerTransform = null;
        audioSource = GetComponent<AudioSource>();

        // AudioSource가 없다면 자동 추가
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // 플레이어가 감지된 상태에서 바라보는 로직
        if (isPlayerInRange && playerTransform != null)
        {
            N_LookAtPlayer();
        }
    }

    private void N_LookAtPlayer()
    {
        // 플레이어 방향 계산
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // 천천히 회전하기
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌 객체가 플레이어 태그를 가진 경우
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isPlayerInRange = true;

            // 오디오 재생 여부 확인 후 재생
            if (!hasPlayedSound && detectionSound != null)
            {
                audioSource.PlayOneShot(detectionSound);
                hasPlayedSound = true; // 한번만 재생되도록 설정
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 범위를 벗어났을 때
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }
    }
}