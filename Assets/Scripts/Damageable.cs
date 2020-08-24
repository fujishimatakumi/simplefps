using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ダメージとライフを制御するコンポーネント
/// </summary>
public class Damageable : MonoBehaviour
{
    /// <summary>初期ライフ</summary>
    [SerializeField, Range(1, 99999)] int m_initialLife = 5000;
    /// <summary>現在のライフ</summary>
    int m_life;
    PhotonView m_photonView;

    private void Start()
    {
        m_life = m_initialLife;
        m_photonView = GetComponent<PhotonView>();
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="playerId">ダメージを与えたプレイヤーのID</param>
    /// <param name="damage">ダメージ量</param>
    public void Damage(int playerId, int damage)
    {
        m_life -= damage;
        Debug.LogFormat("Player {0} が Player {1} の {2} に {3} のダメージを与えた", playerId, m_photonView.Owner.ActorNumber, name, damage);

        object[] parameters = new object[] { m_life };
        m_photonView.RPC("SyncLife", RpcTarget.All, parameters);
    }

    /// <summary>
    /// ダメージを与えたことをクライアント間で同期する
    /// </summary>
    /// <param name="currentLife"></param>
    [PunRPC]
    void SyncLife(int currentLife)
    {
        m_life = currentLife;
        Debug.LogFormat("Player {0} の {1} の残りライフは {2}", m_photonView.Owner.ActorNumber, gameObject.name, m_life);
        if (m_life <= 0)
        {
            NetworkGameManager.Destroy(gameObject);
            Debug.LogFormat("" + m_photonView.Owner.ActorNumber, gameObject.name);
        }
    }
}
