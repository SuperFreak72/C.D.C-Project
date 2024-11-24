using System.Collections;
using UnityEngine;

public class MonsterAI_Grab : MonoBehaviour
{
    public float wanderSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 10f;
    public float viewAngle = 45f;
    public GameObject blindCollider;
    public GameObject grabCollider;
    public float grabSlowDuration = 3f;
    public float grabSlowAmount = 0.5f;
    public float loseSightDistance = 15f;
    public float rotationSpeed = 5f;
    public float jumpAttackChance = 0.2f;  // 점프 공격 확률 (20%)
    public float jumpAttackCooldown = 5f;  // 점프 공격 쿨다운 시간

    private Transform player;
    private bool isChasing;
    private float wanderTimer;
    private Rigidbody rb;
    private Animator animator;
    private PlayerMove playerMovement;
    private Vector3 wanderDirection;
    private bool canJumpAttack = true;     // 점프 공격이 가능한지 여부
    private bool isAttacking = false;      // 공격 중인지 여부

    private Monster_S monsterManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMove>();

        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMove 스크립트가 플레이어에 할당되지 않았습니다.");
        }

        SetRandomWanderDirection();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                grabCollider = playerController.Grab;
            }
        }

        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null)
            {
                blindCollider = playerController.blind; 
            }
        }

        monsterManager = FindObjectOfType<Monster_S>();
    }

    private void Update()
    {
        if (!isAttacking)  // 공격 중이 아닐 때만 추격하거나 이동
        {
            if (CanSeePlayer())
            {
                isChasing = true;
            }

            if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                Wander();
            }
        }

        // 이동 중이면 걷기 애니메이션
        if (rb.velocity.magnitude > 0.1f)
        {
            animator.SetBool("G_Walk", true);
        }
        else
        {
            animator.SetBool("G_Walk", false);
        }
    }

    // 플레이어 감지
    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= viewAngle)
            {
                return true;
            }
        }
        return false;
    }

    // 자유 이동
    private void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= 5f)
        {
            SetRandomWanderDirection();
        }

        Quaternion targetRotation = Quaternion.LookRotation(wanderDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.velocity = wanderDirection * wanderSpeed;
    }

    private void SetRandomWanderDirection()
    {
        wanderTimer = 0f;
        wanderDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        float randomSpeedFactor = Random.Range(0.8f, 1.2f);
        rb.velocity = wanderDirection * wanderSpeed * randomSpeedFactor;
    }

    // 플레이어 추격
    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        rb.velocity = directionToPlayer * chaseSpeed;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 점프 공격 시도
        if (distanceToPlayer <= loseSightDistance && canJumpAttack && Random.value <= jumpAttackChance)
        {
            StartCoroutine(PerformJumpAttack());  // 점프 공격 실행
        }

        if (distanceToPlayer <= 0.5f)
        {
            ActivateRandomSkill();
            Destroy(gameObject);
        }
        else if (distanceToPlayer >= loseSightDistance)
        {
            isChasing = false;
        }
    }

    // 점프 공격 코루틴
    private IEnumerator PerformJumpAttack()
    {
        isAttacking = true;
        canJumpAttack = false;  // 점프 공격 쿨다운 시작

        // 점프 애니메이션 실행
        animator.SetTrigger("G_jump");

        // 속도를 증가시킴
        float originalSpeed = chaseSpeed;  // 원래 속도를 저장
        chaseSpeed *= 3f;  // 속도 증가
        rb.velocity = (player.position - transform.position).normalized * chaseSpeed; // 플레이어 방향으로 속도 조정

        yield return new WaitForSeconds(1f);  // 점프 모션 시간

        // 공격 애니메이션 실행
        animator.SetTrigger("G_Attack");

        // 공격 애니메이션 동안 속도 유지
        rb.velocity = Vector3.zero;  // 공격 중 이동을 멈춤
        yield return new WaitForSeconds(1f);  // 공격 모션 시간

        // 종료 후 idle로 전환
        animator.SetTrigger("G_idle");

        // 원래 속도로 복귀
        chaseSpeed = originalSpeed; // 속도를 원래대로 복귀
        rb.velocity = Vector3.zero;  // 이동을 멈춤

        yield return new WaitForSeconds(0.5f);  // idle 상태로 돌아가기 전 약간의 시간

        isAttacking = false;

        // 점프 공격 쿨다운 타이머
        yield return new WaitForSeconds(jumpAttackCooldown);
        canJumpAttack = true;  // 쿨다운 종료 후 점프 공격 가능
    }


    // 플레이어와 충돌 시
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ActivateRandomSkill();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player")) // 플레이어 태그와 비교
        {
            int damage = 10; // 몬스터가 플레이어에게 주는 데미지
            Debug.Log("Collision with player detected.");

            if (gameObject.layer == 8) // "Monster" 레이어 확인
            {
                // 몬스터의 인덱스를 적절히 설정해야 합니다.
                int monsterIndex = 2;
                monsterManager.M_Struct[monsterIndex].HandleCollision(collision.gameObject, damage); // 해당 몬스터에 피해 주기
            }
        }
    }

    private void ActivateRandomSkill()
    {
        float randomValue = Random.value;

        if (randomValue <= 0.5f)
        {
            ActivateBlind();
        }
        else
        {
            ActivateGrab();
        }
    }

    private void ActivateBlind()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerMove playerController = playerObject.GetComponent<PlayerMove>();
            if (playerController != null && !playerController.blind.activeSelf) // 플레이어의 blindCollider가 비활성화 상태일 때
            {
                playerController.blind.SetActive(true); // 블라인드 활성화
                playerController.DisableBlindAfterDelay(5f); // 5초 후 비활성화 호출
            }
        }
    }

    private IEnumerator DisableBlindAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간 동안 대기

        GameObject blindObject = GameObject.FindGameObjectWithTag("Blind"); // "Blind" 태그를 가진 오브젝트 찾기
        if (blindObject != null)
        {
            blindObject.SetActive(false); // 블라인드 비활성화
        }
    }


    private void ActivateGrab()
    {
        if (!grabCollider.activeSelf)
        {
            grabCollider.SetActive(true);
            StartCoroutine(SlowPlayer());
        }
    }

    private IEnumerator SlowPlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.speed *= grabSlowAmount;
            yield return new WaitForSeconds(grabSlowDuration);
            playerMovement.speed /= grabSlowAmount;
        }
    }
}
