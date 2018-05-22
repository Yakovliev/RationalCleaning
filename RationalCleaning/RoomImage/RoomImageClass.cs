using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RationalCleaning.RoomImage
{
    public class RoomImageClass
    {
        public static int GetRoomImageResource(int roomImageCounter)
        {
            int counter = roomImageCounter % 8;

            int imageResource = 0;

            if (counter == 0)
            {
                imageResource = Resource.Drawable.roomImage1;
            }
            else if (counter == 1)
            {
                imageResource = Resource.Drawable.roomImage2;
            }
            else if (counter == 2)
            {
                imageResource = Resource.Drawable.roomImage3;
            }
            else if (counter == 3)
            {
                imageResource = Resource.Drawable.roomImage4;
            }
            else if (counter == 4)
            {
                imageResource = Resource.Drawable.roomImage5;
            }
            else if (counter == 5)
            {
                imageResource = Resource.Drawable.roomImage6;
            }
            else if (counter == 6)
            {
                imageResource = Resource.Drawable.roomImage7;
            }
            else if (counter == 7)
            {
                imageResource = Resource.Drawable.roomImage8;
            }

            return imageResource;

        }

        public static int GetRoomImageCounter(int imageResource)
        {
            int roomImageCounter = 0;

            if (imageResource == Resource.Drawable.roomImage1)
            {
                roomImageCounter = 0;
            }
            if (imageResource == Resource.Drawable.roomImage2)
            {
                roomImageCounter = 1;
            }
            if (imageResource == Resource.Drawable.roomImage3)
            {
                roomImageCounter = 2;
            }
            if (imageResource == Resource.Drawable.roomImage4)
            {
                roomImageCounter = 3;
            }
            if (imageResource == Resource.Drawable.roomImage5)
            {
                roomImageCounter = 4;
            }
            if (imageResource == Resource.Drawable.roomImage6)
            {
                roomImageCounter = 5;
            }
            if (imageResource == Resource.Drawable.roomImage7)
            {
                roomImageCounter = 6;
            }
            if (imageResource == Resource.Drawable.roomImage8)
            {
                roomImageCounter = 7;
            }

            return roomImageCounter;

        }
    }
}