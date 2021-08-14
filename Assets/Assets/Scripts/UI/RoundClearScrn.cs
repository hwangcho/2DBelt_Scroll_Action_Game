using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//GameOverScrn.cs 와 거의 동일
public class RoundClearScrn : MonoBehaviour {

	public Text text;
	public Gradient ColorTransition;
	public float speed = 3.5f;
	public UIFader fader;
	private bool restartInProgress = false;

	void Start(){
		HideText();
	}

	void OnEnable() {
		EnemyWaveSystem.onLevelComplete += ShowLevelCompleteScrn;
		InputManager.onCombatInputEvent += InputEvent;
	}

	void OnDisable() {
		EnemyWaveSystem.onLevelComplete -= ShowLevelCompleteScrn;
		InputManager.onCombatInputEvent -= InputEvent;
	}

	void ShowLevelCompleteScrn(){
		fader.Fade(UIFader.FADE.FadeOut, .5f, 1);
		Invoke("ShowText", 1.4f);
	}

	void ShowText(){
		text.gameObject.SetActive(true);
	}

	void HideText(){
		text.gameObject.SetActive(false);
	}

	
	//마지막 스테이지일때만 버튼 눌럿을때 재시작하게끔 수정
	void InputEvent(string action){
		if(text.gameObject.activeSelf&&SceneManager.GetActiveScene().buildIndex ==5){ 
			RestartLevel();
		}
	}

	void Update(){

		if(text != null && text.gameObject.activeSelf){
			float t = Mathf.PingPong(Time.time * speed, 1f);
			text.color = ColorTransition.Evaluate(t);
		}

        //여기도 마지막 스테이지에서만 실행되게 수정
        if (Input.GetMouseButtonDown(0) && text.gameObject.activeSelf && SceneManager.GetActiveScene().buildIndex == 5)
        {
            RestartLevel();
        }
    }

	
	void RestartLevel(){
		if(!restartInProgress){
			restartInProgress = true;
			GlobalAudioPlayer.PlaySFX("ButtonStart");

			//button flicker
			ButtonFlicker bf =  GetComponentInChildren<ButtonFlicker>();
			if(bf != null) bf.StartButtonFlicker();

			Invoke("RestartScene", 1f);
		}
	}

	void RestartScene(){
		SceneManager.LoadScene("Stage1");
	}
}
