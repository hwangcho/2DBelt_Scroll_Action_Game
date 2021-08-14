using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(PlayerState))]
public class PlayerCombat : MonoBehaviour {

	[Header("Attack Data")]
	public DamageObject[] PunchAttackData; //DamageObject 클래스를 List로 변수선언


	public Item currentWeapon; //현재들고있는 무기

	public GameObject itemInRange; //범위내에 있는 아이템
	public Transform weaponBone; //무기 들었을때 위치
	public delegate void PlayerEventHandler();
	public static event PlayerEventHandler OnPlayerDeath;

	[SerializeField] private int attackNum = 1; //현재 공격 번호
	private bool continuePunchCombo; //공격중에 펀치키누르면 다음행동 펀치나오게하는 불값
	private PlayerAnimator animator;
	private PlayerState playerState; 
	private float LastAttackTime = 0; //마지막으로 했던 공격 시간
	private bool targetHit; //몬스터 공격맞았는지 체크
	private bool ChangeDirDuringCombo = false;
	private bool ChangeDirAtLastHit = true; 
	private bool BlockAttacksFromBehind = true;
	private int HitKnockDownThreshold = 3; //몇대맞아야 녹다운 되는지
	private int HitKnockDownCount = 0; //현재 맞은 횟수
	private int HitKnockDownResetTime = 2; //시간내에 안맞으면 맞은횟수 초기화
	private float LastHitTime = 0; //마지막에 맞은 시간
	private List<PLAYERSTATE> AttackStates = new List<PLAYERSTATE> { PLAYERSTATE.IDLE, PLAYERSTATE.MOVING, PLAYERSTATE.JUMPING, PLAYERSTATE.PUNCH, PLAYERSTATE.KICK, PLAYERSTATE.DEFENDING }; //공격가능 리스트
	private List<PLAYERSTATE> HitStates = new List<PLAYERSTATE> { PLAYERSTATE.HIT, PLAYERSTATE.DEATH, PLAYERSTATE.KNOCKDOWN }; //맞았을때 리스트
	private float yHitDistance = 0.4f;  //플레이어가 적 공격할수있는 y값범위
	private bool isDead = false; //사망
	private bool defend = false;// 방어

	public static float delay = 0;//장풍 딜레이값
	//활성화될때 델리게이트의 이벤트에 추가함
	private void OnEnable() {
		InputManager.onCombatInputEvent += CombatInputEvent;
	}
	//비활성화때 제거
	private void OnDisable() {
		InputManager.onCombatInputEvent -= CombatInputEvent;
	}

