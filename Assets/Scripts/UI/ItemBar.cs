using UnityEngine;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour
{
    [SerializeField]
    private GameObject itemBar;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private Color itemInactive;
    [SerializeField]
    private Color itemActive;

    [SerializeField]
    private Brush brush;

    [SerializeField]
    private float itemPadding = 5;

    private GameObject[] items;

    public void InitBar()
    {
        Sprite[] icons = brush.GetIcons();
        items = new GameObject[icons.Length];

        RectTransform rect = itemPrefab.GetComponent<RectTransform>();
        float itemSize = rect.rect.width;

        float startX = -(icons.Length - 1) * ((itemSize + itemPadding) / 2);

        for (int i = 0; i < icons.Length; i++)
        {
            GameObject item = Instantiate(itemPrefab, new Vector3(startX + i * (itemSize + itemPadding), 0, 0), itemPrefab.transform.rotation);
            GameObject child = item.transform.GetChild(0).gameObject;

            child.GetComponent<Image>().sprite = icons[i];

            int index = i;
            item.GetComponent<Button>().onClick.AddListener(() => onItemClicked(index));
            item.transform.SetParent(itemBar.transform, false);

            items[i] = item;
        }

        UpdateBar();
    }

    public void UpdateBar()
    {
        int index = brush.GetSelectedPlant();
        for (int i = 0; i < items.Length; i++)
        {
            if (i == index)
            {
                items[i].GetComponent<Image>().color = itemActive;
            }
            else
            {
                items[i].GetComponent<Image>().color = itemInactive;
            }
        }
    }

    private void onItemClicked(int index)
    {
        brush.SelectPlant(index);
        UpdateBar();
    }
}
