using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//게임오버시 나오는 UI
public class GameOverScrn : MonoBehaviour {

	public Text text;//색 변경할 텍스트
	public Gradient ColorTransition; //변화될 컬러
	public float speed = 3.5f;//속도
	public UIFader fader; //화면 어두워지는 페이더
	private bool restartInProgress = false; //재시작이 진행중인지

	void Start(){
		HideText(); // 시작할때 숨김
	}
	//이벤트에 함수 넣어줌
	void OnEnable() {
		PlayerCombat.OnPlayerDeath += ShowGameOverScrn;//죽었을때 게임오버스크린 보여줌
		InputManager.onCombatInputEvent += InputEvent; //공격,대쉬키 눌럿을때 실행되게끔 추가
	}

	void OnDisable() {
		PlayerCombat.OnPlayerDeath -= ShowGameOverScrn;
		InputManager.onCombatInputEvent -= InputEvent;
	}

	//숨겨놧던 게임오버스크린 다시 보여줌
	void ShowGameOverScrn(){
		gameObject.transform.parent.GetChild(3).gameObject.SetActive(false); //몬스터 체력 숨김
		fader.Fade(UIFader.FADE.FadeOut, .5f, 1); //화면 어두워지게
		Invoke("ShowText", 1.4f);// 1.4초후 함수호출
	}
	//게임오버 텍스트 보여주기 함수
	void ShowText(){
		text.gameObject.SetActive(true);
	}
	//게임오버 텍스트 숨기기 함수
	void HideText(){
		text.gameObject.SetActive(false);
	}

	//공격,대쉬키가 눌리면 실행 함수
	void InputEvent(string action){
		if(text.gameObject.activeSelf){//텍스트가 활성화중일때 
			RestartLevel();
		}
	}

	void Update(){

		//text effect
		if(text != null && text.gameObject.activeSelf){
			float t = Mathf.PingPong(Time.time * speed, 1f); //pingpong 사용해서 0~1까지 왓다갔다
			text.color = ColorTransition.Evaluate(t); //색상 변경
		}

		//마우스로 클릭해도 리스타트 가능
		if(Input.GetMouseButtonDown(0) && text.gameObject.activeSelf){
			RestartLevel();
		}
	}

	//맨처음으로 재시작 함수
	void RestartLevel(){
		if(!restartInProgress){
			restartInProgress = true;

			//효과음
			GlobalAudioPlayer.PlaySFX("ButtonStart");

			//리스타트 버튼 깜빡이게 함수 호출
			ButtonFlicker bf =  GetComponentInChildren<ButtonFlicker>();
			if(bf != null) bf.StartButtonFlicker();

			Invoke("RestartScene", 1f);//씬 이동
		}
	}

	//맨처음 씬으로 이동
	void RestartScene(){
		SceneManager.LoadScene("Stage1");
	}
}
