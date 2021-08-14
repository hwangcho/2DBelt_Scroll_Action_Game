using UnityEngine;
using System.Collections;
using System;

//게임 시작할때 세팅
public class GameSettings : ScriptableObject {

	[Header ("Application Settings")]
	public int framerate = 60;
	public float timeScale = 1f;

	[Header ("Audio Settings")]
	public float MusicVolume = .7f;
	public float SFXVolume = .9f;

	[Header ("Game Settings")]
	public  bool CameraBacktrack = false;
	public int MaxAttackers  = 3; 
}