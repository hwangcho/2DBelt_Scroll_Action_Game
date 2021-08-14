using UnityEngine;
using System.Collections;

public class ItemActions : MonoBehaviour  {

	[HideInInspector]
	public GameObject target; //플레이어
	public Item item; 

	//플레이어 체력증가 함수
	public void GiveHealthToPlayer(){
		HealthSystem ph = target.GetComponent<HealthSystem>();
		if(ph != null && item != null) ph.AddHealth(item.data);
	}

	//플레이어 무기 획득 함수
	public void GiveWeaponToPlayer(){
		PlayerCombat pc = target.GetComponent<PlayerCombat>();
		if(pc != null) pc.EquipWeapon(item);
	}

	//드럼통 파괴 함수
	public void DestroyDrumBarrel(){

		//이팩트 생성
		GameObject.Instantiate(Resources.Load("HitEffect"), transform.position + Vector3.up * 1.2f, Quaternion.identity);

		//깨진 드럼통 생성
		GameObject g = GameObject.Instantiate(Resources.Load("DrumbarrelDestroyed"), transform.position, Quaternion.identity) as GameObject;

		//플레이어 방향에따라 좌우 반전
		if(target.transform.position.x > transform.position.x) g.transform.localScale = new Vector3(-1,1,1);

		//생성될 아이템이있으면 아이템 생성
		if(item.SpawnObjectOnDestroy != null) SpawnItem();
	}

	//나무통 파괴 함수
	public void DestroyWoodenCrate(){


		GameObject.Instantiate(Resources.Load("HitEffect"), transform.position + Vector3.up * 1.2f, Quaternion.identity);

	
		GameObject g = GameObject.Instantiate(Resources.Load("WoodenCrateDestroyed"), transform.position, Quaternion.identity) as GameObject;


		if(target.transform.position.x > transform.position.x) g.transform.localScale = new Vector3(-1,1,1);


		if(target.transform.position.x > transform.position.x) g.transform.localScale = new Vector3(-1,1,1);


		if(item.SpawnObjectOnDestroy != null) SpawnItem();
	}

	//아이템 생성 함수
	public void SpawnItem(){
		GameObject.Instantiate(item.SpawnObjectOnDestroy, transform.position, Quaternion.identity);
	}
}
