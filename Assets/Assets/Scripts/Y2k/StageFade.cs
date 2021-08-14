using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageFade : MonoBehaviour
{
    public MainMenu mainMenu;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("startgame", 1.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void startgame()
    {
        mainMenu.startGame(); //메인메뉴의 함수 호출
        //플레이어 스크립트 활성화해서 움직이게함
        player.GetComponent<PlayerCombat>().enabled = true;
        player.GetComponent<PlayerMovement>().enabled = true;
    }
}

