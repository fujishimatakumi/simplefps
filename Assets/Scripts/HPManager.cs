using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class HPManager : MonoBehaviour
{
    /// <summary>初期ライフ</summary>
    [SerializeField, Range(1, 99999)] int m_initialLife = 5000;
    /// <summary>現在のライフ</summary>
    public int m_life { get; set;}
    PhotonView m_photonView;
    public GameSetStatus SetStatus { get; set; } = GameSetStatus.Win;

    // Start is called before the first frame update
    void Start()
    {
        m_life = m_initialLife;
        m_photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int playerId, int damage)
    {
        m_life -= damage;
        Debug.LogFormat("Player {0} が Player {1} の {2} に {3} のダメージを与えた", playerId, m_photonView.Owner.ActorNumber, name, damage);
        
        object[] parameters = new object[] { m_life };
        m_photonView.RPC("SyncLife", RpcTarget.All, parameters);
    }
    public void CallSyncLife(object[] parameters)
    {
        m_photonView.RPC("SyncLife", RpcTarget.All, parameters);
    }

    /// <summary>
    /// ダメージを与えたことをクライアント間で同期する
    /// </summary>
    /// <param name="currentLife"></param>
    [PunRPC]
    void SyncLife(int currentLife,int playerId)
    {
        m_life = currentLife;
        
        if (m_life <= 0)
        {
            SetStatus = GameSetStatus.Lose;
            object[] paramater = new object[] { playerId };
            m_photonView.RPC("Destroy", RpcTarget.All, paramater);
        }
        Debug.LogFormat("Player {0} の {1} の残りライフは {2}", m_photonView.Owner.ActorNumber, gameObject.name, m_life);
    }

    //オブジェクトの破棄処理ここでゲームセットのイベントを起こす
    [PunRPC]
    void Destroy(int playerId)
    {
        RaiseResultEvent(EventCode.gameSet, playerId);
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in gameObjects)
        {
            NetworkPlayerController controller = item.GetComponent<NetworkPlayerController>();
            controller.enabled = false;
        }
        Debug.LogFormat("" + m_photonView.Owner.ActorNumber, gameObject.name);
    }

    private void RaiseResultEvent(EventCode code,int winerId)
    {
        RaiseEventOptions option = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendsOption = new SendOptions();

        PhotonNetwork.RaiseEvent((byte)code, winerId, option, sendsOption);
    }
}
