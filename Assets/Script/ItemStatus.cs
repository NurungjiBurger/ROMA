using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStatus : Status
{
    enum Grade { Normal, Rare, Epic, Unique, Legendary };

    [SerializeField]
    private GameObject[] prefabAttack;
    [SerializeField]
    private string dedicatedOccupation;
    [SerializeField]
    private string mountingPart;
    [SerializeField]
    private Grade grade;
    [SerializeField]
    private GameObject effectBone;
    [SerializeField]
    private string attackType;
    [SerializeField]
    private int price;

    private int enhancingLevel;

    private GameObject inventory;
    private GameObject inspector;

    public int itemPrfNumber = -1;
    public int index = -1;

    private GameData data;
    private Data dataI;

    private GameObject room;
    private GameObject player;

    public Data Data { get { return dataI; } }
    public int CursedRate { get { return dataI.cursedRate; } set { dataI.cursedRate = value; } }
    public int ItemGrade { get { return (int)grade; } }
    public string MountingPart { get { return mountingPart; } }
    public bool IsMount { get { return dataI.isMount; } set { dataI.isMount = value; } }
    public string Occupation { get { return dedicatedOccupation; } }
    public GameObject EffectBone { get { return effectBone; } }
    public string AttackType { get { return attackType; } }
    public int Price { get { return price; } }

    public void DestoryAll()
    {
        data.datas.Remove(dataI);
        for (int idx = index; idx < data.datas.Count; idx++)
        {
            data.datas[index].index = idx;
        }
        Destroy(gameObject);
    }

    private void CurseApply()
    {
        switch(MountingPart)
        {
            case "Head":
               // Debug.Log("체력 감소" + (1 - ((float)dataI.cursedRate / 100)).ToString());
                dataI.maxHP = (int)((1.0f - ((float)dataI.cursedRate / 100)) * (float)dataI.maxHP);
                break;
            case "Hand":
              //  Debug.Log("공격속도 감소" + (1 - ((float)dataI.cursedRate / 100)).ToString());
                dataI.attackSpeed = (1.0f - ((float)dataI.cursedRate / 100)) * dataI.attackSpeed;
                break;
            case "Foot":
              //  Debug.Log("이동속도 감소" + (1 - ((float)dataI.cursedRate / 100)).ToString());
                dataI.moveSpeed = (1.0f - ((float)dataI.cursedRate / 100)) * dataI.moveSpeed;
                break;
            case "Body":
              //  Debug.Log("방어력 감소" + (1 - ((float)dataI.cursedRate / 100)).ToString());
                dataI.defenseRate = (1.0f - ((float)dataI.cursedRate / 100)) * dataI.defenseRate;
                break;
            case "Weapon":
              //  Debug.Log("공격력 감소" + (1 - ((float)dataI.cursedRate / 100)).ToString());
                dataI.power = (int)((1.0f - ((float)dataI.cursedRate / 100)) * (float)dataI.power);
                break;
            default:
                break;//
        }
    }

    public void StatUP(bool enhance)
    {

        player.GetComponent<PlayerStatus>().CalCulateStat(gameObject, -1);

        if (!enhance)
        {
            dataI.maxHP = (int)(dataI.maxHP * 1.5f);
            dataI.power = (int)(dataI.power * 1.3f);
            dataI.jumpPower = dataI.jumpPower * 1.2f;
            dataI.moveSpeed = dataI.moveSpeed * 1.2f;
            dataI.attackSpeed = dataI.attackSpeed * 1.1f;
            dataI.defenseRate = dataI.defenseRate * 1.2f;
        }
        else
        {
            int delta;

            if (Data.enhancingLevel <= 10)
            {
                delta = 70;
            }
            else if (Data.enhancingLevel > 10 && Data.enhancingLevel <= 30)
            {
                delta = 50;
            }
            else if (Data.enhancingLevel > 30 && Data.enhancingLevel <= 60)
            {
                delta = 30;
            }
            else
            {
                delta = 10;
            }

            switch (MountingPart)
            {
                case "Head":
                    dataI.maxHP += (maxHP / delta);
                    dataI.defenseRate += (defenseRate / delta);
                    break;
                case "Hand":
                    dataI.maxHP += (maxHP / delta);
                    dataI.power += (power / delta);
                    dataI.attackSpeed += (attackSpeed / 50);
                    dataI.defenseRate += (defenseRate / delta);
                    break;
                case "Foot":
                    dataI.defenseRate += (defenseRate / delta);
                    dataI.moveSpeed += (moveSpeed / delta);
                    break;
                case "Body":
                    dataI.maxHP += (maxHP / delta);
                    dataI.defenseRate += (defenseRate / delta);
                    break;
                case "Weapon":
                    dataI.power += (power / delta);
                    dataI.attackSpeed += (attackSpeed / 50);
                    break;
                default:
                    break;
            }

            if ((dataI.cursedRate -= 1) < 0) dataI.cursedRate = 0;
            dataI.enhancingLevel++;
        }

        player.GetComponent<PlayerStatus>().CalCulateStat(gameObject, 1);

    }

    private int OccupationCheck()
    {
        if (player.GetComponent<PlayerStatus>().Occupation == dedicatedOccupation) return 1;
        else return 0;
    }
    public GameObject GetAttackAnimation()
    {
        return prefabAttack[OccupationCheck()];
    }

    private void FindRoom()
    {
        for (int idx = 0; idx < GameObject.Find("GameController").GetComponent<GameController>().Room.Count; idx++)
        {
            room = GameObject.Find("GameController").GetComponent<GameController>().Room[idx];

            if (transform.position.x <= room.transform.position.x + 11.0f && transform.position.x >= room.transform.position.x - 11.0f)
            {
                if (transform.position.y <= room.transform.position.y + 7.5f && transform.position.y >= room.transform.position.y - 7.5f)
                {
                    break;
                }
            }
        }
    }

    private void Awake()
    {
        data = GameObject.Find("Data").GetComponent<DataController>().GameData;
    }

    private void Start()
    {
        inventory = GameObject.Find("Canvas").transform.Find("Inventory").gameObject;
        inspector = GameObject.Find("Canvas").transform.Find("Inspector").gameObject;

        player = GameObject.FindGameObjectWithTag("Player");
        if (index == -1)
        {
            int[] arr = new int[2];
            float[] arr2 = new float[7];
            arr[0] = maxHP;
            arr2[0] = defenseRate; arr2[1] = jumpPower; arr2[2] = moveSpeed; arr2[3] = attackSpeed; arr2[4] = bloodAbsorptionRate; arr2[5] = evasionRate; arr2[6] = power;
            index = data.datas.Count;

            data.datas.Add(new Data("Item", itemPrfNumber, index, arr, arr2, -1, -1, false));
            dataI = data.datas[index];

            dataI.cursedRate = Random.Range(0, 50); // 저주율 수치 조정 필요
            dataI.enhancingLevel = 0;
            dataI.isMount = false;
            dataI.SetPosition(transform.position);

            CurseApply();

            if (player.GetComponent<PlayerStatus>().Occupation == dedicatedOccupation) StatUP(false);
        }
        else
        {
            dataI = data.datas[index];
            CursedRate = dataI.cursedRate;


            if (dataI.isAcquired)
            {
                if (dataI.isMount) inspector.GetComponent<Inspector>().AddToInspector(GameObject.Find("GameController").GetComponent<GameController>().CreateItemSlot(gameObject));
                else inventory.GetComponent<Inventory>().AddToInventory(GameObject.Find("GameController").GetComponent<GameController>().CreateItemSlot(gameObject));
            }
        }

        // 방 찾기
        FindRoom();
    }

    private void Update()
    {
        //if (GameObject.Find("GameController").GetComponent<GameController>().GoNext) Destroy(gameObject);

        if (dataI == null) dataI = data.datas[index];
        else dataI.SetPosition(transform.position);

        /*
        Debug.Log(dataI.isAcquired + " " + dataI.isMount + " ");
        if (dataI.isAcquired)
        {
            if (dataI.isMount)
            {
                Debug.Log("보여야하는데?");
                gameObject.SetActive(true);
            }
            else gameObject.SetActive(false);
        }
        */
        //if (!room) FindRoom();
        //else
        {
           // Debug.Log(room);
            //Debug.Log(dataI);
            //if (!room.GetComponent<Room>().isPlayer && !dataI.isAcquired) DestoryAll();
        }
    }
}
