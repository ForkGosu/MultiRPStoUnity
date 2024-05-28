using UnityEngine;
using UnityEngine.UI;


public class LoadingScreenBase : MonoBehaviour
{
    [SerializeField]
    private Image m_loadingCircleImage;
    float m_fillAmount = 0.8f;
    float m_fillAdd = 0.01f;

    Vector3 m_scaleZeroX = new Vector3(0, 1, 1);

    void FixedUpdate()
    {
        m_fillAmount += m_fillAdd;
        m_loadingCircleImage.fillAmount = m_fillAmount;
        m_loadingCircleImage.transform.Rotate(Vector3.back);

        if (m_fillAmount <= 0f || m_fillAmount >= 1f)
        {
            m_fillAdd = -m_fillAdd;
            m_loadingCircleImage.transform.localScale = m_scaleZeroX + (Vector3.left * m_loadingCircleImage.transform.localScale.x);
        }
    }
}