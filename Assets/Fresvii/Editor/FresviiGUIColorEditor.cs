#pragma warning disable 0414

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{

    public class FresviiGUIColorEditor : EditorWindow
    {        
        private bool initialized = false;

        Color background;
        
        Color navigationBarBackground;
        Color navigationBarUnderLine;
        Color navigationBarTitleColor;
        Color navigationBarNormalColor;
        Color navigationBarNegativeColor;
        Color navigationBarPositiveColor;

        Color tabBarBackground;
        Color tabBarTopLine;
        Color tabBarNegativeColor;
        Color tabBarPositiveColor;
        Color tabBarBadgeBgColor;
        Color tabBarBadgeTextColor;

        Color textfieldBackground;
        Color textfieldTopLine;
        Color textfieldText;
        Color textfieldSendTextUnable;
        Color textfieldSendTextEnable;
        Color textfieldSendTextTapped;

        Color cardBackground;
        Color cardUserName;
        Color cardText1;
        Color cardText2;
        Color cardSepearateLine;
        Color cardAddButtonText;
        Color cardHideButtonText;
        Color cardProgressBar;

        Color profileUserName;
        Color profileDescription;
        Color profileFriendBarBackground;
        Color profileFriendBarBackgroundH;
        Color profileFriendBarText;
        Color profileFriendBarLine;
        Color profileNotificationBackground;
        Color profileNotificationText;
        Color profileButtonText;

        Color directMessageBackground;
        Color directMessageSubject;
        Color directMessageText;


        Color chatBalloon;
        Color chatBalloonText;

        static readonly string OrigColorPaletteTextureName = "gui_color_palette_original";

        Texture2D origColorPalette;
        
        private static FresviiGUIColorEditor window = null;

        [MenuItem("Fresvii/ColorEditor")]
        static void Init()
        {
            window = (FresviiGUIColorEditor)EditorWindow.GetWindow(typeof(FresviiGUIColorEditor), false, "GUI Color");
        }

        private Vector2 scrollPosition;

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            #region Intialize
            if (!initialized)
            {
                initialized = true;

                GameObject guiCameraPrefab = PrefabUtility.InstantiatePrefab(Resources.Load("GuiPrefabs/GUIMainCamera")) as GameObject;

                DestroyImmediate(guiCameraPrefab);

                background = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.MainBackground);

                // navigation bar

                navigationBarBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarBackground);

                navigationBarUnderLine = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarUnderLine);

                navigationBarTitleColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarTitle);

                navigationBarNormalColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNormal);

                navigationBarNegativeColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarNegative);

                navigationBarPositiveColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.NavigationBarPositive);

                //  tab bar

                tabBarBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarBackground);

                tabBarTopLine = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarTopLine);

                tabBarNegativeColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarNegative);

                tabBarPositiveColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarPositive);

                tabBarBadgeBgColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarBadgeBgColor);

                tabBarBadgeTextColor = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TabBarBadgeTextColor);

                //  text filed

                textfieldBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldBackground);

                textfieldTopLine = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldTopLine);

                textfieldText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldText);

                textfieldSendTextUnable = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldSendTextUnable);

                textfieldSendTextEnable = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldSendTextEnable);

                textfieldSendTextTapped = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.TextfieldSendTextTapped);
                
                //  card

                cardBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardBackground);

                cardUserName = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardUserName);

                cardText1 = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText1);

                cardText2 = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardText2);

                cardSepearateLine = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardSeperateLine1);

                cardAddButtonText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardAddButtonText);

                cardHideButtonText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardHideButtonText);

                cardProgressBar = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.CardProgressBar);

                // Profile

                profileFriendBarBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileFriendBarBackground);

                profileFriendBarBackgroundH = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileFriendBarBackgroundH);

                profileFriendBarText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileFriendBarText);

                profileFriendBarLine = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileFriendBarLine);

                profileNotificationBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileNotificationBackground);

                profileNotificationText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileNotificationText);

                profileUserName = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileUserName);

                profileDescription = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileDescription);

                profileButtonText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ProfileButtonText);

                // Direct Message
                directMessageBackground = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.DirectMessageBackground);

                directMessageSubject = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.DirectMessageSubject);

                directMessageText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.DirectMessageText);


                // Chat balloon

                chatBalloon = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloon);

                chatBalloonText = FresviiGUIColorPalette.GetColor(FresviiGUIColorPalette.ChatBalloonText);
            }
            #endregion

            #region GUIs
            GUILayout.Space(10f);

            background = EditorGUILayout.ColorField("Background", background);

            EditorGUILayout.Separator();

            #region Navigation bar
            EditorGUILayout.LabelField("Navigation bar");

            navigationBarBackground = EditorGUILayout.ColorField("Background", navigationBarBackground);

            navigationBarUnderLine = EditorGUILayout.ColorField("Under line", navigationBarUnderLine);

            navigationBarTitleColor = EditorGUILayout.ColorField("Title", navigationBarTitleColor);

            navigationBarNormalColor = EditorGUILayout.ColorField("Button Normal", navigationBarNormalColor);

            navigationBarNegativeColor = EditorGUILayout.ColorField("Button Negative", navigationBarNegativeColor);

            navigationBarPositiveColor = EditorGUILayout.ColorField("Button Positive", navigationBarPositiveColor);

            #endregion

            #region Tab bar
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Tab bar");

            tabBarBackground = EditorGUILayout.ColorField("Background", tabBarBackground);

            tabBarTopLine = EditorGUILayout.ColorField("Top line", tabBarTopLine);

            tabBarNegativeColor = EditorGUILayout.ColorField("Button Negative", tabBarNegativeColor);

            tabBarPositiveColor = EditorGUILayout.ColorField("Button Positive", tabBarPositiveColor);

            tabBarBadgeBgColor = EditorGUILayout.ColorField("Badge Background", tabBarBadgeBgColor);

            tabBarBadgeTextColor = EditorGUILayout.ColorField("Badge Text", tabBarBadgeTextColor);
            
            #endregion

            #region Textfield
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Textfield");

            textfieldBackground = EditorGUILayout.ColorField("Background", textfieldBackground);

            textfieldTopLine = EditorGUILayout.ColorField("Top line", textfieldTopLine);

            textfieldText = EditorGUILayout.ColorField("Text", textfieldText);

            textfieldSendTextUnable = EditorGUILayout.ColorField("Send text unable", textfieldSendTextUnable);

            textfieldSendTextEnable = EditorGUILayout.ColorField("Send text enable", textfieldSendTextEnable);

            textfieldSendTextTapped = EditorGUILayout.ColorField("Send text tapped", textfieldSendTextTapped);
            
            #endregion

            #region Card
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Card");

            cardBackground = EditorGUILayout.ColorField("Background", cardBackground);

            cardUserName = EditorGUILayout.ColorField("User name", cardUserName);

            cardText1 = EditorGUILayout.ColorField("Text 1", cardText1);

            cardText2 = EditorGUILayout.ColorField("Text 2", cardText2);

            cardSepearateLine = EditorGUILayout.ColorField("Line", cardSepearateLine);

            cardAddButtonText = EditorGUILayout.ColorField("\"Confirm\" button text", cardAddButtonText);

            cardHideButtonText = EditorGUILayout.ColorField("\"Not Now\" button text", cardHideButtonText);

            cardProgressBar = EditorGUILayout.ColorField("Progress bar", cardProgressBar);

            #endregion

            #region Profile
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Profile");

            profileUserName = EditorGUILayout.ColorField("User name", profileUserName);

            profileDescription = EditorGUILayout.ColorField("Description", profileDescription);

            profileButtonText = EditorGUILayout.ColorField("Button text", profileButtonText);

            profileFriendBarBackground = EditorGUILayout.ColorField("Bar button bg", profileFriendBarBackground);

            profileFriendBarBackgroundH = EditorGUILayout.ColorField("Bar button bg active", profileFriendBarBackgroundH);

            profileFriendBarText = EditorGUILayout.ColorField("Bar button text", profileFriendBarText);

            profileNotificationBackground = EditorGUILayout.ColorField("Notification background", profileNotificationBackground);

            profileNotificationText = EditorGUILayout.ColorField("Notificaton text", profileNotificationText);

            #endregion


            #region DirectMessage
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Direct Message");

            directMessageBackground = EditorGUILayout.ColorField("Background", directMessageBackground);

            directMessageSubject = EditorGUILayout.ColorField("Subject", directMessageSubject);

            directMessageText = EditorGUILayout.ColorField("Text", directMessageText);

            #endregion

            #region Misc
            EditorGUILayout.Separator();

            chatBalloon = EditorGUILayout.ColorField("Chat Balloon", chatBalloon);

            chatBalloonText = EditorGUILayout.ColorField("Chat Balloon Text", chatBalloonText);

            #endregion

            #endregion

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();

            #region Apply Button
            if (GUILayout.Button("Apply"))
            {                
                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.MainBackground, background);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.NavigationBarBackground, navigationBarBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.NavigationBarUnderLine, navigationBarUnderLine);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.NavigationBarTitle, navigationBarTitleColor);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.NavigationBarNormal, navigationBarNormalColor);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.NavigationBarPositive, navigationBarPositiveColor);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TabBarBackground, tabBarBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TabBarTopLine, tabBarTopLine);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TabBarNegative, tabBarNegativeColor);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TabBarPositive, tabBarPositiveColor);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TabBarBadgeBgColor, tabBarBadgeBgColor);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TabBarBadgeTextColor, tabBarBadgeTextColor);
                
                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TextfieldBackground, textfieldBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TextfieldTopLine, textfieldTopLine);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TextfieldText, textfieldText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TextfieldSendTextUnable, textfieldSendTextUnable);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TextfieldSendTextEnable, textfieldSendTextEnable);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.TextfieldSendTextTapped, textfieldSendTextTapped);
                
                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardBackground, cardBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardUserName, cardUserName);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardText1, cardText1);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardText2, cardText2);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardSeperateLine1, cardSepearateLine);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardAddButtonText, cardAddButtonText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardHideButtonText, cardHideButtonText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.CardProgressBar, cardProgressBar);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileDescription, profileDescription);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileFriendBarBackground, profileFriendBarBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileFriendBarBackgroundH, profileFriendBarBackgroundH);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileFriendBarLine, profileFriendBarLine);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileFriendBarText, profileFriendBarText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileNotificationBackground, profileNotificationBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileNotificationText, profileNotificationText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileUserName, profileUserName);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ProfileButtonText, profileButtonText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ChatBalloon, chatBalloon);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.ChatBalloonText, chatBalloonText);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.DirectMessageBackground, directMessageBackground);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.DirectMessageSubject, directMessageSubject);

                FresviiGUIColorPalette.SetColor(FresviiGUIColorPalette.DirectMessageText, directMessageText);


                FresviiGUIColorPalette.Apply();

                AssetDatabase.Refresh();
            }
            #endregion

            #region Reset Button
            if (GUILayout.Button("Revert"))
            {
                FresviiGUIColorPalette.SetPixels(((Texture2D)Resources.Load(FresviiGUIConstants.ResouceTextureFolderName + "/" + OrigColorPaletteTextureName, typeof(Texture2D))).GetPixels());

                FresviiGUIColorPalette.Apply();

                AssetDatabase.Refresh();

                initialized = false;
            }
            #endregion

            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }
    }
}