	private void Awake() {
		animator = GetComponentInChildren<PlayerAnimator> ();
		playerState = GetComponent<PlayerState> ();

		if (animator == null) { 
			Debug.Log ("No player animator found assigned in gameobject " + gameObject.name);
		}
	}
	//float count = 0;

	
	private void Update(){
		if (!Pause.pauseon)
		{
			//방어상태일때와 아닐때 실행
			if (defend)
			{
				defend = false;
				Defend();
			}
			else
			{
				if (playerState.currentState == PLAYERSTATE.DEFENDING)//방어가 끝낫는데 플레이어상태가 방어상태라면
				{
					GetComponent<PlayerMovement>().Idle();
					animator.StopDefend();
				}
			}
		}
	}
	//장풍생성 코루틴
	IEnumerator instantJangpung(float jangpungCount)
	{
		yield return new WaitForSeconds(0.4f);
		if (jangpungCount > 2f && jangpungCount < 5)
		{//딜레이 적게하면 장풍 작게
			int dir = (int)GetComponent<PlayerMovement>().getCurrentDirection();//플레이어 방향값
			Vector3 playerPos = transform.position + new Vector3(dir * 1, 1.5f, 0); //플레이어보다 조금 앞에 생성되게
			GameObject go = Instantiate(Resources.Load("jangpung"), playerPos, Quaternion.identity) as GameObject; //장풍생성
			go.transform.localScale = new Vector3(0.5f, 0.5f, 1);//장풍 스케일
			go.GetComponent<jangpung>().dir = dir;//장풍 dir에 플레이어 방향값을 넣어줌
		}
		else if (jangpungCount >= 5)
		{//기모으기 최대시 장풍크기 최대
			int dir = (int)GetComponent<PlayerMovement>().getCurrentDirection();
			Vector3 playerPos = transform.position + new Vector3(dir * 1, 1.5f, 0);
			GameObject go = Instantiate(Resources.Load("jangpung"), playerPos, Quaternion.identity) as GameObject;

			go.GetComponent<jangpung>().dir = dir;
		}


	}
	//공격키 관련함수
	private void CombatInputEvent(string action) {
		if (AttackStates.Contains (playerState.currentState) && !isDead) {

			//Punch키일때
			if (action == "Punch" && playerState.currentState != PLAYERSTATE.KICK) {//기모으다 펀치하면 모으던기 다 날아감
				GetComponent<HealthSystem>().jangpungCount = 0; //기모으던 장풍값 초기화
				GetComponent<HealthSystem>().SendJangpungCount();//장풍 UI 업데이트

				
				if (playerState.currentState == PLAYERSTATE.JUMPING) {//대쉬중공격못하게 함
					return;

				} else {

					//범위내에 아이템이있으면 실행
					if (itemInRange != null && ObjInYRange (itemInRange)) {
						InteractWithItem();

					} else if (playerState.currentState != PLAYERSTATE.PUNCH&& playerState.currentState != PLAYERSTATE.DEFENDING) {//가드중 펀치못하게 수정

						//현재 착용무기가 Knife라면
						if (currentWeapon != null && currentWeapon.itemName == "Knife") {
							StartThrowAttack();
						} else {

							//펀치
							doPunchAttack();
						}

					} else {

						//현재 공격이 마지막공격이 아니라면
						if (attackNum < PunchAttackData.Length - 1) {
							continuePunchCombo = true;//true로 주어 다음공격 바로 실행
						}
					}
				}
			}

			//Kick 키누르고 있으면
			if (action == "Kick" && playerState.currentState != PLAYERSTATE.PUNCH)
			{

				if (playerState.currentState != PLAYERSTATE.PUNCH && playerState.currentState != PLAYERSTATE.KICK && playerState.currentState != PLAYERSTATE.DEFENDING)
				{
					GetComponent<HealthSystem>().jangpungCount += Time.deltaTime; //장풍카운터이 Time.deltaTime 더해줌
					if (GetComponent<HealthSystem>().jangpungCount >= 5) //5가 넘으면 5로 고정
						GetComponent<HealthSystem>().jangpungCount = 5;
					delay = GetComponent<HealthSystem>().jangpungCount; //delay에 janpungCount 넣어줌
					GetComponent<HealthSystem>().SendJangpungCount();//장풍 슬라이더 업데이트 해줌


				}


			}
			//Kick키 버든 때면
			if (action == "KickUp" && playerState.currentState != PLAYERSTATE.PUNCH && playerState.currentState != PLAYERSTATE.KICK && playerState.currentState != PLAYERSTATE.DEFENDING)
			{
				//장풍카운트가 2보다 클때
				if (GetComponent<HealthSystem>().jangpungCount > 2)
				{
					doKickAttack();
					LastAttackTime = Time.time - 3;//펀치 콤보 0으로 다시 초기화해줌
					StartCoroutine(instantJangpung(GetComponent<HealthSystem>().jangpungCount));//코루틴 실행
					GetComponent<HealthSystem>().jangpungCount = 0; //값 초기화
					GetComponent<HealthSystem>().SendJangpungCount(); //슬라이더 업데이트
				}
				else
				{//기 2초이상 못모으면 초기화
					GetComponent<HealthSystem>().jangpungCount = 0;
					GetComponent<HealthSystem>().SendJangpungCount();
				}

			}
			//방어키 눌럿을때
			if (action == "Defend"&&playerState.currentState != PLAYERSTATE.KICK
				&& playerState.currentState != PLAYERSTATE.PUNCH && playerState.currentState != PLAYERSTATE.THROWKNIFE&& playerState.currentState != PLAYERSTATE.JUMPING)
			{
				defend = true;
			}
		}
	}

	//펀치공격 함수
	private void doPunchAttack() {
		playerState.SetState (PLAYERSTATE.PUNCH);//상태변경
		animator.Punch (GetNextAttackNum ());
		LastAttackTime = Time.time;//시간초기화
	}

	//Kick공격 함수 (장풍)
	void doKickAttack() {
		playerState.SetState (PLAYERSTATE.KICK);
		animator.Kick (GetNextAttackNum ());
		LastAttackTime = Time.time;
	}



