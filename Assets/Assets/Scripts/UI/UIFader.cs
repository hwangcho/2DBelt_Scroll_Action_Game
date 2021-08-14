using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class UIFader : MonoBehaviour {

	public Image img; //검은색 이미지
	public enum FADE { FadeIn, FadeOut } //열거형으로 어둡게할지 밝게할지 설정

	//받아온 매개변수로
	//어두워지는데 얼마나걸릴지
	//밝아지는데 얼마나걸릴지
	//몇초후에 실행될지 
	//함수 실행
	public void Fade(FADE fadeDir, float fadeDuration, float StartDelay){
		if(img != null){

			if (fadeDir == FADE.FadeIn){ //밝아지게
				StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration, StartDelay, true));
			}

			if (fadeDir == FADE.FadeOut){ //어두워지게
				StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration, StartDelay, false));

			}
		}
	}

	IEnumerator FadeCoroutine(float From, float To, float Duration, float StartDelay, bool DisableOnFinish){
		yield return new WaitForSeconds(StartDelay);
		
		float t=0;
		Color col = img.color; 
		img.enabled = true; //이미지를 활성화
		img.color = new Color(col.r, col.g, col.b, From); //알파값을 밝아지게할땐 1, 어둡게할땐 0 으로 초기화

		while(t<1){
			float alpha = Mathf.Lerp (From, To, t); //1~0으로 0~1로 알파값 감소및 증가
			img.color = new Color(col.r, col.g, col.b, alpha); //알파값 변화
			t += Time.deltaTime/Duration; //걸리는 시간
			yield return 0;
		}

		img.color = new Color(col.r, col.g, col.b, To); //와일문 탈출하게되면 목표값인 to 로 변경해서 확실하게
		img.enabled = !DisableOnFinish; // 화면이 밝아졋을때는 비활성화 계속 어두워야할땐 비활성화 상태 유지

	}
}
