using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.EventSystems;

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
        StartCoroutine(CheckHP());
    }

    //リスポーンさせるときに呼ぶオブジェクトを初期化させる関数
    public void ResetStatus()
    {
        m_life = m_initialLife;
    }
    public void Damage(int playerId, int damage)
    {
        m_life -= damage;
        Debug.LogFormat("Player {0} が Player {1} の {2} に {3} のダメージを与えた", playerId, m_photonView.Owner.ActorNumber, name, damage);

        
        object[] parameters = new object[] { m_life ,this.gameObject};
        m_photonView.RPC("SyncLife", RpcTarget.All, parameters);
    }
    public void CallSyncLife(object[] parameters)
    {
        int life = (int)parameters[0];
        if ( life<= 0)
        {
            ResetStatus();
            NetworkGameManager gm = GameObject.Find("GameManager").GetComponent<NetworkGameManager>();
            Vector3 reSpawnPoint = gm.GetReSpawnPoint().position;
            object[] param = new object[] { reSpawnPoint};
            m_photonView.RPC("SyncPosition", RpcTarget.All, param);
            /*
            object[] paramater = new object[] { this.gameObject };
            m_photonView.RPC("Destroy", RpcTarget.All, paramater);
            */

        }
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
        
        Debug.LogFormat("Player {0} の {1} の残りライフは {2}", m_photonView.Owner.ActorNumber, gameObject.name, m_life);
    }

    [PunRPC]
    void SyncPosition(Vector3 position)
    {
        this.gameObject.transform.position = position; 
    }
    //オブジェクトの停止処理ここでリスポーンのイベントを起こす
    [PunRPC]
    void Destroy(GameObject player)
    {
        this.gameObject.SetActive(false);
        RaiseEvent(EventCode.respawn,player);
        Debug.LogFormat("" + m_photonView.Owner.ActorNumber, gameObject.name);
    }

    //イベントコードに対応するイベントを行う
    private void RaiseEvent(EventCode code,object sendObject)
    {   
        RaiseEventOptions option = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All
        };
        SendOptions sendsOption = new SendOptions();

        PhotonNetwork.RaiseEvent((byte)code, sendObject, option, sendsOption);
    }

    private IEnumerator CheckHP()
    {
        bool isCheck = false;
        while (!isCheck)
        {
            if (m_life<=0)
            {
                /*
                NetworkGameManager gm = GameObject.Find("GameManager").GetComponent<NetworkGameManager>();
                gm.ReSpawnPlayer();
                */
                /*
                GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
                foreach (var item in objects)
                {
                    PhotonView view = item.GetComponent<PhotonView>();
                    if (view.IsMine)
                    {
                        HPManager manager = item.GetComponent<HPManager>();
                        DataSave.PlayerDataSave(manager.SetStatus);
                    }
                }
                isCheck = true;
                */
            }
            yield return null;
        }
        yield break;
    }
}
