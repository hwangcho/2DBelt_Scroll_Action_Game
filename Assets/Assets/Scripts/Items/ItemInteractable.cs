using UnityEngine;
using System.Collections;

//ItemAction 상속
public class ItemInteractable : ItemActions {

	
	void OnEnable(){
		ItemManager.AddItemToList(gameObject);
	}

	void OnDisable() {
		ItemManager.RemoveItemFromList(gameObject);
	}

	void Start(){
		SetSortingOrder();
	}

	//플레이어가 무기 콜라이더 범위안에 들어오면
	void OnTriggerEnter2D(Collider2D coll) {
		if(coll.CompareTag ("Player") && item.isPickup){ 
			coll.GetComponent<PlayerCombat>().itemInRange = gameObject; //범위안의 아이템에  현재 아이템 넣어줌
		}
	}

	//무기 콜라이더에서 나오면 다시 null로 변경
	void OnTriggerExit2D(Collider2D coll){
		if(coll.CompareTag ("Player") && item.isPickup){ 
			coll.GetComponent<PlayerCombat>().itemInRange = null;
		}
	}

	//Order in Layer 수정
	void SetSortingOrder() {
		SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
		if(sr != null) sr.sortingOrder = (int)(transform.position.y * -20);
	}

	//아이템 활성화
	public void ActivateItem(GameObject _target){

		//타겟설정(플레이어
		target = _target;

		//callMethod값 으로 인보크 실행
		if(item.callMethod != "") Invoke(item.callMethod, 0);

		//획득 효과음
		GlobalAudioPlayer.PlaySFX(item.sfx);

		//바닦에있던 아이템 삭제
		Destroy(gameObject);
	}
}