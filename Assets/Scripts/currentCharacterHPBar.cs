using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentCharacterHPBar : MonoBehaviour
{
    [SerializeField] int maxHp;
    [SerializeField] int hp; 

    [SerializeField] int maxEnergy;
    [SerializeField] int energy; 

    public GameObject damageBar;
    Vector3 originalDamagePos;
    float originalDamageLength;

    public GameObject usedEnergyBar;
    Vector3 originalEnergyPos;
    float originalEnergyLength;

    // Start is called before the first frame update
    void Start()
    {
        originalDamagePos = damageBar.transform.localPosition;
        originalDamageLength = damageBar.GetComponent<SpriteRenderer>().bounds.size.x;

        originalEnergyPos = usedEnergyBar.transform.localPosition;
        originalEnergyLength = usedEnergyBar.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void getMaxHP(){
        maxHp = GameObject.Find("Main Camera").GetComponent<Player_Select_Script>().Character1.GetComponent<Unit_Script>().maxHp;
    }

    void getHP(){
        hp = GameObject.Find("Main Camera").GetComponent<Player_Select_Script>().Character1.GetComponent<Unit_Script>().hp;
    }
    
    void updateHPBar(){
        damageBar.transform.localScale = new Vector3(((float)maxHp - (float)hp)/(float)maxHp, 1, 1);
        damageBar.transform.localPosition = originalDamagePos;
        damageBar.transform.localPosition += new Vector3(((originalDamageLength * (float)hp/(float)maxHp)/2), 0, 0);
    }

    void getMaxEnergy(){
        maxEnergy = GameObject.Find("Main Camera").GetComponent<Player_Select_Script>().Character1.GetComponent<Player_Script>().maxEnergy;
    }

    void getEnergy(){
        energy = GameObject.Find("Main Camera").GetComponent<Player_Select_Script>().Character1.GetComponent<Player_Script>().energy;
    }
    
    void updateEnergyBar(){
        usedEnergyBar.transform.localScale = new Vector3(((float)maxEnergy - (float)energy)/(float)maxEnergy, 1, 1);
        usedEnergyBar.transform.localPosition = originalEnergyPos;
        usedEnergyBar.transform.localPosition += new Vector3(((originalEnergyLength * (float)energy/(float)maxEnergy)/2), 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("Main Camera").GetComponent<Player_Select_Script>().Character1){
            getMaxHP();
            getHP();
            updateHPBar();

            getMaxEnergy();
            getEnergy();
            updateEnergyBar();
        }

    }
}
