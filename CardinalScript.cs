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
            // 아이템 점수 증가
            GameManager.instance.itemValue += 10;

            // 아이템 삽입 횟수 증가
            GameManager.instance.itemInsertCount++;

            // 아이템 오브젝트 비활성화
            collision.gameObject.SetActive(false);

            // 디버그 로그로 아이템 삽입 횟수 출력
            Debug.Log($"아이템 삽입 횟수: {GameManager.instance.itemInsertCount}");
        }
        else if (collision.collider.CompareTag("Treasure2"))
        {
            // 아이템 점수 증가
            GameManager.instance.itemValue += 20;

            // 아이템 삽입 횟수 증가
            GameManager.instance.itemInsertCount++;

            // 아이템 오브젝트 비활성화
            collision.gameObject.SetActive(false);

            // 디버그 로그로 아이템 삽입 횟수 출력
            Debug.Log($"아이템 삽입 횟수: {GameManager.instance.itemInsertCount}");
        }
        else if (collision.collider.CompareTag("Treasure3"))
        {
            // 아이템 점수 증가
            GameManager.instance.itemValue += 30;

            // 아이템 삽입 횟수 증가
            GameManager.instance.itemInsertCount++;

            // 아이템 오브젝트 비활성화
            collision.gameObject.SetActive(false);

            // 디버그 로그로 아이템 삽입 횟수 출력
            Debug.Log($"아이템 삽입 횟수: {GameManager.instance.itemInsertCount}");
        }
    }
}
