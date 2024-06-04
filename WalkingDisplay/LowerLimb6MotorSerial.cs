using UnityEngine;
using System;
// using System.IO;
// using System.IO.Ports;
//6���[�^�p
//����ڒx�����Ԃ��p�����[�^�ʐM����
//�ڕW�ݒ�2����->4�����ɕύX
//UDPReceiver�i�O���t�`��Ccsv�ۑ��j�Ȃ�
//����t�������@�p, 


public class LowerLimb6MotorSerial : LowerLimb6MotorBase
{
    // [SerializeField]
    // WalkDemoMainController mainController;
    [SerializeField]
    UDPReceiver udpReceiver;


    public string portName = "COM7";    
    public int baudRate = 9600;
    private System.IO.Ports.SerialPort client;

    //public WalkTimer walkTimer;
    public string sendText;

    //�y�_��
    private const float maxAngle = 25f; //�y�_���ő�p�x[mm]
    private const float minAngle = -55f; //�y�_���ŏ��p�x[mm]
    // private const float resolutionPedal = 0.0144f; //[degrees/pulse]
    private const float resolutionPedal = 0.0072f; //[degrees/pulse]
    private const float footLength = 155f;//��]���������[�܂ł̒���
    private float rightPedalUp1_f = 0f;//�E�y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float rightPedalDown1_f = 0f;//�E�y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float leftPedalUp1_f = 0f;//���y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float leftPedalDown1_f = 0f;//���y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float rightPedalUp2_f = 0f;//�E�y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float rightPedalDown2_f = 0f;//�E�y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float leftPedalUp2_f = 0f;//���y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj
    private float leftPedalDown2_f = 0f;//���y�_�����~���̖ڕW�p���X�i�[�p�i�����Łj

    //�X���C�_
    public const int maxPosition = 90;  //[mm]
    private const int minPosition = -90;  //[mm]
    private const float resolutionSlider = 0.012f; //[mm/pulse]
    public float seatRotation = 0f;



    [SerializeField, Range(-30, 360)]
    public float leftPedalUp1 = 25;//���y�_�����[�㏸�ڕW�l[mm]
    [SerializeField, Range(-55, 105)]
    private float leftPedalDown1 = 0;//���y�_�����[���~�ڕW�l[mm]
    [SerializeField, Range(-90, 90)]
    float leftSliderForward1 = 36;////���X���C�_�O�i�ڕW�l[mm]
    [SerializeField, Range(-90, 90)]
    float leftSliderBackward1 = -24;//���X���C�_��ޖڕW�l[mm]


    [SerializeField, Range(-55, 105)]
    public float rightPedalUp1 = 25;//�E�y�_�����[�㏸�ڕW�l[mm]
    [SerializeField, Range(-55, 105)]
    private float rightPedalDown1 = 0;//�E�y�_�����[���~�ڕW�l[mm]

    [SerializeField, Range(-90, 90)]
    float rightSliderForward1 = 36;//�E�X���C�_��ޖڕW�l[mm]
    [SerializeField, Range(-90, 90)]
    float rightSliderBackward1 = -24;//�E�X���C�_��ޖڕW�l[mm]

    //Yaw��]
    [SerializeField, Range(-18, 18)]
    float leftRotationAngle1 = -1.5f;//�����O�i��Yaw��]�p�x[degree]
    [SerializeField, Range(-18, 18)]
    float leftRotationAngle2 = 1.5f;//������ގ�Yaw��]�p�x[degree]

    [SerializeField, Range(-18, 18)]
    float rightRotationAngle1 = -1.5f;//�E���O�i��Yaw��]�p�x[degree]
    [SerializeField, Range(-18, 18)]
    float rightRotationAngle2 = 1.5f;//�E����ގ�Yaw��]�p�x[degree]
    [SerializeField, Range(-18, 18)]

