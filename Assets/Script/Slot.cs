using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabUI;

    private GameObject ui;
    private GameObject slotItem;

    private Sprite itemImage;

    private GameObject player;
    private GameObject inventory;
    private GameObject inspector;

    public GameObject SlotItem { get { return slotItem; } }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void CompareMountItem()
    {
        if (transform.parent.gameObject == inventory)
        {
            if (inspector.transform.Find(slotItem.GetComponent<ItemStatus>().MountingPart).childCount == 1)
            {
                if (inspector.transform.Find(slotItem.GetComponent<ItemStatus>().MountingPart).transform.Find("Slot(Clone)").GetComponent<Slot>().SlotItem.GetComponent<ItemStatus>().Grade > slotItem.GetComponent<ItemStatus>().Grade)
                {
                    if (!ui) ui = Instantiate(prefabUI, transform);
                }
                else
                {
                    if (ui) Destroy(ui.gameObject);
                }
            }
        }
        else
        {
            if (ui) Destroy(ui.gameObject);
        }
    }

    public void Discard()
    {
        Debug.Log("������");
    }
     
    public void Mounting()
    {
        Debug.Log("mount");
        inventory.GetComponent<Inventory>().DiscardItem(transform.GetSiblingIndex());
        transform.SetParent(inspector.transform.Find(slotItem.GetComponent<ItemStatus>().MountingPart).transform);
        transform.position = transform.parent.position;
        GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        transform.Find("Background").GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
        player.GetComponent<PlayerStatus>().CalCulateStat(slotItem, 1);

        if (inspector.transform.Find(slotItem.GetComponent<ItemStatus>().MountingPart).childCount == 2)
        {
            Debug.Log("��ȯ");
            GameObject tmp;
            tmp = inspector.transform.Find(slotItem.GetComponent<ItemStatus>().MountingPart).GetChild(1).gameObject;
            tmp.transform.SetParent(inventory.transform);
            tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(36, 36);
            transform.Find("Background").GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
            player.GetComponent<PlayerStatus>().CalCulateStat(tmp.GetComponent<Slot>().SlotItem, -1);
            inventory.GetComponent<Inventory>().MoveItem(tmp.GetComponent<Slot>().SlotItem);
        }
    }

    public void DisMounting()
    {
        Debug.Log("dismount");
        transform.SetParent(inventory.transform);
        GetComponent<RectTransform>().sizeDelta = new Vector2(36, 36);
        transform.Find("Background").GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
        player.GetComponent<PlayerStatus>().CalCulateStat(slotItem, -1);
        inventory.GetComponent<Inventory>().MoveItem(slotItem);
    }


    public void InsertImage(GameObject item)
    {
        slotItem = item;
        switch(slotItem.GetComponent<ItemStatus>().Grade)
        {
            case 0:
                // ��� �Ͼ�
                transform.Find("Background").GetComponent<Image>().color = new Color(255/255f, 255/255f, 255/255f, 0.5f);
                break;
            case 1:
                // ���� �ϴ�
                transform.Find("Background").GetComponent<Image>().color = new Color(23 / 255f, 224 / 255f, 224 / 255f, 0.5f);
                break;
            case 2:
                // ���� ����
                transform.Find("Background").GetComponent<Image>().color = new Color(159 / 255f, 14 / 255f, 224 / 255f, 0.5f);
                break;
            case 3:
                // ����ũ ���
                transform.Find("Background").GetComponent<Image>().color = new Color(233 / 255f, 241 / 255f, 35 / 255f, 0.5f);
                break;
            case 4:
                // �������� �ʷ�
                transform.Find("Background").GetComponent<Image>().color = new Color(96 / 255f, 236 / 255f, 39 / 255f, 0.5f);
                break;
        }
        transform.Find("Background").transform.Find("Item").GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        if (!inventory) inventory = GameObject.Find("Canvas").transform.Find("Inventory(Clone)").transform.Find("InventoryBackground").gameObject;
        if (!inspector) inspector = GameObject.Find("Canvas").transform.Find("Inspector(Clone)").transform.Find("Background").gameObject;
        CompareMountItem();
    }
}