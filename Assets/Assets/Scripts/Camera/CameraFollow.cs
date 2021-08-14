using UnityEngine;
using System.Collections;

//카메라 플레이어 따라오게하는 스크립트
public class CameraFollow : CameraShake {

	public GameObject followTarget; //타겟설정

	[Header ("Clamped ViewArea")]//카메라 고정위치
	public Vector2 LeftClamp;
	public Vector2 RightClamp;

	private IEnumerator camShakeCoroutine;
	private bool allowBacktrack;

	//델리게이트 이벤트에 함수 추가
	void OnEnable(){
		EnemyWaveSystem.onLevelStart += setPlayerAsTarget;
	}
	//함수 제거
	void OnDisable(){
		EnemyWaveSystem.onLevelStart -= setPlayerAsTarget;
	}

	void Start(){
		

		//설정해둔 세팅값 불러와서 설정
		GameSettings settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
		if(settings != null) allowBacktrack = settings.CameraBacktrack;
	}

	void Update(){
		if(followTarget != null){

			float x = followTarget.transform.position.x;
			float y = followTarget.transform.position.y + AdditionalOffset;

			
			Vector3 currentX = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0,0,0));

			float clampedX = (LeftClamp.x > currentX.x) ? transform.position.x :x;
			y = (y>RightClamp.y) ? y : RightClamp.y;
			Vector3 ClampedPos = new Vector3(Mathf.Clamp(x, clampedX, RightClamp.x), y, transform.position.z);
		
			transform.position = ClampedPos + (Vector3.up * AdditionalOffset);

			if(!allowBacktrack){
				if(x > LeftClamp.x && LeftClamp.x < RightClamp.x) LeftClamp.x = x;
			}
		}
	}

	//카메라 고정값 다음으로 변경함수
	public void SetNewClampPosition(Vector2 Pos, float lerpTime){
		StartCoroutine(LerpToNewClamp(Pos, lerpTime));
	}

	//왼쪽 고정값 변경 함수
	public void SetLeftClampedPosition(Vector2 Pos){
		LeftClamp = Pos;
	}

	//카메라 새로운 위치로갈때 부드럽게 가는 코루틴
	private IEnumerator LerpToNewClamp(Vector2 Pos, float lerpTime){
		float camExtentV =  GetComponent<Camera>().orthographicSize;
		float camExtentH = (camExtentV * Screen.width) / Screen.height;
		float t=0;
		Vector2 startPos = RightClamp;
		Vector2 endPos = new Vector2(Pos.x - camExtentH, Pos.y + camExtentV);

		while(t<1){
			RightClamp = Vector2.Lerp(startPos, endPos, MathUtilities.CoSinLerp(0,1,t));
			t += Time.deltaTime / lerpTime;
			yield return 0;
		}

		RightClamp = endPos;
	}

	//타겟을 플레이어로 설정하는 함수
	void setPlayerAsTarget(){
		followTarget = GameObject.FindGameObjectWithTag("Player");//태그로 플레이어를 찾음
	}
}