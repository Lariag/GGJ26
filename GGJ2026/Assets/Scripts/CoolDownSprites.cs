using System;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownSprites : MonoBehaviour
{
    
    [SerializeField]
    Image imagenSalto, imagenExcavar, imagenPequenio, imagenVolar, imagenAtacar;
    
    
    void Update()
    {
        if (Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Jump)>0)
        {
            ControlCoolDown(Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Jump), imagenSalto);
        }
        else if (imagenSalto.fillAmount != 0)
        {
            imagenSalto.fillAmount = 0;
        }

        if (Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Fly) > 0)
        {
            ControlCoolDown(Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Fly), imagenVolar);
        }
        else if (imagenVolar.fillAmount != 0)
        {
            imagenVolar.fillAmount = 0;
        }

        if (Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Dig) > 0)
        {
            ControlCoolDown(Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Dig), imagenExcavar);
        }
        else if (imagenExcavar.fillAmount != 0)
        {
            imagenExcavar.fillAmount = 0;
        }

        if (Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Mini) > 0)
        {
            ControlCoolDown(Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Mini), imagenPequenio);
        }
        else if (imagenPequenio.fillAmount != 0)
        {
            imagenPequenio.fillAmount = 0;
        }

        if (Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Harm) > 0)
        {
            ControlCoolDown(Managers.Ins.Cooldown.GetProgressCooldown(Enums.MaskType.Harm), imagenAtacar);
        }
        else if (imagenAtacar.fillAmount != 0)
        {
            imagenAtacar.fillAmount = 0;
        }
    }
    public void ControlCoolDown(float relleno,Image img)
    {
        img.fillAmount = relleno;
    }
    
    
}
    
