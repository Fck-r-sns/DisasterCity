using UnityEngine;
using UnityEngine.AI;

public class Tank : Unit
{

    [SerializeField]
    GameObject[] _explosionsPrefabs;



 


    void Start()
    {



        GameControl.instance.RegisterUnit(this);
        _monster = GameControl.instance.GetMonster();
        _shootingTarget = _monster.GetChest();


    }





    void Update()
    {



    }




}
