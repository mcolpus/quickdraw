using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana : MeleeWeapon {
    public bool flipSwordOnReturn;
    public float delayBeforeReturn;

    protected override IEnumerator Swing()
    {
        MyAudio.current.PlayClip(audioClipOnSwing);

        LastSwingFired = Time.time + 100f;

        damageArea.SetActive(true);
        float startTime = Time.time;

        Quaternion targetRot = Quaternion.Euler(swingRotation);
        while(Time.time - startTime < swingTime / 2f)
        {
            holdTransform.localRotation = Quaternion.Lerp(holdTransform.localRotation, targetRot, swingSpeed * Time.deltaTime);
            yield return null;
        }
        

        targetRot = Quaternion.Euler(defaultRotation);
        if(flipSwordOnReturn)
        {
            //create new target rotation
            holdTransform.localRotation = targetRot;
            holdTransform.Rotate(Vector3.up,180,Space.Self);
            targetRot = holdTransform.localRotation;

            //now return to previous rotation
            holdTransform.localRotation = Quaternion.Euler(swingRotation);
            holdTransform.Rotate(Vector3.up, 180, Space.Self);

            yield return new WaitForSeconds(delayBeforeReturn);
        }

        startTime = Time.time;
        while(Time.time - startTime < swingTime / 2f)
        {
            holdTransform.localRotation = Quaternion.Lerp(holdTransform.localRotation, targetRot, swingReturnSpeed * Time.deltaTime);
            yield return null;
        }
        damageArea.SetActive(false);
        holdTransform.localRotation = Quaternion.Euler(defaultRotation);

        LastSwingFired = Time.time;
    }
}
