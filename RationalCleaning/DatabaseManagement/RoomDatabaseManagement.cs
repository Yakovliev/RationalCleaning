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
    public class RoomDatabaseManagement
    {
        private SQLiteDatabase db;
        private ICursor roomCursor;

        private int[] roomIds;
        private string[] roomTitles;
        private int[] roomImageIds;
        private int[] isRoom;

        private int[] timeOfRoomCleaning;
        /// <summary>
        /// Room Cleanness in %
        /// </summary>
        private int[] roomCleanness;

        public int[] GetRoomIds()
        {
            return (int[])roomIds.Clone();
        }

        public string[] GetRoomTitles()
        {
            return (string[])roomTitles.Clone();
        }

        public int[] GetRoomImageIds()
        {
            return (int[])roomImageIds.Clone();
        }

        public int[] GetIsRoom()
        {
            return (int[])isRoom.Clone();
        }

        public int[] GetTimeOfRoomCleaning()
        {
            return (int[])timeOfRoomCleaning.Clone();
        }

        /// <summary>
        /// Get Room Cleanness in %
        /// </summary>
        /// <returns></returns>
        public int[] GetRoomCleanness()
        {
            return (int[])roomCleanness.Clone();
        }


        /// <summary>
        /// Групуємо масиви даних по кімнатам таким чином, щоб першим в списку стояв
        /// розділ для задач для всього будинку, далі йшли усі кімнати, а в кінці був розділ катеорії "Інше"
        /// </summary>
        public void FindRightOrderOfRoomTitles(Context context)
        {
            //Допоміжні масиви
            string[] roomTitlesSpare;
            int[] roomIdsSpare;
            int[] isRoomSpare;
            int[] roomImageIdsSpare;

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            db = rationalCleaningDatabaseHelper.ReadableDatabase;
            roomCursor = db.Query("ROOM_TABLE",
                new string[] { "_id", "IS_ROOM", "TITLE", "IMAGE_ID" },
                null, null, null, null, null);

            roomTitles = new string[roomCursor.Count];
            roomIds = new int[roomCursor.Count];
            isRoom = new int[roomCursor.Count];
            roomImageIds = new int[roomCursor.Count];

            roomTitlesSpare = new string[roomCursor.Count];
            roomIdsSpare = new int[roomCursor.Count];
            isRoomSpare = new int[roomCursor.Count];
            roomImageIdsSpare = new int[roomCursor.Count];

            if (roomCursor.MoveToFirst())
            {
                roomIdsSpare[0] = roomCursor.GetInt(0);
                isRoomSpare[0] = roomCursor.GetInt(1);
                roomTitlesSpare[0] = roomCursor.GetString(2);
                roomImageIdsSpare[0] = roomCursor.GetInt(3);
            }

            for (int i = 1; i < roomCursor.Count; i++)
            {
                if (roomCursor.MoveToNext())
                {
                    roomIdsSpare[i] = roomCursor.GetInt(0);
                    isRoomSpare[i] = roomCursor.GetInt(1);
                    roomTitlesSpare[i] = roomCursor.GetString(2);
                    roomImageIdsSpare[i] = roomCursor.GetInt(3);
                }
            }

            roomCursor.Close();
            db.Close();

            int counterTasksForWholeApartement = 0;
            int counterAnotherTasks = 0;

            for (int i = 0; i < roomIdsSpare.Length; i++)
            {
                if (isRoomSpare[i] == 0 && roomTitlesSpare[i] == context.GetString(Resource.String.another_tasks_text))
                {
                    counterAnotherTasks = i;
                }

                if (isRoomSpare[i] == 0 && roomTitlesSpare[i] == context.GetString(Resource.String.tasks_for_whole_apartement_text))
                {
                    counterTasksForWholeApartement = i;
                }
            }

            roomTitles[0] = roomTitlesSpare[counterTasksForWholeApartement];
            roomIds[0] = roomIdsSpare[counterTasksForWholeApartement];
            isRoom[0] = isRoomSpare[counterTasksForWholeApartement];
            roomImageIds[0] = roomImageIdsSpare[counterTasksForWholeApartement];

            roomTitles[roomTitles.Length - 1] = roomTitlesSpare[counterAnotherTasks];
            roomIds[roomIds.Length - 1] = roomIdsSpare[counterAnotherTasks];
            isRoom[roomIds.Length - 1] = isRoomSpare[counterAnotherTasks];
            roomImageIds[roomIds.Length - 1] = roomImageIdsSpare[counterAnotherTasks];

            int counter = 1;
            for (int i = 0; i < roomTitlesSpare.Length; i++)
            {
                if (i != counterTasksForWholeApartement && i != counterAnotherTasks)
                {
                    roomTitles[counter] = roomTitlesSpare[i];
                    roomIds[counter] = roomIdsSpare[i];
                    isRoom[counter] = isRoomSpare[i];
                    roomImageIds[counter] = roomImageIdsSpare[i];
                    counter++;
                }
            }
        }

        public void CalculateRoomCleannessAndTimeOfRoomCleaning(Context context)
        {
            FindRightOrderOfRoomTitles(context);

            timeOfRoomCleaning = new int[roomIds.Length];
            roomCleanness = new int[roomIds.Length];

            for (int i = 0; i < roomTitles.Length; i++)
            {
                timeOfRoomCleaning[i] = 0;
                roomCleanness[i] = 100; //За замовчуванням якщо для кімнати ще не створено задач, то її чистота рівна 100%
            }

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            SQLiteDatabase db = rationalCleaningDatabaseHelper.ReadableDatabase;

            try
            {
                for (int i = 0; i < roomIds.Length; i++)
                {
                    int countOfTaskForSomeRoomId; //Кількість завдань для конкретної кімнати

                    int[] tasksTimeOfCleaning;
                    int[] tasksCleanness;

                    ICursor taskCursor = db.Query("CLEANING_TASK_TABLE",
                    new string[] { "TIME_OF_CLEANING", "CLEANNESS" },
                    "ROOM_ID = ?", new string[] { roomIds[i].ToString() }, null, null, null);

                    if (taskCursor.Count != 0)
                    {
                        countOfTaskForSomeRoomId = taskCursor.Count;

                        tasksTimeOfCleaning = new int[taskCursor.Count];
                        tasksCleanness = new int[taskCursor.Count];

                        if (taskCursor.MoveToFirst())
                        {
                            tasksTimeOfCleaning[0] = taskCursor.GetInt(0);
                            tasksCleanness[0] = taskCursor.GetInt(1);
                        }

                        for (int j = 1; j < taskCursor.Count; j++)
                        {
                            if (taskCursor.MoveToNext())
                            {
                                tasksTimeOfCleaning[j] = taskCursor.GetInt(0);
                                tasksCleanness[j] = taskCursor.GetInt(1);
                            }
                        }

                        taskCursor.Close();

                        roomCleanness[i] -= 100; //На початку ми встановлювали рівень 100% як рівень за замовчуванням, а зараз нам потрібно буде рахувати від нуля.
                        for (int j = 0; j < tasksTimeOfCleaning.Length; j++)
                        {
                            timeOfRoomCleaning[i] += tasksTimeOfCleaning[j];
                            roomCleanness[i] += tasksCleanness[j];
                        }

                        double doubleRoomCleanness = (double)roomCleanness[i] / taskCursor.Count * 100D;
                        roomCleanness[i] = Convert.ToInt16(Math.Round(doubleRoomCleanness, 0));

                    }

                    taskCursor.Close();
                }
            }
            catch (Exception)
            {

            }

            db.Close();
            
        }








        /// <summary>
        /// Count of rooms (isRoom[i] == 1)
        /// </summary>
        public static int CountOfRooms(Context context)
        {
            int countOfRooms = 0;

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            SQLiteDatabase db = rationalCleaningDatabaseHelper.ReadableDatabase;
            ICursor roomCursor = db.Query("ROOM_TABLE",
                new string[] { "_id" },
                null, null, null, null, null);

            countOfRooms = roomCursor.Count - 2;

            roomCursor.Close();
            db.Close();

            return countOfRooms;
        }

        public static int GetRoomImageId(Context context, int roomId)
        {
            int roomImageId = 0;

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            SQLiteDatabase db = rationalCleaningDatabaseHelper.ReadableDatabase;
            ICursor roomCursor = db.Query("ROOM_TABLE",
                new string[] { "IMAGE_ID" },
                "_id = ?", new string[] { roomId.ToString() }, null, null, null);

            if (roomCursor.MoveToFirst())
            {
                roomImageId = roomCursor.GetInt(0);
            }

            roomCursor.Close();
            db.Close();

            return roomImageId;
        }

        public static string GetRoomTitle(Context context, int roomId)
        {
            string roomTitle = "";

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            SQLiteDatabase db = rationalCleaningDatabaseHelper.ReadableDatabase;
            ICursor roomCursor = db.Query("ROOM_TABLE",
                new string[] { "TITLE" },
                "_id = ?", new string[] { roomId.ToString() }, null, null, null);

            if (roomCursor.MoveToFirst())
            {
                roomTitle = roomCursor.GetString(0);
            }

            roomCursor.Close();
            db.Close();

            return roomTitle;
        }
    }
}