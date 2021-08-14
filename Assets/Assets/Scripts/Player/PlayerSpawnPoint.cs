using UnityEngine;
using System.Collections;

public class PlayerSpawnPoint : MonoBehaviour {

	//활성화되면 EnemyWaveSystem스크립트의 OnLevelEvent 델리게이트의 이벤트 OnLevelStart에 spawnPlayer 추가해줌
	void OnEnable(){
		EnemyWaveSystem.onLevelStart += spawnPlayer;
	}
	//비활성화될때 함수 제거
	void OnDisable(){
		EnemyWaveSystem.onLevelStart -= spawnPlayer;
	}
	//플레이어 스폰 함수
	void spawnPlayer() {
		GameObject player = GameObject.Instantiate(Resources.Load("Player1"), transform.position, Quaternion.identity) as GameObject; //(Resources폴더안의 Player1을 생성,내위치에,회전값)
		player.name = "Player1";
	}
}
