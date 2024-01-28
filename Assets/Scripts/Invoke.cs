using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    public static void Invoke(this MonoBehaviour mb, System.Action f, float delay) {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay) {
        yield return new WaitForSeconds(delay);
        f();
    }
    public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(b.x - a.x, b.y - a.y) * Mathf.Rad2Deg;
    }
}