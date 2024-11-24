using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardinalScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Treasure1"))
        {
            // ������ ���� ����
            GameManager.instance.itemValue += 10;

            // ������ ���� Ƚ�� ����
            GameManager.instance.itemInsertCount++;

            // ������ ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

            // ����� �α׷� ������ ���� Ƚ�� ���
            Debug.Log($"������ ���� Ƚ��: {GameManager.instance.itemInsertCount}");
        }
        else if (collision.collider.CompareTag("Treasure2"))
        {
            // ������ ���� ����
            GameManager.instance.itemValue += 20;

            // ������ ���� Ƚ�� ����
            GameManager.instance.itemInsertCount++;

            // ������ ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

            // ����� �α׷� ������ ���� Ƚ�� ���
            Debug.Log($"������ ���� Ƚ��: {GameManager.instance.itemInsertCount}");
        }
        else if (collision.collider.CompareTag("Treasure3"))
        {
            // ������ ���� ����
            GameManager.instance.itemValue += 30;

            // ������ ���� Ƚ�� ����
            GameManager.instance.itemInsertCount++;

            // ������ ������Ʈ ��Ȱ��ȭ
            collision.gameObject.SetActive(false);

            // ����� �α׷� ������ ���� Ƚ�� ���
            Debug.Log($"������ ���� Ƚ��: {GameManager.instance.itemInsertCount}");
        }
    }
}
