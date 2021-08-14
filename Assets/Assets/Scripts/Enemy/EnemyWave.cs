using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//몬스터 웨이브 스크립트 

[System.Serializable]
public class EnemyWave
{
	public string WaveName = "Wave"; //웨이브 이름
	public Transform PositionMarker; //웨이브 끝나면 카메라 이동할수있는 위치
	public List<GameObject> EnemyList = new List<GameObject> (); //몬스터 리스트

	//웨이브 클리어 함수
	public bool waveComplete()
	{
		if (EnemyList.Count == 0) {//몬스터 리스트에 아무것도없을때 실행
			return true;
		} else {
			return false;
		}
	}

	//몬스터가 죽엇을때 리스트에서 제거하는 함수
	public void RemoveEnemyFromWave(GameObject g)
	{
		if (EnemyList.Contains (g)) //포함되있다면 제거
			EnemyList.Remove (g);
	}

}