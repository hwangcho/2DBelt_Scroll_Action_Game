using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Rigidbody2D))] //자동으로 스크립트,컴포넌트 추가해줌
[RequireComponent (typeof(PlayerState))]
public class PlayerMovement : MonoBehaviour {

	public SpriteRenderer GFX;//플레이어 캐릭터 스프라이트
	public SpriteRenderer Shadow; // 그림자

	[Header ("Settings")]
	public float walkSpeed = 1f; //이동속도


	public Rigidbody2D rb;
	public DIRECTION currentDirection; //현재 좌우방향
	public Vector2 inputDirection; //방향키 입력시 좌우방향 설정을위한 변수

	private PlayerAnimator animator; //플레이어애니메이터 스크립트
	private PlayerState playerState; //플레이어상태
	//private AudioPlayer audioPlayer; 
	private bool isDead = false; //죽었는지
	private float screenEdgeHorizontal = 80f; 
	private float screenEdgeVertical = 18f; 


	//움직일수 있는상태 리스트
	private List<PLAYERSTATE> MovementStates = new List<PLAYERSTATE> {
		PLAYERSTATE.IDLE,
		PLAYERSTATE.JUMPING,
		PLAYERSTATE.MOVING
	};
	//활성화될때 델리게이트의 이벤트에 추가함
	void OnEnable() {
		InputManager.onCombatInputEvent += InputEventAction;
		InputManager.onInputEvent += InputEvent;
	}
	//비활성화때 제거
	void OnDisable() {
		InputManager.onCombatInputEvent -= InputEventAction;
		InputManager.onInputEvent -= InputEvent;
	}

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
		animator = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();
		currentDirection = DIRECTION.Right;

	}

	void Update() {
		if (!Pause.pauseon)
		{
			UpdateSortingOrder();
		}
	}

	//이동 인풋함수 
	void InputEvent(Vector2 dir) {
		inputDirection = dir; //받아온 vector2 값을 inputDirection에 넣어줌

		if (MovementStates.Contains (playerState.currentState) && !isDead) {//플레이어의 현재상태가 MovementStates 상태에 포함되어있다면 그리고 isDead = false 일때

			//y축 이동은 느리게해줌
			dir = new Vector2 (dir.x, dir.y * .7f);
			Move (dir * walkSpeed);//move 함수에 전달

		} else {//위조건이 안맞으면

			
			Move (Vector3.zero);//이동값 0
		}
	}
	//InputManager에 설정된 키가 눌렀을때 action값을 받아옴
	void InputEventAction(string action){
		//Jumping은 수정되어 Dash 입니다. 
		if (action == "Jump" && MovementStates.Contains (playerState.currentState) && !isDead) //action이 Jump 이고 플레이어의 현재상태가 MovementStates 상태에 포함되어있다면 그리고 isDead = false 일때
			if (playerState.currentState != PLAYERSTATE.JUMPING) doDash(); //점프상태에서 점프는 못하기때문에 다시조건을줘서 점프상태가 아닐때 실행	
	}
	//대쉬함수
	void doDash()
    {
		playerState.SetState(PLAYERSTATE.JUMPING);//플레이어 상태 변경
        animator.Jump();//애니메이터의 Jump함수 실행

	}
	//대쉬시 바라보는 방향으로 돌진 코루틴
	public IEnumerator DashAni()
    {
		animator.AddForce(30);//AddForce함수 실행
		transform.GetChild(2).GetComponent<BoxCollider2D>().enabled = false;//자식의 3번째 위치한 오브젝트의 박스콜라이더 비활성화(무적 시간)
		yield return new WaitForSeconds(0.3f);
		transform.GetChild(2).GetComponent<BoxCollider2D>().enabled = true;


	}


	//플레이어 이동
	void Move(Vector3 vector) {
		if (playerState.currentState != PLAYERSTATE.JUMPING) {//점핑 상태가 아니면

			

			//rb가 존재하면 실행 
			if(rb != null)	rb.velocity = vector; // rigidbody2D 의 속도를 vector로 변경

			//vector의 x값과 y값 더한 절대값이 0이상일때
			if (Mathf.Abs (vector.x + vector.y) > 0) {

				
				int i = Mathf.Clamp (Mathf.RoundToInt (vector.x), -1, 1); //vector.x 값을 반올림한 값     Clamp(변수,최소값,최대값) 
				//i가 0이라면  현재 GFX의 localScale.x 값을 반올림해 i에 넣어줌
				if(i == 0) i = Mathf.RoundToInt(GFX.transform.localScale.x);

				currentDirection = (DIRECTION)i; //현재방향에 i값을 곱해줌
				animator.Walk(); //애니메이터의 Walk함수실행

			} else {
				animator.Idle();
			}
			LookToDir (currentDirection); 

		}
		KeepPlayerInCameraView();
	}

	//플레이어가 카메라 밖으로 못나가게 하는 함수
	void KeepPlayerInCameraView(){
		Vector2 playerPosScreen = Camera.main.WorldToScreenPoint(transform.position);

		if(playerPosScreen.x + screenEdgeHorizontal > Screen.width && (playerPosScreen.y - screenEdgeVertical < 0)){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.x + screenEdgeHorizontal > Screen.width){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-screenEdgeHorizontal, playerPosScreen.y, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.x - screenEdgeHorizontal < 0f && (playerPosScreen.y - screenEdgeVertical < 0)){
			transform.position = Camera.main.ScreenToWorldPoint( new Vector3(screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.x - screenEdgeHorizontal < 0f){
			transform.position = Camera.main.ScreenToWorldPoint( new Vector3(screenEdgeHorizontal, playerPosScreen.y, transform.position.z - Camera.main.transform.position.z));

		} else if((playerPosScreen.y - screenEdgeVertical < 0) && (playerPosScreen.x + screenEdgeHorizontal > Screen.width)){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width-screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if((playerPosScreen.y - screenEdgeVertical < 0) && (playerPosScreen.x - screenEdgeHorizontal < 0f)){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenEdgeHorizontal, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));

		} else if(playerPosScreen.y - screenEdgeVertical < 0){
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(playerPosScreen.x, screenEdgeVertical, transform.position.z - Camera.main.transform.position.z));
		}
	}

	//현재상태를 Idle로 변경해주는 함수
	public void Idle() {
		if (playerState.currentState != PLAYERSTATE.JUMPING) {//점핑 상태가 아니라면
			animator.Idle ();
			playerState.SetState (PLAYERSTATE.IDLE);
			rb.velocity = Vector3.zero;//물리적속도 0으로 변경
		}
	}

	//플레이어 좌우반전 함수
	public void LookToDir(DIRECTION dir) {
		if (dir == DIRECTION.Left)//받아온 dir 이 Left라면 
			GFX.transform.localScale = new Vector3(-1,1,1); //스케일 변경으로 왼쪽바라보게함
		else if (dir == DIRECTION.Right)
			GFX.transform.localScale = new Vector3(1,1,1);//스케일 변경으로 오른쪽바라보게
	}

	//플레이어의 y값에따라 order in Layer 값 변경 
	//플레이어가 몬스터보다 위에있으면 몬스터에 가려짐
	void UpdateSortingOrder() {
		GFX.sortingOrder = Mathf.RoundToInt (transform.position.y * -10f); //플레이어의 현재 y값에 -10곱하고 반올림값 넣어줌
		Shadow.sortingOrder = GFX.sortingOrder - 1;//Shadow의 Layer값은 Gfx의 값에 -1
	}

	//현재 플레이어 방향값 반환함수
	public DIRECTION getCurrentDirection() {
		return currentDirection;
	}

	//플레이어 방향 업데이트 함수
	public void updateDirection() {
		if(inputDirection.magnitude > 0){
			int i = Mathf.Clamp (Mathf.RoundToInt (inputDirection.x), -1, 1);
			currentDirection = (DIRECTION)i;
			LookToDir (currentDirection);
		}
	}

	//플레이어 사망
	void Death(){
		isDead = true;
	}


}
//방향값 열거형
public enum DIRECTION {
	Left = -1,
	Right = 1,
	Up = 2,
	Down = -2,
};