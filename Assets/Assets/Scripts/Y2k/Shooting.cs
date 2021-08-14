using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public int dir = 1;//방향
    public float speed = 3;//속도

    public int att = 2;//공격력
    public bool isWind;//태풍인지 확인


    void Start()
    {
        Invoke("destroyObj", 3);//3초후 삭제
    }

    void destroyObj()
    {
        //바람일때랑 아닐때 삭제 다르게
        if(!isWind)
            Destroy(gameObject);
        else
        {
            Destroy(gameObject);
            GameObject a = GameObject.Instantiate(Resources.Load("WindFin"), transform.position, Quaternion.identity) as GameObject;
            Destroy(a, 0.3f);
        }
    }
   
    void Update()
    {
        //플레이어 방향에따라 이동방향 다르게
        if (dir == 1)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        transform.Translate(Vector3.right * dir * speed * Time.deltaTime);

    }
    //플레이어에게 맞췃을때 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 13&&!isWind)
        {
           
            DamageObject d = new DamageObject(att, gameObject);
            d.attackType = AttackType.Default;
            collision.gameObject.transform.parent.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
            

            Destroy(gameObject);
        }else if(collision.gameObject.layer == 13 && isWind)
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
