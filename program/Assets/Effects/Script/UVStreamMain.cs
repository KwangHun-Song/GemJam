using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVStreamMain : MonoBehaviour {

    public float XSpeed = 0.5f; //X 방향 물결치는 속도
    public float YSpeed = 0.0f; //Y 방향 물결치는 속도
    float XOffset;
    float YOffset;

    Renderer _rend;
    Renderer rend {
        get
        {
            if (_rend == null)
            {
                _rend = GetComponent<Renderer>();
                _rend.material = Instantiate<Material>(rend.material);
                _rend.material.GetTextureOffset(TextureName);
            }
            return _rend;
        }
    }

    public string TextureName;
    public float interval = 1.0f;

    [Header("Tilling")]
    public bool mapTillingToScale = false;
    public Transform scaleParent;

    void OnEnable()
    {
        StartCoroutine(UVStream());
        StartCoroutine(CoMapTillingToScale());
    }	

    IEnumerator UVStream() {        
        while(gameObject.activeSelf) {
            while(XSpeed >= 0f ? XOffset < 1f : XOffset > -1f) {
                rend.material.SetTextureOffset(TextureName,new Vector2(XOffset,YOffset));
                
                XOffset += XSpeed * Time.deltaTime;
                YOffset += YSpeed * Time.deltaTime;
                yield return null;                
            }
            XOffset = XOffset > 1f ? XOffset - 1f : XOffset;
            XOffset = XOffset < -1f ? XOffset + 1f : XOffset;
            YOffset = YOffset > 1f ? YOffset - 1f : YOffset;
            YOffset = YOffset < 0f ? YOffset + 1f : YOffset;        

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator CoMapTillingToScale() {
        while (mapTillingToScale && gameObject.activeSelf) {
            Vector3 parentScale = scaleParent.localScale;
            rend.material.SetTextureScale(TextureName, new Vector2(parentScale.x, parentScale.y));
            yield return null;
        }
    }
}
