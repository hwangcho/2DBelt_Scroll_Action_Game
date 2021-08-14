using UnityEngine;
using System.Collections;

//오브젝트 삭제 스크립트
//시간설정해서 몇초뒤 삭제될지 정함
public class TimeToLive : MonoBehaviour {

	public float LifeTime = 1;
	
	void Start(){
		Invoke("DestroyGO", LifeTime);
	}

	void DestroyGO(){
		Destroy(gameObject);
	}
}
