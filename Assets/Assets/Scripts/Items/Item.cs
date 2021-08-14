using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item {

	public string itemName;	//아이템이름
	public string sfx = ""; //효과음 이름
	public string callMethod = ""; //함수호출 이름
	public int data = 0; //체력 증가치 등등
	public bool isPickup; // 픽업 아이템이 맞는지
	public GameObject SpawnObjectOnDestroy; //파괴될때 아이템 생성인지
}