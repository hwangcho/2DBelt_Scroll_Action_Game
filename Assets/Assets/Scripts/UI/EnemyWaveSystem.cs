using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyWaveSystem : MonoBehaviour {

	[Header ("List of enemy Waves")]
	public Transform positionMarkerLeft;//좌측 
	public EnemyWave[] EnemyWaves;//EnemyWave 리스트로 선언

	[SerializeField]
	public int currentWave; //현재 웨이브
	//델리게이트 이벤트 생성
	public delegate void OnLevelEvent(); 
	public static event OnLevelEvent onLevelComplete;
	public static event OnLevelEvent onLevelStart;
	public string nextStage;//다음씬으로 이동
	void OnEnable(){
		Enemy.OnUnitDestroy += onUnitDestroy;
	}

	void OnDisable(){
		Enemy.OnUnitDestroy -= onUnitDestroy;
	}

	void Start(){
		currentWave = 0; //시작할때 현재웨이브 0으로 초기화
		DisableEnemiesAtStart(); //시작하면 모든몬스터 액티브 false
		//카메라 좌측 고정위치 변경
		CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
		if(cam != null) cam.SetLeftClampedPosition(positionMarkerLeft.position);
	}

	//웨이브 시작함수
	public void OnLevelStart(){
		if(onLevelStart != null) onLevelStart();
		StartWave();
	}

	//몬스터 삭제함수
	void onUnitDestroy(	GameObject g){
		if(EnemyWaves.Length > currentWave){//현재 웨이브보다 총 웨이브가 클때
			EnemyWaves[currentWave].RemoveEnemyFromWave(g); //현재웨이브의 몬스터를 지움
			if(EnemyWaves[currentWave].waveComplete()){//만약 현재 웨이브 몬스터가 0명이면
				currentWave += 1; //웨이브 1올림
				if(!allWavesCompleted()){ //모든 웨이브 클리어 아니면
					StartWave(); //웨이브 다시 시작해서 다음 웨이브 시작
				} else{//모든 웨이브 클리어시
					if (onLevelComplete != null)
                    {//이부분 수정해서 페이드아웃하고 보스씬으로 이동함
						//페이드아웃으로 화면 어둡게 변경후 함수 실행
						GameObject a = GameObject.Find("Fader");
						a.GetComponent<UIFader>().Fade(UIFader.FADE.FadeOut, .5f, 1);
						Invoke("NextStageOn", 2);
					}
				}
			}
		}
	}
	//다음씬 불러오는 함수
	void NextStageOn()
    {
		SceneManager.LoadScene(nextStage);

	}
	//웨이브 시작 함수
	public void StartWave(){
		CameraFollow cam = Camera.main.GetComponent<CameraFollow>();

		if(cam != null){
			if(EnemyWaves[currentWave].PositionMarker != null){

				//카메라 고정위치 변경
				if(currentWave == 0){ 
					cam.SetNewClampPosition(EnemyWaves[currentWave].PositionMarker.position, 0f); 
				} else {
					cam.SetNewClampPosition(EnemyWaves[currentWave].PositionMarker.position, 1.5f);
				}

				//비활성화중이던 다음웨이브 몬스터 활성화
				foreach(GameObject g in EnemyWaves[currentWave].EnemyList){
					g.SetActive(true);
				}

			} 
		} 
		Invoke("SetEnemyTactics", .1f); 
	}

	//몬스터 전술 변경 함수
	void SetEnemyTactics(){
		EnemyManager.SetEnemyTactics();
	}

	//모든 웨이브 몬스터 비활성화
	void DisableEnemiesAtStart(){
		foreach(EnemyWave wave in EnemyWaves){//웨이브담고 
			foreach(GameObject g in wave.EnemyList){//그웨이브의 몬스터 리스트들 모두 비호라성화
				g.SetActive(false);
			}
		}
	}

	//모든 웨이브 클리어시 불값 반환 함수
	bool allWavesCompleted(){
		int waveCount = EnemyWaves.Length; //웨이브 갯수
		int waveFinished = 0;
		for(int i=0; i<waveCount; i++){ //모든웨이브 클리어했는지 확인
			if(EnemyWaves[i].waveComplete()) waveFinished += 1; //true 반환되면 1씩 증가시켜줌
		}
		if(waveCount == waveFinished) //웨이브 갯수와 같아지면 true 반환
			return true;
		else 
			return false;
	}
}
