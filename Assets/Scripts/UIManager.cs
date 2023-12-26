using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리자 관련 코드
using UnityEngine.UI; // UI 관련 코드
using System.Threading;

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour {
    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public Text ammoText; // 탄약 표시용 텍스트
    public Text scoreText; // 점수 표시용 텍스트
    public Text waveText; // 적 웨이브 표시용 텍스트
    public Text delayText, waveNotifyText, timeText, ReloadText;
    public GameObject gameoverUI, waveTextOb, delayTextOb, pauseScreen; // 게임 오버시 활성화할 UI 
    GameObject uziUseFrame;
    GameObject ak47UseFrame; 
    GameObject sniperUseFrame;
    GameObject smawUseFrame;
    GameObject spawner, store;
    float delayTime;
    bool timeCheck;
    AudioSource audioPlayer;
    public AudioClip cashSound, failedSound;

    private void Awake() {
        uziUseFrame = GameObject.Find("HUD Canvas/Panel/UseFrame/UziUse");
        ak47UseFrame = GameObject.Find("HUD Canvas/Panel/UseFrame/Ak47Use");
        sniperUseFrame = GameObject.Find("HUD Canvas/Panel/UseFrame/SniperUse");
        smawUseFrame = GameObject.Find("HUD Canvas/Panel/UseFrame/SMAWUse");
        spawner = GameObject.Find("Enemy Spawner");
        store = GameObject.Find("HUD Canvas/Store");
        audioPlayer = GetComponent<AudioSource>();
        store.SetActive(false);
    }

    // 탄약 텍스트 갱신
    public void UpdateAmmoText(int magAmmo, int remainAmmo) {
        ammoText.text = magAmmo + "/" + remainAmmo;
    }

    // 점수 텍스트 갱신
    public void UpdateScoreText(int newScore) {
        scoreText.text = "Point : " + newScore;
    }

    // 적 웨이브 텍스트 갱신
    public void UpdateWaveText(int waves, int count) {
        waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    // 게임 오버 UI 활성화
    public void SetActiveGameoverUI(bool active) {
        gameoverUI.SetActive(active);
    }

    // 게임 재시작
    public void GameRestart() {
        GameManager.buyAk47 = false;
        GameManager.buySinper = false;
        GameManager.buySmaw = false;
        GameManager.useAk47 = false;
        GameManager.useSmaw = false;
        GameManager.useSniper = false;
        GameManager.useUzi = true;
        GameManager.reloading = false;
        GameManager.changing = false;
        GameManager.score = 0;
        GameManager.time = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ChangeWeapon(string s){
        uziUseFrame.SetActive(false);
        ak47UseFrame.SetActive(false);
        sniperUseFrame.SetActive(false);
        smawUseFrame.SetActive(false);
        if(s=="uzi"){
            uziUseFrame.SetActive(true);
        }
        else if(s=="ak47"){
            ak47UseFrame.SetActive(true);
        }
        else if(s=="sniper"){
            sniperUseFrame.SetActive(true);
        }
        else if(s=="smaw"){
            smawUseFrame.SetActive(true);
        }
    }

    private void Update() {
        timeText.text = "Time : " + EndingSceneManager.CalTime(GameManager.time);

        if(timeCheck && delayTime > 0 && !GameManager.instance.isGameover){
            delayTime -= Time.deltaTime;
            delayText.text = System.Convert.ToString(Mathf.Ceil(delayTime));
        }
        else if(timeCheck && delayTime <= 0 && !GameManager.instance.isGameover){
            delayText.text = "";
            timeCheck = false;
            GameManager.instance.isPauseGame = false;
            spawner.GetComponent<EnemySpawner>().SpawnWave();
            delayTextOb.SetActive(false);
        }
    }
    public void DelayTextPrint(int t){
        store.SetActive(false);
        delayTextOb.SetActive(true);
        delayTime = t;
        timeCheck = true;
        delayText.text = System.Convert.ToString(t);
    }

    public void WaveNotify(int wave){
        waveTextOb.SetActive(true);
        waveNotifyText.text = System.Convert.ToString(wave) + " Wave";
        Invoke("WaveTextNull", 1.5f);
    }

    void WaveTextNull(){ 
        waveTextOb.SetActive(false);
    }

    public void CashSoundPlay(){
        audioPlayer.PlayOneShot(cashSound);
    }

    public void FailedSoundPlay(){
        audioPlayer.PlayOneShot(failedSound);
    }

    public void ButtonOnQuit(){
        Application.Quit();
    }

    public void BackButton(){
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
    }

    public void pauseGame(){
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowMeTheMoney(){
        GameManager.instance.AddScore(10000);
        GameManager.instance.isCheating = true;
    }

    public void ReloadTextActive(bool b){
        if(b) ReloadText.text = "Reloading...";
        else ReloadText.text = "";
    }


}