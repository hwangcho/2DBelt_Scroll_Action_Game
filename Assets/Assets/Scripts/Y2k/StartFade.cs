using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//보스방 관련 UI
public class StartFade : MonoBehaviour
{
    public UIFader UI_Fader;
    public Text BossText; //보스 스타트 텍스트
    
    public GameObject player;
    public GameObject boss;
    public Slider BossHp; //보스체력 슬라이더

    public string nextStage;//다음스테이지 저장

    void Start()
    {

      //시작시 화면 밝게
        FadeIn();
      

    }
    private void Update()
    {
        //보스방 입장시
        //보스체력 활성화하고
        //체력 0에서 1까지 차는것처럼 연출
        if (BossText.gameObject.activeSelf)
        {
            BossHp.gameObject.SetActive(true);
            BossHp.value += 0.35f*Time.deltaTime;
        }
        //보스가 죽고 
        //라운드 클리어 ui가 비활성화중이라면
        if (boss.gameObject == null&& !UI_Fader.transform.parent.GetChild(4).transform.GetChild(0).gameObject.activeSelf) //보스죽으면 리스타트 (나중에 수정하면 다음스테이지로 이동가능)
        {
            //마지막 보스라면 어두워지고 리스타트 가능
            if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                UI_Fader.transform.parent.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
                FadeOut();
             
            }
            //마지막 보스아니면 3초후 다음스테이지로 이동
            else
            {
                UI_Fader.transform.parent.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
                FadeOut();
                Invoke("NextStage", 3);
            }
              
        }
    }
    //화면 밝게해주고 텍스트 숨김
    void FadeIn()
    {
        UI_Fader.Fade(UIFader.FADE.FadeIn, 2f, 0f);
        Invoke("HideText",3);
    }
    //다음 스테이지로 이동
    void NextStage()
    {
        SceneManager.LoadScene(nextStage);
    }
    //보스텍스트 사라지면서
    //보스,플레이어 스크립트 활성화해서 움직이게함
    void HideText()
    {
        BossText.gameObject.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerCombat>().enabled = true;
        boss.GetComponent<EnemyAI>().enabled = true;
    }
    //어두워지게
    void FadeOut()
    {
        UI_Fader.Fade(UIFader.FADE.FadeOut, 1f, 0);
       
    }
   
}
