using UnityEngine;
using System.Collections;

public class MainMenuCamera {
    public static bool camIsMoving = false;
    public static Vector3 MainMenuCamPos = new Vector3(-378, 34, -506);
    public static Quaternion MainMenuCamRot = Quaternion.Euler(15.47f, 351.19f, 359.9f);

    public static Vector3 PlayerCreationCamPos = new Vector3(-339, 28, -421);
    public static Quaternion PlayerCreationCamRot = Quaternion.Euler(85, 272, 360);

    public static Vector3 ControlsCamPos = new Vector3(-436, 22, -435);
    public static Quaternion ControlsCamRot = Quaternion.Euler(10, 478, 360);

    public static Vector3 ControlsMidCamPos = new Vector3(-437, 26, -480);
    public static Quaternion ControlsMidCamRot = Quaternion.Euler(14, 406, 360);

    public static Vector3 SettingsCamPos = new Vector3(-380, 19, -267);
    public static Quaternion SettingsCamRot = Quaternion.Euler(6, 180, 360);

    public static Vector3 OTableCamPos = new Vector3(-292, 86, -298);
    public static Quaternion OTableCamRot = Quaternion.Euler(33, 232, 360);

    public static Vector3 MiniCamPos = new Vector3(-366, 13.5f, -409);
    public static Quaternion MiniCamRot = Quaternion.Euler(11, 298, 360);

    public static class CoroutineUtil {
        public static IEnumerator WaitForRealSecs(float time) {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + time) {
                yield return null;
            }//while
        }//WaitForRealSecs

        public static IEnumerator MoveCamera(Transform thisTransform, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float time) {
            camIsMoving = true;
            float i = 0;
            float rate = 1 / time;

            while (i < 1) {
                i += Time.deltaTime * rate;
                thisTransform.position = Vector3.Lerp(startPos, endPos, i);
                thisTransform.rotation = Quaternion.Slerp(startRot, endRot, i);
                yield return 0;
            }//while

            camIsMoving = false;
        }//MoveCamera

        public static IEnumerator MoveCameraTwice(Transform thisTransform, Vector3 startPos, Vector3 midPos, Vector3 endPos,
                                                  Quaternion startRot, Quaternion midRot, Quaternion endRot, float time) {
            camIsMoving = true;
            float i = 0;
            float j = 0;
            float rate = 1 / time;

            while (i < 1) {
                i += Time.deltaTime * rate;
                thisTransform.position = Vector3.Lerp(startPos, midPos, i);
                thisTransform.rotation = Quaternion.Slerp(startRot, midRot, i);
                yield return 0;
            }//while

            while (j < 1) {
                j += Time.deltaTime * rate;
                thisTransform.position = Vector3.Lerp(midPos, endPos, j);
                thisTransform.rotation = Quaternion.Slerp(midRot, endRot, j);
                yield return 0;
            }//while
            camIsMoving = false;
        }//MoveCameraTwice
    }//CoroutineUtil
}

