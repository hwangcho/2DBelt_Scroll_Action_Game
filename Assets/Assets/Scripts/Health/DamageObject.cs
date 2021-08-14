using UnityEngine;
using System.Collections;

[System.Serializable]
public class DamageObject {

	public int damage;//공격력
	public float range;//공격범위
	public AttackType attackType; //공격 타입
	public GameObject inflictor; //공격자
	public float comboResetTime = .5f; //콤보 리셋 시간

	//오버로드 사용
	//함수의 매개변수를 받아와 값을 넣어줌
	public DamageObject(int _damage, GameObject _inflictor){
		damage =  _damage;
		inflictor = _inflictor;
	}

	public DamageObject(int _damage, AttackType _attackType, GameObject _inflictor){
		damage =  _damage;
		attackType = _attackType;
		inflictor = _inflictor;
	}
}

public enum AttackType
{
    Default = 0,
    SoftPunch = 10,
    MediumPunch = 20,
    KnockDown = 30,
    SoftKick = 40,
    HardKick = 50,
    SpecialMove = 60,
    DeathBlow = 70,
};
