using UnityEngine;
using UnityEngine.UI;

public class ProcessingBar_FillControl : MonoBehaviour
{
    public Image Bar;
    public double Fill;

    // Start is called before the first frame update
    void Start()
    {
        Fill = 1.0;
    }

    // Update is called once per frame
    void Update()
    {
        Bar.fillAmount = (float)Fill;
    }
}
