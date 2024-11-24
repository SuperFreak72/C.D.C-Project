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
        originalRotation = transform.rotation;  // ���� ī�޶� �����̼� ����
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // ��ü�� �ٶ󺸵��� LookAt ����
            vCam.LookAt = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // LookAt�� �ʱ�ȭ�Ͽ� ���� �ڸ��� ���ư����� ��
            vCam.LookAt = null;
            transform.rotation = originalRotation;
        }
    }
}
