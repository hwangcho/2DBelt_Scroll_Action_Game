using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Wind : MonoBehaviour
{
    public int dir = 1; //방향값
    public float speed = 3;//이동속도
    public int att = 2;//공격력

    GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Invoke("destroyObj", 3);//시작후 3초뒤 삭제함수 호출
    }
    //삭제 함수
    void destroyObj()
    {
        
        Destroy(gameObject); 
        GameObject a = GameObject.Instantiate(Resources.Load("WindFin"), transform.position, Quaternion.identity) as GameObject; //바람공격 사라지는 모션 생성
        Destroy(a, 0.3f);
        
    }

    void Update()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized; //플레이어 위치를 따라가기위해 계속해서 업데이트

        transform.Translate(new Vector3(dir.x*3,dir.y*1.5f,dir.z) * Time.deltaTime); //플레이어를 따라감
    }
    //공격맞앗을때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 13)
        {
            DamageObject d = new DamageObject(att, gameObject);
            d.attackType = AttackType.Default;
            collision.gameObject.transform.parent.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);

            GameObject a = GameObject.Instantiate(Resources.Load("WindFin"), transform.position, Quaternion.identity) as GameObject;
            Destroy(a, 0.3f);

            Destroy(gameObject);
        }
    }
}
