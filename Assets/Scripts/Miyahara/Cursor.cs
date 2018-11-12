using UnityEngine;

public class Cursor : MonoBehaviour 
{
    public int PlayerID;
    public float Speed;
    private Vector3 pos;
    private Vector3 Oldpos;
    private bool fireflg;
    public bool FireFlg
    {
        get { return fireflg; }
        set { fireflg = value; }
    }

    [SerializeField]
    private MyInputManager myInputManager;

    void Start()
    {
        pos = transform.position;
        fireflg = true;

        myInputManager = GameObject.FindObjectOfType<MyInputManager>();
        if (myInputManager == null)
            Debug.LogError("MyInputManager���V�[���ɂ���܂���");
    }

	void Update () 
	{
        if (!fireflg)
            return;
        
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 OldInputPos = pos;
        if (Input.GetAxis("Horizontal_Player" + myInputManager.joysticks[PlayerID-1].ToString()) < 0)
        {
            pos.x -= Speed;
            Debug.Log(PlayerID + "yoko");
            
        }
        else if (Input.GetAxis("Horizontal_Player" + myInputManager.joysticks[PlayerID - 1].ToString()) > 0)
        {
            pos.x += Speed;
        }
        if (Input.GetAxis("Vertical_Player" + myInputManager.joysticks[PlayerID - 1].ToString()) < 0)
        {
            pos.z -= Speed;
            Debug.Log(PlayerID + "tate");
        }
        else if (Input.GetAxis("Vertical_Player" + myInputManager.joysticks[PlayerID - 1].ToString()) > 0)
        {
            pos.z += Speed;
        }
        

        Ray ray = new Ray();
        ray.direction = Vector3.down;
        ray.origin = pos;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("RayCollider")))
        {
            transform.position = hit.point;
            Oldpos = transform.position;
        }
        else
        {
            if (OldInputPos.x != pos.x) //���͂̃|�W�V����
            {
                transform.position = new Vector3(Oldpos.x, transform.position.y, transform.position.z);
                pos.x = OldInputPos.x;
            }
            if (OldInputPos.z != pos.z) //���͂̃|�W�V����
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, Oldpos.z);
                pos.z = OldInputPos.z;
            }

            
        }
	}
}
