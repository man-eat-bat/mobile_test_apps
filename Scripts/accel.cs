
using UnityEngine;
using System.Globalization;


public class accel : MonoBehaviour
{
    // Start is called before the first frame update
    KalmanFilter kalmanX = new KalmanFilter(f: 1f, h: 1f, q: 2f, r: 15f); // задаем F, H, Q и R
    KalmanFilter kalmanY = new KalmanFilter(f: 1f, h: 1f, q: 2f, r: 15f); // задаем F, H, Q и R
    KalmanFilter kalmanZ = new KalmanFilter(f: 1f, h: 1f, q: 2f, r: 15f); // задаем F, H, Q и R
    public static bool est_potok = false;
    private static float last_update;
    private static Timer timer;
    private Gyroscope g;


    private void OnMouseDrag()
    {

        
    }
  
    private void OnMouseDown()
    {
        updateTime();
        est_potok = true; 
            
        
    }
    void Start()
    {
        timer = new Timer();
        updateTime();
        g = Input.gyro;
        if (!g.enabled)
        {
            Debug.Log("aviable\n");
            Input.gyro.enabled = true;
        }
        kalmanX.SetState(0, 0.1f); // Задаем начальные значение State и Covariance
        kalmanY.SetState(0, 0.1f); // Задаем начальные значение State и Covariance
        kalmanZ.SetState(0, 0.1f); // Задаем начальные значение State и Covariance
    }
   

    // Update is called once per frame
    static public void updateTime()
    {
        
        last_update = timer.Time;
    }
    void Update()
    {
        
        if (est_potok)
        {
            
            Quaternion quatr = g.attitude;
           // Quaternion inversequatr = Quaternion.Inverse(quatr);


            //Vector3 accelvec = g.userAcceleration;


            //inversequatr = inversequatr * accelvec;
            Vector3 accelglobal = quatr * g.userAcceleration;
            accelglobal.x = (float)System.Math.Round((double)accelglobal.x,2);
            accelglobal.y = (float)System.Math.Round((double)accelglobal.y, 2);
            accelglobal.z = (float)System.Math.Round((double)accelglobal.z, 2);
            kalmanX.Correct(accelglobal.x); accelglobal.x = kalmanX.State;
            kalmanY.Correct(accelglobal.y); accelglobal.y = kalmanY.State;
            kalmanZ.Correct(accelglobal.z); accelglobal.z = kalmanZ.State;

            Vector3 coord = inercial.Step(accelglobal, timer.Time - last_update);
            updateTime();
            if (!Input.gyro.enabled)
            {
                Debug.Log("aviable\n");
                Input.gyro.enabled = true;
            }
            string str = "  -{";
            str = str + "coord:{x: " + coord.x.ToString("f5", CultureInfo.InvariantCulture) + ", y: " + coord.y.ToString("f5", CultureInfo.InvariantCulture) + ", z: " + coord.z.ToString("f5", CultureInfo.InvariantCulture) + "} ";
            str = str + ", a:{x: " + accelglobal.x.ToString("f5", CultureInfo.InvariantCulture) + ", y: " + accelglobal.y.ToString("f5", CultureInfo.InvariantCulture) + ", z: " + accelglobal.z.ToString("f5", CultureInfo.InvariantCulture) + "}";
            str = str + ", q: {x: " + quatr.x.ToString("f5", CultureInfo.InvariantCulture) + ", y: " + quatr.y.ToString("f5", CultureInfo.InvariantCulture) + ", z: " + quatr.z.ToString("f5", CultureInfo.InvariantCulture) + ", w: " + quatr.w.ToString("f5", CultureInfo.InvariantCulture) + "}";
            str = str + ", t: " + last_update.ToString();
            str = str + "}\n";
            SocketClient.Send(str);
        }
    }
}
