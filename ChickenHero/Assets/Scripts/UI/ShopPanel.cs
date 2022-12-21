using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopPanel : MonoBehaviour
{
    public TMP_Text powerTxt;

    public RandomSelect randomSelectObj;
    public void PowerUp()
    {
        randomSelectObj.RandomCardOpen();
    }
}
