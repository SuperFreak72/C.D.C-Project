using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI_Crown : MonoBehaviour
{
    public float moveSpeed = 10f;                 // AI 이동 속도
    public float rotationSpeed = 100f;            // 회전 속도
    public float roamRange = 40f;                 // 맵에서 돌아다닐 수 있는 최대 범위
    public float changeTargetInterval = 10f;      // 새로운 지점으로 이동하는 간격 시간
    public float chaseRange = 30f;                // 추격을 시작할 플레이어 감지 거리
    public float stopChaseRange = 35f;            // 추격을 멈출 거리
    public float detectionAngle = 45f;            // 감지할 수 있는 각도
    public Transform player;                       // 플레이어 트랜스폼
    public Animator animator;                      // 애니메이터
    private Rigidbody rb;

    private Vector3 currentWanderTarget;          // 현재 이동할 목표 지점
    private bool isWandering = false;             // 맵을 돌아다니는 상태인지 여부
    private bool isChasing = false;               // 플레이어 추격 상태
    private bool isStopped = false;               // AI가 정지 상태인지 여부
    private bool isAttacking = false;             // 공격 중인지 여부
    private float nextTargetTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();           // Rigidbody 할당
        animator = GetComponent<Animator>();      // Animator 할당
    }

    private void Start()
    {
        StartWandering();                         // 처음에 이동 시작
    }

    private void Update()
    {
        // 플레이어를 감지할 수 있는 상태이고, 멈춰있지 않은 상태일 때만 플레이어를 추격
        if (!isStopped && !isAttacking)
        {
            DetectPlayer();
        }

        // 추격 중일 때 실행
        if (isChasing && !isStopped && !isAttacking)
        {
            ChasePlayer();
        }

        // 맵을 돌아다니는 상태일 때만 실행
        if (isWandering && !isChasing && !isStopped && !isAttacking)
        {
            Wander();
        }
    }

    // 맵을 돌아다니는 기능 시작
    public void StartWandering()
    {
        isWandering = true;  // AI가 방황 상태에 있음을 표시
        isStopped = false;   // AI가 멈춰있지 않음을 표시
        isChasing = false;   // 추격 상태가 아님을 설정
        nextTargetTime = Time.time + changeTargetInterval; // 다음 이동 지점 변경 시간을 현재 시간 + 설정된 간격으로 초기화
        SetNextWanderTarget();  // 방황할 새로운 목표 지점을 설정
        Debug.Log("AI has started wandering.");  // 디버그 메시지 출력
    }

    // 맵을 돌아다니는 기능 중지
    public void StopWandering()
    {
        animator.SetBool("Idle", true);

        // Walk_F 트리거를 실행하고 바로 해제
        animator.SetTrigger("Walk_F");
        StartCoroutine(ResetTriggerAfterDelay("Walk_F", 2f));

        isStopped = true; // AI가 멈춰있음
        isWandering = false;
        isChasing = false; // 추격을 멈춤
        rb.velocity = Vector3.zero;  // 움직임을 멈춤
        Debug.Log("AI has stopped wandering.");

        // 2초 뒤에 Idle 상태 해제
        StartCoroutine(ResetIdleAfterDelay());
    }

    // 트리거 해제를 한 프레임 뒤에 실행
    private IEnumerator ResetTriggerAfterDelay(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        animator.ResetTrigger(triggerName); // 트리거 해제
    }

    private IEnumerator ResetIdleAfterDelay()
    {
        animator.SetBool("Chase", false);

        yield return new WaitForSeconds(2f);
        animator.SetBool("Idle", false);


        Debug.Log("AI is no longer idle.");
    }

    // AI가 방황하는 기능을 수행
    private void Wander()
    {
        StartCoroutine(WanderCoroutine());  // 방황 코루틴을 시작
    }

    // 방황 중인 상태에서 실행되는 코루틴
    private IEnumerator WanderCoroutine()
    {
        animator.SetBool("Pose", true);  // Pose 애니메이션을 실행하여 포즈 취하기

        yield return new WaitForSeconds(2f);  // 2초 동안 포즈 유지

        animator.SetBool("Pose", false);  // 포즈 애니메이션 종료

        // Walk 상태가 아닐 때만 트리거 호출
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            animator.SetTrigger("Walk_T"); // 트리거 실행
            yield return null; // 애니메이션이 시작될 때까지 한 프레임 기다림
            animator.ResetTrigger("Walk_T"); // 트리거 해제
        }

        // 다음 목표 지점을 설정할 시간이 되었는지 확인
        if (Time.time >= nextTargetTime)
        {
            SetNextWanderTarget();  // 새로운 목표 지점을 설정
            nextTargetTime = Time.time + changeTargetInterval;  // 다음 목표 변경 시간을 업데이트
        }

        MoveTowardsTarget(currentWanderTarget);  // 현재 목표 지점으로 이동

        yield return null;  // 다음 프레임까지 대기
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
            StartWandering(); // 다시 맵을 돌아다니기 시작
            Debug.Log("AI has stopped chasing the player.");
        }
    }

    private void ChasePlayer()
    {
        MoveTowardsTarget(player.position);
    }

    // 트리거 영역에 플레이어가 들어오면 공격 시작
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartAttack();
        }
    }

    // 트리거 영역에서 플레이어가 나가면 공격 중지
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
            isChasing = false; // 공격 중에는 추격을 멈춤
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
        while (isAttacking) // 공격 중일 때 계속 반복
        {
            // 공격 애니메이션을 재시작 (looping)
            yield return new WaitForSeconds(3f); // 애니메이션이 끝나는 시간을 기다림
            animator.SetBool("Attack", false);

        }

        // 공격이 끝난 후 다시 추격 모드로 전환
        if (!isStopped)
        {
            StartChasing();
        }
    }
}
