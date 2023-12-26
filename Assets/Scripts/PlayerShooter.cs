using UnityEngine;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Gun gun, uzi, ak47, sniper, smaw; // 사용할 총
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점
    public Transform uziLeftHandMount; 
    public Transform uziRightHandMount;
    public Transform ak47LeftHandMount; 
    public Transform ak47RightHandMount;
    public Transform sniperLeftHandMount; 
    public Transform sniperRightHandMount;
    public Transform smawLeftHandMount; 
    public Transform smawRightHandMount;
    GameObject uziOb;
    GameObject ak47Ob;
    GameObject sniperOb;
    GameObject smawOb;
    public AudioClip changeWeaponSound;
    AudioSource audioPlayer;

    private PlayerInput playerInput; // 플레이어의 입력
    private Animator playerAnimator; // 애니메이터 컴포넌트

    private void Start() {
        // 사용할 컴포넌트들을 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Awake() {
        uziOb = GameObject.Find("Player Character/Gun Pivot/Gun");
        ak47Ob = GameObject.Find("Player Character/Gun Pivot/AK-47");
        sniperOb = GameObject.Find("Player Character/Gun Pivot/Sniper");
        smawOb = GameObject.Find("Player Character/Gun Pivot/SMAW");
        audioPlayer = GetComponent<AudioSource>();
    }
    private void OnEnable() {
        // 슈터가 활성화될 때 총도 함께 활성화
        gun.gameObject.SetActive(true);
    }
    
    private void OnDisable() {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        gun.gameObject.SetActive(false);
    }

    private void Update() {
        // 입력을 감지하고 총 발사하거나 재장전
        if (playerInput.fire)
        {
            gun.Fire(); //Fire()가 실행되면 총을 쏠 준비가 되어있는지 검사를 하여
            //파티클 이펙트, 소리 효과를 실행하고,
            //코루틴을 사용하여 라인렌더러를 0.03초 동안 활성화
        }
        else if (playerInput.reload) //R버튼을 눌러 재장전 
        {
            if (gun.Reload()) // 재장전할 수 있는 상태인지 검사를 하여 소리효과를 낸다음
                              // 1.8초간 대기하고 남은 탄알의 수를 계산해서 탄창을 채움
            {
                playerAnimator.SetTrigger("Reload"); //재장전 애니메이션 실행
            }
        }
        else if (playerInput.uzi && !GameManager.useUzi && !GameManager.reloading && !GameManager.changing){
            audioPlayer.PlayOneShot(changeWeaponSound);
            GameManager.changing = true;
            UIManager.instance.ChangeWeapon("uzi");
            GameManager.useAk47 = false;
            GameManager.useSmaw = false;
            GameManager.useSniper = false;
            GameManager.useUzi = true;
            gun = uzi;
            leftHandMount = uziLeftHandMount;
            rightHandMount = uziRightHandMount;
            OnAnimatorIK(1);
            ak47Ob.SetActive(false);
            sniperOb.SetActive(false);
            smawOb.SetActive(false);
            uziOb.SetActive(true);
            GameManager.changing = false;
        }
        else if (playerInput.ak47 && !GameManager.useAk47 && GameManager.buyAk47 && !GameManager.reloading && !GameManager.changing){
            audioPlayer.PlayOneShot(changeWeaponSound);
            GameManager.changing = true;
            UIManager.instance.ChangeWeapon("ak47");
            GameManager.useSmaw = false;
            GameManager.useSniper = false;
            GameManager.useUzi = false;
            GameManager.useAk47 = true;
            gun = ak47;
            leftHandMount = ak47LeftHandMount;
            rightHandMount = ak47RightHandMount;
            OnAnimatorIK(1);
            sniperOb.SetActive(false);
            smawOb.SetActive(false);
            uziOb.SetActive(false);
            ak47Ob.SetActive(true);
            GameManager.changing = false;
        }
        else if (playerInput.sniper && !GameManager.useSniper && GameManager.buySinper && !GameManager.reloading && !GameManager.changing){
            audioPlayer.PlayOneShot(changeWeaponSound);
            GameManager.changing = true;
            UIManager.instance.ChangeWeapon("sniper");
            GameManager.useSmaw = false;
            GameManager.useUzi = false;
            GameManager.useAk47 = false;
            GameManager.useSniper = true;
            gun = sniper;
            leftHandMount = sniperLeftHandMount;
            rightHandMount = sniperRightHandMount;
            OnAnimatorIK(1);
            smawOb.SetActive(false);
            uziOb.SetActive(false);
            ak47Ob.SetActive(false);
            sniperOb.SetActive(true);
            GameManager.changing = false;
        }
        else if (playerInput.smaw && !GameManager.useSmaw && GameManager.buySmaw && !GameManager.reloading && !GameManager.changing){
            audioPlayer.PlayOneShot(changeWeaponSound);
            GameManager.changing = true;
            UIManager.instance.ChangeWeapon("smaw");
            GameManager.useUzi = false;
            GameManager.useAk47 = false;
            GameManager.useSniper = false;
            GameManager.useSmaw = true;
            gun = smaw;
            leftHandMount = smawLeftHandMount;
            rightHandMount = smawRightHandMount;
            OnAnimatorIK(1);
            uziOb.SetActive(false);
            ak47Ob.SetActive(false);
            sniperOb.SetActive(false);
            smawOb.SetActive(true);
            GameManager.changing = false;
        }
        UpdateUI();

    }

    // 탄약 UI 갱신
    private void UpdateUI() {
        if (gun != null && UIManager.instance != null)
        {
            // UI 매니저의 탄약 텍스트에 탄창의 탄약과 남은 전체 탄약을 표시
            UIManager.instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
        }
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex) {
        //총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        gunPivot.position = playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);
        //IK를 사용하여 왼손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);

        //IK를 사용하여 오른손의 위치와 회전을 총의 왼쪽 손잡이에 맞춤
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandMount.rotation);
    }
}