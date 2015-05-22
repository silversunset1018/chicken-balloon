using UnityEngine;
using System.Collections;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIColorPalette 
    {
        private static readonly string ColorPaletteTextureName = "gui_color_palette";

        //--------------------------------------
        // Color Settings
        //--------------------------------------
        public static readonly Vector2 MainBackground = new Vector2(0f, 0f);
        public static readonly Vector2 ModalBackground = new Vector2(1f, 0f);
        public static readonly Vector2 NotificatinoBackground = new Vector2(2f, 0f);
        public static readonly Vector2 NotificatinoText = new Vector2(3f, 0f);


        // Navigation bar
        public static readonly Vector2 NavigationBarBackground = new Vector2(0f, 1f);
        public static readonly Vector2 NavigationBarUnderLine = new Vector2(1f, 1f);
        public static readonly Vector2 NavigationBarTitle = new Vector2(2f, 1f);
        public static readonly Vector2 NavigationBarNormal = new Vector2(3f, 1f);
        public static readonly Vector2 NavigationBarNegative = new Vector2(4f, 1f);
        public static readonly Vector2 NavigationBarPositive = new Vector2(5f, 1f);

        // Tab bar
        public static readonly Vector2 TabBarBackground = new Vector2(0f, 2f);
        public static readonly Vector2 TabBarTopLine = new Vector2(1f, 2f);
        public static readonly Vector2 TabBarNegative = new Vector2(2f, 2f);
        public static readonly Vector2 TabBarPositive = new Vector2(3f, 2f);
        public static readonly Vector2 TabBarBadgeBgColor = new Vector2(4f, 2f);
        public static readonly Vector2 TabBarBadgeTextColor = new Vector2(5f, 2f);


        //  Input area
        public static readonly Vector2 TextfieldBackground = new Vector2(0f, 3f);
        public static readonly Vector2 TextfieldTopLine = new Vector2(1f, 3f);
        public static readonly Vector2 TextfieldText = new Vector2(2f, 3f);
        public static readonly Vector2 TextfieldSendTextUnable = new Vector2(3f, 3f);
        public static readonly Vector2 TextfieldSendTextEnable = new Vector2(4f, 3f);
        public static readonly Vector2 TextfieldSendTextTapped = new Vector2(5f, 3f);

        //  Card
        public static readonly Vector2 CardBackground = new Vector2(0f, 4f);
        public static readonly Vector2 CardUserName = new Vector2(1f, 4f);
        public static readonly Vector2 CardText1 = new Vector2(2f, 4f);
        public static readonly Vector2 CardText2 = new Vector2(3f, 4f);
        public static readonly Vector2 CardSeperateLine1 = new Vector2(4f, 4f);
        public static readonly Vector2 CardAddButtonText = new Vector2(5f, 4f);
        public static readonly Vector2 CardHideButtonText = new Vector2(6f, 4f);
        public static readonly Vector2 CardSeperateLine2 = new Vector2(7f, 4f);
        public static readonly Vector2 CardDeleteBackground = new Vector2(8f, 4f);
        public static readonly Vector2 CardProgressBar = new Vector2(9f, 4f);

        //  Profile
        public static readonly Vector2 ProfileFriendBarBackground = new Vector2(0f, 5f);
        public static readonly Vector2 ProfileFriendBarText = new Vector2(1f, 5f);
        public static readonly Vector2 ProfileFriendBarLine = new Vector2(2f, 5f);
        public static readonly Vector2 ProfileNotificationBackground = new Vector2(3f, 5f);
        public static readonly Vector2 ProfileNotificationText = new Vector2(4f, 5f);
        public static readonly Vector2 ProfileUserName = new Vector2(5f, 5f);
        public static readonly Vector2 ProfileDescription = new Vector2(6f, 5f);
		public static readonly Vector2 ProfileButtonText = new Vector2(7f, 5f);
		public static readonly Vector2 ProfileButtonTextL = new Vector2(8f, 5f);
        public static readonly Vector2 ProfileFriendBarBackgroundH = new Vector2(9f, 5f);

        public static readonly Vector2 DirectMessageBackground = new Vector2(10f, 5f);
        public static readonly Vector2 DirectMessageSubject = new Vector2(11f, 5f);
        public static readonly Vector2 DirectMessageText = new Vector2(12f, 5f);


        //  Misc
        //  ActionSheet line
        public static readonly Vector2 ActionSheetLine = new Vector2(0f, 9f);

        //  Chat
        public static readonly Vector2 ChatBalloon = new Vector2(1f, 9f);
        public static readonly Vector2 ChatBalloonText = new Vector2(2f, 9f);
        public static readonly Vector2 GroupCardTopShadowLine = new Vector2(3f, 9f);
        public static readonly Vector2 SearchBackground = new Vector2(4f, 9f);
        public static readonly Vector2 GroupCallText = new Vector2(5f, 9f);

        // PopOverMenu
        public static readonly Vector2 PopOverMenuNormal = new Vector2(6f, 9f);
        public static readonly Vector2 PopOverMenuActive = new Vector2(7f, 9f);
        public static readonly Vector2 PopOverMenuText = new Vector2(8f, 9f);
        public static readonly Vector2 PopOverMenuLine = new Vector2(9f, 9f);


        private static FresviiGUIColorPalette instance;

        private Color[] colors;

        private static Texture2D palette;

        public static Texture2D Palette 
        { 
            get 
            {                 
                if(instance == null)
                {
                    instance = new FresviiGUIColorPalette();
                }

                return palette;
            }

            protected set
            {
                palette = value;
            }
        }

        public FresviiGUIColorPalette()
        {
            palette = (Texture2D)Resources.Load(FresviiGUIConstants.ResouceTextureFolderName + "/" + ColorPaletteTextureName, typeof(Texture2D));

            colors = palette.GetPixels();
        }

        public static Color GetColor(Vector2 texturePosition)
        {
            if(instance == null)
            {
                instance = new FresviiGUIColorPalette();
            }

            return instance.colors[(int)(texturePosition.x + Palette.width * texturePosition.y)];
        }

        public static void SetColor(Vector2 texturePosition, Color color)
        {
            if (instance == null)
            {
                instance = new FresviiGUIColorPalette();
            }

            instance.colors[(int)(texturePosition.x + Palette.width * texturePosition.y)] = color;
        }

        public static void SetPixels(Color[] colors)
        {
            if (instance == null)
            {
                return;
            }

            instance.colors = colors;
        }

        public static void Apply()
        {
            if (instance == null)
            {
                return;
            }

            palette.SetPixels(instance.colors);

            palette.Apply();

            System.IO.File.WriteAllBytes(Application.dataPath + "/Fresvii/AppSteroid/GUI/Resources/GuiTextures/" + ColorPaletteTextureName + ".png", Palette.EncodeToPNG());
        }

        public static Rect GetTextureCoods(Vector2 coords)
        {
            if (instance == null)
            {
                instance = new FresviiGUIColorPalette();
            }

            return new Rect(coords.x / Palette.width, coords.y / Palette.height, 1.0f / Palette.width, 1.0f / Palette.height);
        }
    }
}