    //�o�̓p���X�i���M�j
    public int[] targetPulseUp1 = new int[6] { 0, 0, 0, 0, 0, 0 };//�㏸�^�O�i���̖ڕW�p���X�i���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[pulse]
    public int[] targetPulseDown1 = new int[6] { 0, 0, 0, 0, 0, 0 };//���~�^��ގ��̖ڕW�p���X�i���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[pulse]
                                                                    //�쓮���ԁi���M�j
    public int[] driveTimeUp1 = new int[6] { 2000, 560, 560, 560, 560, 560 };//�㏸�^�O�i���̋쓮���ԁi���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[ms]
    public int[] driveTimeDown1 = new int[6] {2000, 840, 280, 840, 840, 840 };//���~�^��ގ��̋쓮���ԁi���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[ms]
                                                                              //�ҋ@���ԁi���M�j
    public int[] delayTimeUp1 = new int[6] { 560, 0, 560, 0, 0, 0 };//�㏸�^�O�i�n�߃��[�^��~���ԁi���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[ms]
    public int[] delayTimeDown1 = new int[6] { 0, 0, 0, 0, 0, 0 };//���~�^��ގn�߃��[�^��~���ԁi���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[ms]
    public int[] delayTimeFirst = new int[6] { 0, 280, 700, 980, 280, 980 };//����ڃ��[�^��~���ԁi���y�_���A���X���C�_�A�E�y�_���A�E�X���C�_�j[ms]
    public int seatRotationPulse;
    public bool walk;




