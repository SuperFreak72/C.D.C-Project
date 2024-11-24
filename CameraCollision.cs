using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform player;        // �÷��̾� Transform
    public float distanceAway = 2f; // ī�޶�� �÷��̾� ���� �Ÿ�
    public float minDistance = 0.5f; // ī�޶� ������ �ּ� �Ÿ�
    public LayerMask collisionMask; // �浹 ������ ���� ���̾� ����ũ

    private Vector3 cameraPosition; // ī�޶� ��ġ ���� ����

    void LateUpdate()
    {
        // �÷��̾� ��ġ�κ��� ī�޶� ��ġ ���
        cameraPosition = player.position - player.forward * distanceAway;

        // ī�޶�� �÷��̾� ���� �Ÿ� ���
        Vector3 direction = cameraPosition - player.position;
        float distance = direction.magnitude;

        // �� �浹 ����
        if (Physics.Raycast(player.position, -player.forward, out RaycastHit hit, distance))
        {
            // ī�޶� ���� ���� �ʵ��� ��ġ ����
            float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, distance);
            cameraPosition = player.position + (-player.forward * adjustedDistance);
        }

        // ī�޶� ��ġ ������Ʈ
        transform.position = cameraPosition;
    }
}
