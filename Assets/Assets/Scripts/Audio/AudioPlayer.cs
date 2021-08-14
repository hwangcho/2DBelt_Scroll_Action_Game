using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

	public AudioItem[] AudioList;//AudioItem 을 리스트로 선언
	private AudioSource source;
	private float musicVolume = 1f; //음악 볼륨
	private float sfxVolume = 1f; //효과음 볼륨

	void Awake(){
		GlobalAudioPlayer.audioPlayer = this; 
		source = GetComponent<AudioSource>();

		//설정해둔 세팅 불러와서 설정
		GameSettings settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		if(settings != null){
			musicVolume = settings.MusicVolume;
			sfxVolume = settings.SFXVolume;
		}
	}

	//효과음 실행
	public void playSFX(string name){//효과음 이름변수 받아옴
		foreach(AudioItem s in AudioList){//AudioList를 S에 넣어주고
			if(s.name == name){//매개변수로 받아온 이름과 s의 이름이 같으면 효과음 실행
				source.PlayOneShot(s.clip[0]);//s안에 들어있는 효과음 클립의 첫번째 실행
				source.volume = s.volume * sfxVolume;//볼륨설정
			}
		}
	}
	//음악 실행 
	public void playMusic(string name){

		//음악 실행할 게임오브젝트 새로 생성
		GameObject music = new GameObject();
		music.name = "Music";
		AudioSource AS = music.AddComponent<AudioSource>(); //오디오소스 컴포넌트 추가

		//음악 리스트의 이름확인후 동일한 이름의 음악 실행
		foreach(AudioItem s in AudioList){
			if(s.name == name){
				AS.clip = s.clip[0];
				AS.loop = true;//무한루프 true
				AS.volume = s.volume * musicVolume;
				AS.Play();
			}
		}
	}
}
