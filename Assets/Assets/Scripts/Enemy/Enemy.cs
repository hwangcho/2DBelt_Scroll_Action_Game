using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//몬스터 공격범위,이동속도 등등 선언 클래스
[System.Serializable]
public class Enemy : MonoBehaviour {

	
	public float attackRange = 1.5f; //공격범위
	public float closeRangeDistance = 2.5f; //플레이어와 가까운간격
	public float midRangeDistance = 3f;//플레이어와 중간간격
	public float farRangeDistance = 4.5f;//플레이어와 멀리있는간격
	public float walkSpeed = 2.5f; //이동속도
	public string enemyName = "";//이름
	public float sightDistance = 50f; //목표물 체크 거리
	public int attackDamage = 2; //공격력
	public float attackInterval = 1f; //공격 딜레이
	public bool targetSpotted; //플레이어 발견
	public bool isBoss;//만약 보스라면추가
	public bool isShooter;//원거리 딜러라면
	    
	//델리게이트 이벤트 생성
	public delegate void UnitEventHandler(GameObject Unit);
	public static event UnitEventHandler OnUnitSpawn;
	public static event UnitEventHandler OnUnitDestroy;

	//몬스터 삭제
	public void DestroyUnit(){
		if(OnUnitDestroy != null) OnUnitDestroy(gameObject);
		Destroy(gameObject);
	}

}