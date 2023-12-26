using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonAction : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnButtonAttack(){
        anim.SetTrigger("Attack");
    }

    public void OnButtonDie(){
        anim.SetTrigger("Die");
    }
    
    public void OnNextScene(){
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
        SceneManager.LoadScene("Main");
    }

    public void OnQuit(){
        Application.Quit();
    }
    void Update()
    {
        
    }
}
