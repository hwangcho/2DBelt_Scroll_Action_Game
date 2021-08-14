using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public UIFader UI_Fader; //화면 어두워지게 하는 페이더
	private bool startGameInProgress = false; //게임진행중인지

	//이벤트에 넣어줌
	void OnEnable() {
		InputManager.onCombatInputEvent += InputEvent;
	}

	void OnDisable() {
		InputManager.onCombatInputEvent -= InputEvent;
	}

	//키입력시 게임스타트
	void InputEvent(string action){
			StartGame();
	}

	void Start(){

		//시작시 어두운화면 밝게
		Invoke("FadeIn", 1f);
	}

	public void StartGame(){
		if(!startGameInProgress){

			startGameInProgress = true;

			//효과음 실행
			GlobalAudioPlayer.PlaySFX("ButtonStart");

			//스타트 텍스트 깜빡이게 
			ButtonFlicker bf =  GetComponentInChildren<ButtonFlicker>();
			if(bf != null) bf.StartButtonFlicker();

			//화면 어둡게
			FadeOut();

			//게임시작 함수
			Invoke("startGame", 1.8f);
		}
	}

	//화면 밝게
	void FadeIn(){
		UI_Fader.Fade(UIFader.FADE.FadeIn, .5f, 0f);
	}
	//화면 어둡게
	void FadeOut(){
		UI_Fader.Fade(UIFader.FADE.FadeOut, .5f, 1f);
	}

	//게임시작 함수
	public void startGame(){
		FadeIn();//화면 밝게
		gameObject.SetActive(false); //메인메뉴 비활성화

		//첫번쨰 웨이브 시작 호출
		EnemyWaveSystem EWS = GameObject.FindObjectOfType<EnemyWaveSystem>();
		if(EWS != null) EWS.OnLevelStart();
	}
}