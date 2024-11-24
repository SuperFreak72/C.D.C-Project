using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform player;        // 플레이어 Transform
    public float distanceAway = 2f; // 카메라와 플레이어 간의 거리
    public float minDistance = 0.5f; // 카메라가 벽과의 최소 거리
    public LayerMask collisionMask; // 충돌 감지를 위한 레이어 마스크

    private Vector3 cameraPosition; // 카메라 위치 저장 변수

    void LateUpdate()
    {
        // 플레이어 위치로부터 카메라 위치 계산
        cameraPosition = player.position - player.forward * distanceAway;

        // 카메라와 플레이어 간의 거리 계산
        Vector3 direction = cameraPosition - player.position;
        float distance = direction.magnitude;

        // 벽 충돌 감지
        if (Physics.Raycast(player.position, -player.forward, out RaycastHit hit, distance))
        {
            // 카메라가 벽에 닿지 않도록 위치 조정
            float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            cameraPosition = player.position + (-player.forward * adjustedDistance);
        }

        // 카메라 위치 업데이트
        transform.position = cameraPosition;
    }
}
