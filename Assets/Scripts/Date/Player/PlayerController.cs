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
        BlowAway,
        Atack,
        RightMove,
        LeftMove,
        Dead,
        Win
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
   
    // 手足制御用オブジェクト
    public GameObject m_bodyObj;
    public GameObject m_rightHandObj;
    public GameObject m_leftHandObj;
    public GameObject m_rightFootObj;
    public GameObject m_leftFootObj;

    //プレイヤーの手足の先のTransform
    //public Transform Hand_R;
    //public Transform Hand_L;
    //public Transform Calf_R;
    //public Transform Calf_L;
    // 手足のMaterial取得
    public Material Hand_R;
    public Material Hand_L;
    public Material Calf_R;
    public Material Calf_L;
    // デフォルトの肌の色を保存用
    private Color SaveColor;


    // 手足制御オブジェクト（Rigidbody）
    /*
    private Rigidbody m_body_rg;
    private Rigidbody m_rightHand_rg;
    private Rigidbody m_leftHand_rg;
    private Rigidbody m_rightFoot_rg;
    private Rigidbody m_leftFoot_rg;
    */
    // 自分自身の制御オブジェクト (Rigidbody)阿野追加
    private Rigidbody m_Player_rg;
    private string[] InputName = new string[(int)EInput.MAX];

    /*
    // 回転する力（パラメータ）
    [SerializeField]
    private Force m_bodyForce;
    [SerializeField]
    private Force m_HandForce;
    [SerializeField]
    private Force m_FootForce;
    */
    /*
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
    */
    // 向きを変える力（パラメータ）
    [SerializeField]
    private float m_directionAngle;

    private EState m_state;

    //private RayTest.RayDirection dir;
    //public RayTest rayTest;
    //確率
    [Range(0, 100)]
    public int CriticalProbability = 20;
    //ヒット時の力
    [Range(0, 10)]
    public float HitPower = 8;
    //クリティカル時の力
    [Range(0, 20)]
    public float CriticalPower = 20;
    //飛んで動けなくなる制御時間(バグ防止)
    private float FlayTime_Max = 1;
    //飛んでる経過時間
    private float FlayNowTime = 0;
    private bool[] m_isInputFlg = new bool[(int)EInput.MAX];
    // 攻撃された相手のプレイヤーID
    private int m_hitReceivePlayerID;
    public int HitReceivePlayerID
    {
        get { return m_hitReceivePlayerID; }
    }


    private CrushPointManager m_crushPointManager;

<<<<<<< HEAD:Assets/Scripts/Date/PlayerController.cs
    public GameObject m_shrinkRightHandObj;
    public GameObject m_shrinkLeftHandObj;
    public GameObject m_shrinkRightFootObj;
    public GameObject m_shrinkLeftFootObj;
    public float shrinkTime = 0;
    private PlayerCamera P_Camera;
=======

    private PlayerExtendAndShrink   m_extendAndShrink;
    private PlayerMoving            m_moving;
    private PlayerRay               m_ray;

