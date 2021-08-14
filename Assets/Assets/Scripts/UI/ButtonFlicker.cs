using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonFlicker : MonoBehaviour {

	public Text buttonText; //깜빡일 텍스트
	public float flickerSpeed = 40f; //깜빡이는 속도
	public float flickerDuration = 1f; //지속시간

	//코루틴 호출함수
	public void StartButtonFlicker () {
		StartCoroutine(ButtonFlickerCoroutine());
	}
	
	//텍스트 깜빡임 코루틴
	IEnumerator ButtonFlickerCoroutine(){
		float t =0;
		while(t < flickerDuration){
			float i = Mathf.Sin(Time.time * flickerSpeed); //sin 활용해서 깜빡임
			buttonText.enabled = (i>0); //활성화 비활성화
			t += Time.deltaTime; //t를 증가시킴
			yield return null;
		}
		buttonText.enabled = true;//마지막에 활성화 되야되기때문에 적용
	}
}
