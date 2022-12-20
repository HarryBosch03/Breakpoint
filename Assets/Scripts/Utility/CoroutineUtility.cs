using System.Collections;
using UnityEngine;

public static class CoroutineUtility
{
    public static IEnumerator Wait (float secconds)
    {
        yield return new WaitForSeconds (secconds);
    }
}