>>>>>>> 手足伸ばす処理改良:Assets/Scripts/Date/Player/PlayerController.cs
    void Awake()
    {
        //       m_lifeFlg = false;
        //       m_inputFlg = false;
        m_state = EState.Init;
        // デフォルトの肌の色の数値を代入する
        SaveColor = new Color(1.0f, 0.78f, 0.55f, 1.0f);
        /* 色が変わった状態でHumanが無くなるとMaterialが変更されてままになるので生成時に初期色入れておく*/
        Hand_R.color = SaveColor;
        Hand_L.color = SaveColor;
        Calf_R.color = SaveColor;
        Calf_L.color = SaveColor;
    }
    // Use this for initialization
    void Start()
    {
        /*
        m_body_rg = m_bodyObj.GetComponent<Rigidbody>();
        m_rightHand_rg = m_rightHandObj.GetComponent<Rigidbody>();
        m_leftHand_rg = m_leftHandObj.GetComponent<Rigidbody>();
        m_rightFoot_rg = m_rightFootObj.GetComponent<Rigidbody>();
        m_leftFoot_rg = m_leftFootObj.GetComponent<Rigidbody>();
        m_Player_rg = GetComponent<Rigidbody>();
        P_Camera = GetComponent<PlayerCamera>();
        m_bodyForce.Init();
        m_HandForce.Init();
        m_FootForce.Init();
<<<<<<< HEAD:Assets/Scripts/Date/PlayerController.cs
        m_bodyMoveForce.Init();

        //       m_state = EState.Idle;


        dir = rayTest.Dir;
=======
        */
       // m_bodyMoveForce.Init();
        CreateEffect = null;
        //       m_state = EState.Idle;

        //dir = rayTest.Dir;
>>>>>>> 手足伸ばす処理改良:Assets/Scripts/Date/Player/PlayerController.cs

        InputName[0] = "Horizontal_Player" ;
        InputName[1] = "Vertical_Player" ;

        InputName[2] = "A_Player";
        InputName[3] = "B_Player";
        InputName[4] = "X_Player";
        InputName[5] = "Y_Player";

        myInputManager = GameObject.FindObjectOfType<MyInputManager>();
        if(myInputManager == null)
            Debug.LogError("MyInputManagerがシーンに存在しません");

        m_hitReceivePlayerID = PlayerID;
        m_crushPointManager = FindObjectOfType<CrushPointManager>();

        // 手足伸ばすスクリプト
        m_extendAndShrink = GetComponent<PlayerExtendAndShrink>();
        if (m_extendAndShrink == null) Debug.LogError("PlayerExtendAndShrinkをアタッチしてください。");

        if (!DebugModeGame.GetProperty().m_debugMode) return;
        // デバッグモードONの時の設定
        if (DebugModeGame.GetProperty().m_debugPlayerEnable)
        {
            m_state = EState.Idle;
        }
        if (DebugModeGame.GetProperty().m_controllerDisable)
        {
            myInputManager.joysticks[m_playerID - 1] = m_playerID;
        }
    }
    void Update()
    {
        Debug.Log(m_state);
    
    }
    public EState GetMyState()
    {
        return m_state;
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (m_state == EState.Init ||
            m_state == EState.Dead ||
            m_state == EState.Wait ||
            m_state == EState.Win) return;

        if(m_state==EState.BlowAway)
        {
            FlayNowTime += Time.deltaTime;
            if(FlayNowTime>=FlayTime_Max)
            {
                FlayNowTime = 0;
                m_state = EState.Idle;
            }
            return;
        }
        // 右方向
        float lsh = Input.GetAxis(InputName[(int)EInput.Horizontal] + myInputManager.joysticks[m_playerID-1].ToString());
        float lsv = Input.GetAxis(InputName[(int)EInput.Vertical] + myInputManager.joysticks[m_playerID-1].ToString());
     
        if (lsh == 0.0f)
        {
            m_state = EState.Idle;
            m_isInputFlg[(int)EInput.Vertical] = m_isInputFlg[(int)EInput.Horizontal] = false;
        }
        else if (lsh > 0.0f)
        {
            //    Move(1);
            m_moving.Move(m_ray.Dir, true);
            m_state = EState.RightMove;
            m_isInputFlg[(int)EInput.Horizontal] = true;
        }
        // 左方向
        else if (lsh < 0.0f)
        {
            //    Move(-1);
            m_moving.Move(m_ray.Dir, false);
            m_state = EState.LeftMove;
            m_isInputFlg[(int)EInput.Horizontal] = true;
        }

        if (lsv > 0.0f)
        {
            //Direction(1);
            m_moving.Rotation(true);
            m_isInputFlg[(int)EInput.Vertical] = true;
        }
        else if (lsv < 0.0f)
        {
            //Direction(-1);
            m_moving.Rotation(false);
            m_isInputFlg[(int)EInput.Vertical] = true;
        }



        if (Input.GetButton(InputName[(int)EInput.A] + myInputManager.joysticks[m_playerID - 1]))
        {
<<<<<<< HEAD:Assets/Scripts/Date/PlayerController.cs

            //DebugExtend(m_rightHandObj, m_shrinkRightHandObj, m_HandExtend);
            Extend(m_rightHand_rg, m_HandExtend);
            //カラー変更
            Hand_R.color = Color.Lerp(Hand_R.color, Color.red, Time.deltaTime * 3.0f);
=======
           // DebugExtend(m_rightHandObj, m_shrinkRightHandObj, m_HandExtend);
//            Extend(m_rightHand_rg, m_HandExtend);
            //エフェクト発生
            if (CreateEffect==null)
            {
                CreateEffect = Instantiate(PickupPrefab, Hand_R.position, Quaternion.identity);
                PickupEffect = GetComponent<PickUpEffect>();
            }
>>>>>>> 手足伸ばす処理改良:Assets/Scripts/Date/Player/PlayerController.cs

            m_isInputFlg[(int)EInput.A] = true;
        }
        else
        {
            //元の色に戻す
            Hand_R.color = Color.Lerp(Hand_R.color, SaveColor, Time.deltaTime * 3.0f);
            m_isInputFlg[(int)EInput.A] = false;


        }

        if (Input.GetButton(InputName[(int)EInput.B] + myInputManager.joysticks[m_playerID - 1]))
        {
<<<<<<< HEAD:Assets/Scripts/Date/PlayerController.cs
            Extend(m_leftHand_rg, -m_HandExtend);
            //カラー変更
            Hand_L.color = Color.Lerp(Hand_L.color, Color.red, Time.deltaTime * 3.0f);
=======
        //    Extend(m_leftHand_rg, -m_HandExtend);
            //エフェクト発生
            if (CreateEffect == null)
            {
                CreateEffect = Instantiate(PickupPrefab, Hand_L.position, Quaternion.identity);
                PickupEffect = GetComponent<PickUpEffect>();
            }

>>>>>>> 手足伸ばす処理改良:Assets/Scripts/Date/Player/PlayerController.cs
            m_isInputFlg[(int)EInput.B] = true;
        }
        else
        {
            //元の色に戻す
            Hand_L.color = Color.Lerp(Hand_L.color, SaveColor, Time.deltaTime * 3.0f);
            m_isInputFlg[(int)EInput.B] = false;
        }

        if (Input.GetButton(InputName[(int)EInput.X] + myInputManager.joysticks[m_playerID - 1]))
        {
<<<<<<< HEAD:Assets/Scripts/Date/PlayerController.cs
            Extend(m_rightFoot_rg, m_FootExtendR);
            //カラー変更
            Calf_R.color = Color.Lerp(Calf_R.color, Color.red, Time.deltaTime * 3.0f);
=======
        //    Extend(m_rightFoot_rg, m_FootExtendR);
            //エフェクト発生
            if (CreateEffect == null)
            {
                CreateEffect = Instantiate(PickupPrefab, Calf_R.position, Quaternion.identity);
                PickupEffect = GetComponent<PickUpEffect>();
            }

>>>>>>> 手足伸ばす処理改良:Assets/Scripts/Date/Player/PlayerController.cs
            m_isInputFlg[(int)EInput.X] = true;
        }
        else
        {
            //元の色に戻す
            Calf_R.color = Color.Lerp(Calf_R.color, SaveColor, Time.deltaTime * 3.0f);
            m_isInputFlg[(int)EInput.X] = false;
        }


        if (Input.GetButton(InputName[(int)EInput.Y] + myInputManager.joysticks[m_playerID - 1]))
        {
<<<<<<< HEAD:Assets/Scripts/Date/PlayerController.cs
            Extend(m_leftFoot_rg, m_FootExtendL);
            //カラー変更
            Calf_L.color = Color.Lerp(Calf_L.color, Color.red, Time.deltaTime * 3.0f);
=======
        //    Extend(m_leftFoot_rg, m_FootExtendL);
            
            //エフェクト発生
            if (CreateEffect==null)
            {
                CreateEffect = Instantiate(PickupPrefab, Calf_L.position, Quaternion.identity);
                PickupEffect = GetComponent<PickUpEffect>();
            }

>>>>>>> 手足伸ばす処理改良:Assets/Scripts/Date/Player/PlayerController.cs
            m_isInputFlg[(int)EInput.Y] = true;
        }
        else
        {
            //元の色に戻す
            Calf_L.color = Color.Lerp(Calf_L.color, SaveColor, Time.deltaTime * 3.0f);
            m_isInputFlg[(int)EInput.Y] = false;
        }


        // 回転減衰
        if (m_state == EState.LeftMove || m_state == EState.RightMove)
        {
            /*
            m_bodyForce.cntForce.x *= m_bodyForce.decayForce.x;
            m_bodyForce.cntForce.y *= m_bodyForce.decayForce.y;
            m_bodyForce.cntForce.z *= m_bodyForce.decayForce.z;
            m_bodyMoveForce.cntForce.x *= m_bodyMoveForce.decayForce.x;
            */
        }
        else
        {/*
            m_bodyForce.Init();
            m_bodyMoveForce.Init();
            */
        }

        // キャラクターの向き
        //if (dir != rayTest.Dir)
        //{
        // パラメータ初期化
        //  dir = rayTest.Dir;
        if (m_ray.IsDIffDirection())
        {
            m_state = EState.Idle;
            m_ray.InitDiffDirection();

            m_moving.Init();
        }
            /*
            m_bodyForce.Init();
            m_HandForce.Init();
            m_FootForce.Init();
            */
        //}
        //Debug.Log(m_state);
    }


    /*
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
            
            if (value > 0)
                Debug.Log("右移動：キャラクターの向き（前）");
            else
                Debug.Log("左移動：キャラクターの向き（前）");
                
        }
        // 後ろ
        else if (dir == RayTest.RayDirection.Back)
        {
            m_leftHand_rg.AddRelativeForce(-worldHandVelocity, ForceMode.Force);
            m_rightHand_rg.AddRelativeForce(-worldHandVelocity, ForceMode.Force);
            m_rightFoot_rg.AddRelativeForce(worldFootVelocity, ForceMode.Force);
            m_leftFoot_rg.AddRelativeForce(worldFootVelocity, ForceMode.Force);
            if (value > 0)
                Debug.Log("右移動：キャラクターの向き（後）");
            else
                Debug.Log("左移動：キャラクターの向き（後）");
                
        }
    }
    */

    /*
    void Direction(float value)
    {
        m_body_rg.AddTorque(0.0f, m_directionAngle * value, 0.0f);
    }
    */
    /*
    void Extend(Rigidbody rigidbody, Vector3 vec)
    {
        //joint.spring = 0;
        Vector3 worldRightHandVelocity = transform.TransformDirection(vec);
        
        // Debug.Log(worldRightHandVelocity);
        rigidbody.AddForce(worldRightHandVelocity, ForceMode.Force);
    }
    */
    /*// 縮こまる
    void ShrinkSink(GameObject ctlObj,GameObject shrinkObj,Vector3 vec)
    {
        ctlObj.transform.position = Vector3.Lerp(ctlObj.transform.position, shrinkObj.transform.position, shrinkTime);

        shrinkTime += Time.deltaTime;
    }
    */
    public void Dead()
    {
        // m_lifeFlg = false;
        // 自分が獲得したポイントをCostManager登録
        if (m_state == EState.Dead) return;
        //if (m_crushPointManager == null) m_crushPointManager = FindObjectOfType<CrushPointManager>();
        //m_crushPointManager.DamageDead(m_playerID, m_hitReceivePlayerID);
        m_state = EState.Dead;
        GetComponent<Rigidbody>().isKinematic = true;
        P_Camera.CameraDelete();
        Destroy(gameObject);
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
    //吹っ飛び状態にする
    public void BlowAwayNow(int hitReceiveID)
    {
        m_hitReceivePlayerID = hitReceiveID;
        m_state = EState.BlowAway;
    }
    // 入力フラグ（クリティカルヒット用）
    public bool IsInputFlagParts(EInput eInput)
    {
        return m_isInputFlg[(int)eInput];
    }
    // 入力フラグ（クリティカルヒット用）
    public bool IsInputAllFlags()
    {
        for(int i = 0; i < m_isInputFlg.Length; ++i)
        {
            if (m_isInputFlg[i]) return true;
        }
        return false;
    }

    // 最後のアタックしたPlayerID
    public int LastAttackPlayerID()
    {
        return m_hitReceivePlayerID;
    }

    // 勝利
    public void Win()
    {
        m_state = EState.Win;
        GetComponent<Rigidbody>().isKinematic = true;
        P_Camera.CameraDelete();
    }

    private void OnTriggerEnter(Collider other)
    {
        //吹っ飛び中に地面に当たると吹っ飛び状態解除

        if (DebugModeGame.GetProperty().m_debugMode && DebugModeGame.GetProperty().m_debugPlayerEnable) return;
        if (m_state != EState.Init) return;
        if (LayerMask.LayerToName(other.gameObject.layer) != "Ground") return;
        m_state = EState.Wait;
        
//        Destroy(GetComponent<BoxCollider>());
    }
}
