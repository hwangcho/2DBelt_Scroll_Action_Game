using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jangpung : MonoBehaviour
{
    public int dir = 1; //방향
    public float speed = 3; //속도
	public float range = 3; //공격 범위

	private float yHitDistance = 2; //공격가능 y축범위
	public int att = 2; //공격력

	

	void Start()
    {
		StartCoroutine(AttackDelay());
		Invoke("DestroyObject", PlayerCombat.delay*0.5f);//오브젝트 삭제 인보크로 바꿈
    }

    
    void Update()
    {
		//플레이어 방향에따라 좌우 반전해주고 이동방향 정함
        if(dir == 1)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        transform.Translate(Vector3.right *dir * speed * Time.deltaTime);

	

	}

	//공격 히트간격 코루틴
	IEnumerator AttackDelay()
    {
		//크기작을때 공격범위 좁게,히트간격 느리게
		if (gameObject.transform.localScale== new Vector3(0.5f, 0.5f, 1))
        {
			range = 0.2f;
			while (true)
			{
				CheckForHit();
				yield return new WaitForSeconds(0.12f); //0.12초마다 공격맞게
			}
        }
        else
        {//아닐땐 그대로
			while (true)
			{
				CheckForHit();
				yield return new WaitForSeconds(0.1f);
			}
		}
			
    }

	void DestroyObject()
    {
		Destroy(gameObject);
	}

	//공격맞았는지 체크
	public void CheckForHit()
	{


		Vector3 playerPos = transform.position+Vector3.down*0.5f;
		LayerMask enemyLayerMask = LayerMask.NameToLayer("Enemy");


		RaycastHit2D[] hits = Physics2D.RaycastAll(playerPos, Vector3.right * dir, range, 1 << enemyLayerMask );
		Debug.DrawRay(playerPos, Vector3.right * dir, Color.red, range);


		for (int i = 0; i < hits.Length; i++)
		{
				GameObject enemy = hits[i].collider.gameObject;
				if (ObjInYRange(enemy))
				{
					DealDamageToEnemy(hits[i].collider.gameObject);		
				}
		}


	}


	//y축 범위 체크
	bool ObjInYRange(GameObject obj)
	{
		float YDist = Mathf.Abs(obj.transform.position.y - transform.position.y);
		if (YDist < yHitDistance)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	//몬스터에게 대미지줌
	private void DealDamageToEnemy(GameObject enemy)
	{
		DamageObject d = new DamageObject(0, gameObject);

	
		d.damage = att;
		d.attackType = AttackType.KnockDown;
		d.inflictor = GameObject.FindGameObjectWithTag("Player");

		HealthSystem hs = enemy.GetComponent<HealthSystem>();
		if (hs != null)
		{
			hs.SubstractHealth(d.damage);
		}

		enemy.GetComponent<EnemyAI>().Hit(d);
	}




}
