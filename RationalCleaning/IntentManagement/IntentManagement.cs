using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Content.Res;
using Java.Util;
using Android.Database;
using Android.Database.Sqlite;
using RationalCleaning.ActionBar;
using RationalCleaning.RecyclerView;
using RationalCleaning.Dialog;
using RationalCleaning.Database;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.Design.Widget;

namespace RationalCleaning.IntentManagement
{
    public class IntentManagement
    {

        public const string ROOM_ID = "roomId";


        public static void CreateSimpleIntent(Context context, Type type)
        {
            Intent intent = new Intent(context, type);
            context.StartActivity(intent);
        }

        public static void RoomManagementIntent(Context context, int roomId)
        {
            Intent intent = new Intent(context, typeof(RoomManagement));
            intent.PutExtra(ROOM_ID, roomId);
            context.StartActivity(intent);
        }

        public static void CreateIntentToUpdateCleaningTask(Context context, int taskId, int roomId)
        {
            Intent intent = new Intent(context, typeof(CreateCleaningTask));
            intent.PutExtra(RoomManagement.TASK_ID, taskId); // Якщо буде передаватися taskId = -1, то це означає, що ми створюємо Cleaning Task, а не редагуємо
            intent.PutExtra(ROOM_ID, roomId); //Якщо буде передаватися конкретний roomId, то в roomSpinner відразу буде вказана обрана кімната і
                                              //без зміни цього спіннера зміна roomId відбуватися не буде
            context.StartActivity(intent);
        }
    }
}