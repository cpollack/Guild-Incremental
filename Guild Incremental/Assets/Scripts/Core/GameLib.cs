using System.Collections;

namespace UnityEngine.UI
{
    public class GameLib
    {
        public static void RebuildLayout(MonoBehaviour monoBehaviour, GameObject obj)
        {
            /*foreach (Transform child in obj.transform)
            {
                RebuildLayout(child.gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(obj.GetComponent<RectTransform>());*/
            monoBehaviour.StartCoroutine(DoLayoutRebuild(obj));
        }

        private static IEnumerator DoLayoutRebuild(GameObject obj)
        {
            yield return new WaitForEndOfFrame();
            RebuildLayoutRecursive(obj);
        }

        private static void RebuildLayoutRecursive(GameObject obj)
        {
            foreach (Transform child in obj.transform)
            {
                RebuildLayoutRecursive(child.gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(obj.GetComponent<RectTransform>());
        }

        public static void HideIfClickedOutside(GameObject panel)
        {
            if (Input.GetMouseButton(0) && panel.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    panel.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    Camera.main))
            {
                panel.SetActive(false);
            }
        }
    }
}