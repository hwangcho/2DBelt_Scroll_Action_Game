using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//메인메뉴 뒷배경 움직이게 함
public class MainMenu_ScrollingBG : MonoBehaviour {

	private RawImage rawImage; 
	public float scrollSpeed = .01f;//움직일 스피드

	void Awake(){
		rawImage = GetComponent<RawImage>();
	}

	void Update(){
		if(rawImage != null){ 
			rawImage.uvRect = new Rect(Vector2.right * Time.time * scrollSpeed, rawImage.uvRect.size); //오른쪽으로 계속 이동
		}
	}
}
