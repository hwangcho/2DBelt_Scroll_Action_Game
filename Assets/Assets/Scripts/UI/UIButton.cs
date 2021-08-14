using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
  
//공격 버튼
public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public string actionDown = ""; //어떤 공격을할지 적어둠
	public string actionUp = ""; //버튼땔때 실행될 공격이있으면 적어둠
	public bool updateEveryFrame = false; //누르고있을때 실행시킬건지
	private bool pressed; //버튼 누르고있나 확인
	
	//버튼 눌럿을때
	public void OnPointerDown(PointerEventData eventData){
		InputManager.CombatInputEvent(actionDown);//어떤 공격인지 보내고 함수호출
		pressed = true;
	}
	//버튼 때면
	public void OnPointerUp(PointerEventData eventData){
		if(actionUp != "") InputManager.CombatInputEvent(actionUp); //actionUp이 있다면 실행
		pressed = false;
	}

	void Update(){
		//가드 & 장풍 기모으기 처럼 누르고있을때 지속적으로 실행해야되는것들 실행시켜줌
		if(updateEveryFrame && pressed) InputManager.CombatInputEvent(actionDown);
	}
}
