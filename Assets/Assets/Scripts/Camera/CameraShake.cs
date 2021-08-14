using UnityEngine;
using System.Collections;

//카메라 흔들림 효과 스크립트
public class CameraShake : MonoBehaviour {

	[HideInInspector]
	public float AdditionalOffset;//흔들릴 값

	//cam shake presets
	public void CamShakeSmall(){
		StopAllCoroutines();
		StartCoroutine( DoCamShake(40f, 0.06f, 0.9f));
	}

	public void CamShakeMedium(){
		StopAllCoroutines();
		StartCoroutine( DoCamShake(50f, 0.10f, 1.1f));
	}

	public void CamShakeBig(){
		StopAllCoroutines();
		StartCoroutine( DoCamShake(50f, 0.18f, 1.3f));
	}

	//흔들기 코루틴
	IEnumerator DoCamShake(float speed, float intensity, float duration){ //속도,강도,지속시간
		float t=0;
		float dampValue = 1f;
		AdditionalOffset = 0;
		while (t<1){
			AdditionalOffset = Mathf.Sin(t*speed) * intensity * dampValue;
			dampValue = MathUtilities.Sinerp(1f, 0f, t);
			t += Time.deltaTime / duration;
			yield return null;
		}
		AdditionalOffset = 0;
	}
}
