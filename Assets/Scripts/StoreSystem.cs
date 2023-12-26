using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreSystem : MonoBehaviour
{
    public static StoreSystem instance // 싱글톤 접근용 프로퍼티
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<StoreSystem>();
            }

            return m_instance;
        }
    }
    private static StoreSystem m_instance; // 싱글톤이 할당될 변수
    private string selected = "uzi";
    GameObject uziSelectFrame, ak47SelectFrame, sniperSelectFrame, smawSelectFrame, healthSelectFrame, magazineButton, player, upgradeButton, resultTextOb;
    Gun uziGun, ak47Gun, sniperGun, smawGun;
    public Text upgradePriceText, magazinePriceText, upgradeDetailText, magazineDetailText, upgradeButtonText, magazineButtonText,
    uziItemText, ak47ItemText, sniperItemText, smawItemText, healthItemText, uziUpgradeText, ak47UpgradeText, sniperUpgradeText, smawUpgradeText, 
    healthUpgradeText, resultText;
    string uzi="uzi", ak47="ak47", sniper="sniper", smaw="smaw", health="health", upgrade="Upgrade", buy="Buy", magazine="Magazine", recover="Recover";
    private int uziUpgradeStatus=0, ak47UpgradeStatus=0, sniperUpgradeStatus=0, smawUpgradeStatus=0, healthUpgradeStatus=0, 
    ak47Price=15000, sniperPrice=40000, smawPrice=50000, magazinePrice=600, uziUpPrice=1200, ak47UpPrice=2200, healthUpPrice=1800, 
    sniperUpPrice=3500, smawUpPrice=4000;
    private float uziUpFigure=1, ak47UpFigure=2, sniperUpFigure=10, smawUpFigure=10, healthUpFigure=10, upPriceIncrease=(float)1.2;
    delegate void TotalUpdate(string s);
    TotalUpdate totalUpdate;
    private void Awake() {
        uziSelectFrame = GameObject.Find("HUD Canvas/Store/SelectFrame/" + uzi + "SelectFrame");
        ak47SelectFrame = GameObject.Find("HUD Canvas/Store/SelectFrame/" + ak47 + "SelectFrame");
        sniperSelectFrame = GameObject.Find("HUD Canvas/Store/SelectFrame/" + sniper + "SelectFrame");
        smawSelectFrame = GameObject.Find("HUD Canvas/Store/SelectFrame/" + smaw + "SelectFrame");
        healthSelectFrame = GameObject.Find("HUD Canvas/Store/SelectFrame/" + health + "SelectFrame");
        uziGun = GameObject.Find("Player Character/Gun Pivot/Gun").GetComponent<Gun>();
        ak47Gun = GameObject.Find("Player Character/Gun Pivot/AK-47").GetComponent<Gun>();
        sniperGun = GameObject.Find("Player Character/Gun Pivot/Sniper").GetComponent<Gun>();
        smawGun = GameObject.Find("Player Character/Gun Pivot/SMAW").GetComponent<Gun>();
        magazineButton = GameObject.Find("HUD Canvas/Store/Etc/Button/magazineButton");
        player = GameObject.Find("Player Character");
        upgradeButton = GameObject.Find("HUD Canvas/Store/Etc/Button/upgradeButton");
        resultTextOb = GameObject.Find("HUD Canvas/Store/Etc/Text/resultText");
        totalUpdate += ItemUIUpdate; totalUpdate += MagazineUIUpdate; totalUpdate += UpgradeUIUpdate; 
    }

    private void OnEnable() {
        totalUpdate(ak47);
        totalUpdate(sniper);
        totalUpdate(smaw);
        totalUpdate(health);
        totalUpdate(uzi);
        SelectStoreUIUpdate(selected);
    }

    private void Start() {
        totalUpdate += ItemUIUpdate; totalUpdate += MagazineUIUpdate; totalUpdate += UpgradeUIUpdate;
    }
    // Update is called once per frame
    public void SelectButtonClick(string s){
        SelectStoreUIUpdate(s);
    }

    void SelectStoreUIUpdate(string s){
        if(s==uzi){ //s가 uzi면 uzi프레임 활성화 하고 다른 프레임 비활성화
            ak47SelectFrame.SetActive(false);
            sniperSelectFrame.SetActive(false);
            smawSelectFrame.SetActive(false);
            healthSelectFrame.SetActive(false);
            uziSelectFrame.SetActive(true);
            selected = s;
            MagazineButtonSetActive(true); //uzi는 처음부터 구매 상태이므로 총알구매 UI 활성화
            MagazineUIUpdate(s);  //총알구매 UI업데이트
        }

        else if(s==ak47){  //s가 ak47이면 ak47프레임 활성화하고 다른 프레임 비활성화
            uziSelectFrame.SetActive(false);
            sniperSelectFrame.SetActive(false);
            smawSelectFrame.SetActive(false);
            healthSelectFrame.SetActive(false);
            ak47SelectFrame.SetActive(true);
            selected = s;
            if(GameManager.buyAk47){  //ak47을 구매한 상태면 총알 구매 버튼 활성화
                MagazineButtonSetActive(true);
                MagazineUIUpdate(s);   //총알구매 UI업데이트
                }
            else{ MagazineButtonSetActive(false); }  //구매하지 않았으면 총알 구매 버튼과 가격, 디테일 텍스트 비활성화
        }

        else if(s==sniper){  //s가 스나이퍼면 스나이퍼 프레임 활성화하고 다른 프레임 비활성화
            uziSelectFrame.SetActive(false);
            ak47SelectFrame.SetActive(false);
            smawSelectFrame.SetActive(false);
            healthSelectFrame.SetActive(false);
            sniperSelectFrame.SetActive(true);
            selected = s;
            if(GameManager.buySinper){   //스나이퍼를 구매한 상태면 총알 구매 버튼 활성화
                MagazineButtonSetActive(true);
                MagazineUIUpdate(s);   //총알구매 UI업데이트
                }
            else{ MagazineButtonSetActive(false); }  //구매하지 않았으면 총알 구매 버튼과 가격, 디테일 텍스트 비활성화
        }

        else if(s==smaw){  //s가 smaw면 smaw 프레임 활성화하고 다른 프레임 비활성화
            uziSelectFrame.SetActive(false);
            ak47SelectFrame.SetActive(false);
            sniperSelectFrame.SetActive(false);
            healthSelectFrame.SetActive(false);
            smawSelectFrame.SetActive(true);
            selected = s;
            if(GameManager.buySmaw){   //smaw를 구매한 상태면 총알 구매 버튼 활성화
                MagazineButtonSetActive(true);
                MagazineUIUpdate(s);  //총알구매 UI업데이트
                }
            else{ MagazineButtonSetActive(false); }  //구매하지 않았으면 총알 구매 버튼과 가격, 디테일 텍스트 비활성화
        }

        else if(s==health){  //s가 health면 health프레임 활성화하고 다른 프레임 비활성화
            uziSelectFrame.SetActive(false);
            ak47SelectFrame.SetActive(false);
            sniperSelectFrame.SetActive(false);
            smawSelectFrame.SetActive(false);
            healthSelectFrame.SetActive(true);
            selected = s;
            MagazineButtonSetActive(true);  //health는 recover이므로 총알구매 버튼 활성화
            MagazineUIUpdate(s);  //총알구매 UI업데이트
        }

        UpgradeUIUpdate(s); //업그레이드 UI 업데이트
        ItemUIUpdate(s); //아이템UI업데이트
    }

    void MagazineUIUpdate(string s){  //총알구매 UI의 디테일, 가격을 업데이트 시켜준다
        magazinePriceText.text = "Price : " + magazinePrice;  //총알구매, 체력회복 가격

        if(s!=health){  //s가 health가 아니면
            magazineButtonText.text = magazine; //s는 무기종류이므로 총알구매 버튼 텍스트는 magazine으로 설정
            int remainAmmo=0, magCapacity=0;

            if(s==uzi){  //해당 총의 Gun스크립트로 가서 현재 남은 탄약정보를 가져와서 remainAmmo에 넣고 탄창용량을 가져와 magCapacity에 넣음
                remainAmmo = (int)uziGun.ammoRemain + (int)uziGun.magAmmo;
                magCapacity = uziGun.magCapacity;
            }
            else if(s==ak47){ //해당 총의 Gun스크립트로 가서 현재 남은 탄약정보를 가져와서 remainAmmo에 넣고 탄창용량을 가져와 magCapacity에 넣음
                remainAmmo = (int)ak47Gun.ammoRemain + (int)ak47Gun.magAmmo;
                magCapacity = ak47Gun.magCapacity;
            }
            else if(s==sniper){ //해당 총의 Gun스크립트로 가서 현재 남은 탄약정보를 가져와서 remainAmmo에 넣고 탄창용량을 가져와 magCapacity에 넣음
                remainAmmo = (int)sniperGun.ammoRemain + (int)sniperGun.magAmmo;
                magCapacity = sniperGun.magCapacity;
            }
            else if(s==smaw){ //해당 총의 Gun스크립트로 가서 현재 남은 탄약정보를 가져와서 remainAmmo에 넣음
                remainAmmo = (int)smawGun.ammoRemain + (int)smawGun.magAmmo;
                magCapacity = 1; //smaw의 탄창 용량은 업글해도 1발로 고정이므로 1로 설정
            }

            magazineDetailText.text = "Remain Ammo : " + remainAmmo + "\nAmmo +" + magCapacity;  //총알구매 디테일 텍스트 설정
        }
        else{ //s가 health면 총알구매버튼 텍스트를 Recover로 변경하고 디테일은 HP recover로 변경
            string detail="HP recovery\nCurrent HP : ";
            magazineButtonText.text = recover;
            detail += (int)player.GetComponent<LivingEntity>().health + "/" + (int)player.GetComponent<LivingEntity>().playerMaxHealth;
            magazineDetailText.text = detail;
        }
        
    }

    void MagazineButtonSetActive(bool b){  //총알구매 버튼과 디테일, 가격 텍스트를 활성화 하거나 비활성화 한다
        if(b){ magazineButton.SetActive(true); }
        else{ 
            magazineButton.SetActive(false); 
            magazineDetailText.text = "";
            magazinePriceText.text = "";
        }
    }

    void UpgradeUIUpdate(string s){  //업그레이드 UI의 버튼, 디테일, 가격을 업데이트 시켜준다
        string uDT="", uBT=upgrade;  //upgradeButtonText의 기본값은 업그레이드, upgradeDetailText 기본값은 공백
        int price=0;

        if(s==uzi && uziUpgradeStatus < 10){  //uzi의 업글이 10미만이면
            uDT = UpgradeDetailCal(s);  //udt와 price는 함수로 계산하여 넣음
            price = uziUpPrice;  //uzi는 Buy할 필요 없으니 uBT는 기본값 그대로
        }
        else if(s==health && healthUpgradeStatus < 10){ //업글이 10미만이면
            uDT = UpgradeDetailCal(s);  //udt와 price는 함수로 계산하여 넣음
            price = healthUpPrice;  //health는 Buy할 필요 없으니 uBT는 기본값 그대로
        }
        else if(s==ak47 && ak47UpgradeStatus < 10){  //업글이 10미만이면
            if(GameManager.buyAk47){ //ak47 구매한 상태면 uBT는 기본값 그대로
                uDT = UpgradeDetailCal(s); 
                price = ak47UpPrice;
            }
            else{  //구매하지 않은 상태면 uBT에 buy넣고 price는 구매가격 넣음, uDT는 구매하지 않은 상태니 공백상태 그대로
                uBT= buy;
                price = ak47Price;
            }
        }
        else if(s==sniper && sniperUpgradeStatus < 10){  //업글이 10미만이면
            if(GameManager.buySinper){ //스나이퍼 구매한 상태면 uBT는 기본값 그대로
                uDT = UpgradeDetailCal(s); //udt와 price는 함수로 계산하여 넣음
                price = sniperUpPrice;
            }
            else{ //구매하지 않은 상태면 uBT에 buy넣고 price는 구매가격 넣음, uDT는 구매하지 않은 상태니 공백상태 그대로
                uBT = buy; 
                price = sniperPrice;
            }
        }
        else if(s==smaw && smawUpgradeStatus < 10){  //업글이 10미만이면
            if(GameManager.buySmaw){  //smaw 구매한 상태면 uBT는 기본값 그대로
                uDT = UpgradeDetailCal(s);//udt와 price는 함수로 계산하여 넣음
                price = smawUpPrice;
            }
            else{ //구매하지 않은 상태면 uBT에 buy넣고 price는 구매가격 넣음, uDT는 구매하지 않은 상태니 공백상태 그대로
                uBT = buy; 
                price = smawPrice;
            }
        }

        upgradeButtonText.text = uBT;
        upgradeDetailText.text = uDT;
        if(price!=0) { 
            upgradePriceText.text = "Price : " + price;  //price가 0이 아니면 가격 출력
            upgradeButton.SetActive(true);
        }
        else {  //price가 0이면 풀업글이니까 업글버튼 비활성화 하고 price와 디테일 텍스트는 공백
            upgradePriceText.text = ""; 
            upgradeButton.SetActive(false);  
        }
    }

    int CalPrice(string s){  //업그레이드 가격을 반환해줌
        int result;

        if(s==uzi){
            result = uziUpPrice;  //result에 업글 가격 넣음
        }
        else if(s==ak47){
            result = ak47UpPrice;
        }
        else if(s==sniper){
            result = sniperUpPrice;
        }
        else if(s==smaw){
            result = smawUpPrice;
        }
        else{
            result = healthUpPrice;
        }
        return result; //result리턴
    }

    string UpgradeDetailCal(string s){  //업그레이드 UI의 디테일을 계산해서 반환해줌
        string result="";

        if(s==uzi){
            if(uziUpgradeStatus==4){ result+="Shooting Range +3\n"; } 
            else if(uziUpgradeStatus==9){ result+="Magazine Capacity +25\nRate of Fire -0.02\n"; }
            result+="Damage +" + uziUpFigure;
        }
        else if(s==ak47){
            if(ak47UpgradeStatus==4){ result+="Shooting Range +5\n"; }
            else if(ak47UpgradeStatus==9){ result+="Magazine Capacity +15\nRate of Fire -0.02\n"; }
            result+="Damage +" + ak47UpFigure;
        }
        else if(s==sniper){
            if(sniperUpgradeStatus==4){ result+="Shooting Range +10\n"; }
            else if(sniperUpgradeStatus==9){ result+="Rate of Fire -0.1\nPiercing Shoot\n"; }
            result+="Damage +" + sniperUpFigure;
        }
        else if(s==smaw){
            if(smawUpgradeStatus==9){ result+="Self Damage Impossible\n"; }
            result+="Damage +" + smawUpFigure;
        }
        else{
            result+="Max HP +" + healthUpFigure;
        }

        return result;
    }

    void ItemUIUpdate(string s){  //아이템 정보를 업데이트 해주는 함수
        string info="";

        if(s!=health){  //s가 health가 아닐 경우
            float damage=0, rate=0, range=0, reload=0, magCapacity=0;
            string etc="";

            if(s==uzi){  //해당 무기의 Gun스크립트에서 정보를 가져온다
                damage = uziGun.damage;
                rate = uziGun.timeBetFire;
                range = uziGun.fireDistance;
                reload = uziGun.reloadTime;
                magCapacity = uziGun.magCapacity;
                etc = uziGun.etc;
                ResultInfo();  //info에 가져온 정보를 스트링으로 넣음
                uziItemText.text = info;  //아이템 정보 업데이트
                if(uziUpgradeStatus > 0){ uziUpgradeText.text = "+" + uziUpgradeStatus; }//업글1이상 일경우 몇번 강화했는지 표시
            }
            else if(s==ak47){
                damage = ak47Gun.damage;
                rate = ak47Gun.timeBetFire;
                range = ak47Gun.fireDistance;
                reload = ak47Gun.reloadTime;
                magCapacity = ak47Gun.magCapacity;
                etc = ak47Gun.etc;
                ResultInfo();  //info에 가져온 정보를 스트링으로 넣음
                ak47ItemText.text = info;  //아이템 정보 업데이트
                if(ak47UpgradeStatus > 0){ ak47UpgradeText.text = "+" + ak47UpgradeStatus; }//업글1이상 일경우 몇번 강화했는지 표시
            }
            else if(s==sniper){
                damage = sniperGun.damage;
                rate = sniperGun.timeBetFire;
                range = sniperGun.fireDistance;
                reload = sniperGun.reloadTime;
                magCapacity = sniperGun.magCapacity;
                etc = sniperGun.etc;
                ResultInfo();  //info에 가져온 정보를 스트링으로 넣음
                sniperItemText.text = info;  //아이템 정보 업데이트
                if(sniperUpgradeStatus > 0){ sniperUpgradeText.text = "+" + sniperUpgradeStatus; }//업글1이상 일경우 몇번 강화했는지 표시
            }
            else{
                damage = smawGun.damage;
                rate = smawGun.timeBetFire;
                range = smawGun.fireDistance;
                reload = smawGun.reloadTime;
                etc = smawGun.etc;
                ResultInfo();  //info에 가져온 정보를 스트링으로 넣음
                smawItemText.text = info;  //아이템 정보 업데이트
                if(smawUpgradeStatus > 0){ smawUpgradeText.text = "+" + smawUpgradeStatus; } //업글1이상 일경우 몇번 강화했는지 표시
            }
            
            void ResultInfo(){  //가져온 정보를 info변수에 넣어주는 함수
                info = "Damage : " + damage + "\nRate of Fire : " + rate + "\nShooting Range : " + range + "\nReload Time : " + reload;
                if(s!=smaw) { info += "\nMagazine Capacity : " + magCapacity; }
                if(etc!=""){ info += "\nETC : " + etc; } //etc가 있다면 넣고 없다면 넣지 않는다
            }
            
        }

        else{  //s가 health라면 player의 PlayerHealth스크립트에서 정보를 받아온다
            float maxHp = player.GetComponent<PlayerHealth>().playerMaxHealth;
            float health = player.GetComponent<PlayerHealth>().health;
            info = "Max HP : " + maxHp + "\nCurrent HP : " + (int)health + "\nYou can recover or upgrade HP";
            healthItemText.text = info;  //아이템 정보 업데이트
            if(healthUpgradeStatus > 0){ healthUpgradeText.text = "+" + healthUpgradeStatus; }  //업글1이상 일경우 몇번 강화했는지 표시
        }
    }

    public void MagAndRecButton(){  //magazine, recover버튼을 누르면 호출되는 함수
        if(GameManager.score >= magazinePrice){  //포인트가 magazinePrice 이상이면
            GameManager.instance.AddScore(-magazinePrice);  //포인트 500감소
            if(selected==uzi){   //각 무기에 맞는 Gun스크립트를 가져와 탄창용량 추가
                uziGun.ammoRemain += uziGun.magCapacity; 
            }
            else if(selected==ak47){ 
                ak47Gun.ammoRemain += ak47Gun.magCapacity; 
            }
            else if(selected==sniper){ 
                sniperGun.ammoRemain += sniperGun.magCapacity; 
            }
            else if(selected==smaw){ 
                smawGun.ammoRemain += 1; 
            }
            else{ //health가 선택됐다면 체력회복
                player.GetComponent<PlayerHealth>().RestoreHealth(1); 
            }
            MagazineUIUpdate(selected); //매거진UI업데이트
            ResultTextPrint(true); //구매결과 텍스트 출력
        }
        else{ ResultTextPrint(false); }  //포인트가 500미만일경우 구매X 바로 구매결과 텍스트 출력
    }

    void ResultTextPrint(bool b){  //상점 구매결과 텍스트를 출력해주는 함수
        resultTextOb.SetActive(true);
        if(b){ //구매성공이면 컴플리트 출력, 사운드재생
            resultText.text = "Complete!"; 
            UIManager.instance.CashSoundPlay();
        }  
        else{  //실패하면 포인트부족 출력, 사운드재생
            resultText.text = "Lack of Point!"; 
            UIManager.instance.FailedSoundPlay();
        } 
        Invoke("Null", 1f); //결과 텍스트 1초 출력후 공백으로 만듬
    }
    void Null() { resultTextOb.SetActive(false); }  //결과 텍스트 공백으로 만드는 함수
    

    public void UpAndBuyButton(){  //업글이나 buy버튼 누르면 호출되는 함수
        if(selected==ak47){
            if(GameManager.buyAk47){ Upgrade(selected); }  //ak47이 구매된 상태면 업그레이드 시도
            else{ Buy(selected); }  //구매되지 않았으면 구매시도
        }
        else if(selected==sniper){
            if(GameManager.buySinper){ Upgrade(selected); }//스나이퍼가 구매된 상태면 업그레이드 시도
            else{ Buy(selected); } //구매되지 않았으면 구매시도
        }
        else if(selected==smaw){
            if(GameManager.buySmaw){ Upgrade(selected); } //smaw가 구매된 상태면 업그레이드 시도
            else{ Buy(selected); }  //구매되지 않았으면 구매시도
        }
        else{ Upgrade(selected); }  //ak47, sniper, smaw가 아니면 buy없고 업그레이드만 있으므로 바로 업그레이드 시도
    }

    public void Upgrade(string s){  //업그레이드를 실행해주는 함수
        int score = GameManager.score;
        int price = 0;

        if(s==uzi && uziUpgradeStatus < 10 && score >= uziUpPrice){  //업글이 10미만일 경우 가진 포인트가 업글 비용보다 같거나 많으면
            if(uziUpgradeStatus==4){ uziGun.fireDistance += 3; }  //5업하면 사거리 증가
            else if(uziUpgradeStatus==9){  //10업하면 탄창용량 증가 발사속도 감소
                uziGun.magCapacity = 75;
                uziGun.timeBetFire = 0.1f;
            }
            price = uziUpPrice;
            uziUpPrice = (int)(uziUpPrice * upPriceIncrease);  //업글 가격 업데이트
            uziGun.damage += uziUpFigure;  //데미지 증가
            uziUpgradeStatus++;  //업글 스탯 1증가
        }
        else if(s==ak47 && ak47UpgradeStatus < 10 && score >= ak47UpPrice){  //업글이 10미만일 경우 가진 포인트가 업글 비용보다 같거나 많으면
            if(ak47UpgradeStatus==4){ ak47Gun.fireDistance += 5; } //5업하면 사거리 증가
            else if(ak47UpgradeStatus==9){  //10업하면 탄창용량과 발사속도 증가
                ak47Gun.magCapacity = 45;
                ak47Gun.timeBetFire = 0.08f;
            }
            price = ak47UpPrice;
            ak47UpPrice = (int)(ak47UpPrice * upPriceIncrease); //업글 가격 업데이트
            ak47Gun.damage += ak47UpFigure;  //데미지 증가
            ak47UpgradeStatus++;  //업글 스탯 1증가
        }
        else if(s==sniper && sniperUpgradeStatus < 10 && score >= sniperUpPrice){  //업글이 10미만일 경우 가진 포인트가 업글 비용보다 같거나 많으면
            if(sniperUpgradeStatus==4){ sniperGun.fireDistance += 10; } //5업하면 사거리 증가
            else if(sniperUpgradeStatus==9){  //10업하면 피어싱샷 가능과 발사속도 감소
                sniperGun.sniperPiercingShoot = true;
                sniperGun.timeBetFire = 0.65f;
                //sniperGun.magAmmo = 15;
                sniperGun.etc = "Piercing Shoot";  //etc에 피어싱샷 추가
            }
            price = sniperUpPrice;
            sniperUpPrice = (int)(sniperUpPrice * upPriceIncrease); //업글 가격 업데이트
            sniperGun.damage += sniperUpFigure;  //데미지 증가
            sniperUpgradeStatus++;  //업글 스탯 1증가
        }
        else if(s==smaw && smawUpgradeStatus < 10 && score >= smawUpPrice){  //업글이 10미만일 경우 가진 포인트가 업글 비용보다 같거나 많으면
            if(smawUpgradeStatus==9){  //10업하면 장전시간 감소, 본인에게 범위피해X
                smawGun.reloadTime = 4;
                smawGun.smawSelfDamage = false;
                smawGun.etc = "Range Attack,\nSelf Damage Impossible"; //etc갱신
            }
            price = smawUpPrice;
            smawUpPrice = (int)(smawUpPrice * upPriceIncrease);  //업글가격 증가
            smawGun.damage += smawUpFigure;  //데미지 증가
            smawUpgradeStatus++;  //업글 스탯 1증가
        }
        else if(s==health && healthUpgradeStatus < 10  && score >= healthUpPrice){  //업글이 10미만일 경우 가진 포인트가 업글 비용보다 같거나 많으면
            player.GetComponent<PlayerHealth>().playerMaxHealth += healthUpFigure;  //Maxhealth를 증가시킴
            player.GetComponent<PlayerHealth>().healthSlider.maxValue = player.GetComponent<PlayerHealth>().playerMaxHealth;  //슬라이더의 최대값 수정
            player.GetComponent<PlayerHealth>().RestoreHealth(1);  //체력회복
            healthUpgradeStatus++; //업글 스탯 1증가
            price = healthUpPrice;
            healthUpPrice = (int)(healthUpPrice * upPriceIncrease); //업글가격 업데이트
        }
        else{  //포인트가 price만큼 없을 때
            ResultTextPrint(false);  //구매실패 텍스트 출력
            return;
        }
        GameManager.instance.AddScore(-price); //포인트 차감
        totalUpdate(s);  //델리게이트로 업글, 아이템, 매거진 UI업데이트
        ResultTextPrint(true);
    }

    public void Buy(string s){  //무기를 구매하는 함수

        if(s==ak47 && GameManager.score >= ak47Price){  //해당 무기의 가격 이상의 포인트를 가지고 있으면
            GameManager.instance.AddScore(-ak47Price);  //포인트 차감
            GameManager.buyAk47=true;  //해당무기 구매
        }
        else if(s==sniper && GameManager.score >= sniperPrice){  //해당 무기의 가격 이상의 포인트를 가지고 있으면
            GameManager.instance.AddScore(-sniperPrice);  //포인트 차감
            GameManager.buySinper=true;  //해당무기 구매
        }
        else if(s==smaw && GameManager.score >= smawPrice){  //해당 무기의 가격 이상의 포인트를 가지고 있으면
            GameManager.instance.AddScore(-smawPrice);  //포인트 차감
            GameManager.buySmaw=true;  //해당무기 구매
        }
        else{  //해당 무기가격 미만의 포인트를 가지고 있으면
            ResultTextPrint(false);  //구매실패 텍스트 출력
            return;  //Buy함수 종료
        }

        GameObject storeImageIcon = GameObject.Find("HUD Canvas/Store/Item/" + s + "/" + s + "Image");  //스토어의 해당 무기이미지를 가져옴
        GameObject invenImageIcon = GameObject.Find("HUD Canvas/Panel/" + s);  //인벤토리의 해당 무기이미지를 가져옴
        ResultTextPrint(true);  //구매성공 메시지 출력
        storeImageIcon.transform.SetAsLastSibling();  //해당무기의 하이어라키 순서 바꿈
        invenImageIcon.transform.SetSiblingIndex(4);  //해당무기의 하이어라키 순서 바꿈
        MagazineButtonSetActive(true);  //구매했으니 총알구매 버튼 활성화
        MagazineUIUpdate(s);  //매거진과 업그레이드 UI 업데이트
        UpgradeUIUpdate(s);
    }

    public void CloseButton(){  //close버튼을 눌렀을 때 호출되는 함수
        UIManager.instance.DelayTextPrint(3);
    }

}