	//방어 함수
	private void Defend(){
		playerState.SetState (PLAYERSTATE.DEFENDING);
		animator.StartDefend();
	}


	//공격 번호 반환함수
	private int GetNextAttackNum() {
		if (playerState.currentState == PLAYERSTATE.PUNCH) {//펀치일때
			attackNum = Mathf.Clamp (attackNum += 1, 0, PunchAttackData.Length);//공격번호에 1더해줌,최소값 0, 최대값 공격개수
			if(attackNum != 3)//공격번호가 3이아니라면
            {
				if (Time.time - LastAttackTime > PunchAttackData[attackNum].comboResetTime )//마지막 공격시간이 콤보리셋시간보다 커지면
					attackNum = 0;//첫번째 공격실행
				return attackNum;//아니면 콤보 이어짐
            }
            else//attackNum이 3이면 무조건 0으로 첫번째 공격으로 초기화
            {
				attackNum = 0;
				return attackNum;
			}
			
			

		} else if (playerState.currentState == PLAYERSTATE.KICK) {//킥(장풍) 상태면 무조건 1반환
				attackNum = 1;
				return attackNum;    
		}
		return 0;
	}

	//새로운 상태로 업데이트 함수
	public void Ready() {

		//continuePunchCombo true 이면 
		if (continuePunchCombo) {
			doPunchAttack ();//바로 공격실행
			continuePunchCombo = false; //콤보 초기화


			//콤보중 몬스터가 맞지않으면 방향 변경가능
			if (ChangeDirDuringCombo || !targetHit) {
				GetComponent<PlayerMovement> ().updateDirection ();
			}

			//마지막 공격은 무조건 방향 변경가능
			if (playerState.currentState == PLAYERSTATE.PUNCH && ChangeDirAtLastHit && attackNum == PunchAttackData.Length - 1) { 
				GetComponent<PlayerMovement> ().updateDirection ();
			} 

		} else {

			//idle 상태로 변경
			playerState.SetState (PLAYERSTATE.IDLE);
		}

	}

	//맞았는지 체크 함수
	public void CheckForHit() {

		int dir = (int)GetComponent<PlayerMovement> ().getCurrentDirection (); //플레이어 방향값
		Vector3 playerPos = transform.position + Vector3.up * 1.5f; //생성 위치값
		LayerMask enemyLayerMask = LayerMask.NameToLayer ("Enemy"); //레이어
		LayerMask itemLayerMask = LayerMask.NameToLayer ("Item");//레이어

		//레이를 쏴서 체크된 모든것을 리스트에 담음   
		//레이어는 Enemy && Item 만 체크
		RaycastHit2D[] hits = Physics2D.RaycastAll (playerPos, Vector3.right * transform.GetChild(1).localScale.x, getAttackRange(), 1 << enemyLayerMask | 1 << itemLayerMask);
		Debug.DrawRay (playerPos, Vector3.right * dir* getAttackRange(), Color.red,1);//레이를 그려서 체크

		//hits 에 들어온 모든것을 확인
		for (int i = 0; i < hits.Length; i++) {

			LayerMask layermask = hits [i].collider.gameObject.layer; 

			//레이어 마스크가 Enemy 이고 녹다운 상태가 아니면
			if (layermask == enemyLayerMask &&hits[i].collider.GetComponent<EnemyAI>().enemyState !=ENEMYSTATE.KNOCKDOWNGROUNDED)//이부분 수정해서 녹다운때 안맞게함
			{
				GameObject enemy = hits [i].collider.gameObject;
				if (ObjInYRange (enemy)) {
					DealDamageToEnemy (hits [i].collider.gameObject); //함수에 매개변수로 몬스터를 넎어줌
					targetHit = true;
					
				}
			}

			//아이템이라면
			if (layermask == itemLayerMask) {
				GameObject item = hits [i].collider.gameObject;
				if (ObjInYRange (item)) {
					item.GetComponent<ItemInteractable> ().ActivateItem (gameObject);
					ShowHitEffectAtPosition (hits [i].point);
				}
			}
		}

		
		if(hits.Length == 0){ 
			targetHit = false;
		}
	}

	//obj의 위치 y값과 플레이어 y값의 차이 체크해서 불값 반환함수
	bool ObjInYRange(GameObject obj) {
		float YDist = Mathf.Abs (obj.transform.position.y - transform.position.y);
		if (YDist < yHitDistance) {
			return true;
		} else {
			return false;
		}
	}

