using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

//制限時間を管理し同期する
//時間はマスタークライアントを基準とする
public class TimeManager : MonoBehaviour
{
    [SerializeField]
    float m_limit = 60;
    float m_timer;
    PhotonView m_view;
    void Start()
    {
        m_view = GetComponent<PhotonView>();
        StartCoroutine(AddTime(m_limit));
    }

    public IEnumerator AddTime(float limit)
    {  
        m_view = GetComponent<PhotonView>();
        m_timer = limit;
        while (m_timer >= 0)
        {
            m_timer -= Time.deltaTime;
            m_view.RPC("SyincTimeText", RpcTarget.All, null);
            Debug.Log(m_timer);
            yield return null;
        }
    }
    
    [PunRPC]
    public void SyincTimeText()
    {
        NetworkGameManager.TimeText.text = m_timer.ToString();
    }
}
