using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI_Snuffer : MonoBehaviour
{
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public GameObject smellObject;
    public float rotationSpeed = 5f;
    public float wanderRadius = 20f;
    public float wanderInterval = 5f;
    public float chaseDelay = 4f;
    public float sniffDuration = 4f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing;
    private bool isWaitingToChase;
    private bool isSniffing;
    private float wanderTimer;

    public Animator animator;

    private Monster_S monsterManager;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트 가져오기
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        wanderTimer = wanderInterval; // 초기 타이머 값 설정

        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                smellObject = playerController.smell; // 플레이어의 smellObject를 가져옴
            }
        }

        monsterManager = FindObjectOfType<Monster_S>();
    }

    private void Update()
    {
        if (smellObject != null && smellObject.activeSelf && !isWaitingToChase && !isChasing && !isSniffing)
        {
            StartSniffAnimation();
        }

        if (smellObject != null && !smellObject.activeSelf && isChasing)
        {
            StopChasing();
        }

        if (isChasing)
        {
            ChaseSmell();
        }
        else if (!isWaitingToChase && !isSniffing)
        {
            Wander();
        }
    }

    private void StartSniffAnimation()
    {
        isSniffing = true;
        agent.isStopped = true;
        animator.SetBool("Sniff", true);
        StartCoroutine(WaitForSniffToEnd());
    }

    private IEnumerator WaitForSniffToEnd()
    {
        yield return new WaitForSeconds(sniffDuration);
        animator.SetBool("Sniff", false);
        StartPoseAnimation();
    }

    private void StartPoseAnimation()
    {
        animator.SetBool("Pose", true);
        StartCoroutine(WaitBeforeChase());
    }

    private IEnumerator WaitBeforeChase()
    {
        isWaitingToChase = true;
        yield return new WaitForSeconds(chaseDelay);
        isChasing = true;
        animator.SetBool("Pose", false);
        animator.SetBool("Chase", true);
        agent.isStopped = false;
        isWaitingToChase = false;
        isSniffing = false;
    }

    private void StopChasing()
    {
        isChasing = false;
        animator.SetBool("Chase", false);
        agent.isStopped = false;
        Wander();
    }

    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            agent.speed = wanderSpeed;
            wanderTimer = 0f;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    private void ChaseSmell()
    {
        agent.speed = chaseSpeed;
        agent.destination = smellObject.transform.position;

        Vector3 directionToSmell = smellObject.transform.position - transform.position;
        directionToSmell.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToSmell);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // 플레이어와 충돌할 때 Attack 애니메이션 재생
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isChasing)  // 추적 중인 경우에만 공격
        {
            agent.isStopped = true;  // 충돌 시 이동 중지
            animator.SetBool("Attack", true);  // Attack 애니메이션 활성화

            // 애니메이션이 끝나면 다시 이동 재개 (필요한 경우 일정 시간 대기)
            StartCoroutine(ResumeAfterAttack());
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어의 위치를 가져와서 방향 설정
            Vector3 directionToPlayer = (collision.transform.position - transform.position).normalized;

            // 몬스터가 플레이어 쪽을 바라보게 회전
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f);

            animator.SetTrigger("Attack2");
        }

        if (collision.gameObject.CompareTag("Player")) // 플레이어 태그와 비교
        {
            int damage = 10; // 몬스터가 플레이어에게 주는 데미지
            Debug.Log("Collision with player detected.");

            if (gameObject.layer == 8) // "Monster" 레이어 확인
            {
                // 몬스터의 인덱스
                int monsterIndex = 0;
                monsterManager.M_Struct[monsterIndex].HandleCollision(collision.gameObject, damage); // 해당 몬스터에 피해 주기
            }
        }

    }

    // 공격 후 이동 재개
    private IEnumerator ResumeAfterAttack()
    {
        yield return new WaitForSeconds(1.5f);  // 공격 애니메이션이 끝날 때까지 대기
        animator.SetBool("Attack", false);  // Attack 애니메이션 비활성화
        agent.isStopped = false;  // 이동 재개
    }
}