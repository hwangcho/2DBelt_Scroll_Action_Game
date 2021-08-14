using UnityEngine;

[System.Serializable]//유니티 인스펙터창에 보여주게함
public class AudioItem {

	public string name; //효과음 이름
	public float volume = 1f;//효과음 볼륨
	public AudioClip[] clip;//효과음

}
