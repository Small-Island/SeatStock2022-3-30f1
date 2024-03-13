[UnityEngine.ExecuteInEditMode]
public class CameraWave : UnityEngine.MonoBehaviour
{
    public VestibularDisplay vestibularDisplay;
    public UnityEngine.Transform centerEye;
    public UnityEngine.Transform room;
    [UnityEngine.SerializeField, UnityEngine.Range(1, 100)]
    public float scale = 10;
    [UnityEngine.SerializeField, UnityEngine.Range(1.6f, 2.0f)]
    public float humanHeight = 1.6f;
    [UnityEngine.SerializeField, UnityEngine.Range(0f, 2.0f)]
    public float amptitude = 0.4f;
    [UnityEngine.SerializeField, UnityEngine.Range(0f, 2f), ReadOnly]
    public float displacement = 0f;

    [UnityEngine.SerializeField]
    private UnityEngine.Vector2 thumbStick;

    void Start() {
    }

    private bool thumbStickFlag = false;
    void Update() {
        this.room.localScale = new UnityEngine.Vector3(this.scale/10f, this.scale/10f, this.scale/10f);
        this.room.position = new UnityEngine.Vector3(
            this.centerEye.position.x,
            this.centerEye.position.y - this.humanHeight - this.displacement,
            this.centerEye.position.z
        );

        this.thumbStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        if (this.thumbStick.y > 0.9 && !thumbStickFlag) {
            thumbStickFlag = true;
            this.amptitude += 0.01f;
        }
        if (this.thumbStick.y < -0.9 && !thumbStickFlag) {
            thumbStickFlag = true;
            this.amptitude -= 0.01f;
        }
        if (System.Math.Abs(this.thumbStick.y) < 0.1) {
            this.thumbStickFlag = false;
        }
    }

    public async void UpAsync() {
        int i = 0;
        int n = 100;
        while (i < n) {
            this.displacement = (float)(1.0 - System.Math.Cos((float)i/(float)n*System.Math.PI))/2f*amptitude;
            await System.Threading.Tasks.Task.Delay((int)(1000f/(float)n*vestibularDisplay.period));
            i++;
        }
        return;
    }

    public async void downAsync() {
        int i = 0;
        int n = 100;
        while (i < n) {
            this.displacement = (float)(1.0 + System.Math.Cos((float)i/(float)n*System.Math.PI))/2f*amptitude;
            await System.Threading.Tasks.Task.Delay((int)(1000f/(float)n*vestibularDisplay.period));
            i++;
        }
        return;
    }
}