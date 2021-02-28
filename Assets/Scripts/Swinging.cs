using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Swinging : MonoBehaviour
{
    public float swingSensitivity = 100.0f;


    void FixedUpdate()
    {
        if(GetComponent<Rigidbody2D>() == null)
        {
            Destroy(this);
            return;
        }

        //Получить величну из наклона
        float swing = InputManager.Instance.sidewaysMotion;

        //Вычислить прилагаемую силу
        Vector2 force = new Vector2(swing * swingSensitivity, 0);

        //Приложить силу
        GetComponent<Rigidbody2D>().AddForce(force);
    }
}
