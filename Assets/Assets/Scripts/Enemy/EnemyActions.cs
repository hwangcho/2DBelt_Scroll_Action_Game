using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyActions : Enemy {//Enemy를 상속받음

	public GameObject target; //몬스터의 타겟
	public RANGE range; //타겟과의 범위
	public ENEMYTACTIC enemyTactic;//현재 전술
	public ENEMYSTATE enemyState;//몬스터 상태
	public EnemyAnimator animator;
	public GameObject GFX;
	public SpriteRenderer Shadow;
	public Rigidbody2D rb;
	public bool isDead;
	public float YHitDistance = 0.4f; //공격가능 y축 범위
	private float MoveThreshold = 0.1f; //이동가능범위
	public float lastAttackTime; //마지막 공격 시간


	public float bossNoHitTime;//보스 슈퍼아머시간

	//공격1 함수
	public void ATTACK() {
		if(target == null) SetTarget2Player(); //타겟이 null 이라면 타겟을 플레이어로 설정

		if(Time.time - lastAttackTime > attackInterval){//마지막 공격시간이 attackInterval 보다 크면 공격실행

			enemyState = ENEMYSTATE.ATTACK;//몬스터 현재 상태 변경
			Move(Vector3.zero, 0); //이동 멈춤
			LookAtTarget();// 공격할때 플레이어 바라보게
			animator.Attack1();//공격 애니실행
			lastAttackTime = Time.time;//공격시간 초기화


		}
	}
	public void ATTACK2() 
	{
		if (target == null) SetTarget2Player();

		if (Time.time - lastAttackTime > attackInterval)
		{

			enemyState = ENEMYSTATE.ATTACK;
			Move(Vector3.zero, 0);
			LookAtTarget();
			animator.Attack2();
			lastAttackTime = Time.time;

		}
	}
	//############################보스1###############################
	public void Skill1()
	{
		if (target == null) SetTarget2Player();

		if (Time.time - lastAttackTime > attackInterval)
		{

			enemyState = ENEMYSTATE.ATTACK;
			Move(Vector3.zero, 0);
			LookAtTarget();
			animator.Skill1();
			lastAttackTime = Time.time;
		}
	}
	public void Skill2()
	{
		if (target == null) SetTarget2Player();

		if (Time.time - lastAttackTime > attackInterval)
		{

		
			enemyState = ENEMYSTATE.ATTACK;
			Move(Vector3.zero, 0);
			LookAtTarget();
			animator.Skill2();
			
		}
	}
	public void Skill3()
	{
		if (target == null) SetTarget2Player();

		if (Time.time - lastAttackTime > attackInterval)
		{

		
			enemyState = ENEMYSTATE.ATTACK;
			Move(Vector3.zero, 0);
			LookAtTarget();
			animator.Skill3();
			lastAttackTime = Time.time;
			
		}
	}
	public void Skill4()
	{
		if (target == null) SetTarget2Player();

		if (Time.time - lastAttackTime > attackInterval)
		{

			
			enemyState = ENEMYSTATE.ATTACK;
			Move(Vector3.zero, 0);
			LookAtTarget();
			animator.Skill4();
			lastAttackTime = Time.time;
			
		}
	}
	//보스1 번개스킬 생성
	public void skill1Thunder()
    {
		//플레이어 위치에 바로 생성해도되지만
		//플레이어가 왼쪽바라볼때 약간 어색해서 
		//플레이어 현재방향에따라 생성위치 약간 조정
		if(target.GetComponent<PlayerMovement>().currentDirection == DIRECTION.Right)
        {
			GameObject go = Instantiate(Resources.Load("BossThunder"), target.transform.position, Quaternion.identity) as GameObject;

		}else if(target.GetComponent<PlayerMovement>().currentDirection == DIRECTION.Left)
        {
			GameObject go = Instantiate(Resources.Load("BossThunder"), target.transform.position+Vector3.right*0.25f, Quaternion.identity) as GameObject;
		}

	}
	//순간이동 포지션 변경
	public void Skill2ChangePos()
    {
		//플레이어 현재방향 에따라 순간이동 위치 다르게
		if (target.GetComponent<PlayerMovement>().currentDirection == DIRECTION.Right)
			transform.position = target.transform.position+new Vector3(0.05f,0,0);
		else if(target.GetComponent<PlayerMovement>().currentDirection == DIRECTION.Left)
			transform.position = target.transform.position - new Vector3(0.05f, 0, 0);

	}
	//플레이어에게 이동
	public void MoveTo(float distance, float speed){
		LookAtTarget();//플레이어를 바라보게하는 함수

		//x축 이동
		//현재 플레이어 플레이어와의 x축 거리 - distance가 이동가능범위보다 크면
		if (Mathf.Abs(DistanceToTargetX()-distance) > MoveThreshold){

			//x축 이동
			Move(Vector3.right * (int)Dir2DistPoint(distance), speed);

			//y축이동
		} else if (Mathf.Abs(DistanceToTargetY()-distance)+0.1 > MoveThreshold){

			
			Move(Vector3.up * DirToVerticalLine(), speed/1.5f);
		} 
	}

	//보스2 플레이어 앞으로 이동스킬
	public IEnumerator Boss2Skill1()
    {
		
		float t = 0;
		bool right = transform.position.x < target.transform.position.x ? false : true; 
		
		Vector3 targetpos = target.transform.position; //플레이어 위치값 
		LookAtTarget();
		
		//몬스터가 플레이어 앞까지 달려가게함
		//플레이어가 몬스터보다 왼쪽에있으면 플레이어 오른쪽으로 이동
		//플레이어가 몬스터보다 오른쪽에있으면 플레이어 왼쪽으로 이동
		while (t < 1f)
        {
			if (!Pause.pauseon)
			{
				t += Time.deltaTime * 6f;
				if (right)
				{
					transform.position = Vector2.Lerp(transform.position, targetpos + new Vector3(1.5f, 0, 0), t);
				}
				else
				{
					transform.position = Vector2.Lerp(transform.position, targetpos - new Vector3(1.5f, 0, 0), t);
				}
			}
			yield return null;

        }
    }
	//보스2 플레이어 뒤로 순간이동 스킬
	public void Boss2Skill2() 
    {
		bool right = transform.position.x < target.transform.position.x ? false : true;//현재 플레이어가 몬스터보다 왼쪽에있는지 오른쪽에있는지 체크

		//플레이어 뒤로 순간이동!
		if (!right)
		{
			transform.position = target.transform.position + new Vector3(1.5f, 0, 0);
			LookAtTarget();

		}
		else
		{
			transform.position = target.transform.position - new Vector3(1.5f, 0, 0);
			LookAtTarget();

		}

	}
	//몬스터 공격시에 폭탄생성 함수
	public void Boss2AttackBall() 
	{
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1 : 1; //현재 몬스터의 flipX이 true인지 false인지 삼항연산자로 판단해 선언

		Vector3 playerPos = transform.position + new Vector3(dir * 1, 1.37f, 0);//생성 위치
		GameObject go = Instantiate(Resources.Load("Boss2Boom"), playerPos, Quaternion.identity) as GameObject; //폭탄 생성
		go.GetComponent<Boss2Ball>().dir = dir;//생성된 폭탄에 dir을 설정해줌

	}
	//보스들 슈퍼아머 시간
	public void BossHitTime() 
    {
		bossNoHitTime = Time.time;
    }

	//현재 방향값 반환함수
	DIRECTION Dir2DistPoint(float distance){
		if(target == null) SetTarget2Player();

		//몬스터가 플레이어보다 왼쪽에있으면
		if(transform.position.x < target.transform.position.x){
			float distancepointX = target.transform.position.x - distance;
			if(transform.position.x < distancepointX) 
				return DIRECTION.Right;
			else 
				return DIRECTION.Left;
		} else {

		
			float distancepointX = target.transform.position.x + distance;
			if(transform.position.x > distancepointX) 
					return DIRECTION.Left;
				else 
					return DIRECTION.Right;
		}
	}

	//Order in Layer 업데이트 함수
	public void UpdateSpriteSorting(){
		GFX.GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(transform.position.y*-10f); 
		Shadow.sortingOrder = GFX.GetComponent<SpriteRenderer>().sortingOrder-1;
	}

	//플레이어 바라보게하는 함수
	//플레이어와 몬스터의 x축 위치에따라 flipx 로 좌우반전
	public void LookAtTarget()	{
		if(target != null){
			if(transform.position.x >= target.transform.position.x){
				GFX.GetComponent<SpriteRenderer>().flipX = true;
			} else {
				GFX.GetComponent<SpriteRenderer>().flipX = false;
			}
		}
	}

	//현재방향 반환함수
	//flipX 로 좌우 체크
	public DIRECTION getCurrentDirection(){
		if(GFX.GetComponent<SpriteRenderer>().flipX){
			return DIRECTION.Left;
		} else 
			return DIRECTION.Right;
	}

	//방향값 반환 함수
	//몬스터 위치가 xpos 보다 크면 Left 아니면 right
	public DIRECTION DirectionToPos(float xPos){
		if (transform.position.x >= xPos) return DIRECTION.Left;
		return DIRECTION.Right;
	}

	//y축 이동방향 반환함수
	int DirToVerticalLine(){
		if(transform.position.y > target.transform.position.y) 
			return -1;
		else 
			return 1;
	}

	//idle 상태로 변경함수
	public void IDLE()	{
		Move(Vector3.zero, 0);
		enemyState = ENEMYSTATE.IDLE;
		animator.Idle();
	}

	//이동하는 함수
	public void Move(Vector3 vector, float speed){
		rb.velocity = vector * speed; //받아온값으로 rigidbody 속도에 넣어줌

		if(speed>0) 
			animator.Walk();
		else
			animator.Idle();
	}


	//플레이어와 몬스터 x축거리 반환
	public float DistanceToTargetX(){
		if(target != null)
			return Mathf.Abs(transform.position.x - target.transform.position.x);
		else
			return -1;
	}

	//플레이어와 몬스터 y축거리 반환
	public float DistanceToTargetY(){
		if(target != null)
			return Mathf.Abs(transform.position.y - target.transform.position.y);
		else
			return -1;
	}

	//플레이어가 맞앗는지 체크함수
	public void CheckForHit(){
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1:1; //현재 몬스터 바라보는방향 체크
		Vector3 playerPos = transform.position + Vector3.up * 1.5f; //레이 나오는 위치
		LayerMask enemyLayerMask = LayerMask.NameToLayer("PlayerCollider");//체크하는 레이어
		

		//몬스터의 공격범위만큼 레이를 발사해서 체크
		RaycastHit2D hits = Physics2D.Raycast(playerPos, Vector3.right * dir, attackRange, LayerMask.GetMask("PlayerCollider") );
		Debug.DrawRay(playerPos, Vector3.right *attackRange *dir, Color.red, attackRange);

		//레이에 플레이어가 닿으면 실행
		if(hits.collider != null)
        {
			
			DealDamageToTarget();
		}

	}
	//아래방향으로 레이 발사해 맞았는지 체크
	public void CheckForHitDown()
	{
		Vector3 playerPos = transform.position + new Vector3(0, 1.5f, 0);
		LayerMask enemyLayerMask = LayerMask.NameToLayer("PlayerCollider");


		RaycastHit2D hits = Physics2D.Raycast(playerPos, Vector3.down, 0.5f, LayerMask.GetMask("PlayerCollider"));
		Debug.DrawRay(playerPos, Vector3.down * 0.5f, Color.red, 2);


		//we have hit something
		if (hits.collider != null)
		{

			DealDamageToTarget();
		}
	}

	//플레이어에게 데미지를줌
	void DealDamageToTarget(){
		if(target != null){
			DamageObject d = new DamageObject(attackDamage, gameObject); // 몬스터의 공격력 넣어서 생성
			d.attackType = AttackType.Default; //어택타입 초기화
			target.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver); //타겟에 Hit 함수 호출
		}
	}

	//몬스터 녹다운 함수
	public IEnumerator KnockDown(DIRECTION dir){
		float t=0;
		float travelSpeed = 2f;

		//녹다운 애니실행
		animator.KnockDown();

		//방향값에따라 왼쪽,오른쪽으로 밀리게함
		while(t<1 && !isDead){
			rb.velocity = Vector2.left * (int)dir * travelSpeed;
			t += Time.deltaTime;
			yield return 0;
		}

		//와일문 빠져나오면 속도 초기화
		//상태 변경
		rb.velocity = Vector2.zero;
		enemyState = ENEMYSTATE.KNOCKDOWNGROUNDED;
	}

	//몬스터 삭제전 깜빡임 효과 코루틴
	public IEnumerator RemoveEnemy(){

		float osc;
		float speed = 3f;
		float startTime = Time.time;

		//1.5초후에 실행
		yield return new WaitForSeconds(1.5f);

		//몬스터 스프라이트를 비활성화해서 깜빡이다가 speed 가 35를넘으면 몬스터삭제
		while(true){
			osc = Mathf.Sin((Time.time - startTime) * speed);
			speed += Time.deltaTime * 10;

			//Switch between sprites
			GFX.GetComponent<SpriteRenderer>().enabled = (osc>0);

			if(speed > 35) DestroyUnit();
			yield return null;
		}
	}

	//피격이펙트 생성
	public void ShowHitEffectAtPosition(Vector3 pos) {
		GameObject.Instantiate (Resources.Load ("HitEffect"), pos, Quaternion.identity);
	}

	//타겟설정 함수
	public void SetTarget2Player(){
		target = GameObject.FindGameObjectWithTag("Player");
	}

	//기본몬스터 이동속도&&공격시간 랜덤 선언 함수
	public void RandomizeValues(){
        if (!isBoss&&!isShooter)//보스일때 이속,공속 정해진값 고정
        {
			walkSpeed *= Random.Range(.8f, 1.2f);
			attackInterval *= Random.Range(.8f, 1.2f);
		}
	
	}
	//enemy5 총알 함수
	public void Shoot1()
    {
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1 : 1; //몬스터 방향값

		Vector3 playerPos = transform.position + new Vector3(dir * 1, 1.37f, 0); //생성위치
		GameObject go = Instantiate(Resources.Load("Shot1"), playerPos, Quaternion.identity) as GameObject; //총알 생성
		go.GetComponent<Shooting>().dir = dir; //생성된 총알에 dir값 선언
		
	}
	//enemy6 총알
	public void Shoot2()
	{
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1 : 1;

		Vector3 playerPos = transform.position + new Vector3(dir * 1, 1.59f, 0);
		GameObject go = Instantiate(Resources.Load("Shot2"), playerPos, Quaternion.identity) as GameObject;
		go.GetComponent<Shooting>().dir = dir;

	}
	//enemy9 총알
	public void Shoot3()
	{
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1 : 1;

		Vector3 playerPos = transform.position + new Vector3(dir * 1, 1.5f, 0);
		GameObject go = Instantiate(Resources.Load("Arrow"), playerPos, Quaternion.identity) as GameObject;
		go.GetComponent<Shooting>().dir = dir;

	}
	public void Boss3Wind()
	{
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1 : 1;

		Vector3 playerPos = transform.position + new Vector3(dir * 1.5f, 0, 0);
		GameObject go = Instantiate(Resources.Load("Wind"), playerPos, Quaternion.identity) as GameObject;
		go.GetComponent<Shooting>().dir = dir;

	} 
	public void Boss3Wind2()
	{
		int dir = GFX.GetComponent<SpriteRenderer>().flipX == true ? -1 : 1;

		Vector3 playerPos = transform.position + new Vector3(dir * 1.5f, 0, 0);
		GameObject go = Instantiate(Resources.Load("Wind2"), playerPos, Quaternion.identity) as GameObject;
		go.GetComponent<Boss3Wind>().dir = dir;

	}
}