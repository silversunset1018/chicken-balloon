using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;


namespace Fresvii.AppSteroid.Gui
{
    public class FresviiAndroidManifestGenerator
    {
        public static void GenerateManifest()
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

            // only copy over a fresh copy of the AndroidManifest if one does not exist
            if (File.Exists(outputFile))
            {
                if (EditorUtility.DisplayDialog("Generate Manifest", "AndroidManifest already exists. Replace it?", "OK", "Cancel"))
                {
                    int fileCount = 0;

                    string rename = "";

                    do
                    {
                        fileCount++;

                        rename = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest_old_" + fileCount + ".xml");
                    }
                    while (File.Exists(rename));

                    File.Move(outputFile, rename); 
                }
                else
                {
                    Debug.LogError("AndroidManifest already exists.");

                    return;
                }
            }
            var inputFile = Path.Combine(Application.dataPath, "Fresvii/Editor/AndroidManifestDefault.xml");

            Debug.Log("path : " + inputFile);

            File.Copy(inputFile, outputFile);

            UpdateManifest(outputFile);

            EditorUtility.DisplayDialog("Generate Manifest", "Manifest file was generated.", "OK");
        }

        public static void UpdateManifest(string fullPath)
        {
            FASSettings fasSetting = FASSettings.Settings;

            XmlDocument doc = new XmlDocument();
            
            doc.Load(fullPath);

            if (doc == null)
            {
                Debug.LogError("Couldn't load " + fullPath);
 
                return;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");

            if (manNode.Attributes["package"] != null)
            {
                manNode.Attributes["package"].Value = PlayerSettings.bundleIdentifier;
            }
            else
            {
                XmlAttribute packageAttr = doc.CreateAttribute("package");

                packageAttr.Value = PlayerSettings.bundleIdentifier;

                manNode.Attributes.Append(packageAttr);
            }

            XmlNode appNode = FindChildNode(manNode, "application");

            if (appNode == null)
            {
                Debug.LogError("Error parsing " + fullPath);

                return;
            }

            string ns = appNode.GetNamespaceOfPrefix("android");

            XmlElement p7 = doc.CreateElement("uses-permission");
            p7.SetAttribute("name", ns, "android.permission.INTERNET");
            manNode.AppendChild(p7);

            XmlElement p8 = doc.CreateElement("uses-permission");
            p8.SetAttribute("name", ns, "android.permission.ACCESS_NETWORK_STATE");
            manNode.AppendChild(p8);

            XmlElement p9 = doc.CreateElement("uses-permission");
            p9.SetAttribute("name", ns, "android.permission.WRITE_EXTERNAL_STORAGE");
            manNode.AppendChild(p9);

            XmlElement p10 = doc.CreateElement("uses-permission");
            p10.SetAttribute("name", ns, "android.permission.READ_EXTERNAL_STORAGE");
            manNode.AppendChild(p10);

            XmlElement pDoc = doc.CreateElement("uses-permission");
            pDoc.SetAttribute("name", ns, "android.permission.MANAGE_DOCUMENTS");
            manNode.AppendChild(pDoc);
            
            #region pushnotification
            if (fasSetting.pushNotification)
            {
                XmlElement serviceElement = doc.CreateElement("service");
                serviceElement.SetAttribute("name", ns, "com.fresvii.sdk.unity.GCMIntentService");
                appNode.AppendChild(serviceElement);

                XmlElement recieverElement = doc.CreateElement("receiver");
                recieverElement.SetAttribute("name", ns, "com.fresvii.sdk.unity.GCMReceiver");
                recieverElement.SetAttribute("permission", ns, "com.google.android.c2dm.permission.SEND");
                
                XmlElement intentFilterElement = doc.CreateElement("intent-filter");

                XmlElement action1 = doc.CreateElement("action");
                action1.SetAttribute("name", ns, "com.google.android.c2dm.intent.RECEIVE");

                XmlElement action2 = doc.CreateElement("action");
                action2.SetAttribute("name", ns, "com.google.android.c2dm.intent.REGISTRATION");

                XmlElement category = doc.CreateElement("category");
                category.SetAttribute("name", ns, PlayerSettings.bundleIdentifier);

                intentFilterElement.AppendChild(action1);

                intentFilterElement.AppendChild(action2);
                
                intentFilterElement.AppendChild(category);

                recieverElement.AppendChild(intentFilterElement);

                appNode.AppendChild(recieverElement);

                XmlNode activityNode = FindChildNode(appNode, "activity", "android:name", "com.unity3d.player.UnityPlayerNativeActivity");

                if (activityNode == null)
                {
                    activityNode = FindChildNode(appNode, "activity", "android:name", "com.unity3d.player.UnityPlayerActivity");

                    if (activityNode == null)
                    {
                        Debug.LogError("Error parsing " + fullPath);

                        return;
                    }
                }

                XmlElement intentFilterElement2 = doc.CreateElement("intent-filter");

                XmlElement action3 = doc.CreateElement("action");

                action3.SetAttribute("name", ns, PlayerSettings.bundleIdentifier + ".NotificationAction");

                XmlElement category2 = doc.CreateElement("category");
                
                category2.SetAttribute("name", ns, "android.intent.category.DEFAULT");

                intentFilterElement2.AppendChild(action3);
                
                intentFilterElement2.AppendChild(category2);

                activityNode.AppendChild(intentFilterElement2);

                if (activityNode.Attributes["android:launchMode"] != null)
                {
                    activityNode.Attributes["android:launchMode"].Value = "singleTask";
                }
                else
                {
                    XmlAttribute launchModeAttr = doc.CreateAttribute("android:launchMode");

                    launchModeAttr.Value = "singleTask";

                    activityNode.Attributes.Append(launchModeAttr);
                }

                // permissions
                XmlElement p1 = doc.CreateElement("permission");
                p1.SetAttribute("name", ns, PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE");
                p1.SetAttribute("protectionLevel", ns, "signature");
                manNode.AppendChild(p1);

                XmlElement p2 = doc.CreateElement("uses-permission");
                p2.SetAttribute("name", ns, PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE");
                manNode.AppendChild(p2);

                XmlElement p3 = doc.CreateElement("uses-permission");
                p3.SetAttribute("name", ns, "com.google.android.c2dm.permission.RECEIVE");
                manNode.AppendChild(p3);

                XmlElement p4 = doc.CreateElement("uses-permission");
                p4.SetAttribute("name", ns, "android.permission.WAKE_LOCK");
                manNode.AppendChild(p4);

                if (fasSetting.underAndroidApiLevel15)
                {
                    XmlElement p5 = doc.CreateElement("uses-permission");
                    p5.SetAttribute("name", ns, "android.permission.GET_ACCOUNTS");
                    manNode.AppendChild(p5);
                }

                if (fasSetting.pushVibration)
                {
                    XmlElement p6 = doc.CreateElement("uses-permission");
                    p6.SetAttribute("name", ns, "android.permission.VIBRATE");
                    manNode.AppendChild(p6);
                }
            }
            #endregion

            #region BackupManager
            if (!string.IsNullOrEmpty(fasSetting.backUpApiKey))
            {
                XmlElement metaElement = doc.CreateElement("meta-data");

                metaElement.SetAttribute("name", ns, "com.google.android.backup.api_key");
                
                metaElement.SetAttribute("value", ns, fasSetting.backUpApiKey);

                appNode.AppendChild(metaElement);

                if (appNode.Attributes["android:allowBackup"] != null)
                {
                    appNode.Attributes["android:allowBackup"].Value = "true";
                }
                else
                {
                    XmlAttribute allowBackUpAttr = doc.CreateAttribute("allowBackup", ns);
                
                    allowBackUpAttr.Value = "true";
                    
                    appNode.Attributes.Append(allowBackUpAttr);
                }

                if (appNode.Attributes["android:backupAgent"] != null)
                {
                    appNode.Attributes["android:backupAgent"].Value = "com.fresvii.sdk.unity.TheBackupAgent";
                }
                else
                {
                    XmlAttribute backUpAgentAttr = doc.CreateAttribute("backupAgent", ns);

                    backUpAgentAttr.Value = "com.fresvii.sdk.unity.TheBackupAgent";
                    
                    appNode.Attributes.Append(backUpAgentAttr);
                }
            }
            else
            {
                Debug.Log("Back up api key is null or empty.");
            }


            #endregion

            #region Acitivities

            XmlElement imagePickerActivityElement = doc.CreateElement("activity");

            imagePickerActivityElement.SetAttribute("name", ns, "com.fresvii.sdk.unity.ImagePickerActivity");

            appNode.AppendChild(imagePickerActivityElement);

            XmlElement videoPlayerActivityElement = doc.CreateElement("activity");

            videoPlayerActivityElement.SetAttribute("name", ns, "com.fresvii.sdk.unity.VideoPlayerActivity");

            videoPlayerActivityElement.SetAttribute("theme", ns, "@android:style/Theme.Holo.NoActionBar.Fullscreen");

            videoPlayerActivityElement.SetAttribute("configChanges", ns, "orientation|screenSize");

            appNode.AppendChild(videoPlayerActivityElement);

            #endregion

            #region Voice chat
            XmlElement p11 = doc.CreateElement("uses-permission");
            
            p11.SetAttribute("name", ns, "android.permission.RECORD_AUDIO");

            manNode.AppendChild(p11);
            #endregion

            doc.Save(fullPath);

			AssetDatabase.Refresh();
        }

        private static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;

            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        private static XmlNode FindChildNode(XmlNode parent, string name, string attr, string attrValue)
        {
            XmlNode curr = parent.FirstChild;

            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    if (curr.Attributes[attr] != null)
                    {
                        if (curr.Attributes[attr].Value == attrValue)
                            return curr;
                    }
                }

                curr = curr.NextSibling;
            }

            return null;
        }
        
    }
}