using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public float sensitivity = 2f;      // ���콺 ����
    public float minYAngle = -80f;      // ī�޶� ���� ȸ�� ���� (�ּ� ����)
    public float maxYAngle = 80f;       // ī�޶� ���� ȸ�� ���� (�ִ� ����)

    private float yaw = 0f;             // �¿� ȸ���� (Yaw)
    private float pitch = 0f;           // ���� ȸ���� (Pitch)

    void Start()
    {
        // Ŀ���� ȭ�� �߾ӿ� �����ϰ� ���� ó��
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ������ �� ī�޶� ȸ���� �ʱ�ȭ
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleCameraRotation();
    }

    // ���콺 �Է¿� ���� ī�޶� ȸ����Ű�� �Լ�
    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity; // ���콺 �¿� �Է�
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity; // ���콺 ���� �Է�

        // �¿� ȸ�� (Yaw)
        yaw += mouseX;

        // ���� ȸ�� (Pitch) - ���� ����
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle); // ���� ���� ����

        // ī�޶� ȸ�� ����
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // ���� ���� ī�޶� Ȱ��ȭ
    public void ActivateFreeLook()
    {
        gameObject.SetActive(true);

        // Ŀ�� ��� �� �����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ���� ���� ī�޶� ��Ȱ��ȭ
    public void DeactivateFreeLook()
    {
        gameObject.SetActive(false);

        // Ŀ�� ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

