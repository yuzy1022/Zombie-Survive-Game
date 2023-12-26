using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookMouse : MonoBehaviour
{
    
    public Camera came;

    // Start is called before the first frame update
    int layerMask;

    private void Awake() {
        layerMask = 1 << LayerMask.NameToLayer("map");  //map레이어만 충돌체크하게 하는 레이어마스크
    }


    // Update is called once per frame
    void Update()
    {
        lookMouse();
    } 

    /*public void lookMouse(){
        if(!GameManager.instance.isGameover && !GameManager.instance.isPauseGame && Time.timeScale!=0){
            Ray ray = came.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitResult;
            if(Physics.Raycast(ray, out hitResult)){
                Vector3 mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                transform.forward = mouseDir;
            }
        }
        
    }*/

    public void lookMouse(){
        if(!GameManager.instance.isGameover && !GameManager.instance.isPauseGame && Time.timeScale!=0){
            Ray ray = came.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitResult;
            if(Physics.Raycast(ray, out hitResult, 100f ,layerMask)){  //레이어 마스크로 10번 map레이어에만 충돌하도록함 충돌하고 싶지 않은 오브젝트는 10번 외의 다른 레이어로 지정
                Vector3 mouseDir = new Vector3(hitResult.point.x, transform.position.y, hitResult.point.z) - transform.position;
                transform.forward = mouseDir;
            }
        }
        
    }
}
