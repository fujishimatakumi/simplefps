using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
//using UnityEngine.UIElements;

/// <summary>
/// プレイヤーの基本操作を処理するコンポーネント
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class NetworkPlayerController : MonoBehaviour
{
    /// <summary>FPS のカメラ</summary>
    [SerializeField] Camera m_mainCamera;
    [SerializeField] Camera m_SubCamera;
    [SerializeField] GameObject m_Arch;
    [SerializeField] Material m_myMaterial;
    [SerializeField] Material m_otherMaterial;
    /// <summary>照準となる UI オブジェクト</summary>
    [SerializeField] Image m_crosshair;
    /// <summary>照準に敵を捕らえていない時の色</summary>    
    [SerializeField] Color m_noTarget;
    /// <summary>照準に敵を捕らえている時の色</summary>
    [SerializeField] Color m_onTarget;
    /// <summary>射撃可能距離</summary>
    [SerializeField, Range(10, 200)] float m_shootRange = 100f;
    /// <summary>照準の Ray が当たる Layer</summary>
    [SerializeField] LayerMask m_shootingLayer;
    /// <summary>ヘッドショット判定用フラグ/// </summary>
    bool m_isHed;
    /// <summary>攻撃したらダメージを与えられる対象</summary>
    Damageable m_target;
    PhotonView m_photonView;
    private CharacterController m_control;
    private Transform m_cameraTransform;

    private Vector3 m_moveDirection;
    private Vector3 m_camRotation;

    public float speed = 6;
    [SerializeField] public float speedUp;
    public float jumpSpeed = 1.5f;
    public float gravity = 0.5f;

    /// <summary>下に向くことのできる最小角度</summary>
    [Range(-45, -15)]
    public int minAngle = -30;
    /// <summary>上に向くことのできる最大角度</summary>
    [Range(30, 80)]
    public int maxAngle = 45;
    /// <summary>マウスの感度</summary>
    [Range(50, 500)]
    public int sensitivity = 200;

    /// <summary>足音/// </summary>
    private new AudioSource audio;
    [SerializeField] AudioClip footSound;

    private void Start()
    {
        m_control = GetComponent<CharacterController>();
        m_photonView = GetComponent<PhotonView>();

        //足音を鳴らすための準備
        audio = GetComponent<AudioSource>();
        audio.clip = footSound;

        //マウスカーソルを非表示にする
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MeshRenderer r = m_Arch.GetComponent<MeshRenderer>();

        // 自分が生成したプレイヤーのカメラだけを有効にする
        if (m_photonView.IsMine)
        {
            m_mainCamera.gameObject.SetActive(true);
            m_cameraTransform = m_mainCamera.transform;
            m_SubCamera.gameObject.SetActive(true);
            m_Arch.gameObject.SetActive(true);
            r.material = m_myMaterial;
        }
        else
        {
            m_mainCamera.gameObject.SetActive(false);
            m_SubCamera.gameObject.SetActive(false);
            r.material = m_otherMaterial;
        }
    }

    private void Update()
    {
        // 自分に対してだけ入力を検出して行われる処理をする
        if (m_photonView.IsMine)
        {
            Aim();
            Shoot();
            Rotate();
            Move();
        }
    }

    /// <summary>
    /// Controls camera rotation oriented by mouse position
    /// </summary>
    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        m_camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        m_camRotation.x = Mathf.Clamp(m_camRotation.x, minAngle, maxAngle);

        m_cameraTransform.localEulerAngles = m_camRotation;
    }

    /// <summary>
    /// 照準を操作する
    /// 照準にダメージを与えられる対象がいる場合に照準の色を変え、その対象を m_target に保存する
    /// </summary>
    void Aim()
    {
        Ray ray = m_mainCamera.ScreenPointToRay(m_crosshair.rectTransform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_shootRange, m_shootingLayer))
        {
            m_target = hit.collider.GetComponent<Damageable>();
            if (hit.collider.gameObject.tag == "Hed")
            {
                m_isHed = true;
            }
            else
            {
                m_isHed = false;
            }
            if (m_target)
            {
                m_crosshair.color = m_onTarget;
            }
            else
            {
                m_crosshair.color = m_noTarget;
            }
        }
        else
        {
            m_target = null;
            m_crosshair.color = m_noTarget;
            m_isHed = false;
        }
    }

    /// <summary>
    /// Controls movement with CharacterController
    /// </summary>
    private void Move()
    {
        if (m_control.isGrounded)
        {
            m_moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            m_moveDirection = transform.TransformDirection(m_moveDirection);
            if (m_moveDirection.x != 0 || m_moveDirection.z != 0)
            {
                Debug.Log("音");
                audio.Play();
            }

            if (Input.GetButtonDown("Jump"))
            {
                m_moveDirection.y = jumpSpeed;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            m_moveDirection.y -= gravity * Time.deltaTime;
            m_control.Move(m_moveDirection *( speed * speedUp) * Time.deltaTime);
        }

        m_moveDirection.y -= gravity * Time.deltaTime;
        m_control.Move(m_moveDirection * speed * Time.deltaTime);

    }

    /// <summary>
    /// 敵を撃つ
    /// </summary>
    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (m_target)
            {
                m_target.Damage(PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (m_target && m_isHed)
            {
                m_target.Damage(PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }
}