    // Use this for initialization
    void Start()
    {
        client = new System.IO.Ports.SerialPort(portName, baudRate, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        client.Open();
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))//�p�����[�^�ύX
        {
            dataUpdate();

        }

    }
    void dataUpdate()
    {

        targetCalculate();//�ڕW�l�v�Z
                          //���M����f�[�^�𕶎���ł܂Ƃ߂�
        sendText = "update" + ",";
        for (int i = 0; i < 6; i++)
        {
            sendText += targetPulseUp1[i].ToString() + "," + targetPulseDown1[i].ToString() + ",";
            sendText += driveTimeUp1[i].ToString() + "," + driveTimeDown1[i].ToString() + ",";
            sendText += delayTimeUp1[i].ToString() + "," + delayTimeDown1[i].ToString() + ",";
            sendText += delayTimeFirst[i].ToString() + ",";
        }
        sendText += "/";//�I���̖ڈ�
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//���M���镶�����byte�ɕϊ�
        client.Write(sendByte, 0, sendByte.Length);//���M
        Debug.Log(sendText);

    }
    void targetCalculate()//�U���l�imm�j���o�̓p���X�ϊ�
    {
        //�y�_���ڕW�l�v�Z
        leftPedalUp1_f = -(Mathf.Asin(leftPedalUp1 / footLength) * Mathf.Rad2Deg / resolutionPedal);
        leftPedalDown1_f = -(Mathf.Asin(leftPedalDown1 / footLength) * Mathf.Rad2Deg) / resolutionPedal;
        rightPedalUp1_f = Mathf.Asin(rightPedalUp1 / footLength) * Mathf.Rad2Deg / resolutionPedal;
        rightPedalDown1_f = Mathf.Asin(rightPedalDown1 / footLength) * Mathf.Rad2Deg / resolutionPedal;

        leftPedalUp1_f = leftPedalUp1 / resolutionPedal;
        leftPedalDown1_f = leftPedalDown1 / resolutionPedal;

        //�ڕW�p���X�𐮐��^�Ŋi�[
        targetPulseUp1[0] = (int)leftPedalUp1_f;//-(Up)
        targetPulseDown1[0] = (int)leftPedalDown1_f;
        targetPulseUp1[1] = (int)(leftSliderForward1 / resolutionSlider);
        targetPulseDown1[1] = (int)(leftSliderBackward1 / resolutionSlider);
        targetPulseUp1[2] = (int)rightPedalUp1_f;
        targetPulseDown1[2] = (int)rightPedalDown1_f;
        targetPulseUp1[3] = (int)(rightSliderForward1 / resolutionSlider);
        targetPulseDown1[3] = (int)(rightSliderBackward1 / resolutionSlider);
        targetPulseUp1[4] = (int)(-leftRotationAngle1 * 10000 * 11 / 120);//��]�p�x*�i�쓮���[�^1��]�̃p���X��/���[�^1��]�ł̃��[�����]�p�x�j
        targetPulseDown1[4] = (int)(-leftRotationAngle2 * 10000 * 11 / 120);//��]�p�x*�i�쓮���[�^1��]�̃p���X��/���[�^1��]�ł̃��[�����]�p�x�j
        targetPulseUp1[5] = (int)(-rightRotationAngle1 * 10000 * 11 / 120);//��]�p�x*�i�쓮���[�^1��]�̃p���X��/���[�^1��]�ł̃��[�����]�p�x�j
        targetPulseDown1[5] = (int)(-rightRotationAngle2 * 10000 * 11 / 120);//��]�p�x*�i�쓮���[�^1��]�̃p���X��/���[�^1��]�ł̃��[�����]�p�x�j
        seatRotationPulse = (int)(-seatRotation * 10000 * 11 / 120);

    }

    public override void WalkBack()
    {
        //���i�p�����[�^�ݒ�
        leftPedalUp1 = 24;
        leftPedalDown1 = 0;
        leftSliderForward1 = 38;
        leftSliderBackward1 = -22;
        leftRotationAngle1 = 0;
        leftRotationAngle2 = 0;
        rightPedalUp1 = 24;
        rightPedalDown1 = 0;
        rightSliderForward1 = 38;
        rightSliderBackward1 = -22;
        rightRotationAngle1 = 0;
        rightRotationAngle2 = 0;
        seatRotation = 0f;

        targetCalculate();//�ڕW�l�v�Z

        //���M����f�[�^�𕶎���ł܂Ƃ߂�

        sendText = "back" + ",";
        for (int i = 0; i < 6; i++)
        {

            sendText += targetPulseUp1[i].ToString() + "," + targetPulseDown1[i].ToString() + ",";
            sendText += driveTimeUp1[i].ToString() + "," + driveTimeDown1[i].ToString() + ",";
            sendText += delayTimeUp1[i].ToString() + "," + delayTimeDown1[i].ToString() + ",";
            sendText += delayTimeFirst[i].ToString() + ",";
        }
        sendText += seatRotationPulse.ToString() + ",";
        sendText += "/";//�I���̖ڈ�
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//���M���镶�����byte�ɕϊ�
        client.Write(sendByte, 0, sendByte.Length);//���M
        Debug.Log(sendText);

        walk = true;
        command = true;

    }
    public override void WalkLeft()
    {
        //������p�����[�^�ݒ�
        leftPedalUp1 = 24;
        leftPedalDown1 = 0;
        leftSliderForward1 = 45.3f;
        leftSliderBackward1 = -48.4f;
        leftRotationAngle1 = 4.08f;
        leftRotationAngle2 = -1.41f;//5.49�������U��
        rightPedalUp1 = 24;
        rightPedalDown1 = 0;
        rightSliderForward1 = 47.8f;
        rightSliderBackward1 = -43.7f;
        rightRotationAngle1 = 4.83f;
        rightRotationAngle2 = 0.61f;
        seatRotation = 3.15f;
        targetCalculate();//�ڕW�l�v�Z
        //���M����f�[�^�𕶎���ł܂Ƃ߂�
        if (!walk)
        {
            sendText = "start" + ",";
        }
        else
        {
            sendText = "update" + ",";
        }

        for (int i = 0; i < 6; i++)
        {

            sendText += targetPulseUp1[i].ToString() + "," + targetPulseDown1[i].ToString() + ",";
            sendText += driveTimeUp1[i].ToString() + "," + driveTimeDown1[i].ToString() + ",";
            sendText += delayTimeUp1[i].ToString() + "," + delayTimeDown1[i].ToString() + ",";
            sendText += delayTimeFirst[i].ToString() + ",";
        }
        sendText += seatRotationPulse.ToString() + ",";
        sendText += "/";//�I���̖ڈ�
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//���M���镶�����byte�ɕϊ�
        client.Write(sendByte, 0, sendByte.Length);//���M
        Debug.Log(sendText);

        walk = true;
        command = true;//mainController�ԐM�p

    }

    public override void WalkRight()
    {
        //�E����p�����[�^�ݒ�
        leftPedalUp1 = 24;
        leftPedalDown1 = 0;
        leftSliderForward1 = 47.8f;
        leftSliderBackward1 = -43.7f;
        leftRotationAngle1 = -4.83f;
        leftRotationAngle2 = -0.61f;
        rightPedalUp1 = 24;
        rightPedalDown1 = 0;
        rightSliderForward1 = 45.3f;
        rightSliderBackward1 = -48.3f;
        rightRotationAngle1 = -4.08f;
        rightRotationAngle2 = 1.41f;
        seatRotation = -3.15f;
        targetCalculate();//�ڕW�l�v�Z

        //���M����f�[�^�𕶎���ł܂Ƃ߂�
        if (!walk)
        {
            sendText = "start" + ",";
        }
        else
        {
            sendText = "update" + ",";
        }

        for (int i = 0; i < 6; i++)
        {

            sendText += targetPulseUp1[i].ToString() + "," + targetPulseDown1[i].ToString() + ",";
            sendText += driveTimeUp1[i].ToString() + "," + driveTimeDown1[i].ToString() + ",";
            sendText += delayTimeUp1[i].ToString() + "," + delayTimeDown1[i].ToString() + ",";
            sendText += delayTimeFirst[i].ToString() + ",";
        }
        sendText += seatRotationPulse.ToString() + ",";
        sendText += "/";//�I���̖ڈ�
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//���M���镶�����byte�ɕϊ�
        client.Write(sendByte, 0, sendByte.Length);//���M
        Debug.Log(sendText);

        walk = true;
        command = true;//mainController�ԐM�p
    }
    public override void WalkStop()
    {
        sendText = "stop" + "," + "/";

        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//���M���镶�����byte�ɕϊ�
        if (client != null)
        {
            client.Write(sendByte, 0, sendByte.Length);//���M
        }
        Debug.Log(sendText);
        walk = false;
        command = true;//mainController�ԐM�p

    }

    public override void WalkStraight()
    {
        targetCalculate();//�ڕW�l�v�Z

        //���M����f�[�^�𕶎���ł܂Ƃ߂�
        if (!walk)
        {
            sendText = "start" + ",";
        }
        else
        {
            sendText = "update" + ",";
        }

        for (int i = 0; i < 6; i++)
        {

            sendText += targetPulseUp1[i].ToString() + "," + targetPulseDown1[i].ToString() + ",";
            sendText += driveTimeUp1[i].ToString() + "," + driveTimeDown1[i].ToString() + ",";
            sendText += delayTimeUp1[i].ToString() + "," + delayTimeDown1[i].ToString() + ",";
            sendText += delayTimeFirst[i].ToString() + ",";
        }
        sendText += seatRotationPulse.ToString() + ",";
        sendText += "/";//�I���̖ڈ�
        byte[] sendByte = System.Text.Encoding.ASCII.GetBytes(sendText);//���M���镶�����byte�ɕϊ�
        if (client != null)
        {
            client.Write(sendByte, 0, sendByte.Length);//���M
        }
        Debug.Log(sendText);

        walk = true;
        command = true;

    }

    void OnDestroy()//Scene�����Ƃ�
    {

        if (walk)
        {
            WalkStop();
        }


    }
}


