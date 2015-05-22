using UnityEngine;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class LoadingSpinner : MonoBehaviour
    {
        public Texture2D textureSpinner;

        public float rotationSpeedDeg = 30.0f;

		public Rect Position {get; set;}

        private float angle;

        private Vector2 pivot;

        public float tickDuration = 0.25f;

        private int guiDepth;

        public static Fresvii.AppSteroid.Gui.LoadingSpinner Show(Rect position, int guiDepth)
        {
            Fresvii.AppSteroid.Gui.LoadingSpinner instance = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/LoadingSpinner"))).GetComponent<Fresvii.AppSteroid.Gui.LoadingSpinner>();

            instance.guiDepth = guiDepth;
			instance.Position = position;
            instance.pivot = new Vector2(position.x + 0.5f * position.width, position.y + 0.5f * position.height);
            instance.StartAnimation(instance);

            return instance;
        }

        public static Fresvii.AppSteroid.Gui.LoadingSpinner Show(Rect position)
        {
            Fresvii.AppSteroid.Gui.LoadingSpinner instance = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/LoadingSpinner"))).GetComponent<Fresvii.AppSteroid.Gui.LoadingSpinner>();

            instance.Position = position;
            instance.pivot = new Vector2(position.x + 0.5f * position.width, position.y + 0.5f * position.height);

            instance.StartAnimation(instance);

            return instance;
        }

        public static void Show(Rect position, float rotationSpeedDeg, float tickDuration)
        {
            Fresvii.AppSteroid.Gui.LoadingSpinner instance = ((GameObject)Instantiate(Resources.Load("GuiPrefabs/LoadingSpinner"))).GetComponent<Fresvii.AppSteroid.Gui.LoadingSpinner>();

            instance.pivot = new Vector2(position.x + 0.5f * position.width, position.y + 0.5f * position.height);
			instance.Position = position;
            instance.rotationSpeedDeg = rotationSpeedDeg;
            instance.tickDuration = tickDuration;

            instance.StartAnimation(instance);
        }

        public void Hide()
        {
            if(this != null)
                Destroy(this.gameObject);
        }

        private void StartAnimation(MonoBehaviour mono)
        {
            mono.StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            while (true)
            {
                if (tickDuration > 0)
                    angle += rotationSpeedDeg;
                else
                    angle += rotationSpeedDeg * Time.deltaTime;

                yield return new WaitForSeconds(tickDuration);
			}
        }

        void OnGUI()
        {
            GUI.depth = guiDepth;

            Matrix4x4 matrixBackup = GUI.matrix;

			pivot = new Vector2(Position.x + 0.5f * Position.width, Position.y + 0.5f * Position.height);

            GUIUtility.RotateAroundPivot(angle, pivot);

			GUI.DrawTexture(Position, textureSpinner);

            GUI.matrix = matrixBackup;
        }

    }
}