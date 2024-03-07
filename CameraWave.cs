public class CameraWave : UnityEngine.MonoBehaviour
{
    public UnityEngine.Transform centerEye;
    public UnityEngine.Transform room;
    public float offset = 0;
    void Start() {

    }
    void Update() {
        this.room.position = new UnityEngine.Vector3(
            this.centerEye.position.x,
            this.centerEye.position.y - offset,
            this.centerEye.position.z
        );
    }
}