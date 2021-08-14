using UnityEngine;
using System.Collections;

public class EnemyAnimator : MonoBehaviour {

	private Animator animator;
	private AudioPlayer audioPlayer;
	private bool animatorActive;

	void Awake(){
		animator = GetComponent<Animator>(); 
	}

	
	public void Idle(){
		if(animator.isInitialized){ 
			animator.SetTrigger("Idle");
		}
	}

	public void Walk(){
		if(animator.isInitialized){ 
			animator.SetTrigger("Walk");
		}
	}

	public void Attack1(){		
		if(animator.isInitialized){ 
			animator.SetTrigger("Attack1");
			CancelInvoke(); //실행중인 Invoke가 있으면 꼬일수있어서 초기화
			Invoke("WaitForAnimationFinish", getAnimationLength("Enemy_Attack1"));//애니메이션 길이만큼 기다렸다가 함수실행
		}
	}
	public void Attack2()
	{
		if (animator.isInitialized)
		{
			animator.SetTrigger("Attack2");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Boss_Attack2"));
		}
	}
	public void Skill1()
	{
		if (animator.isInitialized)
		{
			animator.SetTrigger("Skill1");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Boss_Skill1"));
		}
	}
	public void Skill2()
	{
		if (animator.isInitialized)
		{
			animator.SetTrigger("Skill2");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Boss_Skill2"));
		}
	}
	public void Skill3()
	{
		if (animator.isInitialized)
		{
			animator.SetTrigger("Skill3");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Boss_Skill3"));
		}
	}
	public void Skill4()
	{
		if (animator.isInitialized)
		{
			animator.SetTrigger("Skill4");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Boss_Skill4"));
		}
	}
	public void Hit(){		
		if(animator.isInitialized){ 
			animator.SetTrigger("Hit");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Enemy_Hit")+0.15f);
		}
	}

	public void Death(){
		if(animator.isInitialized){ 
			animator.SetTrigger("Death");
		}
	}

	public void KnockDown(){
		if(animator.isInitialized) {
			animator.SetTrigger("KnockDown");
			CancelInvoke();
			Invoke("WaitForAnimationFinish", getAnimationLength("Enemy_KnockDown"));
		}
	}
   //##########################애니메이션 이벤트 함수####################
	public void skill1ThunderOn()//보스1 번개생성
    {
		transform.parent.GetComponent<EnemyActions>().skill1Thunder();
	}
	public void Skill2PosChange()//보스 위치 변경
    {
		transform.parent.GetComponent<EnemyActions>().Skill2ChangePos();

	}
	public void Boss2Skill1On()//보스2 돌진스킬
    {
		transform.parent.GetComponent<EnemyActions>().StartCoroutine("Boss2Skill1");
    }
	public void Boss2Skill2On()//보스2 순간이동 스킬
    {
		transform.parent.GetComponent<EnemyActions>().Boss2Skill2();

	}
	public void Boss2BoomOn()//보스2 공격2 폭발생성
    {
		transform.parent.GetComponent<EnemyActions>().Boss2AttackBall();
    }
	public void Boss2HitTimeOn()//슈퍼아머 시간
    {
		transform.parent.GetComponent<EnemyActions>().BossHitTime();
    }
	public void Shoot1On()//총알생성
    {
		transform.parent.GetComponent<EnemyActions>().Shoot1();
    }
	public void Shoot2On()
	{
		transform.parent.GetComponent<EnemyActions>().Shoot2();
	}
	public void Shoot3On()
	{
		transform.parent.GetComponent<EnemyActions>().Shoot3();
	}
	public void Boss3WindOn()//보스3 태풍생성
	{
		transform.parent.GetComponent<EnemyActions>().Boss3Wind();
	}
	public void Boss3Wind2On()//보스3 유도 태풍 생성
	{
		transform.parent.GetComponent<EnemyActions>().Boss3Wind2();
	}
	public void Check4Hit(){//공격 체크
		transform.parent.GetComponent<EnemyActions>().CheckForHit();
	}
	public void Check4HitDown()
	{
		transform.parent.GetComponent<EnemyActions>().CheckForHitDown();
	}


	public void PlaySFX(string name){
		GlobalAudioPlayer.PlaySFX(name);
	}

	public void CamShakeSmall(){
		Camera.main.GetComponent<CameraFollow>().CamShakeSmall();
	}
	//####################################################

	//몬스터 상태 Idle로 변경 함수
	public void WaitForAnimationFinish()
	{
		transform.parent.GetComponent<EnemyAI>().IDLE();
	}
	//애니메이션 길이 반환함수
	float getAnimationLength(string animName){
		if(animator.isInitialized){
			RuntimeAnimatorController ac = animator.runtimeAnimatorController;
			for(int i = 0; i<ac.animationClips.Length; i++){
				if(ac.animationClips[i].name == animName){
					return ac.animationClips[i].length;
         		}
         	}
		}
		return 0;
	}
}
