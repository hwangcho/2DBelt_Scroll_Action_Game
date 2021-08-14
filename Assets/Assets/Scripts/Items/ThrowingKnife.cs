using UnityEngine;
using System.Collections;

public class ThrowingKnife : MonoBehaviour
{

	public float speed; //칼 날아가는 속도
	public int Damage = 10; //대미지
	public string sfx = "PunchHit"; //효과음

	//칼 던지는 함수
	//플레이어 방향값 받아와서 좌우 반전후 코루틴 실행
	public void ThrowKnife(int direction)
	{
		transform.localScale = new Vector3(direction, 1, 1);
		StartCoroutine (startTravel(direction));
	}
	//칼 날아가는 코루틴(무한반복)
	IEnumerator startTravel(int direction)
	{
		while (true) {
			transform.position += (Vector3.right * direction * speed) * Time.deltaTime;
			yield return null;
		}
	}
	//몬스터에 닿앗을때
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag ("Enemy")) {
			DamageObject d = new DamageObject(Damage, gameObject); //DamageObject 에 넣어서 선언
			d.attackType = AttackType.KnockDown; //어택타입 녹다운
			col.GetComponent<EnemyAI>().Hit(d); //몬스터 피격
			col.GetComponent<HealthSystem>().SubstractHealth(d.damage);//몬스터 체력감소
			GlobalAudioPlayer.PlaySFX(sfx); // 효과음
			ShowHitEffect(); //타격 이펙트
			Destroy(gameObject);//삭제
		}
	}

	//타격 이펙트
	void ShowHitEffect()
	{
		GameObject.Instantiate(Resources.Load("HitEffect"), transform.position, Quaternion.identity);
	}
}
