using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingSceneManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    float bestTime=0, playerTime=0;

    // Start is called before the first frame update
    void Start()
    {
        playerTime = GameManager.time;

        if (PlayerPrefs.HasKey("BestTime")){ //HasKey는 DB없이 간단한 데이터를 저장할수 있다. , BestTime값이 있는지 검사
            bestTime = PlayerPrefs.GetFloat("BestTime"); //있으면 bestTime에 저장, 없으면 초기값0으로 놔둠
        }
        if(bestTime > playerTime && !GameManager.instance.isCheating){  //베스트 시간보다 빠르게 클리어 했을 경우, 치트를 쓰지 않았을경우
            PlayerPrefs.SetFloat("BestTime", playerTime);  //HasKey로 BestTime에 플레이어 시간을 저장
        }

        resultText.text = "Best Time : " + CalTime(bestTime) + "\nYour Time : " + CalTime(playerTime); //resultText의 내용 설정
    }


    public static string CalTime(float t){
        int hour=0, minute=0, second=0;
        string hourString="", minuteString="", secondString="", result="";

        hour += (int)(t/3600);  //hour에 t를 3600초(1시간)로 나눈 몫을 저장
        minute += (int)((t%3600)/60);  //minute에 t를 3600초로 나눈 나머지를 60초(1분)로 나눈 몫을 저장
        second += (int)((t%3600)%60);  //second에 t를 3600초로 나눈 나머지를 60초(1분)로 나눈 나머지를 저장
        if(hour < 10 || hour == 0) hourString += "0"; //hour가 10보다 작거나 0이면 hourString 앞에 0을 붙인다
        if(minute < 10 || minute == 0) minuteString += "0"; //minute가 10보다 작거나 0이면 minuteString  앞에 0을 붙인다
        if(second < 10 || second == 0) secondString += "0"; //second가 10보다 작거나 0이면 secondString 앞에 0을 붙인다
        hourString += hour;  //hourString뒤에 hour를 붙인다
        minuteString += minute;  //minuteString뒤에 minute를 붙인다
        secondString += second; //secondString뒤에 second를 붙인다
        result += hourString + ":" + minuteString + ":" + secondString;  //result설정
        return result;
    }    
}
