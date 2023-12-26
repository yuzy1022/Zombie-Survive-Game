using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

// 적 AI를 구현한다
public class Enemy : LivingEntity {
    public LayerMask whatIsTarget; // 추적 대상 레이어

    private LivingEntity targetEntity; // 추적할 대상
    private NavMeshAgent pathFinder; // 경로계산 AI 에이전트

    public ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    public AudioClip deathSound; // 사망시 재생할 소리
    public AudioClip hitSound; // 피격시 재생할 소리

    private Animator enemyAnimator; // 애니메이터 컴포넌트
    private AudioSource enemyAudioPlayer; // 오디오 소스 컴포넌트
    private Renderer enemyRenderer; // 렌더러 컴포넌트

    public float damage = 20f; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)  //프로퍼티에 조건추가
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake() {
        // 초기화
        pathFinder = GetComponent<NavMeshAgent>();
        enemyAnimator = GetComponent<Animator>();
        enemyAudioPlayer = GetComponent<AudioSource>();
        enemyRenderer = GetComponentInChildren<Renderer>();
    }

    // 적 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(float newHealth, float newDamage, float newSpeed, Color skinColor, float newPoint) {
        startingHealth = newHealth; //체력설정
        health = newHealth;//공격력설정
        point = newPoint;
        damage = newDamage;//내비메쉬 에이전트의 이동속도 설정
        pathFinder.speed = newSpeed; //렌더러가 사용중인 머티리얼의 컬러를 변경, 외형색이 변함
        enemyRenderer.material.color = skinColor;
    }

    private void Start() {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update() {
        // 추적 대상의 존재 여부에 따라 다른 애니메이션을 재생
        enemyAnimator.SetBool("HasTarget", hasTarget);
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    private IEnumerator UpdatePath() {
        
        while (!dead)// 좀비가 살아있는 동안 무한 루프
        {
            if(hasTarget){//추적대상 존재 : 경로를 갱신하고 AI이동을 계속 진행
                pathFinder.isStopped = false;
                pathFinder.SetDestination(targetEntity.transform.position);
            }

            else {//추적 대상 없음 : AI이동 중지
                pathFinder.isStopped = true;
                //좀비의 포지션을 기준으로 20유닛의 반지름을 가진 가상의 구를 그렸을때 구와 겹치는 모든 콜라이더를 가져옴. 단, WhatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget); 
                //살아있는 LivingEntity찾기
                for (int i = 0; i<colliders.Length; i++){
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();

                    if(livingEntity != null && !livingEntity.dead){ //LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아있다면
                        targetEntity = livingEntity; //추적대상을 해당 LivingEntity로 변경
                        break;
                    }
                }
            }
            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) {
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        if(!dead){ //좀비가 살아 있다면
            hitEffect.transform.position = hitPoint; //공격받은 지점과 방향으로 파티클 효과 발생
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            enemyAudioPlayer.PlayOneShot(hitSound);  //피격 효과음 재생
        }
        base.OnDamage(damage, hitPoint, hitNormal); //부모 (LivingEntity)의 OnDamage를 실행하여 데미지 적용
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();
        Collider[] enemyColliders = GetComponents<Collider>();  //다른 AI를 방해하지 않도록 자신의 모든 콜라이더를 비활성화
        for(int i = 0; i<enemyColliders.Length; i++){
            enemyColliders[i].enabled = false;
        }
        pathFinder.isStopped = true;  //Ai추적을 중지하고 내비메쉬 컴포넌트 비활성화
        pathFinder.enabled = false;
        enemyAnimator.SetTrigger("Die"); //사망 애니메이션 재생
        enemyAudioPlayer.PlayOneShot(deathSound); //효과음 재생
    }

    private void OnTriggerStay(Collider other) {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행   
        if(!dead && Time.time >= lastAttackTime + timeBetAttack){
            LivingEntity attackTarget = other.GetComponent<LivingEntity>(); //상대방의 LivingEntity타입 가져오기 시도
            
            if(attackTarget != null && attackTarget == targetEntity){ //상대방의 LivingEntity가 자신의 추적 대상이라면 공격 실행
                lastAttackTime = Time.time; //최근 공격시간 갱신
                Vector3 hitPoint = other.ClosestPoint(transform.position); //상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitNormal = transform.position - other.transform.position;
                attackTarget.OnDamage(damage, hitPoint, hitNormal); //공격실행
            }
        }
    }
}