	//이펙트 호출 함수
	private void ShowHitEffectAtPosition(Vector3 pos) {
		GameObject.Instantiate (Resources.Load ("HitEffect"), pos, Quaternion.identity);
	}

	//몬스터에게 데미지 주는 함수
	private void DealDamageToEnemy(GameObject enemy) {
		DamageObject d = new DamageObject (0, gameObject); //DamageObject d 생성

		if (playerState.currentState == PLAYERSTATE.PUNCH) {
			d = PunchAttackData [attackNum]; //데미지 선언
		} else if (playerState.currentState == PLAYERSTATE.THROWKNIFE) {
			d.damage = currentWeapon.data;
			d.attackType = AttackType.KnockDown;//공격맞으면 녹다운 상태되게
		} 

		d.inflictor = gameObject;

		//받아온 몬스터의 HealthSystem 컴포넌트
		HealthSystem hs = enemy.GetComponent<HealthSystem>();
		if(hs != null&&hs.CurrentHp >0){ //체력0 이상이면 체력 깎이게 함수실행
			hs.SubstractHealth (d.damage);
		}

		enemy.GetComponent<EnemyAI>().Hit (d);
	}

	//플레이어랑 몬스터가 바라보고있는지 확인
	public bool isFacingTarget(GameObject g) {
		DIRECTION dir = GetComponent<PlayerMovement> ().getCurrentDirection ();//현재 플레이어 방향값
		//플레이어 바라보는 방향과 몬스터와 플레이어의 x값을 체크함
		if ((g.transform.position.x > transform.position.x && dir == DIRECTION.Right) || (g.transform.position.x < transform.position.x && dir == DIRECTION.Left))
			return true;
		else
			return false;
	}

	//공격범위값 반환함수
	private float getAttackRange() {
		if (playerState.currentState == PLAYERSTATE.PUNCH && attackNum <= PunchAttackData.Length) {
			return PunchAttackData [attackNum].range;//공격 데이터안의 range 반환
		} else {
			return 0f;
		}
	}

	//무기던지는 함수
	void StartThrowAttack() {
		playerState.SetState (PLAYERSTATE.THROWKNIFE);//상태변경
		animator.Throw ();//Throw 애니 함수 실행
		Invoke ("ThrowKnife", .08f); //0.08초후 함수실행
		Destroy(weaponBone.GetChild(0).gameObject); //무기 게임오브젝트 삭제
	}

	//투척칼 생성함수
	public void ThrowKnife() {
		GameObject knife = GameObject.Instantiate (Resources.Load ("ThrowingKnife")) as GameObject; //칼생성
		int dir = (int)GetComponent<PlayerMovement> ().getCurrentDirection (); //플레이어 방향값 
		knife.transform.position = transform.position + Vector3.up * 1.5f + Vector3.right * dir * .7f; //나이프 생성위치 선언
		knife.GetComponent<ThrowingKnife> ().ThrowKnife (dir); //나이프 방향 선언
		resetWeapon (); 
	}

