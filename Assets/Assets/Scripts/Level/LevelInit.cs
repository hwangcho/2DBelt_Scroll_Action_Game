using UnityEngine;
using System.Collections;

//레벨 시작시 필요한 게임오브젝트들 생성
public class LevelInit : MonoBehaviour {

	public string LevelMusic = "Music"; //실행 음악이름
	public bool playMusic = true; 
	private GameObject audioplayer;
	private GameSettings settings;

	void Awake() {

		//설정해둔 게임세팅 불러와서 설정
		settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		if(settings != null){
			Time.timeScale = settings.timeScale;
			Application.targetFrameRate = settings.framerate;
		}

		//오디오플레이어 생성
		if(!GameObject.FindObjectOfType<AudioPlayer>())	audioplayer = GameObject.Instantiate(Resources.Load("AudioPlayer"), Vector3.zero, Quaternion.identity) as GameObject;

		//키입력매니저 생성
		if(!GameObject.FindObjectOfType<InputManager>()) GameObject.Instantiate(Resources.Load("InputManager"), Vector3.zero, Quaternion.identity);

		//UI생성
		if(!GameObject.FindGameObjectWithTag("UI"))	GameObject.Instantiate(Resources.Load("UI"), Vector3.zero, Quaternion.identity);
	
		//카메라 생성
		if(!GameObject.FindObjectOfType<CameraFollow>()) GameObject.Instantiate(Resources.Load("GameCamera"), new Vector3(0,0,-10), Quaternion.identity);

        //음악 실행
        if (playMusic) Invoke("PlayMusic", .1f);
    }

	//음악 실행함수
	void PlayMusic(){
		if(audioplayer != null)	audioplayer.GetComponent<AudioPlayer>().playMusic(LevelMusic);
	}
}