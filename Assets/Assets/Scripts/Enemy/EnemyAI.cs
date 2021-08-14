using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : EnemyActions {
	
	public float XDistance = 0; //플레이어와 몬스터 x축 거리
	public float YDistance = 0; //플레이어와 몬스터 y측 거리
	public bool enableAI; //AI 가능여부
	private List<ENEMYSTATE> ActiveAIStates = new List<ENEMYSTATE> { ENEMYSTATE.IDLE, ENEMYSTATE.RUN, ENEMYSTATE.WALK }; //활동가능 리스트
	private List<ENEMYSTATE> HitStates = new List<ENEMYSTATE> { ENEMYSTATE.DEATH, ENEMYSTATE.KNOCKDOWN, ENEMYSTATE.KNOCKDOWNGROUNDED }; // 피격 리스트
	float boss1AtkTime;//보스 공격딜레이

	void Start(){
		animator = GFX.GetComponent<EnemyAnimator>();
		rb = GetComponent<Rigidbody2D>();
		EnemyManager.enemyList.Add(gameObject); //시작할때 몬스터를 list에 추가해줌
		RandomizeValues();//이동속도,공격간격 랜덤
		boss1AtkTime = Time.time; //공격시간 초기화
		bossNoHitTime = Time.time;// 슈퍼아머 시간 초기화

	}
	//활성화되면 타겟을 플레이어로 변경
	void OnEnable(){
		SetTarget2Player(); 
	}

	void Update(){
		//죽어있지않고 enableAI가 true일때
		if(!isDead && enableAI){
			if(ActiveAIStates.Contains(enemyState) && targetSpotted){ //현재 몬스터상태가 활동가능상태에 포함되어있고 targetSpotted가 true일때
				AI();

			} else {

				
			 	Look4Target();
			}
		}
		UpdateSpriteSorting();//order in Layer 값 계속 변경
	}

	//몬스터 AI함수
	//플레이어에게 이동하고 공격 관리
	void AI(){
		LookAtTarget();//플레이어를 바라봄
		range = GetRangeToTarget();//플레이어와의 간격을 받아옴

		
		//공격가능 범위일때 
		//몬스터의 전술상태에 따라 다르게행동
		//EnGage 일때 플레이어를 공격
		//StandStill일때는 가만히 서있고
		//나머지는 각각 유지해야될 범위로 걸어감
		if(range == RANGE.ATTACKRANGE){
			if(enemyTactic == ENEMYTACTIC.ENGAGE)ATTACK();
			if(enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE) MoveTo(midRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE) MoveTo(farRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}

		//가까운 거리상태일때
		//ENGAGE일땐 플레이어에게 걸어가고
		//나머지는 위와 동일하게 거리유지
		//그러나 아까와똑같이 조건없이 MoveTo 함수를 사용하면 몬스터가 제자리걸음을 하기때문에
		//유지해야될 일정거리가되면 가만히있게 조건문 생성
		if(range == RANGE.CLOSERANGE){
			if(enemyTactic == ENEMYTACTIC.ENGAGE) MoveTo(attackRange-.7f, walkSpeed);
			if (enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE)
			{
				if (XDistance > closeRangeDistance - 0.2f && XDistance < closeRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(closeRangeDistance, walkSpeed);
			}
			if (enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE)
			{
				if (XDistance > midRangeDistance - 0.2f && XDistance < midRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(midRangeDistance, walkSpeed);
			}
			if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE)
			{
				if (XDistance > farRangeDistance - 0.2f && XDistance < farRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(farRangeDistance, walkSpeed);
			}
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}

		//중간거리
		if(range == RANGE.MIDRANGE){
			if(enemyTactic == ENEMYTACTIC.ENGAGE) MoveTo(attackRange -.7f, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if (enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE)
			{
				if (XDistance > midRangeDistance - 0.2f && XDistance < midRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(midRangeDistance, walkSpeed);
			}
			if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE)
			{
				if (XDistance > farRangeDistance - 0.2f && XDistance < farRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(farRangeDistance, walkSpeed);
			}
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}

		//먼 거리
		if(range == RANGE.FARRANGE){ 
			if(enemyTactic == ENEMYTACTIC.ENGAGE) MoveTo(attackRange -.7f, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPSHORTDISTANCE) MoveTo(closeRangeDistance, walkSpeed);
			if(enemyTactic == ENEMYTACTIC.KEEPMEDIUMDISTANCE)
				if (XDistance > midRangeDistance - 0.2f && XDistance < midRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(midRangeDistance, walkSpeed);
			if (enemyTactic == ENEMYTACTIC.KEEPFARDISTANCE)
			{
				if (XDistance > farRangeDistance - 0.2f && XDistance < farRangeDistance + 0.2f && YDistance < 0.1f)
				{
					IDLE();
				}
				else
					MoveTo(farRangeDistance, walkSpeed);
			}
			if(enemyTactic == ENEMYTACTIC.STANDSTILL) IDLE();
		}
		//보스1 패턴 
		if(enemyTactic == ENEMYTACTIC.Boss)
        {
			if (range == RANGE.ATTACKRANGE && Time.time - boss1AtkTime > 1.5f) //공격 가능범위이고 공격시간이 1.5초 지나면 공격실행
			{
				//랜덤값을 줘서 몬스터가 공격을쓸지 스킬을쓸지 설정
				//switch 사용해서 0,1일땐 공격 2일땐 스킬
				int randAtk = Random.Range(0, 3);
				switch (randAtk)
				{
					case 0:
					case 1:
						ATTACK();
						boss1AtkTime = Time.time;//공격시간 초기화
						bossNoHitTime = Time.time; //보스 슈퍼아머 시간 초기화

						break;
					case 2:
						Skill1();
						boss1AtkTime = Time.time;
						bossNoHitTime = Time.time;

						break;
				}
			}
			else if(range == RANGE.ATTACKRANGE && Time.time - boss1AtkTime <= 1.5f) IDLE(); //만약 공격범위내에있고  공격시간이 1.5초이하면 가만히서있게
			if (range == RANGE.CLOSERANGE && Time.time - boss1AtkTime > 1.5f)//가까운범위이고 공격1.5초후
			{
				//마찬가지로 랜덤값을줘서 스킬1사용할지 스킬2 사용할지 설정했지만
				//조건문으로 체력이 50%이하가되면 다른패턴의 스킬을 사용하게 변경
				int randAtk = Random.Range(0, 3);
				
				//체력 50%보다 클때
				if(GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
                {
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill1();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
					}
                }
				//체력이 50%히아일때
                else if( GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
                {
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill3();
                            boss1AtkTime = Time.time + 1.2f; //스킬마다 애니메이션 시간이달라서 공격가능시간에 1.2초 더해줌
							bossNoHitTime = Time.time;

							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
					}
				}

				
				//가까운거리고 공격시간이1.5초이하면 공격범위내로 이동
			}else if(range == RANGE.CLOSERANGE && Time.time - boss1AtkTime <= 1.5f)
				MoveTo(attackRange - .7f, walkSpeed);
			if (range == RANGE.MIDRANGE && Time.time - boss1AtkTime > 1.5f)
			{
				//위와 동일
				int randAtk = Random.Range(0, 3);
				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill1();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
					}
				}
				else if(  GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp/2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill3();
							boss1AtkTime = Time.time+1.2f;
							bossNoHitTime = Time.time;
							break;
					}
				}
			}
			else if (range == RANGE.MIDRANGE && Time.time - boss1AtkTime <= 1.5f)
				MoveTo(attackRange - .7f, walkSpeed);
			if (range == RANGE.FARRANGE && Time.time - boss1AtkTime > 1.5f)
			{
				int randAtk = Random.Range(0, 3);

				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill1();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill3();
							boss1AtkTime = Time.time + 1.2f;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
					}
				}
			}
			else if (range == RANGE.FARRANGE && Time.time - boss1AtkTime <= 1.5f)
				MoveTo(attackRange - .7f, walkSpeed);
			
		}
		//#####보스2#####
		//보스1과 코드 거의 비슷
		if (enemyTactic == ENEMYTACTIC.Boss2)
		{
			//보스2는 기본공격또한 체력 50%이상 이하일때 다른 공격을 실행

			if (range == RANGE.ATTACKRANGE && Time.time - boss1AtkTime > 2.2f)
			{
				int randAtk = Random.Range(0, 3);
				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							ATTACK();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
					}
				}else if(GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
                {
					switch (randAtk)
					{
						case 0:
						case 1:
							ATTACK2();
							boss1AtkTime = Time.time-0.1f;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill4();
							boss1AtkTime = Time.time+1.5f;
							bossNoHitTime = Time.time;

							break;
					}
				}
			}
			else if (range == RANGE.ATTACKRANGE && Time.time - boss1AtkTime <= 2.2f) IDLE();
			if (range == RANGE.CLOSERANGE && Time.time - boss1AtkTime > 2.2f)
			{
				int randAtk = Random.Range(0, 3);

				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill1();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill3();
							boss1AtkTime = Time.time + 2f;
							bossNoHitTime = Time.time;

							break;
						case 2:
							Skill4();
							boss1AtkTime = Time.time + 1.5f;
							bossNoHitTime = Time.time;

							break;
					}
				}



			}
			else if (range == RANGE.CLOSERANGE && Time.time - boss1AtkTime <= 2.2f)
				MoveTo(attackRange - .7f, walkSpeed);
			if (range == RANGE.MIDRANGE && Time.time - boss1AtkTime > 2.2f)
			{

				int randAtk = Random.Range(0, 3);
				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
						case 2:
							Skill1();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill4();
							boss1AtkTime = Time.time + 1.5f;
							bossNoHitTime = Time.time;

							break;
						case 2:
							Skill3();
							boss1AtkTime = Time.time + 2f;
							bossNoHitTime = Time.time;

							break;
					}
				}
			}
			else if (range == RANGE.MIDRANGE && Time.time - boss1AtkTime <= 2.2f)
				MoveTo(attackRange - .7f, walkSpeed);
			if (range == RANGE.FARRANGE && Time.time - boss1AtkTime > 2.2f)
			{
				int randAtk = Random.Range(0, 4);

				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						
						case 0:
						case 1:

							Skill2();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
						case 2:
						case 3:

							Skill1();
							boss1AtkTime = Time.time;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{

						case 0:
						case 1:
							Skill4();
							boss1AtkTime = Time.time + +1.5f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						case 3:
							Skill3();
							boss1AtkTime = Time.time+ 2f;
							bossNoHitTime = Time.time;

							break;
					}
				}
			}
			else if (range == RANGE.FARRANGE && Time.time - boss1AtkTime <= 2.2f)
				MoveTo(attackRange - .7f, walkSpeed);
		}
		//보스3
		//위에 보스코드와 거의비슷
		if (enemyTactic == ENEMYTACTIC.Boss3)
		{
			if (range == RANGE.ATTACKRANGE && Time.time - boss1AtkTime > 2f)//1.6깎음
			{
				int randAtk = Random.Range(0,4);
				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							ATTACK();
							boss1AtkTime = Time.time+0.7f;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill2();
							boss1AtkTime = Time.time+0.3f;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							ATTACK2();
							boss1AtkTime = Time.time+1.3f;
							bossNoHitTime = Time.time;
							break;
						case 2:
							Skill4();
							boss1AtkTime = Time.time+1.4f;
							bossNoHitTime = Time.time;
							break;
						case 3:
							Skill3();
							boss1AtkTime = Time.time+0.8f;
							bossNoHitTime = Time.time;
							break;
					}
				}
			}
			else if (range == RANGE.ATTACKRANGE && Time.time - boss1AtkTime <= 2) IDLE();
			if (range == RANGE.CLOSERANGE && Time.time - boss1AtkTime > 2)
			{
				int randAtk = Random.Range(0, 4);

				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill1();
							boss1AtkTime = Time.time+0.5f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						case 3:

							Skill2();
							boss1AtkTime = Time.time + 0.3f;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill3();
							boss1AtkTime = Time.time + 0.8f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						case 3:
							Skill4();
							boss1AtkTime = Time.time + 1.4f;
							bossNoHitTime = Time.time;

							break;
					}
				}



			}
			else if (range == RANGE.CLOSERANGE && Time.time - boss1AtkTime <= 2)
				MoveTo(attackRange - .7f, walkSpeed);
			if (range == RANGE.MIDRANGE && Time.time - boss1AtkTime > 2)
			{

				int randAtk = Random.Range(0,3);
				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill2();
							boss1AtkTime = Time.time + 0.3f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						
							Skill1();
							boss1AtkTime = Time.time + 0.5f;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{
						case 0:
						case 1:
							Skill4();
							boss1AtkTime = Time.time + 1.4f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						case 3:
							Skill3();
							boss1AtkTime = Time.time + 0.8f;
							bossNoHitTime = Time.time;

							break;
					}
				}
			}
			else if (range == RANGE.MIDRANGE && Time.time - boss1AtkTime <= 2)
				MoveTo(attackRange - .7f, walkSpeed);
			if (range == RANGE.FARRANGE && Time.time - boss1AtkTime > 2)
			{
				int randAtk = Random.Range(0, 3);

				if (GetComponent<HealthSystem>().CurrentHp > GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{

						case 0:
						case 1:

							Skill2();
							boss1AtkTime = Time.time + 0.3f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						

							Skill1();
								boss1AtkTime = Time.time+0.5f;
							bossNoHitTime = Time.time;

							break;
					}
				}
				else if (GetComponent<HealthSystem>().CurrentHp <= GetComponent<HealthSystem>().MaxHp / 2)
				{
					switch (randAtk)
					{

						case 0:
						case 1:
							Skill4();
							boss1AtkTime = Time.time + 1.4f;
							bossNoHitTime = Time.time;

							break;
						case 2:
						case 3:

							Skill3();
							boss1AtkTime = Time.time + 0.8f;
							bossNoHitTime = Time.time;

							break;
					}
				}
			}
			else if (range == RANGE.FARRANGE && Time.time - boss1AtkTime <= 2)
				MoveTo(attackRange - .7f, walkSpeed);
		}
	}

	
	//콤보 텍스트
	GameObject Combotxt;
	public void ComboTxt()
	{
		ComboText.combo++; //콤보숫자 증가
		Combotxt = GameObject.Find("ComboText");  //ComboText 이름의 게임오브젝트 찾아서 선언
		
		if (Combotxt.GetComponent<ComboText>().go == false) //go 가 false일때 실행
		 Combotxt.GetComponent<ComboText>().ComboStart();

	}

	//피격 함수
	public void Hit(DamageObject d){

		//이동 멈춤
		Move(Vector3.zero, 0);
		//플레이어를 바라봄
		LookAtTarget();

		//몬스터의 체력이 0이되고 isDead가 false 상태일때
		if (GetComponent<HealthSystem>() != null && GetComponent<HealthSystem>().CurrentHp == 0 && !isDead)
		{
			Move(Vector3.zero, 0);//이동 멈춤
			GlobalAudioPlayer.PlaySFX("PunchHit");//피격효과음 실행
			ShowHitEffectAtPosition(transform.position + Vector3.up * Random.Range(1.0f, 2.0f));//피격 이펙트 실행

			UnitIsDead();//죽는 함수
		}
		
		//타격이펙트 2개씩나와서 수정했음.
		if (HitStates.Contains(enemyState) && !isDead){
			ShowHitEffectAtPosition(transform.position + Vector3.up * Random.Range(1.0f, 2.0f));
			GlobalAudioPlayer.PlaySFX ("PunchHit");

			ComboTxt();
		}

		
		//enemy can be hit
		else if(!HitStates.Contains(enemyState) && !isDead){

			ComboTxt();
			ShowHitEffectAtPosition(transform.position + Vector3.up * Random.Range(1.0f, 2.0f));

			GlobalAudioPlayer.PlaySFX ("PunchHit");


			//공격타입이 녹다운이면
			if (d.attackType == AttackType.KnockDown){
				//보스 슈퍼아머시간 조건문을줘서 피격애니메이션 실행안되게 수정
				if ((enemyTactic == ENEMYTACTIC.Boss2 && Time.time - bossNoHitTime < 0.7f && Time.time - bossNoHitTime > 0.04f)
					|| (enemyTactic == ENEMYTACTIC.Boss && Time.time - bossNoHitTime < 0.5f && Time.time - bossNoHitTime > 0.1f)
					|| (enemyTactic == ENEMYTACTIC.Boss3 && Time.time - bossNoHitTime < 0.8f && Time.time - bossNoHitTime > 0.03f))
					return;
				else
				{
					//위에조건이 안맞을땐 녹다운
					enemyState = ENEMYSTATE.KNOCKDOWN;
					StartCoroutine(KnockDown(DirectionToPos(d.inflictor.transform.position.x)));
				}
			} else {

				//기본 피격애니도 똑같이 조건줘서 슈퍼아머
				if ((enemyTactic == ENEMYTACTIC.Boss2 && Time.time - bossNoHitTime < 0.7f && Time.time - bossNoHitTime > 0.04f) 
					|| (enemyTactic == ENEMYTACTIC.Boss && Time.time - bossNoHitTime < 0.5f && Time.time - bossNoHitTime > 0.1f)
					|| (enemyTactic == ENEMYTACTIC.Boss3 && Time.time - bossNoHitTime < 0.8f && Time.time - bossNoHitTime > 0.03f))
					return;
                else
                {
					animator.Hit();
					enemyState = ENEMYSTATE.HIT;
				}
					


			}
		}

	}

	//몬스터 죽는 함수
	void UnitIsDead(){
		isDead = true;
		enableAI = false;
		gameObject.GetComponent<BoxCollider2D>().enabled = false; //박스콜라이더를 꺼서 더이상 피격안되게
		Move(Vector3.zero, 0);
		enemyState = ENEMYSTATE.DEATH; //상태변경
		animator.Death(); //애니실행
		StartCoroutine(RemoveEnemy()); //몬스터 삭제 코루틴 실행
		EnemyManager.RemoveEnemyFromList(gameObject); //리스트에서 죽은 몬스터 제거
	}

	//현재 플레이어와의 간격을 받아와서 반환시켜줌
	private RANGE GetRangeToTarget(){
		XDistance = DistanceToTargetX();
		YDistance = DistanceToTargetY();

		//플레이어와의 거리가 공격가능거리보다 작으면 공격범위
		if(XDistance <= attackRange && YDistance <= .2f) return RANGE.ATTACKRANGE;

		//공격범위보다크고 가까운거리보다 작을때
		if(XDistance > attackRange && XDistance <= closeRangeDistance) return RANGE.CLOSERANGE;

		//가까운거리보다 크고 중간거리보다 작을때
		if(XDistance > closeRangeDistance && XDistance <= midRangeDistance) return RANGE.MIDRANGE;

		//먼거리보다 멀떄
		if(XDistance > farRangeDistance) return RANGE.FARRANGE;

		//다아닐때
		return RANGE.FARRANGE;
	}

	//몬스터 전술 변경
	public void SetEnemyTactic(ENEMYTACTIC tactic){
		enemyTactic = tactic;
	}

	//목표물이 거리내있는지 확인
	void Look4Target(){
		targetSpotted = DistanceToTargetX() < sightDistance;
	}
}

//몬스터 상태 열거형
public enum ENEMYSTATE {
	IDLE,
	ATTACK,
	WALK,
	RUN,
	HIT,
	KNOCKDOWN,
	KNOCKDOWNGROUNDED,
	DEATH,
}
//몬스터 전술 열거형
public enum ENEMYTACTIC {
	ENGAGE = 0,
	KEEPSHORTDISTANCE = 1,
	KEEPMEDIUMDISTANCE = 2,
	KEEPFARDISTANCE = 3,
	STANDSTILL = 4,
	Boss = 5,
	Boss2 = 6,
	Boss3 = 7


}
//거리 열거형
public enum RANGE {
	ATTACKRANGE,
	CLOSERANGE,
	MIDRANGE,
	FARRANGE,
}
