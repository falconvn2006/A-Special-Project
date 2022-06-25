using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    public float damage = 999f;
    public float speedAnim = 2f;

    public float x_pos = -0.66f;
    public float y_pos;
    public float z_pos;

    bool isAttacking = false;    

    // Start is called before the first frame update
    void Start()
    {
        if(x_pos == 0f){
            x_pos = transform.localPosition.x;
        }

        if(y_pos == 0f){
            y_pos = transform.localPosition.y;
        }

        if(z_pos == 0f){
            z_pos = transform.localPosition.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttacking){
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(x_pos, y_pos, z_pos), Time.deltaTime * speedAnim);
        }
    }

    public void AttackRef(){
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
}
