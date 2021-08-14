using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {

	private Animator animator;
	private AudioPlayer audioPlayer;

	void Awake() {
		animator = GetComponent<Animator> ();
	}

	//플레이어 애니 Idle로 변경
	public void Idle() {
		animator.SetTrigger ("Idle");
		animator.SetBool("Walk", false);
	}
	//Walk 애니 함수
	public void Walk() {
		animator.SetBool("Walk", true);
	}
	//펀치 애니 함수
	public void Punch(int id) {//id : 몇번째 펀치팔지 변수 받아옴
		animator.SetTrigger ("Punch" + id); // +id 로 각각 다른 펀치 애니 실행
		StopAllCoroutines();//기존에 실행중이던 코루틴과 꼬일수있어서 실행중이던 코루틴 종료
		StartCoroutine (WaitForAnimationFinish ("Punch" + id)); 
	}

	//Kick 애니 함수
	public void Kick(int id) {
		animator.SetTrigger ("Kick" + id);
		StartCoroutine (WaitForAnimationFinish ("Kick" + id));
	}
	//방어 애니 함수
	public void StartDefend() {		
		animator.SetBool ("Defend", true);
	}
	
	public void StopDefend() {		
		animator.SetBool ("Defend", false);
	}

	//공격에 몬스터가 맞았는지 확인하는 이벤트 함수
	public void Check4Hit() {
		transform.parent.GetComponent<PlayerCombat>().CheckForHit (); //PlayerCombat의 함수 호출
	}
	//대쉬 이벤트 함수
	public void Dash()
    {
		transform.parent.GetComponent<PlayerMovement>().StopCoroutine("DashAni"); //꼬임방지 초기화
		transform.parent.GetComponent<PlayerMovement>().StartCoroutine("DashAni"); //대쉬코루틴으로 플레이어 앞으로 이동and 무적
    }
	//대쉬 애니 함수
	public void Jump() {
		animator.SetBool("Walk", false);
		animator.SetTrigger ("Jump");
		StopAllCoroutines();//추가해서 코루틴 이상해지는거 막음
		StartCoroutine (WaitForAnimationFinish ("Jump"));
	}
	//피격 함수
	public void Hit() {
		animator.SetTrigger ("Hit");
		StartCoroutine (WaitForAnimationFinish ("Hit"));
	}
	//죽음 함수
	public void Death() {
		animator.SetTrigger ("Death");
	}
	//아이템 줍는 애니 함수
	public void PickUpItem() {
		animator.SetTrigger ("Pickup");
		StartCoroutine (WaitForAnimationFinish ("Pickup"));
	}
	//무기 던지기 함수
	public void Throw() {		
		animator.SetTrigger ("Throw");
		StartCoroutine (WaitForAnimationFinish ("Throw"));
	}
	//녹다운 함수
	public void KnockDown() {
		animator.SetTrigger ("KnockDown");
		StopAllCoroutines();//공격맞는순간 펀치하고 끊기고 펀치연타하면 뭔가이상해져서 넣음
		StartCoroutine(WaitForAnimationFinish ("KnockDown"));
	}

	//피격 이펙트 생성 함수
	public void ShowHitEffect() {
		GameObject.Instantiate (Resources.Load ("HitEffect"), transform.position+Vector3.up*1.5f, Quaternion.identity);
	}

	//공격 방어시 이펙트 생성 함수
	public void ShowDefendEffect() {
		Vector3 offset = Vector3.up * 1.7f + Vector3.right * (int)transform.parent.GetComponent<PlayerMovement> ().getCurrentDirection () * .2f; //플레이어 방향값을 받아와 좌,우 위치선정
		GameObject.Instantiate (Resources.Load ("DefendEffect"), transform.position + offset, Quaternion.identity);
	}

	//효과음 실행 함수
	public void PlaySFX(string sfxName) {
		GlobalAudioPlayer.PlaySFX (sfxName);
	}

	//카메라 진동 효과 함수
	public void CamShakeSmall() {
		Camera.main.GetComponent<CameraFollow> ().CamShakeSmall ();
	}

	//rigidbody에 속도 변경 함수
	public void AddForce(float force) {
		StartCoroutine (AddForceCoroutine(force));
	}

	//addforce 코루틴
	IEnumerator AddForceCoroutine(float force) {
		Rigidbody2D rb = transform.parent.GetComponent<Rigidbody2D> (); //부모의 rigidbody 컴포넌트
		DIRECTION dir = transform.parent.GetComponent<PlayerMovement> ().getCurrentDirection (); //현재 플레이어 방향 받아옴
		float speed = 4f; //t의 증가 속도
		float t = 0;


		//while 문으로 rb.velcitiy의 속도가 force값 에서 0까지 되게함 
		while (t < 1f) {
			rb.velocity = Vector2.right * transform.localScale.x * Mathf.Lerp (force, 0, MathUtilities.Sinerp (0, 1, t));//현재 플레이어 방향값을받아 좌,우로 갈지 설정하고 Mathf.Lerp(시작값,끝값,Mathf.Sin을 통해 1~0까지)
			t += Time.deltaTime * speed; //t의 증가치
			yield return null;
		}
	}

	//애니메이션 끝을 알려주는 코루틴
	IEnumerator WaitForAnimationFinish(string animName) {
		float time = GetAnimDuration(animName);//time 에 반환값 넣어줌
		yield return new WaitForSeconds(time); //그시간만큼 기다림
		transform.parent.GetComponent<PlayerCombat>().Ready();//Ready 함수 실행시켜 다음행동 가능하게함
	}


	float GetAnimDuration(string animName)
	{
		RuntimeAnimatorController ac = animator.runtimeAnimatorController; //현재 애니메이터에 사용되는 애니메이션
		for (int i = 0; i < ac.animationClips.Length; i++) //애니메이션 개수만큼 포문
		{

			if (ac.animationClips[i].name == animName)//애니메이션 이름과 받아온 animName이 같으면
			{
				return ac.animationClips[i].length; // 그 애니메이션 길이만큼 리턴해줌
			}
		}
		return 0f; //만약 없을때는 0값 리턴
	}
}
