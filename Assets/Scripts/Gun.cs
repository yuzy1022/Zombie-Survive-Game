using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

// 총을 구현한다
public class Gun : MonoBehaviour {
    // 총의 상태를 표현하는데 사용할 타입을 선언한다
    public enum State { //열거형 변수. 처음에 설정된 변수를 기준으로 순서대로 설정됨 ex) ready=5면 empty=6 reloading=7
                        //초기값을 설정 안하면 자동으로 0부터 시작됨
        Ready, // 발사 준비됨
        Empty, // 탄창이 빔
        Reloading // 재장전 중
    }
    public string gun;
    public State state {get; private set; } // 현재 총의 상태

    public Transform fireTransform; // 총알이 발사될 위치

    public ParticleSystem muzzleFlashEffect; // 총구 화염 효과
    public ParticleSystem shellEjectEffect; // 탄피 배출 효과

    private LineRenderer bulletLineRenderer; // 총알 궤적을 그리기 위한 렌더러

    private AudioSource gunAudioPlayer; // 총 소리 재생기
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage; // 공격력
    public float fireDistance; // 사정거리

    public int ammoRemain; // 남은 전체 탄약
    public int magCapacity; // 탄창 용량
    public int magAmmo; // 현재 탄창에 남아있는 탄약
    public string etc = "";

    public float timeBetFire; // 총알 발사 간격
    public float reloadTime; // 재장전 소요 시간
    // 총을 마지막으로 발사한 시점, smaw의 범위(큰, 중간, 작은)
    private float lastFireTime, smawBigRange=(float)2.25, smawMiddleRange=(float)1.5, smawsmallRange=(float)0.75, piercingDecrease=(float)0.30; 

    public GameObject boomEffect;
    public bool sniperPiercingShoot = false, smawSelfDamage=true;
    private int piercingNum = 4;
    int layerMask;  //레이캐스트 할때 item레이어를 제외하기 위한 레이어마스크



    private void Awake() {
        gun = gameObject.name;
        // 사용할 컴포넌트들의 참조를 가져오기
        gunAudioPlayer = GetComponent<AudioSource>();
        bulletLineRenderer = GetComponent<LineRenderer>();
        bulletLineRenderer.positionCount = 2;//라인렌더러의 시작점과 끝점
        //시작점-총구끝, 끝점- 총에맞은 좀비위 위치, 총에 안맞으면 50미터 앞
        bulletLineRenderer.enabled = false; //라인렌더러는 총을 쏠때만 활성화
        layerMask = ~(1 << LayerMask.NameToLayer("Item"));  //레이어마스크. 레이캐스트 할때 아이템 레이어는 충돌검사 하지 않는다
        if(gun == "Gun"){
            damage = 20;
            fireDistance = 5;
            ammoRemain = 150;
            magCapacity = 50;
            timeBetFire = 0.12f;
            reloadTime = 1.8f;
        }
        else if(gun == "AK-47"){
            damage = 35;
            fireDistance = 10;
            ammoRemain = 90;
            magCapacity = 30;
            timeBetFire = 0.1f;
            reloadTime = 2.5f;
            gameObject.SetActive(false);
        }
        else if(gun == "Sniper"){
            damage = 150;
            fireDistance = 20;
            ammoRemain = 30;
            magCapacity = 10;
            timeBetFire = 0.75f;
            reloadTime = 3.2f;
            gameObject.SetActive(false);
        }
        else if(gun == "SMAW"){
            damage = 160;
            fireDistance = 30;
            ammoRemain = 3;
            magCapacity = 1;
            timeBetFire = 3f;
            reloadTime = 5f;
            etc = "Range Attack,\nSelf Damage Possible";
            gameObject.SetActive(false);
        }
        magAmmo = magCapacity; //현재 탄창을 가득채우기
        state = State.Ready; //총의 현재 상태를 총을 쏠 준비가 된 상태로 변경
        lastFireTime = 0; // 마지막으로 총을 쏜 시점을 초기화
    }


    // 발사 시도
    public void Fire() {
        //현재 발사가 가능하며, 마지막 총 발사시점에서 0.12초가 지났다면
        if (state == State.Ready && Time.time >= lastFireTime + timeBetFire && !GameManager.changing)
        {
            lastFireTime = Time.time; //마지막 총 발사시간 갱신
            Shot();//실제 발사 처리
        }
    }

