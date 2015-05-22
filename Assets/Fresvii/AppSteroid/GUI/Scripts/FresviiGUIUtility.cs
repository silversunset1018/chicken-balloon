using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui
{
    public class FresviiGUIUtility : MonoBehaviour
    {
        public enum DrawPosition { Left, Center, Right };

        public static void DrawButtonFrameX9(Rect position, Texture2D image)
        {
            //  left top
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(0f, 5f / 9f, 4f / 9f, 4f / 9f));
         
            //  right top
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * 4f / 9f, position.y, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(5f / 9f, 5f / 9f, 4f / 9f, 4f / 9f));
            
            // right bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * 4f / 9f, position.y + position.height - 4f / 9f * image.height, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(5f / 9f, 0f, 4f / 9f, 4f / 9f));
            
            // left bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - 4f / 9f * image.height, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(0f, 0f, 4f / 9f, 4f / 9f));

            //  Left line
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + image.width * 4f / 9f, image.width * 4f / 9f, position.height - image.height * 8f / 9f), image, new Rect(0f, 5f / 9f, 4f / 9f, 1f / 9f));

            // Right line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * 4f / 9f, position.y + image.width * 4f / 9f, image.width * 4f / 9f, position.height - image.height * 8f / 9f), image, new Rect(5f / 9f, 5f / 9f, 4f / 9f, 1f / 9f));

            // bottom line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * 4f / 9f, position.y + position.height - 4f / 9f * image.height, position.width - 8f / 9f * image.width, image.height * 4f / 9f), image, new Rect(4f / 9f, 0, 1f / 9f, 4f / 9f));

            // top line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * 4f / 9f, position.y, position.width - image.width * 8f / 9f, image.height * 4f / 9f), image, new Rect(5f / 9f, 5f / 9f, 1f / 9f, 4f / 9f));

            // inside
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * 4f / 9f, position.y + image.width * 4f / 9f, position.width - 8f / 9f * image.width, position.height - 8f / 9f * image.width), image, new Rect(4f / 9f, 4f / 9f, 1f / 9f, 1f / 9f));
        }

        public static void DrawButtonFrame(Rect position, Texture2D image, float scaleFactor)
        {
            float arcLength = (image.width - scaleFactor) * 0.5f;

            float lineLength = scaleFactor;

            //  left top
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect(0f, (arcLength + lineLength) / image.width, arcLength / image.width, arcLength / image.width));

            //  right top
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * arcLength / image.width, position.y, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect((arcLength + lineLength) / image.width, (arcLength + lineLength) / image.width, arcLength / image.width, arcLength / image.width));

            // right bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * arcLength / image.width, position.y + position.height - arcLength / image.width * image.height, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect((arcLength + lineLength) / image.width, 0f, arcLength / image.width, arcLength / image.width));

            // left bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - arcLength / image.width * image.height, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect(0f, 0f, arcLength / image.width, arcLength / image.width));

            //  Left line
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + image.width * arcLength / image.width, image.width * arcLength / image.width, position.height - image.height * (image.width - lineLength) / image.width), image, new Rect(0f, (arcLength + lineLength) / image.width, arcLength / image.width, lineLength / image.width));

            // Right line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * arcLength / image.width, position.y + image.width * arcLength / image.width, image.width * arcLength / image.width, position.height - image.height * (image.width - lineLength) / image.width), image, new Rect((arcLength + lineLength) / image.width, (arcLength + lineLength) / image.width, arcLength / image.width, lineLength / image.width));

            // bottom line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * arcLength / image.width, position.y + position.height - arcLength / image.width * image.height, position.width - (image.width - lineLength) / image.width * image.width, image.height * arcLength / image.width), image, new Rect(arcLength / image.width, 0, lineLength / image.width, arcLength / image.width));

            // top line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * arcLength / image.width, position.y, position.width - image.width * (image.width - lineLength) / image.width, image.height * arcLength / image.width), image, new Rect((arcLength + lineLength) / image.width, (arcLength + lineLength) / image.width, lineLength / image.width, arcLength / image.width));

            // inside
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * arcLength / image.width, position.y + image.width * arcLength / image.width, position.width - (image.width - lineLength) / image.width * image.width, position.height - (image.width - lineLength) / image.width * image.width), image, new Rect(arcLength / image.width, arcLength / image.width, lineLength / image.width, lineLength / image.width));
        }

        public static void DrawMenuTopButtonFrame(Rect position, Texture2D image)
        {
            //  left top
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(0f, 5f / 9f, 4f / 9f, 4f / 9f));

            //  right top
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * 4f / 9f, position.y, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(5f / 9f, 5f / 9f, 4f / 9f, 4f / 9f));

            // top line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * 4f / 9f, position.y, position.width - image.width * 8f / 9f, image.height * 4f / 9f), image, new Rect(5f / 9f, 5f / 9f, 1f / 9f, 4f / 9f));

            // inside
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + image.width * 4f / 9f, position.width, position.height - 4f / 9f * image.width), image, new Rect(4f / 9f, 4f / 9f, 1f / 9f, 1f / 9f));
        }

        public static void DrawMenuCeneterButtonFrame(Rect position, Texture2D image)
        {
            // inside
            GUI.DrawTextureWithTexCoords(position, image, new Rect(4f / 9f, 4f / 9f, 1f / 9f, 1f / 9f));
        }

        public static void DrawMenuBottomButtonFrame(Rect position, Texture2D image)
        {
            // right bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * 4f / 9f, position.y + position.height - 4f / 9f * image.height, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(5f / 9f, 0f, 4f / 9f, 4f / 9f));

            // left bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - 4f / 9f * image.height, image.width * 4f / 9f, image.height * 4f / 9f), image, new Rect(0f, 0f, 4f / 9f, 4f / 9f));

            // bottom line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * 4f / 9f, position.y + position.height - 4f / 9f * image.height, position.width - 8f / 9f * image.width, image.height * 4f / 9f), image, new Rect(4f / 9f, 0, 1f / 9f, 4f / 9f));

            // inside
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, position.width, position.height - 4f / 9f * image.width), image, new Rect(4f / 9f, 4f / 9f, 1f / 9f, 1f / 9f));
        }

        public static void DrawSplitTexture(Rect position, Texture2D image, float splitCenterLength)
        {
            float arcLength = (image.width - splitCenterLength) * 0.5f;

            float lineLength = splitCenterLength;

            //  left top
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect(0f, (arcLength + lineLength) / image.width, arcLength / image.width, arcLength / image.width));

            //  right top
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * arcLength / image.width, position.y, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect((arcLength + lineLength) / image.width, (arcLength + lineLength) / image.width, arcLength / image.width, arcLength / image.width));

            // right bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * arcLength / image.width, position.y + position.height - arcLength / image.width * image.height, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect((arcLength + lineLength) / image.width, 0f, arcLength / image.width, arcLength / image.width));

            // left bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - arcLength / image.width * image.height, image.width * arcLength / image.width, image.height * arcLength / image.width), image, new Rect(0f, 0f, arcLength / image.width, arcLength / image.width));

            //  Left line
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + image.width * arcLength / image.width, image.width * arcLength / image.width, position.height - image.height * (image.width - lineLength) / image.width), image, new Rect(0f, (arcLength + lineLength) / image.width, arcLength / image.width, lineLength / image.width));

            // Right line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - image.width * arcLength / image.width, position.y + image.width * arcLength / image.width, image.width * arcLength / image.width, position.height - image.height * (image.width - lineLength) / image.width), image, new Rect((arcLength + lineLength) / image.width, (arcLength + lineLength) / image.width, arcLength / image.width, lineLength / image.width));

            // bottom line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * arcLength / image.width, position.y + position.height - arcLength / image.width * image.height, position.width - (image.width - lineLength) / image.width * image.width, image.height * arcLength / image.width), image, new Rect(arcLength / image.width, 0, lineLength / image.width, arcLength / image.width));

            // top line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * arcLength / image.width, position.y, position.width - image.width * (image.width - lineLength) / image.width, image.height * arcLength / image.width), image, new Rect((arcLength + lineLength) / image.width, (arcLength + lineLength) / image.width, lineLength / image.width, arcLength / image.width));

            // inside
            GUI.DrawTextureWithTexCoords(new Rect(position.x + image.width * arcLength / image.width, position.y + image.width * arcLength / image.width, position.width - (image.width - lineLength) / image.width * image.width, position.height - (image.width - lineLength) / image.width * image.width), image, new Rect(arcLength / image.width, arcLength / image.width, lineLength / image.width, lineLength / image.width));
        }

        public static void DrawSplitTexture(Rect position, Texture2D image, float left, float right, float top, float bottom)
        {
            //  left top
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, left, top), image, new Rect(0f, 1.0f - top / image.height, left / image.width, top / image.height));

            //  right top
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right, position.y, right, top), image, new Rect((image.width - right) / image.width, 1.0f - top / image.height, right / image.width, top / image.height));

            // right bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right, position.y + position.height - bottom, right, bottom), image, new Rect((image.width - right) / image.width, 0f, right / image.width, bottom / image.height));

            // left bottom
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - bottom, left, bottom), image, new Rect(0f, 0f, left / image.width, bottom / image.height));

            //  Left line
            GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + top, left, position.height - top - bottom), image, new Rect(0f, bottom / image.height, left / image.width, (image.height - top - bottom) / image.height));

            // Right line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right, position.y + top, right, position.height - top - bottom), image, new Rect((image.width - right) / image.width, bottom / image.height, right / image.width, (image.height - top - bottom) / image.height));

            // bottom line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + left, position.y + position.height - bottom, position.width - left - right, bottom), image, new Rect(left / image.width, 0f, (image.width - right - left) / image.width, bottom / image.height));

            // top line
            GUI.DrawTextureWithTexCoords(new Rect(position.x + left, position.y, position.width - left - right, top), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width,  top / image.height));

            // inside
            GUI.DrawTextureWithTexCoords(new Rect(position.x + left, position.y + top, position.width - left - right, position.height - top - bottom), image, new Rect(left/ image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, (image.height - top - bottom) / image.height));
        }

        public static void DrawSplitTexture(Rect position, Texture2D image, float left, float right, float top, float bottom, DrawPosition drawPosition)
        {
            if (drawPosition == DrawPosition.Left)
            {
                //  left top
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, left, top), image, new Rect(0f, 1.0f - top / image.height, left / image.width, top / image.height));

                // left bottom
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - bottom, left, bottom), image, new Rect(0f, 0f, left / image.width, bottom / image.height));

                //  Left line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + top, left, position.height - top - bottom), image, new Rect(0f, bottom / image.height, left / image.width, (image.height - top - bottom) / image.height));

                // Right line
                GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right * 0.5f, position.y, right * 0.5f, position.height), image, new Rect((image.width - right) / image.width, bottom / image.height, right / image.width, (image.height - top - bottom) / image.height));

                // bottom line
                GUI.DrawTextureWithTexCoords(new Rect(position.x + left, position.y + position.height - bottom, position.width - left, bottom), image, new Rect(left / image.width, 0f, (image.width - right - left) / image.width, bottom / image.height));

                // top line
                GUI.DrawTextureWithTexCoords(new Rect(position.x + left, position.y, position.width - left, top), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, top / image.height));

                // inside
                GUI.DrawTextureWithTexCoords(new Rect(position.x + left * 0.5f, position.y + top, position.width - left * 0.5f, position.height - top - bottom), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, (image.height - top - bottom) / image.height));
            }
            else if (drawPosition == DrawPosition.Center)
            {
                //  Left line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, left * 0.5f, position.height), image, new Rect(0f, bottom / image.height, left / image.width, (image.height - top - bottom) / image.height));

                // Right line
                GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right * 0.5f, position.y, right * 0.5f, position.height), image, new Rect((image.width - right) / image.width, bottom / image.height, right / image.width, (image.height - top - bottom) / image.height));

                // bottom line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - bottom, position.width, bottom), image, new Rect(left / image.width, 0f, (image.width - right - left) / image.width, bottom / image.height));

                // top line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, position.width, top), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, top / image.height));

                // inside
                GUI.DrawTextureWithTexCoords(new Rect(position.x + left * 0.5f, position.y + top, position.width - left * 0.5f - right * 0.5f, position.height - top - bottom), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, (image.height - top - bottom) / image.height));

            }
            else if(drawPosition == DrawPosition.Right)
            {                
                //  right top
                GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right, position.y, right, top), image, new Rect((image.width - right) / image.width, 1.0f - top / image.height, right / image.width, top / image.height));

                // right bottom
                GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right, position.y + position.height - bottom, right, bottom), image, new Rect((image.width - right) / image.width, 0f, right / image.width, bottom / image.height));
                
                //  Left line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, left * 0.5f, position.height), image, new Rect(0f, bottom / image.height, left / image.width, (image.height - top - bottom) / image.height));

                // Right line
                GUI.DrawTextureWithTexCoords(new Rect(position.x + position.width - right, position.y + top, right, position.height - top - bottom), image, new Rect((image.width - right) / image.width, bottom / image.height, right / image.width, (image.height - top - bottom) / image.height));

                // bottom line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y + position.height - bottom, position.width - right, bottom), image, new Rect(left / image.width, 0f, (image.width - right - left) / image.width, bottom / image.height));

                // top line
                GUI.DrawTextureWithTexCoords(new Rect(position.x, position.y, position.width - right, top), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, top / image.height));

                // inside
                GUI.DrawTextureWithTexCoords(new Rect(position.x + left * 0.5f, position.y + top, position.width - left * 0.5f - right, position.height - top - bottom), image, new Rect(left / image.width, 1.0f - top / image.height, (image.width - right - left) / image.width, (image.height - top - bottom) / image.height));
            }
        }


        public static Rect RectScale(Rect rect, float scale)
        {
            return new Rect(rect.x * scale, rect.y * scale, rect.width * scale, rect.height * scale);
        }

        public static RectOffset RectOffsetScale(RectOffset rect, float scale)
        {
            return new RectOffset((int)(rect.left * scale), (int)(rect.right * scale), (int)(rect.top * scale), (int)(rect.bottom * scale));
        }

        public static string CurrentTimeSpan(System.DateTime dt)
        {
            System.TimeSpan ts = System.DateTime.Now.Subtract(dt);

            string timespan = "";

            if (ts.Days > 1)
                timespan = ts.Days + " " + FresviiGUIText.Get("days") + FresviiGUIText.Get("ago");
            else if (ts.Days > 0)
                timespan =ts.Days + " " + FresviiGUIText.Get("day") + FresviiGUIText.Get("ago");
            else if (ts.Hours > 1)
                timespan =ts.Hours + " " + FresviiGUIText.Get("hours") + FresviiGUIText.Get("ago");
            else if (ts.Hours > 0)
                timespan =ts.Hours + " " + FresviiGUIText.Get("hour") + FresviiGUIText.Get("ago");
            else if (ts.Minutes > 1)
                timespan =ts.Minutes + " " + FresviiGUIText.Get("minutes") + FresviiGUIText.Get("ago");
            else if (ts.Minutes > 0)
                timespan =ts.Minutes + " " + FresviiGUIText.Get("minute") + FresviiGUIText.Get("ago");
            else
                timespan =FresviiGUIText.Get("now");

            return timespan;
        }

        public static string CurrentTimeSpanShort(System.DateTime dt)
        {
            System.TimeSpan ts = System.DateTime.Now.Subtract(dt);

            string timespan = "";

            if (ts.Days > 1)
                timespan = ts.Days + " " + FresviiGUIText.Get("days");
            else if (ts.Days > 0)
                timespan = ts.Days + " " + FresviiGUIText.Get("day");
            else if (ts.Hours > 1)
                timespan = ts.Hours + " " + FresviiGUIText.Get("hours");
            else if (ts.Hours > 0)
                timespan = ts.Hours + " " + FresviiGUIText.Get("hour");
            else if (ts.Minutes > 1)
                timespan = ts.Minutes + " " + FresviiGUIText.Get("min");
            else if (ts.Minutes > 0)
                timespan = ts.Minutes + " " + FresviiGUIText.Get("min");
            else
                timespan = FresviiGUIText.Get("now");

            return timespan;
        }

        public static string TimeSpanWatch(System.DateTime dt)
        {
            System.TimeSpan ts = System.DateTime.Now.Subtract(dt);

            string timespan = "";

            if (ts.Hours > 1)
                timespan += ts.Hours + ":";
            
            timespan += ts.Minutes.ToString("00") + ":";

            timespan += ts.Seconds.ToString("00");
            
            return timespan;
        }

        public static string TimeSpan(long elapsedTime)
        {
            string timespan = "";

            long sec = elapsedTime / 1000;

            long min = sec / 60;

            long hour = min / 60;

            if (hour > 1)
                timespan += hour + ":";

            timespan += min.ToString("00") + ":";

            timespan += sec.ToString("00");

            return timespan;
        }

        public static string Truncate(string source, GUIStyle guiStyle, float width, string truncateString)
        {
			if(source == null) return "";

            int tuncateStringNum = 2;

            GUIContent content = new GUIContent(source);

            while (guiStyle.CalcScreenSize(guiStyle.CalcSize(content)).x > width)
            {
                if (source.Length - tuncateStringNum < 2) break;

                content = new GUIContent(source.Substring(0, source.Length - tuncateStringNum) + truncateString);

                tuncateStringNum++;
            }

            return content.text;
        }

        public static List<string> GetUrls(string text)
        {
            List<string> urls = new List<string>();

            if (string.IsNullOrEmpty(text))
            {
                return urls;
            }

            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");

            for (System.Text.RegularExpressions.Match m = reg.Match(text); m.Success; m = m.NextMatch())
            {
                urls.Add(m.Value);
            }

            return urls;
        }

        public static string ColorToHex(Color col)
        {
            return "#" + ((int)(col.r * 255f)).ToString("x2") + ((int)(col.g * 255f)).ToString("x2") + ((int)(col.b * 255f)).ToString("x2");
        }

    }
}