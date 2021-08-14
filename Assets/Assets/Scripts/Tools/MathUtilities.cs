using UnityEngine;
using System.Collections;

public static class MathUtilities {

	//사인을 활용한 start에서 end까지 lerp함수
	public static float Sinerp(float start, float end, float value)	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
		//Mathf.Sin(value * Mathf.PI * 0.5f) 활용해서 0~1까지 
	}

	//코사인활용  1~0이됨
	public static float Coserp(float start, float end, float value)	{
		return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
	}


	public static float CoSinLerp(float start, float end, float value) {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }
}
