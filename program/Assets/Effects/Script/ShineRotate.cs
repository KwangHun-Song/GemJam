using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShineRotate : MonoBehaviour {

    public float xSpeed; //X축 회전 스피드
    public float ySpeed; //Y축 회전 스피드
    public float zSpeed; //Z축 회전 스피드

	void Start()
	{

	}

		void Update () {
		//회전
		transform.Rotate(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime, zSpeed * Time.deltaTime);
		
	}
}
