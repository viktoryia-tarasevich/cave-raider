using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    //Величина смещения. -1.0 = максимально влево, 1 = максимально вправо
    private float _sidewaysMotion = 0.0f;

    public float sidewaysMotion
    {
        get
        {
            return _sidewaysMotion;
        }
    }

    void Update()
    {
        Vector3 accel = Input.acceleration;
        //Сохраняем величину отклонения в каждом кадре
        _sidewaysMotion = accel.x;
    }
}
