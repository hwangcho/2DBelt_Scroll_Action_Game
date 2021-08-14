using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
  
//방향키 버튼
public class UIDPadButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public Vector2 dir; //이동할 방향
	private bool pressed; //버튼을 안떗는지 확인

	//버튼눌럿을때 누른버튼의 방향으로 이동
	public void OnPointerDown(PointerEventData eventData){
		InputManager.InputEvent(dir);
		pressed = true;
	}

	//버튼을 때면 초기화
	public void OnPointerUp(PointerEventData eventData){
		InputManager.InputEvent(Vector2.zero);
		pressed = false;
	}
	private void Update()
	{
		//버튼 누른상태에서 공격하면 멈춰버리고 
		//방향키 다시 눌러야되서 수정
		if (pressed)
			InputManager.InputEvent(dir);

	}
}
