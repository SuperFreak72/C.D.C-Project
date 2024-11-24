using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFocusOnTag : MonoBehaviour
{
    public string targetTag = "Focus";
    private Quaternion originalRotation;
    private CinemachineVirtualCamera vCam;

    private void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        originalRotation = transform.rotation;  // 원래 카메라 로테이션 저장
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // 물체를 바라보도록 LookAt 설정
            vCam.LookAt = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // LookAt을 초기화하여 원래 자리로 돌아가도록 함
            vCam.LookAt = null;
            transform.rotation = originalRotation;
        }
    }
}
