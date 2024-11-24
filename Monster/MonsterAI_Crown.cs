using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI_Crown : MonoBehaviour
{
    public float moveSpeed = 10f;                 // AI �̵� �ӵ�
    public float rotationSpeed = 100f;            // ȸ�� �ӵ�
    public float roamRange = 40f;                 // �ʿ��� ���ƴٴ� �� �ִ� �ִ� ����
    public float changeTargetInterval = 10f;      // ���ο� �������� �̵��ϴ� ���� �ð�
    public float chaseRange = 30f;                // �߰��� ������ �÷��̾� ���� �Ÿ�
    public float stopChaseRange = 35f;            // �߰��� ���� �Ÿ�
    public float detectionAngle = 45f;            // ������ �� �ִ� ����
    public Transform player;                       // �÷��̾� Ʈ������
    public Animator animator;                      // �ִϸ�����
    private Rigidbody rb;

    private Vector3 currentWanderTarget;          // ���� �̵��� ��ǥ ����
    private bool isWandering = false;             // ���� ���ƴٴϴ� �������� ����
    private bool isChasing = false;               // �÷��̾� �߰� ����
    private bool isStopped = false;               // AI�� ���� �������� ����
    private bool isAttacking = false;             // ���� ������ ����
    private float nextTargetTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();           // Rigidbody �Ҵ�
        animator = GetComponent<Animator>();      // Animator �Ҵ�
    }

    private void Start()
    {
        StartWandering();                         // ó���� �̵� ����
    }

    private void Update()
    {
        // �÷��̾ ������ �� �ִ� �����̰�, �������� ���� ������ ���� �÷��̾ �߰�
        if (!isStopped && !isAttacking)
        {
            DetectPlayer();
        }

        // �߰� ���� �� ����
        if (isChasing && !isStopped && !isAttacking)
        {
            ChasePlayer();
        }

        // ���� ���ƴٴϴ� ������ ���� ����
        if (isWandering && !isChasing && !isStopped && !isAttacking)
        {
            Wander();
        }
    }

    // ���� ���ƴٴϴ� ��� ����
    public void StartWandering()
    {
        isWandering = true;  // AI�� ��Ȳ ���¿� ������ ǥ��
        isStopped = false;   // AI�� �������� ������ ǥ��
        isChasing = false;   // �߰� ���°� �ƴ��� ����
        nextTargetTime = Time.time + changeTargetInterval; // ���� �̵� ���� ���� �ð��� ���� �ð� + ������ �������� �ʱ�ȭ
        SetNextWanderTarget();  // ��Ȳ�� ���ο� ��ǥ ������ ����
        Debug.Log("AI has started wandering.");  // ����� �޽��� ���
    }

    // ���� ���ƴٴϴ� ��� ����
    public void StopWandering()
    {
        animator.SetBool("Idle", true);

        // Walk_F Ʈ���Ÿ� �����ϰ� �ٷ� ����
        animator.SetTrigger("Walk_F");
        StartCoroutine(ResetTriggerAfterDelay("Walk_F", 2f));

        isStopped = true; // AI�� ��������
        isWandering = false;
        isChasing = false; // �߰��� ����
        rb.velocity = Vector3.zero;  // �������� ����
        Debug.Log("AI has stopped wandering.");

        // 2�� �ڿ� Idle ���� ����
        StartCoroutine(ResetIdleAfterDelay());
    }

    // Ʈ���� ������ �� ������ �ڿ� ����
    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay); // ������ �ð���ŭ ���
        animator.ResetTrigger(triggerName); // Ʈ���� ����
    }

    private IEnumerator ResetIdleAfterDelay()
    {
        animator.SetBool("Chase", false);

        yield return new WaitForSeconds(2f);
        animator.SetBool("Idle", false);


        Debug.Log("AI is no longer idle.");
    }

    // AI�� ��Ȳ�ϴ� ����� ����
    private void Wander()
    {
        StartCoroutine(WanderCoroutine());  // ��Ȳ �ڷ�ƾ�� ����
    }

    // ��Ȳ ���� ���¿��� ����Ǵ� �ڷ�ƾ
    private IEnumerator WanderCoroutine()
    {
        animator.SetBool("Pose", true);  // Pose �ִϸ��̼��� �����Ͽ� ���� ���ϱ�

        yield return new WaitForSeconds(2f);  // 2�� ���� ���� ����

        animator.SetBool("Pose", false);  // ���� �ִϸ��̼� ����

        // Walk ���°� �ƴ� ���� Ʈ���� ȣ��
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            animator.SetTrigger("Walk_T"); // Ʈ���� ����
            yield return null; // �ִϸ��̼��� ���۵� ������ �� ������ ��ٸ�
            animator.ResetTrigger("Walk_T"); // Ʈ���� ����
        }

        // ���� ��ǥ ������ ������ �ð��� �Ǿ����� Ȯ��
        if (Time.time >= nextTargetTime)
        {
            SetNextWanderTarget();  // ���ο� ��ǥ ������ ����
            nextTargetTime = Time.time + changeTargetInterval;  // ���� ��ǥ ���� �ð��� ������Ʈ
        }

        MoveTowardsTarget(currentWanderTarget);  // ���� ��ǥ �������� �̵�

        yield return null;  // ���� �����ӱ��� ���
    }

    private void SetNextWanderTarget()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-roamRange, roamRange), 0, Random.Range(-roamRange, roamRange));
        currentWanderTarget = transform.position + randomDirection;
        Debug.Log("New wander target set at: " + currentWanderTarget);
    }

    private void MoveTowardsTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;

        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    private void DetectPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && IsPlayerInFront())
        {
            StartChasing();
        }
        else if (distanceToPlayer >= stopChaseRange)
        {
            StopChasing();
        }
    }

    private bool IsPlayerInFront()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        return angleToPlayer <= detectionAngle / 2;
    }

    private void StartChasing()
    {
        if (!isChasing)
        {
            animator.SetBool("Chase", true);
            isChasing = true;
            isWandering = false;
            Debug.Log("AI has started chasing the player.");
        }
    }

    private void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            animator.SetBool("Chase", false);
            StartWandering(); // �ٽ� ���� ���ƴٴϱ� ����
            Debug.Log("AI has stopped chasing the player.");
        }
    }

    private void ChasePlayer()
    {
        MoveTowardsTarget(player.position);
    }

    // Ʈ���� ������ �÷��̾ ������ ���� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartAttack();
        }
    }

    // Ʈ���� �������� �÷��̾ ������ ���� ����
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAttack();
        }
    }

    private void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            isChasing = false; // ���� �߿��� �߰��� ����
            animator.SetTrigger("Attack_T");
            Debug.Log("AI started attacking the player.");
            StartCoroutine(AttackCoroutine());
        }
    }

    private void StopAttack()
    {
        isAttacking = false;
        animator.SetBool("Attack", false);
        Debug.Log("AI stopped attacking the player.");
    }

    private IEnumerator AttackCoroutine()
    {
        while (isAttacking) // ���� ���� �� ��� �ݺ�
        {
            // ���� �ִϸ��̼��� ����� (looping)
            yield return new WaitForSeconds(3f); // �ִϸ��̼��� ������ �ð��� ��ٸ�
            animator.SetBool("Attack", false);

        }

        // ������ ���� �� �ٽ� �߰� ���� ��ȯ
        if (!isStopped)
        {
            StartChasing();
        }
    }
}