    // 실제 발사 처리
    private void Shot() {
        RaycastHit hit;// 레이캐스트에 의한 충돌 정보를 저장하는 컨테이너
        Vector3 hitPosition = Vector3.zero; //탄알이 맞은곳을 저장할 변수
        //레이캐스트(시작 지점, 방향, 충돌 정보 컨테이너, 사정거리)
        if(gun != "SMAW" ){
            if(gun != "Sniper" || (gun=="Sniper" && !sniperPiercingShoot)){  //gun이 스나이퍼가 아니거나, 스나이퍼지만 업글 10미만일경우
                if (Physics.Raycast(fireTransform.position, fireTransform.forward, out hit, fireDistance, layerMask))  //레이가 어떤 물체와 충돌한 경우
                {   
                    hitPosition = hit.point;//레이가 충돌한 위치 저장
                    //충돌한 상대방으로부터 IDamageable 오브젝트 가져오기 시도
                    IDamageable target = hit.collider.GetComponent<IDamageable>();
                    //상대방으로부터 IDamageable 오브젝트를 가져오는데 성공했다면
                if (target != null)
                {   
                    //상대방의 OnDamage 함수를 실행시켜 상대방에 대미지 주기
                    target.OnDamage(damage, hitPosition, hit.normal);
                }
                
                }
                else //레이가 아무것도 부딪히지 않았다면..
                { 
                //레이가 다른 물체와 충돌하지 않았다면
                //탄알이 최대 사정거리까지 날아갔을 때의 위치를 충돌 위치로 사용
                hitPosition = fireTransform.position +
                    fireTransform.forward * fireDistance;
                }
            }

            else if(gun=="Sniper" && sniperPiercingShoot){  //gun이 스나이퍼이고 업글 10인경우
                RaycastHit[] hits = Physics.RaycastAll(fireTransform.position, fireTransform.forward, fireDistance, layerMask);  //ray를 오브젝트를 관통시켜서 쏜 뒤 충돌한 오브젝트를 배열로 모두 가져옴
                if(hits.Length != 0){  //충돌한 오브젝트가 0 보다 많을 때
                    System.Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));  //충돌한 오브젝트를 거리순으로 정렬
                    int n=0;

                    if(hits.Length <= piercingNum){  //충돌한 오브젝트 개수가 piercingNum 이하일때
                        hitPosition = hits[hits.Length-1].point;  //가장 마지막 오브젝트의 위치를 hitPosition으로 설정
                        n = hits.Length;  //오브젝트 개수만큼 반복
                    }
                    else{  //충돌한 오브젝트 개수가 piercingNum 보다 많을 때
                        hitPosition = hits[piercingNum-1].point;   //piercingNum번째 오브젝트의 위치를 hitPosition으로 설정
                        n = piercingNum;  //piercingNum 수만큼 만복
                    }

                    int hitCount=0;
                    for(int i = 0; i < n; i++){ //위에서 설정한 반복 횟수만큼 반복, 충돌한 오브젝트를 불러와 데미지를 준다
                        IDamageable target = hits[i].collider.GetComponent<IDamageable>();
                        int piercingDamage = (int)(damage - damage*piercingDecrease*hitCount++);
                        if (target != null) target.OnDamage(piercingDamage, hitPosition, hits[i].normal); //상대방의 OnDamage 함수를 실행시켜 상대방에 대미지 주기
                    }
                }

                else{ hitPosition = fireTransform.position + fireTransform.forward * fireDistance; } //충돌한 오브젝트가 없을 때 탄알이 최대 사정거리까지 날아갔을 때의 위치를 충돌 위치로 사용
                
            }
            
