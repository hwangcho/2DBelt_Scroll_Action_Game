using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
//닷트윈사용 콤보 텍스트
public class ComboText : MonoBehaviour
{
    public static int combo = 0;//현재 콤보숫자
    public bool go = false; //활성화중인지
    Text tmp_text; 
    public Transform pos;//콤보텍스트 위치선정
    public void ComboStart()
    {
        go = true;
        
        //위치로 이동시키고 완료되면
        transform.DOMove(pos.position, 1).OnComplete(() =>
        {
            //서서히  위로 이동 완료되면 초기화
            transform.DOMove(transform.position + Vector3.up * 50, 4).OnComplete(() =>
            {
                combo = 0;
                go = false;
                GetComponent<RectTransform>().anchoredPosition = new Vector3(300,0,0);
            });

        });
    }

    void Start()
    {
        go = false;
        tmp_text = GetComponent<Text>();
    }

    private void Update()
    {
        tmp_text.text = "Combo\n+" + combo; //콤보숫자 업데이트
    }


}
