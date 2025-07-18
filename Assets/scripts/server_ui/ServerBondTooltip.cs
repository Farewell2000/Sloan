using UnityEngine;
using UnityEngine.UI;

namespace chARpack
{
    public class ServerBondTooltip : ServerTooltip
    {
        public Button collapse_button;
        public Button deleteButton;
        public Button modifyButton;
        public Button orderButton1;
        public Button orderButton2;
        public Button orderButton3;
        public Bond linkedBond;


        public override void Start()

        {
            base.Start();
            collapse_button.onClick.AddListener(delegate { resize(); });
        }


        public void resize()
        {
            if (isSmall)
            {
                isSmall = false;
                deleteButton.gameObject.SetActive(true);
                modifyButton.gameObject.SetActive(true);
                if (orderButton1 != null) orderButton1.gameObject.SetActive(true);
                if (orderButton2 != null) orderButton2.gameObject.SetActive(true);
                if (orderButton3 != null) orderButton3.gameObject.SetActive(true);
                infobox.SetActive(true);
                rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y - 160);
            }
            else
            {
                isSmall = true;
                deleteButton.gameObject.SetActive(false);
                modifyButton.gameObject.SetActive(false);
                if (orderButton1 != null) orderButton1.gameObject.SetActive(false);
                if (orderButton2 != null) orderButton2.gameObject.SetActive(false);
                if (orderButton3 != null) orderButton3.gameObject.SetActive(false);
                infobox.SetActive(false);
                rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y + 160);
            }
        }
    }
}
