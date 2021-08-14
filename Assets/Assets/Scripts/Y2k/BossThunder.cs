using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//삭제,맞았는지 체크, 효과음 다 애니메이션 이벤트에 넣어줌
public class BossThunder : MonoBehaviour
{
    GameObject target;
    public int damage;//공격력

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
       
    }


    public void DestroyObject()
    {
        Destroy(gameObject);
    }
    //플레이어가 맞앗는지 레이를 활용해서 체크
    public void CheckForHit()
    {
   
        Vector3 playerPos = transform.position + new Vector3(-0.1f,2f,0); //레이 생성위치
        LayerMask enemyLayerMask = LayerMask.NameToLayer("PlayerCollider"); //찾아낼 레이어

        //번개이기때문에 밑으로 발사
        RaycastHit2D hits = Physics2D.Raycast(playerPos, Vector3.down, 1f, LayerMask.GetMask("PlayerCollider"));
        Debug.DrawRay(playerPos, Vector3.down*1, Color.red, 2);

        //we have hit something
        if (hits.collider != null)
        {

            DealDamageToTarget();
        }
    }
    //플레이어에게 데미지를줌
    void DealDamageToTarget()
    {
        if (target != null)
        {
            DamageObject d = new DamageObject(damage, gameObject);
            d.attackType = AttackType.Default;
            target.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
        }
    }
    public void PlaySFX(string sfxName)
    {
        GlobalAudioPlayer.PlaySFX(sfxName);
    }
}
