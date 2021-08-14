using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EnemyManager {

	public static List<GameObject> enemyList = new List<GameObject>(); //생성된 모든 몬스터를 리스트에 담아줌
	public static List<GameObject> activeEnemies = new List<GameObject>(); //현재 활성화되어있는 몬스터
	static GameSettings settings; 

	//설정해둔 게임세팅
	static GameSettings GetSettings(){
		if(settings == null) settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		return settings;
	}

	//리스트에서 몬스터 제거
	public static void RemoveEnemyFromList( GameObject g ){
		enemyList.Remove(g);
		SetEnemyTactics();
	}


	//몬스터 전술 세팅함수
	public static void SetEnemyTactics(){
		getActiveEnemies();

		if(activeEnemies.Count > 0){//활성화중인 몬스터가 0이아니면
			for(int i=0; i<activeEnemies.Count; i++){ //활성화중인 몬스터만큼 실행
				if(i < MaxEnemyAttacking()){//활성화되어있는 몬스터가 공격가능 몬스터값 보다 작으면
					activeEnemies[i].GetComponent<EnemyAI>().SetEnemyTactic(ENEMYTACTIC.ENGAGE); //몬스터 전술상태 ENGAGE
				} else {
					activeEnemies[i].GetComponent<EnemyAI>().SetEnemyTactic(ENEMYTACTIC.KEEPMEDIUMDISTANCE);//그게 아니면 거리유지 상태로
				}
			}
		}
	}

	//몬스터 상태 변경함수
	public static void ForceEnemyTactic(ENEMYTACTIC tactic){
		getActiveEnemies();
		if(activeEnemies.Count > 0){ //활성화중인 몬스터가 0명이 아니면 활성화중인 몬스터 전술 변경
			for(int i=0; i<activeEnemies.Count; i++){
				activeEnemies[i].GetComponent<EnemyAI>().SetEnemyTactic(tactic);
			}
		}
	}

	//현재 활성화중인 몬스터 리스트에 추가함수
	public static void getActiveEnemies(){
		activeEnemies.Clear(); //리스트 클리어하고 
		foreach(GameObject enemy in enemyList){//활성화중인 몬스터 리스트에 다시 담아줌
			if(enemy != null && enemy.activeSelf)activeEnemies.Add(enemy);
		}
	}

	//플레이어가 죽으면 몬스터가 뒤로 빠지게 전술 변경
	public static void PlayerHasDied(){
		ForceEnemyTactic(ENEMYTACTIC.KEEPMEDIUMDISTANCE);
		enemyList.Clear();
	}

	//공격가능 최대 몬스터 숫자 반환
	static int MaxEnemyAttacking(){
		return GetSettings().MaxAttackers;
	}
}
