using UnityEngine;
using System.Collections;

//플레이어 상태 스크립트.
public class PlayerState : MonoBehaviour {
	//플레이어 현재상태 변수
	public PLAYERSTATE currentState = PLAYERSTATE.IDLE;

	//플레이서 현재상태 변경 함수 (매개변수로 상태받아와서 변경)
	public void SetState(PLAYERSTATE state){
		currentState = state;
	}
}
//플레이어 상태 종류
public enum PLAYERSTATE 
{
	IDLE,
	MOVING,
	JUMPING,
	PUNCH,
	KICK,
	DEFENDING,
	HIT,
	DEATH,
	THROWKNIFE,
	PICKUPITEM,
	KNOCKDOWN,
};