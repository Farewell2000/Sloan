using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace chARpack
{
    public class BondParametersServer : MonoBehaviour
    {
        public bool isSmall = false;
        [HideInInspector] public RectTransform rect;
        public GameObject title;
        public GameObject textbox1;
        public GameObject textbox2;
        public TMP_Text topText;
        public TMP_InputField topInput;
        public TMP_Text bottomText;
        public Button saveButton;
        public Button orderButton1;
        public Button orderButton2;
        public Button orderButton3;
        private ForceField.BondTerm bt_;
        private ForceField.AngleTerm at_;
        private ForceField.TorsionTerm tt_;
        public ForceField.BondTerm bt { get => bt_; set { bt_ = value; initTextFieldsBT(); } }

        void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent("return")))
            {
                saveButton.GetComponent<Button>().onClick.Invoke();
            }
        }

        private void initTextFieldsBT()
        {
            topText.text += SettingsData.useAngstrom ? " (\u00C5)" : " (pm)";
            var text = SettingsData.useAngstrom ? (bt.eqDist * 0.01f).ToString() : bt.eqDist.ToString();
            topInput.text = text;
            bottomText.text = "Order";
            HighlightOrderButton(bt.order);
        }

        private void HighlightOrderButton(float order)
        {
            orderButton1.image.color = (order == 1) ? Color.yellow : Color.white;
            orderButton2.image.color = (order == 2) ? Color.yellow : Color.white;
            orderButton3.image.color = (order == 3) ? Color.yellow : Color.white;
        }

        public ForceField.AngleTerm at { get => at_; set { at_ = value; initTextFieldsAT(); } }

        private void initTextFieldsAT()
        {
            topText.text = "Equilibrium Angle";
            bottomText.text = "kAngle";
            topInput.text = at.eqAngle.ToString();
            bottomText.text = "kAngle";
        }

        public ForceField.TorsionTerm tt { get => tt_; set { tt_ = value; initTextFieldsTT(); } }

        private void initTextFieldsTT()
        {
            topText.text = "Equilibrium Angle";
            bottomText.text = "vk";
            topInput.text = tt.eqAngle.ToString();
            bottomText.text = "vk";
        }

        // Start is called before the first frame update
        void Start()
        {
            var canvas = UICanvas.Singleton.GetComponent<Canvas>();
            transform.SetParent(canvas.transform);
            rect = transform as RectTransform;
            this.transform.localScale = new Vector2(1.1f, 1.1f);
            Vector2 save = SpawnManager.Singleton.GetSpawnLocalPosition(rect);
            rect.position = save;
            var drag = title.gameObject.AddComponent<Draggable>();
            drag.target = transform;
            orderButton1.onClick.AddListener(() => SetOrder(1));
            orderButton2.onClick.AddListener(() => SetOrder(2));
            orderButton3.onClick.AddListener(() => SetOrder(3));
        }

        private void SetOrder(float order)
        {
            bt_.order = order;
            HighlightOrderButton(order);
        }

        public void changeBondParametersBT()
        {
            bt_.eqDist = SettingsData.useAngstrom ? float.Parse(topInput.text) * 100 : float.Parse(topInput.text);
        }

        /// <summary>
        /// Changes the bond parameters of an angle bond 
        /// according to the text input.
        /// </summary>
        public void changeBondParametersAT()
        {
            at_.eqAngle = float.Parse(topInput.text);
            at_.kAngle = float.Parse(bottomText.text);
        }

        /// <summary>
        /// Changes the bond parameters of a torsion bond 
        /// according to the text input.
        /// </summary>
        public void changeBondParametersTT()
        {
            tt_.eqAngle = float.Parse(topInput.text);
            tt_.vk = float.Parse(bottomText.text);
        }
        public void closeThis()
        {
            Destroy(this.gameObject);
        }



        // Update is called once per frame
        public void resize()
        {
            if (isSmall)
            {
                isSmall = false;
                saveButton.gameObject.SetActive(true);
                textbox1.SetActive(true);
                textbox2.SetActive(true);
                rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y - 130);
            }
            else
            {
                isSmall = true;
                saveButton.gameObject.SetActive(false);
                textbox1.SetActive(false);
                textbox2.SetActive(false);
                rect.offsetMin = new Vector2(rect.offsetMin.x, rect.offsetMin.y + 130);
            }
        }
    }
}