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
using Android.Text.Format;

namespace RationalCleaning.DatabaseManagement
{
    public class CleaningTaskDatabaseManagement
    {
        private SQLiteDatabase db;
        private ICursor taskCursor;

        private Context context;


        /// <summary>
        /// Array of ids of tasks
        /// </summary>
        private int[] taskIds;

        /// <summary>
        /// Array of time of cleaning for each task
        /// </summary>
        private int[] tasksTimeOfCleaning;

        /// <summary>
        /// Array of cleanness of tasks. Value: 0 or 1
        /// </summary>
        private int[] tasksCleanness;

        /// <summary>
        /// Array of cleanness of tasks in percentages. If tasksCleanness[i] == 0 then tasksCleannessInPercentages = 0, if tasksCleanness[i] == 1 then tasksCleannessInPercentages = 100
        /// </summary>
        private int[] tasksCleannessInPercentages;

        /// <summary>
        /// Array of titles of tasks
        /// </summary>
        private string[] taskTitles;

        private int[] roomIds;

        private int[] year;

        private int[] month;

        private int[] dayOfMonth;

        private int[] dateDefault;

        private int[] yearOfChange;

        private int[] monthOfChange;

        private int[] dayOfMonthOfChange;

        private int[] periodicity;

        private int[] hour;

        private int[] minute;

        /// <summary>
        /// Array of strings of date each task
        /// </summary>
        private string[] dateOfTaskString;



        public int[] GetTaskIds()
        {
            return (int[])taskIds.Clone();
        }

        public int[] GetTasksTimeOfCleaning()
        {
            return (int[])tasksTimeOfCleaning.Clone();
        }

        public int[] GetTasksCleanness()
        {
            return (int[])tasksCleanness.Clone();
        }

        public int[] GetTasksCleannessInPercentages()
        {
            return (int[])tasksCleannessInPercentages.Clone();
        }

        public string[] GetTaskTitles()
        {
            return (string[])taskTitles.Clone();
        }

        public CleaningTaskDatabaseManagement(Context context)
        {
            this.context = context;
        }


        

        public void GetRoomTasksFromDatabase(int roomId)
        {
            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            db = rationalCleaningDatabaseHelper.ReadableDatabase;
            taskCursor = db.Query("CLEANING_TASK_TABLE",
                new string[] { "_id", "TIME_OF_CLEANING", "CLEANNESS", "TITLE" },
                "ROOM_ID = ?", new string[] { roomId.ToString() }, null, null, null);

            taskIds = new int[taskCursor.Count];
            tasksTimeOfCleaning = new int[taskCursor.Count];
            tasksCleanness = new int[taskCursor.Count];
            taskTitles = new string[taskCursor.Count];

            if (taskCursor.MoveToFirst())
            {
                taskIds[0] = taskCursor.GetInt(0);
                tasksTimeOfCleaning[0] = taskCursor.GetInt(1);
                tasksCleanness[0] = taskCursor.GetInt(2);
                taskTitles[0] = taskCursor.GetString(3);
            }


            for (int i = 1; i < taskCursor.Count; i++)
            {
                if (taskCursor.MoveToNext())
                {
                    taskIds[i] = taskCursor.GetInt(0);
                    tasksTimeOfCleaning[i] = taskCursor.GetInt(1);
                    tasksCleanness[i] = taskCursor.GetInt(2);
                    taskTitles[i] = taskCursor.GetString(3);
                }
            }

            taskCursor.Close();
            db.Close();
        }


        public static int CountOfCleaningTask(Context context, int roomId)
        {
            int countOfCleaningTask = 0;

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            SQLiteDatabase db = rationalCleaningDatabaseHelper.ReadableDatabase;
            ICursor taskCursor = db.Query("CLEANING_TASK_TABLE",
                new string[] { "_id" },
                "ROOM_ID = ?", new string[] { roomId.ToString() }, null, null, null);

            countOfCleaningTask = taskCursor.Count;

            taskCursor.Close();
            db.Close();

            return countOfCleaningTask;
        }


    }
}