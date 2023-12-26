using UnityEngine;

// 총알을 충전하는 아이템
public class AmmoPack : MonoBehaviour, IItem {
    public int ammo; // 충전할 총알 수

    public void Use(GameObject target) {
        // 전달 받은 게임 오브젝트로부터 PlayerShooter 컴포넌트를 가져오기 시도
        PlayerShooter playerShooter = target.GetComponent<PlayerShooter>();
        if(GameManager.useUzi){
            ammo = GameObject.Find("Player Character/Gun Pivot/Gun").GetComponent<Gun>().magCapacity;
        }
        else if(GameManager.useAk47){
            ammo = GameObject.Find("Player Character/Gun Pivot/AK-47").GetComponent<Gun>().magCapacity;
        }
        else if(GameManager.useSniper){
            ammo = GameObject.Find("Player Character/Gun Pivot/Sniper").GetComponent<Gun>().magCapacity;
        }
        else if(GameManager.useSmaw){
            ammo = GameObject.Find("Player Character/Gun Pivot/SMAW").GetComponent<Gun>().magCapacity;
        }

        // PlayerShooter 컴포넌트가 있으며, 총 오브젝트가 존재하면
        if (playerShooter != null && playerShooter.gun != null)
        {
            // 총의 남은 탄환 수를 ammo 만큼 더한다
            playerShooter.gun.ammoRemain += ammo;
        }

        // 사용되었으므로, 자신을 파괴
        Destroy(gameObject);
    }
}