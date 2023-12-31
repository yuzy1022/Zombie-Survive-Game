﻿using System.Collections.Generic;
using UnityEngine;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour {
    public Enemy enemyPrefab; // 생성할 적 AI

    public Transform[] spawnPoints; // 적 AI를 소환할 위치들

    public float damageMax = 0; // 최대 공격력
    public float damageMin = 0; // 최소 공격력

    public float firstHealthMin = 100f, firstDamageMin = 20f; 
    public float healthMax = 0; // 최대 체력
    public float healthMin = 0; // 최소 체력

    public float speedMax = 3f; // 최대 속도
    public float speedMin = 1f; // 최소 속도

    public Color strongEnemyColor = Color.red; // 강한 적 AI가 가지게 될 피부색

    private List<Enemy> enemies = new List<Enemy>(); // 생성된 적들을 담는 리스트
    private int wave=0; // 현재 웨이브
    public GameObject store;
    int spawnCount=2, spawnedCount=0; //스폰 해야할 좀비 수와 현재까지 스폰된 좀비수
    public bool isWaveEnd = true;

    private void Awake() {
        SpawnWave();
    }

    private void Update() {
        // 게임 오버 상태일때는 생성하지 않음
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        // 적을 모두 물리친 경우 다음 스폰 실행
        if (enemies.Count <= 0 && !GameManager.instance.isPauseGame && wave <=49 && !isWaveEnd)//적이 0마리이고 게임정지가 아닌상태이며 49웨이브 이하이고 웨이브가 끝나지 않은 상태면
        {
            store.SetActive(true);
            GameManager.instance.isPauseGame = true;
            isWaveEnd=true;
        }
        else if(enemies.Count <= 0 && !GameManager.instance.isPauseGame && wave ==50 && !isWaveEnd){ //적 0마리, 게임정지x, 웨이브50 일경우 게임 클리어
            GameManager.instance.ClearGame();
        }

        if(spawnCount > spawnedCount && enemies.Count < 20 && !GameManager.instance.isPauseGame && !GameManager.instance.isGameover){
            CreateEnemy();  //정지상태 아니고 게임오버 아닌 상태에서 생성된 좀비수가 생성해야될 좀비 수보다 적으면 생성시도
        }

        // UI 갱신
        UpdateUI();
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        // 현재 웨이브와 남은 적의 수 표시
        UIManager.instance.UpdateWaveText(wave, spawnCount-spawnedCount+enemies.Count);
    }

    // 현재 웨이브에 맞춰 적을 생성
    public void SpawnWave() {
        isWaveEnd=false;
        wave++;
        UIManager.instance.WaveNotify(wave);
        spawnCount = Mathf.RoundToInt(wave*1.5f); //현재웨이브 * 1.5를 반올림한 수만큼 spawnCount설정
        spawnedCount = 0;
        CalZombieUp();
        CreateEnemy();
    }

    // 적을 생성하고 생성한 적에게 추적할 대상을 할당
    private void CreateEnemy() {
        float intensity = Random.Range(0f, 1f);  //적의 세기를 0~1 (0~100%)사이에서 랜덤 설정
        //intensity를 기반으로 적의 능력치 결정
        float health = Mathf.Lerp(healthMin, healthMax, intensity);  
        float damage = Mathf.Lerp(damageMin, damageMax, intensity);
        float speed = Mathf.Lerp(speedMin, speedMax, intensity);
        float point = health;

        Color skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity); //intensity를 기반으로 하얀색과 enemyStrength사이에서 적의 피부색 결정
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; //생성할 위치 랜덤 설정
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation); //적 프리펩으로부터 적 생성
        enemy.Setup(health, damage, speed, skinColor, point); //생성한 적의 능력치와 추적 대상 설정
        enemies.Add(enemy); //생성된 적을 리스트에 추가
        enemy.onDeath += () => enemies.Remove(enemy); //적의 OnDeath이벤트에 익명 메소드등록
        enemy.onDeath += () => Destroy(enemy.gameObject, 10f);
        enemy.onDeath += () => GameManager.instance.AddScore((int)enemy.point);
        spawnedCount++;
    }

    void CalZombieUp(){
        if(wave <= 40 && wave > 1){
            healthMin = (float)(wave*0.015*firstHealthMin) + firstHealthMin;
            damageMin = (float)(wave*0.015*firstDamageMin) + firstDamageMin;
        }
        else if(wave > 40 && wave <= 50){
            healthMin = (float)(wave*0.025*firstHealthMin) + firstHealthMin;
            damageMin = (float)(wave*0.025*firstDamageMin) + firstDamageMin;
            speedMin = 2f;
            speedMax = 3.5f;
        }
        healthMax = healthMin*2;
        damageMax = damageMin*2;
    }


}