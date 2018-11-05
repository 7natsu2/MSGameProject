﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum EInput
    {
        Horizontal,
        Vertical,
        A, B, X, Y,
        MAX
    }

    public enum EState
    {
        Init,
        Wait,
        Idle,
        RightMove,
        LeftMove,
        Dead
    }

    [System.Serializable]
    struct Force
    {
        public Vector3 cntForce;
        public Vector3 maxForce;
        public Vector3 decayForce;
        public void Init()
        {
            cntForce = maxForce;
        }
    }

    // プレイヤー番号
    [SerializeField]
    private int m_playerID;
    public int PlayerID
    {
        get { return m_playerID; }
    }

    [SerializeField]
    private MyInputManager myInputManager;
    /*
    // ジョイスティック番号
    [SerializeField]
    private int m_joystickNum;
    public int JoystickNum
    {
        set { m_joystickNum = value; }
        get { return m_joystickNum; }
    }
    */
    /*   // 生存フラグ
       private bool m_lifeFlg;
       public bool LifeFlag
       {
           get { return m_lifeFlg; }
       }
       // 入力フラグ
       private bool m_inputFlg;
       public bool InputFlag
       {
           get { return m_inputFlg; }
           set { m_inputFlg = value; }
       }
   */
    // 手足制御用オブジェクト
    public GameObject m_bodyObj;
    public GameObject m_rightHandObj;
    public GameObject m_leftHandObj;
    public GameObject m_rightFootObj;
    public GameObject m_leftFootObj;

    // 手足制御オブジェクト（Rigidbody）
    private Rigidbody m_body_rg;
    private Rigidbody m_rightHand_rg;
    private Rigidbody m_leftHand_rg;
    private Rigidbody m_rightFoot_rg;
    private Rigidbody m_leftFoot_rg;

    private string[] InputName = new string[(int)EInput.MAX];

    // 回転する力（パラメータ）
    [SerializeField]
    private Force m_bodyForce;
    [SerializeField]
    private Force m_HandForce;
    [SerializeField]
    private Force m_FootForce;

    // 伸ばす力（パラメータ）
    [SerializeField]
    private Vector3 m_HandExtend;
    [SerializeField]
    private Vector3 m_FootExtendR;
    [SerializeField]
    private Vector3 m_FootExtendL;

    // 移動する力（パラメータ）
    [SerializeField]
    private Force m_bodyMoveForce;

    // 向きを変える力（パラメータ）
    [SerializeField]
    private float m_directionAngle;

    private EState m_state;

    private RayTest.RayDirection dir;
    public RayTest rayTest;
    void Awake()
    {
        //       m_lifeFlg = false;
        //       m_inputFlg = false;
        m_state = EState.Init;
    }
    // Use this for initialization
    void Start()
    {

        m_body_rg = m_bodyObj.GetComponent<Rigidbody>();
        m_rightHand_rg = m_rightHandObj.GetComponent<Rigidbody>();
        m_leftHand_rg = m_leftHandObj.GetComponent<Rigidbody>();
        m_rightFoot_rg = m_rightFootObj.GetComponent<Rigidbody>();
        m_leftFoot_rg = m_leftFootObj.GetComponent<Rigidbody>();

        m_bodyForce.Init();
        m_HandForce.Init();
        m_FootForce.Init();
        m_bodyMoveForce.Init();

        //       m_state = EState.Idle;

        dir = rayTest.Dir;

        InputName[0] = "Horizontal_Player" ;
        InputName[1] = "Vertical_Player" ;
        InputName[2] = "A_Player";
        InputName[3] = "B_Player";
        InputName[4] = "X_Player";
        InputName[5] = "Y_Player";

        myInputManager = GameObject.FindObjectOfType<MyInputManager>();
        if(myInputManager == null)
            Debug.LogError("MyInputManagerがシーンに存在しません");
    }
    void Update()
    {
        Debug.Log(m_state);
    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_state == EState.Init || m_state == EState.Dead || m_state == EState.Wait) return;
        // 右方向
        float lsh = Input.GetAxis(InputName[(int)EInput.Horizontal] + myInputManager.joysticks[m_playerID-1].ToString());
        //       Debug.Log("横" + lsh);
        float lsv = Input.GetAxis(InputName[(int)EInput.Vertical] + myInputManager.joysticks[m_playerID-1].ToString());
        //       Debug.Log("縦" + lsv);

        if (lsh == 0.0f)
        {
            m_state = EState.Idle;
        }
        else if (lsh > 0.0f)
        {
            Move(1);
            m_state = EState.RightMove;
        }
        // 左方向
        else if (lsh < 0.0f)
        {
            Move(-1);
            m_state = EState.LeftMove;
        }

        if (lsv > 0.0f)
        {
            Direction(1);
        }
        else if (lsv < 0.0f)
        {
            Direction(-1);
        }



        if (Input.GetButton(InputName[(int)EInput.A] + myInputManager.joysticks[m_playerID-1]))
        {
  //          Debug.Log(InputName[(int)EInput.A] + m_playerID);
            Extend(m_rightHand_rg, m_HandExtend);
        }
        if (Input.GetButton(InputName[(int)EInput.B] + myInputManager.joysticks[m_playerID-1]))
        {
            Extend(m_leftHand_rg, -m_HandExtend);
        }
        if (Input.GetButton(InputName[(int)EInput.X] + myInputManager.joysticks[m_playerID-1]))
        {
            Extend(m_rightFoot_rg, m_FootExtendR);
        }
        if (Input.GetButton(InputName[(int)EInput.Y] + myInputManager.joysticks[m_playerID-1]))
        {
            Extend(m_leftFoot_rg, m_FootExtendL);
        }

        // 回転減衰
        if (m_state == EState.LeftMove || m_state == EState.RightMove)
        {
            m_bodyForce.cntForce.x *= m_bodyForce.decayForce.x;
            m_bodyForce.cntForce.y *= m_bodyForce.decayForce.y;
            m_bodyForce.cntForce.z *= m_bodyForce.decayForce.z;
            m_bodyMoveForce.cntForce.x *= m_bodyMoveForce.decayForce.x;
        }
        else
        {
            m_bodyForce.Init();
            m_bodyMoveForce.Init();
        }

        // キャラクターの向き
        if (dir != rayTest.Dir)
        {
            // パラメータ初期化
            dir = rayTest.Dir;
            m_state = EState.Idle;
            m_bodyForce.Init();
            m_HandForce.Init();
            m_FootForce.Init();
        }
    //    Debug.Log(m_state);
    }



    void Move(float value)
    {

        //m_rightHand.cntForce.x *= value;
        //m_leftHand.cntForce.x *= value;
        Vector3 worldAngulerVelocity = transform.TransformDirection(m_bodyForce.cntForce * value);
        Vector3 worldMoveVelocity = transform.TransformDirection(m_bodyMoveForce.cntForce * value);
        Vector3 worldHandVelocity = transform.TransformDirection(m_HandForce.cntForce);
        Vector3 worldFootVelocity = transform.TransformDirection(m_FootForce.cntForce);


        m_body_rg.angularVelocity = worldAngulerVelocity;
        m_body_rg.AddRelativeForce(worldMoveVelocity);
        // 前
        if (dir == RayTest.RayDirection.Forward)
        {
            m_rightHand_rg.AddRelativeForce(worldHandVelocity, ForceMode.Force);
            m_leftHand_rg.AddRelativeForce(worldHandVelocity, ForceMode.Force);
            m_rightFoot_rg.AddRelativeForce(worldFootVelocity, ForceMode.Force);
            m_leftFoot_rg.AddRelativeForce(worldFootVelocity, ForceMode.Force);
            /*
            if (value > 0)
                Debug.Log("右移動：キャラクターの向き（前）");
            else
                Debug.Log("左移動：キャラクターの向き（前）");
            */    
        }
        // 後ろ
        else if (dir == RayTest.RayDirection.Back)
        {
            m_leftHand_rg.AddRelativeForce(-worldHandVelocity, ForceMode.Force);
            m_rightHand_rg.AddRelativeForce(-worldHandVelocity, ForceMode.Force);
            m_rightFoot_rg.AddRelativeForce(worldFootVelocity, ForceMode.Force);
            m_leftFoot_rg.AddRelativeForce(worldFootVelocity, ForceMode.Force);
            /*if (value > 0)
                Debug.Log("右移動：キャラクターの向き（後）");
            else
                Debug.Log("左移動：キャラクターの向き（後）");
                */
        }
    }

    void Direction(float value)
    {

        m_body_rg.AddTorque(0.0f, m_directionAngle * value, 0.0f);
    }

    void Extend(Rigidbody rigidbody, Vector3 vec)
    {
        //joint.spring = 0;
        Vector3 worldRightHandVelocity = transform.TransformDirection(vec);
        // Debug.Log(worldRightHandVelocity);
        rigidbody.AddForce(worldRightHandVelocity, ForceMode.Force);
    }

    public void Dead()
    {
        // m_lifeFlg = false;
        m_state = EState.Dead;
    }

    public void PlayStart()
    {
        m_state = EState.Idle;
    }

    // フラグ取得関数
    public bool IsDead()
    {
        if (m_state == EState.Dead) return true;
        return false;
    }

    public bool IsInit()
    {
        if (m_state == EState.Init) return true;
        return false;
    }

    public bool IsWait()
    {
        if (m_state == EState.Wait) return true;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_state != EState.Init) return;
        if (LayerMask.LayerToName(other.gameObject.layer) != "Ground") return;
        m_state = EState.Wait;
        Destroy(GetComponent<BoxCollider>());
    }
}

