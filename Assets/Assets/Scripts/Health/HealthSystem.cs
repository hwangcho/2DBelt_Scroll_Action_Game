using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour {

	public int MaxHp = 20; //최대체력
	public int CurrentHp = 20; //현재체력
	public float jangpungCount; //장풍 기모으는 시간
	//델리게이트 이벤트 생성 
	public delegate void OnHealthChange(float percentage, GameObject GO);
	public static event OnHealthChange onHealthChange;
	public delegate void OnJangpungCount(float count,GameObject Go); //장풍 슬라이더 델리게이트 만듬
	public static event OnJangpungCount onjangpungCount;

	//체력 감소 함수
	public void SubstractHealth(int damage){

			//현재체력 감소하는데
			//clamp 사용해서 최소값이 0까지만 떨어지게함
			CurrentHp = Mathf.Clamp(CurrentHp -= damage, 0, MaxHp);

			//체력 슬라이더 업데이트
			SendUpdateEvent();
		
	}

	//체력 회복 함수
	public void AddHealth(int amount){
		CurrentHp = Mathf.Clamp(CurrentHp += amount, 0, MaxHp);//clamp 사용해서 최대체력까지만 현재체력이 증가하게 함
		SendUpdateEvent();
	}


	//현재 체력 슬라이더 업데이트
	void SendUpdateEvent(){
		float CurrentHealthPercentage = 1f/MaxHp * CurrentHp;
		if(onHealthChange != null) onHealthChange(CurrentHealthPercentage, gameObject);
	}
	//장풍 게이지 업데이트
	public void SendJangpungCount()
    {
		if (onjangpungCount != null) onjangpungCount(jangpungCount, gameObject);
    }
}
