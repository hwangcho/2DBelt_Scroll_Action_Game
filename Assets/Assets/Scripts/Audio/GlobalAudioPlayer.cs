using UnityEngine;
using System.Collections;

//정적으로 설정해서 어디서든 사용할수있음게함.
public static class GlobalAudioPlayer {

	public static AudioPlayer audioPlayer; 
	
	public static void PlaySFX(string sfxName){
		if(audioPlayer != null && sfxName != "") audioPlayer.playSFX(sfxName);
	}

	public static void PlayMusic(string musicName){
		if(audioPlayer != null) audioPlayer.playMusic(musicName);
	}
}
