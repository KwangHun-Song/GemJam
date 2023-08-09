using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVStreamController02 : MonoBehaviour
{

    public float xoffsetStart = 0.0f;
    public float xoffsetEnd = 1.0f;
    public float yoffsetStart = 0.0f;
    public float yoffsetEnd = 1.0f;
    public float xSpeedDefault = 1.0f;
    public float ySpeedDefault = 1.0f;
    public float RotationSetting = 0.0f;
    public float delayTime = 1.0f;

    public string TextureName;
    float xOffset = 0.0f;
    float yOffset = 0.0f;
    float rotationSet = 0.0f;

    public float refStrength = 0;

    Image image;  

    void Awake()
    {        
        image = GetComponent<Image>();
        image.material = Instantiate<Material>(image.material);       
    }

    void OnEnable()
    {
        StartCoroutine(TintStream());
    }  

    void OnDisable()
    {
        
    }

    private IEnumerator TintStream ()
    {
        while(true)
        {
            // TODO: 시작과 끝은 Start, End 로 표현한다.
            // TODO: 속도는 무조건 양수로 적는다.(음수로 적을 경우 절대값 사용)

            yOffset = yoffsetStart;
            xOffset = xoffsetStart;
            float rotationSet = RotationSetting;
            var xFinished = false;
            var yFinished = false;
            //var rotationFinished = false;
            //if (RotationSpeed == 0) {
                //rotationSet = 0.0F;
            //    rotationFinished = true;
            //} 
            if (ySpeedDefault == 0) {
                yFinished = true;
            }
            if (xSpeedDefault == 0) {
                xFinished = true;
            }
            var xSpeed = Mathf.Abs(xSpeedDefault);
            if (xoffsetStart > xoffsetEnd) {
                xSpeed *= -1;
            }
            var ySpeed = Mathf.Abs(ySpeedDefault);
            if (yoffsetStart > yoffsetEnd) {
                ySpeed *= -1;
            }

            
            while (xFinished == false || yFinished == false) {
                if (yOffset * ySpeed > yoffsetEnd * ySpeed) {
                    yFinished = true;
                }
                
                if (xOffset * xSpeed > xoffsetEnd * xSpeed) {
                    xFinished = true;
                }

                //if (rotationSet >= -Mathf.PI && rotationSet <= Mathf.PI) {
                //  rotationFinished = true;
                //}
            

                image.material.SetTextureOffset(TextureName, new Vector2(xOffset, yOffset));
                image.material.SetFloat("_UVRotation",rotationSet);
                image.material.SetFloat("_RefStrength",refStrength);
                
                if (xFinished == false) xOffset += Time.deltaTime * xSpeed;
                if (yFinished == false) yOffset += Time.deltaTime * ySpeed;                 
                //if (rotationFinished == false) rotationSet += Time.deltaTime;
        
                yield return null;
            }

            yield return new WaitForSeconds(delayTime);
        }
        

        // else
        // {
        //     while(true)
        //     {
        //         xOffset = 0;
        //         yOffset = 0;
        
        //         while(rotationSet >= -Mathf.PI && rotationSet <= Mathf.PI)
        //         {
        //             image.material.SetTextureOffset(TextureName, new Vector2(xOffset, yOffset));
        //             image.material.SetFloat("_UVRotation",rotationSet);                    
        //             rotationSet += Time.deltaTime * RotationSpeed;
                
        //             yield return null;
        //         }         

        //     yield return new WaitForSeconds(delayTime);
        //     }
        // }               
    }
    
}
