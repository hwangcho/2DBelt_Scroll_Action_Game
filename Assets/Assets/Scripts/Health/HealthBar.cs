using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour {

	public Text nameField; //보여줄 이름
	public Slider HpSlider; //체력 슬라이더
	public bool isPlayer; //플레이어인지 몬스터인지
	public Slider JangpungSldier; //장풍 슬라이더

	void OnEnable() {
		HealthSystem.onHealthChange += UpdateHealth;
		HealthSystem.onjangpungCount += UpdateJangpungCount; //장풍 슬라이더 델리게이트 추가
	}

	void OnDisable() {
		HealthSystem.onHealthChange -= UpdateHealth;
		HealthSystem.onjangpungCount -= UpdateJangpungCount;

	}
	//시작할때 플레이어라면 체력바 활성화
	void Start(){
		HpSlider.gameObject.SetActive(isPlayer);
	}

	//체력 슬라이더 업데이트
	void UpdateHealth(float percentage, GameObject go){
		//받아온 게임오브젝트의 태그가 플레이어고 isPlayer가 true일때 
		if(isPlayer && go.CompareTag("Player")){
			HpSlider.value = percentage; //체력 업데이트
		} 	

		if(!isPlayer && go.CompareTag("Enemy")){ //몬스터일때
			HpSlider.gameObject.SetActive(true); //비활성화중이던 체력바 활성화
			HpSlider.value = percentage; //체력 업데이트
			nameField.text = go.GetComponent<Enemy>().enemyName; //몬스터이름 넣어줌
			if(percentage == 0) Invoke("HideOnDestroy", 2); //체력이 0이되면 체력바 비활성화
		}
	}
	//플레이어일때만 장풍게이지 업데이트
    private void UpdateJangpungCount(float jangpungcount,GameObject go) //장풍 기모으기 슬라이더 업데이트 
    {
		if (isPlayer && go.CompareTag("Player"))
		{
			JangpungSldier.value = jangpungcount;
		}
	}

	//체력바 && 이름 초기화함수
    void HideOnDestroy(){
		HpSlider.gameObject.SetActive(false);
		nameField.text = "";
	}
}
