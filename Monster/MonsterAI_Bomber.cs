using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI_Bomber : MonoBehaviour
{
    public float speed = 3f;             // 기본 몬스터 이동 속도
    public float chaseSpeed = 6f;        // 플레이어 추적 속도
    public Transform player;             // 플레이어의 위치
    public float detectionRange = 5f;    // 몬스터가 플레이어를 감지할 거리
    public float detectionAngle = 45f;   // 플레이어를 감지할 각도
    public GameObject smellObject;       // 플레이어의 Smell 오브젝트
    public float rotationSpeed = 5f;     // 몬스터의 회전 속도
    public float wanderRadius = 20f;     // AI가 자유롭게 돌아다닐 수 있는 반경
    public float wanderInterval = 5f;    // 새로운 랜덤 위치로 이동하는 간격
    public Animator animator;            // 몬스터의 애니메이션을 처리하는 Animator

    private NavMeshAgent navMeshAgent;   // NavMeshAgent 컴포넌트
    private float wanderTimer;           // 자유롭게 이동하는 시간 타이머
    private bool isWaiting = false;      // 대기 중인지 여부를 나타내는 변수

    private PlayerMove playerMove; // 플레이어 스크립트 참조 변수

    private Monster_S monsterManager;

    private void Start()
    {
        // NavMeshAgent를 가져오고 기본 이동 속도를 설정
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;

        // 초기 타이머 값 설정
        wanderTimer = wanderInterval;

        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        // 초기 랜덤 위치 설정
        Wander();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                player = playerObject.transform; // 플레이어의 Transform을 가져옴
                playerMove = playerObject.GetComponent<PlayerMove>(); // 플레이어 Move 스크립트 가져오기
                smellObject = playerController.smell; // 플레이어의 smellObject를 가져옴
            }
        }

        monsterManager = FindObjectOfType<Monster_S>();

        // NavMeshAgent가 비활성화되어 있다면 활성화
        if (navMeshAgent != null && !navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.enabled = true;
        }
    }

    private void Update()
    {
        // NavMeshAgent가 활성화되어 있는지 확인
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled)
        {
            Debug.LogWarning("NavMeshAgent is not active or not found!");
            return;
        }

        // 플레이어와의 거리 및 시야 확인
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (IsPlayerInSight(distanceToPlayer) && !isWaiting)
        {
            // 플레이어를 감지하면 코루틴 실행 (1초 대기 및 애니메이션)
            StartCoroutine(DetectAndPause());
        }
        else if (!isWaiting)
        {
            // 자유롭게 맵을 돌아다니기
            Wander();
        }
    }

    // 플레이어를 감지하고 1초 대기하는 코루틴
    private IEnumerator DetectAndPause()
    {
        isWaiting = true; // 대기 상태로 전환
        animator.SetBool("Pose", true);

        // 2초 대기
        yield return new WaitForSeconds(2f);
        animator.SetBool("Pose", false);

        // 추적 속도와 애니메이션 설정
        animator.SetBool("Run", true);
        navMeshAgent.speed = chaseSpeed;  // 추적할 때 속도를 빠르게 변경

        // 대기 상태를 해제하고 다시 플레이어 추적
        navMeshAgent.SetDestination(player.position);
        LookAtPlayer();

        isWaiting = false; // 대기 상태 해제
    }

    // AI가 자유롭게 돌아다니는 로직
    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            // 새로운 랜덤 위치 설정
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);
            wanderTimer = 0f;

            // 추적하지 않을 때 기본 속도로 변경
            navMeshAgent.speed = speed;
            animator.SetBool("Run", false); // AI가 플레이어를 놓치면 다시 idle 상태가 됨
        }
    }

    // 지정된 반경 내에서 랜덤한 NavMesh 상의 위치를 찾는 함수
    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance; // 반경 내에서 랜덤한 방향 설정
        randomDirection += origin; // 시작 위치에 더함

        NavMeshHit navHit;
        // NavMesh 상의 유효한 위치를 찾아 반환
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    // 몬스터의 머리가 플레이어를 향하도록 회전
    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // 플레이어와 충돌 시 Smell 오브젝트를 활성화하고 몬스터를 제거
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (smellObject != null)
            {
                smellObject.SetActive(true);
            }

            // 플레이어 스태미너를 40 감소시킴
            if (playerMove != null)
            {
                playerMove.ReduceStamina(40f);
            }

            if (collision.gameObject.CompareTag("Player")) // 플레이어 태그와 비교
            {
                int damage = 10; // 몬스터가 플레이어에게 주는 데미지
                Debug.Log("Collision with player detected.");

                if (gameObject.layer == 8) // "Monster" 레이어 확인
                {
                    // 몬스터의 인덱스를 적절히 설정
                    int monsterIndex = 1;
                    monsterManager.M_Struct[monsterIndex].HandleCollision(collision.gameObject, damage); // 해당 몬스터에 피해 주기
                }
            }

            Destroy(gameObject);
        }
    }

    // 플레이어가 몬스터의 시야에 있는지 확인하는 함수
    private bool IsPlayerInSight(float distanceToPlayer)
    {
        // 플레이어가 감지 범위 내에 있는지 확인
        if (distanceToPlayer > detectionRange) return false;

        // 플레이어가 캐릭터의 정면 각도 범위 내에 있는지 확인
        Vector3 directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < detectionAngle;
    }
}
