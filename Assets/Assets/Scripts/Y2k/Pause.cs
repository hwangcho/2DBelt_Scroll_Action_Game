using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 멈춤
public class Pause : MonoBehaviour
{
     public static bool pauseon; //멈췃는지 불값 확인
     AudioSource audio; //음악
     AudioSource audio2;// 효과음

    // Start is called before the first frame update
    void Start()
    {
        pauseon = false; 
        audio = GameObject.FindGameObjectWithTag("Audio1").GetComponent<AudioSource>();
        audio2 = GameObject.FindGameObjectWithTag("Audio2").GetComponent<AudioSource>();

    }

  
    void Update()
    {
        //esc키누르면 pauseOn 반전시킴
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseon = !pauseon;
            
        }
        if (!pauseon&& !audio.isPlaying) //pauer 풀엇을때
        {
            Time.timeScale = 1.3f;//멈춰있던거 품
            transform.GetChild(0).gameObject.SetActive(false); //화면 어두워졋던거 비활성화
            audio.UnPause(); //멈췃던 효과음,음악 다시나오게
            audio2.UnPause();

        }
        else if(pauseon && audio.isPlaying)
        {
            Time.timeScale = 0;//게임멈춤
            transform.GetChild(0).gameObject.SetActive(true);//화면 어둡게
            audio.Pause(); //나오던 음악,효과음 멈춤
            audio2.Pause();

        }
    }
}
