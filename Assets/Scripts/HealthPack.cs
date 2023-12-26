using UnityEngine;

// 체력을 회복하는 아이템
public class HealthPack : MonoBehaviour, IItem {
    float health = (float)0.333; // 체력을 회복할 수치%

    public void Use(GameObject target) {
        // 전달받은 게임 오브젝트로부터 LivingEntity 컴포넌트 가져오기 시도
        //LivingEntity life = target.GetComponent<LivingEntity>();
        PlayerHealth life = target.GetComponent<PlayerHealth>();

        // LivingEntity컴포넌트가 있다면
        if (life != null)
        {
            //다형성에 의해 부모객체인 life가 playerHealth의 restoreHealth함수를 실행
            // 체력 회복 실행
            life.RestoreHealth(health);
        }

        // 사용되었으므로, 자신을 파괴
        Destroy(gameObject);
    }
}