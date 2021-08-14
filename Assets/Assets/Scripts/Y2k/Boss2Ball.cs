using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Ball : MonoBehaviour
{
    public int dir = 1; //방향값
    public int att = 2;//공격력


    Vector3 finPos; //몬스터 위치저장
    Vector3 vel = Vector3.zero;

    void Awake()
    {

        finPos = transform.position;
    }


    void Update()
    {
        //몬스터 좌우 방향에따라 이동방향 다르게하고
        //폭탄을 부드럽게 이동시킴 
        if(dir == 1)
        transform.position = Vector3.SmoothDamp(transform.position, finPos + Vector3.right*1.2f , ref vel, 0.5f);
        else
            transform.position = Vector3.SmoothDamp(transform.position, finPos + Vector3.right * -1.2f, ref vel, 0.5f);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어에게 닿으면 플레이어에게 데미지를줌
        if (collision.gameObject.layer == 13)
        {

            DamageObject d = new DamageObject(att, gameObject);
            d.attackType = AttackType.Default;
            collision.gameObject.transform.parent.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
            GetComponent<CircleCollider2D>().enabled = false;



        }
    }
    //공격 콜라이더 활성화
    public void ColliderOn()
    {
        GetComponent<CircleCollider2D>().enabled = true;
    }
    //공격 콜라이더 비활성화
    public void ColliderOff()
    {
        GetComponent<CircleCollider2D>().enabled = false;
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    //효과음 실행
    public void PlaySFX(string name)
    {
        GlobalAudioPlayer.PlaySFX(name);
    }
}
