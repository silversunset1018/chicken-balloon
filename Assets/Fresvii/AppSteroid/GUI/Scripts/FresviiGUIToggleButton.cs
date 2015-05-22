using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
[RequireComponent (typeof(Button))]
public class FresviiGUIToggleButton : MonoBehaviour {

    public bool initIsOn;

    private bool isOn;

    public bool IsOn
    {
        get { return isOn; }

        set
        {
            isOn = value;

            image.sprite = (isOn) ? onSprite : offSprite;
        }
    }

    public Sprite onSprite, offSprite;

    private Image image;

	public Button button;

	// Use this for initialization
	void Start () 
    {
        image = GetComponent<Image>();

		button = GetComponent<Button>();

        IsOn = initIsOn;
	}
	
}