            StartCoroutine(ShotEffect(hitPosition));//발사 이펙트 시작
        }

        else if(gun == "SMAW"){  //gun이 smaw인 경우
            if(Physics.SphereCast(fireTransform.position, (float)0.25, fireTransform.forward, out hit, fireDistance, layerMask))
            {//레이가 어떤 물체와 충돌한 경우
                hitPosition = hit.point;//레이가 충돌한 위치 저장
                StartCoroutine(ShotEffect(hitPosition));//발사 이펙트 시작
                Instantiate(boomEffect, hitPosition, hit.transform.rotation);

                Collider[] hitCollidersSmallCircle = Physics.OverlapSphere(hitPosition, smawsmallRange);  //작은범위
                Collider[] hitCollidersMiddleCircle = Physics.OverlapSphere(hitPosition, smawMiddleRange);  //중간범위
                Collider[] hitCollidersBigCircle = Physics.OverlapSphere(hitPosition, smawBigRange); //큰범위
                
                for(int i = 0; i < hitCollidersSmallCircle.Length; i++){  //smallcircle안에 오브젝트가 있다면 데미지 적용
                    IDamageable smallTarget = hitCollidersSmallCircle[i].GetComponent<IDamageable>();
                    //스몰서클은 100%데미지,  상대방의 OnDamage 함수를 실행시켜 상대방에 대미지 주기 
                    if((smawSelfDamage || (!smawSelfDamage && hitCollidersSmallCircle[i].tag != "Player")) && smallTarget != null){ smallTarget.OnDamage(damage, hitPosition, hit.normal); } 
                }  

                for(int i = 0; i < hitCollidersMiddleCircle.Length; i++){ //미들서클 범위를 적용할 오브젝트 찾기
                    int count = 0;

                    for(int u = 0; u < hitCollidersSmallCircle.Length; u++){ //미들서클 안에 있으면서 스몰서클 안에도 있으면 대상에서 제외(이미 스몰서클 데미지를 받았으므로)
                        if(hitCollidersMiddleCircle[i] == hitCollidersSmallCircle[u]){
                            count++;
                            break;
                        }
                    } 
                
                    if(count == 0){ //미들서클안에 있으면서 스몰서클 안에는 없다면 데미지 적용
                        IDamageable MiddleTarget = hitCollidersMiddleCircle[i].GetComponent<IDamageable>();
                        //상대방으로부터 IDamageable 오브젝트를 가져오는데 성공했다면
                        if ((smawSelfDamage || (!smawSelfDamage && hitCollidersMiddleCircle[i].tag != "Player")) && MiddleTarget != null)
                        {
                            //상대방의 OnDamage 함수를 실행시켜 상대방에 대미지 주기
                            MiddleTarget.OnDamage((float)(damage*0.8), hitPosition, hit.normal); //미들서클은 80%데미지
                        }
                    } 
                }

                for(int i = 0; i<hitCollidersBigCircle.Length; i++){  //빅서클 범위를 적용할 오브젝트 찾기
                    int count = 0;

                    for(int u = 0; u < hitCollidersMiddleCircle.Length; u++){  //빅서클 안에 있으면서 미들서클 안에도 있으면 제외
                        if(hitCollidersBigCircle[i] == hitCollidersMiddleCircle[u]){
                            count++;
                            break;
                        }
                    }

                    if(count == 0){  //빅서클 안에 있으면서 미들서클 안에 없다면 데미지 적용
                        IDamageable BigTarget = hitCollidersBigCircle[i].GetComponent<IDamageable>();
                        //상대방으로부터 IDamageable 오브젝트를 가져오는데 성공했다면
                        if ((smawSelfDamage || (!smawSelfDamage && hitCollidersBigCircle[i].tag != "Player")) && BigTarget != null)
                        {
                            //상대방의 OnDamage 함수를 실행시켜 상대방에 대미지 주기
                            BigTarget.OnDamage((float)(damage*0.6), hitPosition, hit.normal);  //빅서클은 60%데미지
                        }
                    } 
                }
            }

            else //레이가 아무것도 부딪히지 않았다면..
            {
                //레이가 다른 물체와 충돌하지 않았다면
                //탄알이 최대 사정거리까지 날아갔을 때의 위치를 충돌 위치로 사용
                hitPosition = fireTransform.position +
                fireTransform.forward * fireDistance;
                StartCoroutine(ShotEffect(hitPosition));//발사 이펙트 시작
            }
        }
        
        magAmmo--; //남은 탄알의수 -1
        if (magAmmo <= 0)
        {
            state = State.Empty;//탄창에 남은 탄알이 없으면 총의 상태를 Empty로..
        }
    }

    // 발사 이펙트와 소리를 재생하고 총알 궤적을 그린다
    private IEnumerator ShotEffect(Vector3 hitPosition) {
        muzzleFlashEffect.Play();//총구화염 효과 재생
        if(gun != "SMAW") {shellEjectEffect.Play();} //탄피배출 효과 재생
        gunAudioPlayer.PlayOneShot(shotClip); //총소리
        //선의 시작점은 총구의 위치
        bulletLineRenderer.SetPosition(0, fireTransform.position);
        //끝점은 입력으로 들어온 충돌위치
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;
        // 라인 렌더러를 활성화하여 총알 궤적을 그린다.
        // 0.03초 동안 잠시 처리를 대기
        yield return new WaitForSeconds(0.03f);

        // 라인 렌더러를 비활성화하여 총알 궤적을 지운다
        bulletLineRenderer.enabled = false;
    }

    // 재장전 시도
    public bool Reload() {
        //현재 상태가 리로딩 중이거나, 남은 탄의 수가 없거나, 현재 탄창의 수가 25이상이라면 재장전안됨
        if (state == State.Reloading || ammoRemain <= 0 || magAmmo >= magCapacity || GameManager.changing)
        {
            return false;
        }
        StartCoroutine(ReloadRoutine()); //재장전 코루틴 함수 실행
        return true;
    }

    // 실제 재장전 처리를 진행
    private IEnumerator ReloadRoutine() {
        // 현재 상태를 재장전 중 상태로 전환
        UIManager.instance.ReloadTextActive(true);
        GameManager.reloading = true;
        state = State.Reloading;
        gunAudioPlayer.PlayOneShot(reloadClip);
        // 재장전 소요 시간 만큼 처리를 쉬기
        yield return new WaitForSeconds(reloadTime);  //처리를 쉬는동안 컴파일러가 쉬는것이 아니라 다른 처리를 하고 돌아오도록 한다.
        int ammoToFill = magCapacity - magAmmo;//탄창에 채울 탄알을 계산
        //탄창에 채워야할 탄알이 남은 탄앏다 많다면
        //채워야할 탄알 수를 남은 탄알 수에 맞춰 줄임
        if (ammoRemain < ammoToFill)
        {
            ammoToFill = ammoRemain;
        }
        magAmmo += ammoToFill; //탄창을 채움
        ammoRemain -= ammoToFill; //남은 탄알에서 탄창에 채운만큼 탄알을 뺌 ex) 탄창에 15발 남았을때 재장전시 15발 남은건 버리는게 아니라 10발만 더 탄창에 충전한다.
        // 총의 현재 상태를 발사 준비된 상태로 변경
        state = State.Ready; //총의 현재 상태를 발사 준비된 상태로 변경
        GameManager.reloading = false;
        UIManager.instance.ReloadTextActive(false);
    }
}