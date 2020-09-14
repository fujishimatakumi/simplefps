using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ダメージとライフを制御するコンポーネント
/// </summary>
public class Damageable : MonoBehaviour
{
    [SerializeField] HPManager m_manager;
    //与えるダメージ
    [SerializeField] int m_damage = 10;
    [SerializeField] PhotonView m_photonView;

    private void Start()
    {
        
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="playerId">ダメージを与えたプレイヤーのID</param>
    /// <param name="damage">ダメージ量</param>
    public void Damage(int playerId)
    {
        m_manager.m_life -= m_damage;
        Debug.LogFormat("Player {0} が Player {1} の {2} に {3} のダメージを与えた", playerId, m_photonView.Owner.ActorNumber, name, m_damage);

        object[] parameters = new object[] { m_manager.m_life };
        m_manager.CallSyncLife(parameters);
    }

    
}