	//피격 함수
	private void Hit(DamageObject d) {
		if (!HitStates.Contains (playerState.currentState)&&!isDead) {//죽고 안맞게 !isDead추가
		
			bool wasHit = true;
			UpdateHitCounter();

			//방어 상태일때
			if(playerState.currentState == PLAYERSTATE.DEFENDING){
				if(!BlockAttacksFromBehind || isFacingTarget (d.inflictor)) wasHit = false;//여기 바꿔서 뒤로가드시 공격맞게함
				if(!wasHit){
					GlobalAudioPlayer.PlaySFX ("Defend");//가드 효과음 실행
					animator.ShowDefendEffect();//이펙트 생성
					animator.CamShakeSmall();//카메라 진동
					HitKnockDownCount = 1;//녹다운 카운터 초기화

					//가드시 몬스터 위치에따라 밀리는 방향 다르게
					if (isFacingTarget(d.inflictor)){ 
						animator.AddForce(-1.5f);
					} else {
						animator.AddForce(1.5f);
					}
				}
			}

			//녹다운 횟수 다체우면 녹다운
			if (HitKnockDownCount >= HitKnockDownThreshold) { 
				d.attackType = AttackType.KnockDown; 
			}

			//피격시 효과음 실행
			if(wasHit) GlobalAudioPlayer.PlaySFX ("PunchHit");

			
			//여기바꿔서 가드 뒤로했을때 타격나오게랑 공격 초기화
			//공격에 맞으면
			if(wasHit && playerState.currentState != PLAYERSTATE.KNOCKDOWN) {
				GetComponent<HealthSystem> ().SubstractHealth (d.damage);//체력 깎음
				animator.ShowHitEffect ();//이펙트 생성
				//we are dead
				if (GetComponent<HealthSystem>() != null && GetComponent<HealthSystem>().CurrentHp <= 0) //체력이 0이하면 죽음
				{
					gameObject.SendMessage("Death");
				}
				//
				if (playerState.currentState == PLAYERSTATE.DEFENDING) //가드시에 가드 초기화
                {
					defend = false;
					GetComponent<PlayerMovement>().Idle();
					animator.StopDefend();
				}	
				//어택타입이 녹다운이라면 플레이어 녹다운
                if (d.attackType == AttackType.KnockDown) {
					playerState.SetState (PLAYERSTATE.KNOCKDOWN);//상태변경
					HitKnockDownCount = 0;//코루틴 이상해서 여기에 추가
					StartCoroutine (KnockDown (d.inflictor));//녹다운 코루틴 실행

				} else {
					//공격타입이 녹다운이 아니면 힛상태로
					playerState.SetState (PLAYERSTATE.HIT);
					animator.Hit ();
				}
				attackNum = 3;//공격맞으면 공격콤보 초기화


			}
		}
	}

	//녹다운 카운터 업데이트
	void UpdateHitCounter() {
		if (Time.time - LastHitTime < HitKnockDownResetTime) { //리셋시간보다 작으면 1씩증가
			HitKnockDownCount += 1;
		} else {
			HitKnockDownCount = 1;
		}
		LastHitTime = Time.time;
	}

	//무기 착용
	public void EquipWeapon(Item weapon) {
		currentWeapon = weapon;//현재 착용 무기 설정

		if(weapon.itemName == "Knife"){ //칼이라면
			GameObject knife = GameObject.Instantiate(Resources.Load("KnifeHandWeapon"), weaponBone.position, Quaternion.identity) as GameObject; //착용 칼 생성
			knife.transform.parent = weaponBone;//부모설정
			knife.transform.localPosition = Vector3.zero;//위치 설정
			knife.transform.localRotation = Quaternion.identity;//회전 설정
			knife.transform.localScale = transform.localScale;//크기 설정
		}
	}

	//무기 초기화
	public void resetWeapon() {
		currentWeapon = null;
	}

	//아이템 획득함수
	public void InteractWithItem(){
		if (itemInRange != null){
			Item item = itemInRange.GetComponent<ItemInteractable>().item;
			if (item != null && item.isPickup){
				itemInRange.GetComponent<ItemInteractable> ().ActivateItem (gameObject);
				animator.PickUpItem();
				playerState.SetState (PLAYERSTATE.PICKUPITEM);
			}
		}
	}

	//녹다운 코루틴
	public IEnumerator KnockDown(GameObject inflictor) {
		animator.KnockDown (); //녹다운 애니실행
		float t = 0;
		float travelSpeed = 2f;
		Rigidbody2D rb = GetComponent<Rigidbody2D> ();

		//get the direction of the attack
		int dir = inflictor.transform.position.x > transform.position.x ? 1 : -1;

		//look towards the direction of the incoming attack
		GetComponent<PlayerMovement>().LookToDir ((DIRECTION)dir);

		//몬스터와 플레이어 위치값 받아와서 녹다운시 밀리는 방향 설정
		while (t < 1) {
			rb.velocity = Vector2.left * dir * travelSpeed;
			t += Time.deltaTime;
			yield return 0;
		}

		//속도 초기화
		rb.velocity = Vector2.zero;
		yield return new WaitForSeconds (0.2f);

		//리셋
		playerState.currentState = PLAYERSTATE.IDLE;
		animator.Idle ();

	}

	//플레이어 사망함수
	void Death(){
		isDead = true;
		animator.Death();
		Invoke("GameOver", 2f); //2초후 함수 실행
		EnemyManager.PlayerHasDied();
	}

	//gameOver
	void GameOver(){
		OnPlayerDeath();
	}
}
