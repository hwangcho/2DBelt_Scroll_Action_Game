using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	public enum CONTROLLER { KEYBOARD, TOUCHSCREEN } //컨트롤러 열거형
	[Header("Current Controller")]
	public CONTROLLER controller = CONTROLLER.KEYBOARD; //현재 컨트롤러
	private GameObject TSC; // 터치스크린
	private bool levelInProgress; //게임 진행중일때

	//키보드 키설정
	[Header("Keyboard keys")]
	public KeyCode Left = KeyCode.LeftArrow;
	public KeyCode Right = KeyCode.RightArrow;
	public KeyCode Up = KeyCode.UpArrow;
	public KeyCode Down = KeyCode.DownArrow;
	public KeyCode PunchKey = KeyCode.Z;
	public KeyCode KickKey = KeyCode.X;
	public KeyCode DefendKey = KeyCode.C;
	public KeyCode JumpKey = KeyCode.Space;


	//델리게이트 이벤트 생성해서 공격키,이동키 눌럿을때 실행하게해줌
	public delegate void InputEventHandler(Vector2 dir);
	public static event InputEventHandler onInputEvent;
	public delegate void CombatInputEventHandler(string action);
	public static event CombatInputEventHandler onCombatInputEvent;
	private GameSettings settings;

	void OnEnable(){
		EnemyWaveSystem.onLevelStart += OnLevelStart;
		EnemyWaveSystem.onLevelComplete += OnLevelEnd;
	}

	void OnDisable(){
		EnemyWaveSystem.onLevelStart -= OnLevelStart;
		EnemyWaveSystem.onLevelComplete -= OnLevelEnd;
	}
	//이벤트 실행함수
	public static void InputEvent(Vector2 dir){
		if( onInputEvent != null) onInputEvent(dir);
	}
	//이벤트 실행함수
	public static void CombatInputEvent(string action){
		if( onCombatInputEvent != null) onCombatInputEvent(action);
	}

	void Update(){
		if (!Pause.pauseon)
		{
			//키보드 사용중일때
			if (controller == CONTROLLER.KEYBOARD) KeyboardControls();

	
			//터치스크린 사용중일때
			if (controller == CONTROLLER.TOUCHSCREEN && !TSC) CreateTouchScreenControls(); //터치스크린 없으면 생성
			if (TSC) TSC.SetActive((controller == CONTROLLER.TOUCHSCREEN)); //컨트롤러가 터치스크린일때만 활성화
			if (TSC && !levelInProgress) TSC.SetActive(false); //진행중이 아니라면 비활성화
		}
	}

	void KeyboardControls(){
		
		//이동값
		float x = 0f;
	 	float y = 0f;
		//방향키 눌럿을때 이동값 
		if(Input.GetKey(Left)) x = -1f;
		if(Input.GetKey(Right)) x = 1f;
		if(Input.GetKey(Up)) y = 1f;
		if(Input.GetKey(Down)) y = -1f;

		Vector2 dir = new Vector2(x,y); //이동값 dir에 넣어줌
		InputEvent(dir);


		//공격키 누르면 이벤트에 넣어줌
		if(Input.GetKeyDown(PunchKey)){
			CombatInputEvent("Punch");
		}

		if (Input.GetKey(KickKey))
		{
			CombatInputEvent("Kick");
		}
		if (Input.GetKeyUp(KickKey))
		{
			CombatInputEvent("KickUp");
		}

		if (Input.GetKey(DefendKey)){
			CombatInputEvent("Defend");
		}

		if(Input.GetKeyDown(JumpKey)){
			CombatInputEvent("Jump");
		}
	}


	//터치스크린 생성
	void CreateTouchScreenControls(){
		GameObject canvas = GameObject.FindObjectOfType<Canvas>().gameObject; //Canvas 타입을 찾아 넣어줌
		if(canvas != null) { //null이 아닐때 리소스폴더에서 생성시킨뒤 부모 설정해줌
			TSC = GameObject.Instantiate(Resources.Load("UI_TouchScreenControls")) as GameObject;
			TSC.transform.SetParent(canvas.transform, false);
		}
	}
	//레벨 시작시 실행함수
	void OnLevelStart(){
		levelInProgress = true;
	}
	//레벨 끝낫을때 실행함수
	void OnLevelEnd(){
		levelInProgress = false;
	}
}

public enum INPUTTYPE {	
	KEYBOARD = 0,	
	JOYPAD = 2,	
	TOUCHSCREEN = 4, 
}
