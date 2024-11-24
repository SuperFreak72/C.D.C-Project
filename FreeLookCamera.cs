using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    public float sensitivity = 2f;      // 마우스 감도
    public float minYAngle = -80f;      // 카메라 상하 회전 제한 (최소 각도)
    public float maxYAngle = 80f;       // 카메라 상하 회전 제한 (최대 각도)

    private float yaw = 0f;             // 좌우 회전값 (Yaw)
    private float pitch = 0f;           // 상하 회전값 (Pitch)

    void Start()
    {
        // 커서를 화면 중앙에 고정하고 숨김 처리
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 시작할 때 카메라 회전을 초기화
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleCameraRotation();
    }

    // 마우스 입력에 따라 카메라를 회전시키는 함수
    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity; // 마우스 좌우 입력
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity; // 마우스 상하 입력

        // 좌우 회전 (Yaw)
        yaw += mouseX;

        // 상하 회전 (Pitch) - 각도 제한
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle); // 상하 각도 제한

        // 카메라 회전 적용
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // 자유 시점 카메라 활성화
    public void ActivateFreeLook()
    {
        gameObject.SetActive(true);

        // 커서 잠금 및 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 자유 시점 카메라 비활성화
    public void DeactivateFreeLook()
    {
        gameObject.SetActive(false);

        // 커서 복구
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

