public class CameraWave : UnityEngine.MonoBehaviour
{
    public UnityEngine.Transform centerEye;
    public UnityEngine.Transform room;
    [UnityEngine.SerializeField, UnityEngine.Range(1, 100)]
    public int scale = 10;
    [UnityEngine.SerializeField, UnityEngine.Range(1.6f, 2.0f)]
    public float height = 1.6f;
    void Start() {
    }
    void Update() {
        this.room.localScale = new UnityEngine.Vector3(this.scale, this.scale, this.scale);
        this.room.position = new UnityEngine.Vector3(
            this.centerEye.position.x,
            this.centerEye.position.y - height,
            this.centerEye.position.z
        );
    }
}