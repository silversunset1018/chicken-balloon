using UnityEngine;
using System.Collections;


namespace Fresvii.AppSteroid.Gui
{
    public class ActivityIndicator
    {
		private static Fresvii.AppSteroid.Gui.LoadingSpinner loadingSpinner;

		public static void Show(MonoBehaviour mono){

			mono.StartCoroutine(Load());
		}

		private static IEnumerator Load()
		{
			#if UNITY_IOS
#if UNITY_5
            Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.White);
#else
           	Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.White);
#endif

#elif UNITY_ANDROID
			Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif

            Handheld.StartActivityIndicator();

			yield return new WaitForSeconds(0);

		}

		public static void Hide(){

			Handheld.StopActivityIndicator();

		}
	}
}