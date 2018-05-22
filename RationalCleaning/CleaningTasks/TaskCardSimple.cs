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
using Android.Support.Design.Widget;
using RationalCleaning.DatabaseManagement;


namespace RationalCleaning.CleaningTasks
{
    public class TaskCardSimple
    {
        public int TaskId { get; private set; }
        public string TaskTitle { get; set; }
        public int TimeImageId { get; set; }
        public string TimeOfCleaning { get; set; }
        public int CleannessImageId { get; set; }
        public string Cleanness { get; set; }
        public int CleannessInteger { get; set; }
        public bool SwitchOn { get; set; }
        public string ActionTextViewText { get; set; }

        public string ActionTextViewText2 { get; set; }


        public string RoomTitle { get; set; }
        public List<TaskCardSimple> TaskCardSimpleList { get; set; }
        public bool IsTodayTask { get; set; }

        private SQLiteDatabase db;
        private ICursor taskCursor;


        public TaskCardSimple(int taskId, string taskTitle, int timeImageId, string timeOfCleaning,
            int cleannessImageId, string cleanness, int cleannessInteger, string roomTitle, bool switchOn,
            string actionTextViewText)
        {
            TaskId = taskId;
            TaskTitle = taskTitle;
            TimeImageId = timeImageId;
            TimeOfCleaning = timeOfCleaning;
            CleannessImageId = cleannessImageId;
            Cleanness = cleanness;
            CleannessInteger = cleannessInteger;
            RoomTitle = roomTitle;
            SwitchOn = switchOn;
            ActionTextViewText = actionTextViewText;
        }

        public TaskCardSimple(int taskId, string taskTitle, int timeImageId, string timeOfCleaning,
            int cleannessImageId, string cleanness, int cleannessInteger, string roomTitle, bool switchOn,
            string actionTextViewText, string actionTextViewText2)
        {
            TaskId = taskId;
            TaskTitle = taskTitle;
            TimeImageId = timeImageId;
            TimeOfCleaning = timeOfCleaning;
            CleannessImageId = cleannessImageId;
            Cleanness = cleanness;
            CleannessInteger = cleannessInteger;
            RoomTitle = roomTitle;
            SwitchOn = switchOn;
            ActionTextViewText = actionTextViewText; //В даному випадку цей текст не відповідає за текст Action TextView
            ActionTextViewText2 = actionTextViewText2;
        }

        public TaskCardSimple()
        {

        }

        public int GetRoomId(Context context)
        {
            int roomId = 0;

            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            db = rationalCleaningDatabaseHelper.ReadableDatabase;
            taskCursor = db.Query("CLEANING_TASK_TABLE",
                new string[] { "ROOM_ID" },
                "_id = ?", new string[] { TaskId.ToString() }, null, null, null);

            if (taskCursor.MoveToFirst())
            {
                roomId = taskCursor.GetInt(0);
            }

            taskCursor.Close();
            db.Close();

            return roomId;
        }

        public void InitializeTasksForToday(Context context)
        {
            TaskCardSimpleList = new List<TaskCardSimple>();

            int timeImageId = Resource.Drawable.stopwatch;
            int cleannessImageId = Resource.Drawable.performance;

            CleaningTaskForToday cleaningTaskForToday = new CleaningTaskForToday(context);
            cleaningTaskForToday.CalculateTasksForToday();
            cleaningTaskForToday.UpdateCleannessToZero();

            if (cleaningTaskForToday.GetTaskIdsForToday() != null)
            {
                for (int i = 0; i < cleaningTaskForToday.GetTaskIdsForToday().Length; i++)
                {
                    bool switchOn = false;
                    if (cleaningTaskForToday.GetTasksCleannessForToday()[i] == 1)
                    {
                        switchOn = true;
                    }

                    TaskCardSimpleList.Add(new TaskCardSimple(cleaningTaskForToday.GetTaskIdsForToday()[i],
                        cleaningTaskForToday.GetTaskTitlesForToday()[i], timeImageId,
                        cleaningTaskForToday.GetTasksTimeOfCleaningForToday()[i].ToString() + " " + context.GetString(Resource.String.minute_text),
                        cleannessImageId,
                        cleaningTaskForToday.GetTasksCleannessInPercentagesForToday()[i].ToString() + "%",
                        cleaningTaskForToday.GetTasksCleannessForToday()[i],
                        RoomDatabaseManagement.GetRoomTitle(context, cleaningTaskForToday.GetRoomIdsForToday()[i]),
                        switchOn, context.GetString(Resource.String.reschedule_for_the_next_day)));

                }
            }        
        }

        public void InitializeTasksForNextDay(Context context)
        {
            TaskCardSimpleList = new List<TaskCardSimple>();

            int timeImageId = Resource.Drawable.stopwatch;
            int cleannessImageId = Resource.Drawable.performance;

            CleaningTaskForNextDay cleaningTaskForNextDay = new CleaningTaskForNextDay(context);
            cleaningTaskForNextDay.CalculateTasksForNextDay();

            DateTime dateTimeOfTasksForNextDay = cleaningTaskForNextDay.DateTimeOfTasksForNextDay;

            string dateString = context.GetString(Resource.String.today_text) + ": " +
                DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() +
                "/" + DateTime.Now.Day.ToString() + "\r\n" +
            context.GetString(Resource.String.date_text) + ": " +
                dateTimeOfTasksForNextDay.Year.ToString() + "/" + dateTimeOfTasksForNextDay.Month.ToString() +
                "/" + dateTimeOfTasksForNextDay.Day.ToString();

            if (cleaningTaskForNextDay.GetTaskIdsForNextDay() != null)
            {
                for (int i = 0; i < cleaningTaskForNextDay.GetTaskIdsForNextDay().Length; i++)
                {
                    bool switchOn = false;
                    if (cleaningTaskForNextDay.GetTasksCleannessForNextDay()[i] == 1)
                    {
                        switchOn = true;
                    }

                    TaskCardSimpleList.Add(new TaskCardSimple(cleaningTaskForNextDay.GetTaskIdsForNextDay()[i],
                        cleaningTaskForNextDay.GetTaskTitlesForNextDay()[i], timeImageId,
                        cleaningTaskForNextDay.GetTasksTimeOfCleaningForNextDay()[i].ToString() + " " + context.GetString(Resource.String.minute_text),
                        cleannessImageId,
                        cleaningTaskForNextDay.GetTasksCleannessInPercentagesForNextDay()[i].ToString() + "%",
                        cleaningTaskForNextDay.GetTasksCleannessForNextDay()[i],
                        RoomDatabaseManagement.GetRoomTitle(context, cleaningTaskForNextDay.GetRoomIdsForNextDay()[i]),
                        switchOn, dateString,
                        context.GetString(Resource.String.transfer_task_to_today_task)));

                }
            }
        }


        public void InitializeFullListOfTasks(Context context)
        {
            TaskCardSimpleList = new List<TaskCardSimple>();

            int timeImageId = Resource.Drawable.stopwatch;
            int cleannessImageId = Resource.Drawable.performance;

            CleaningTaskFullList cleaningTaskFullList = new CleaningTaskFullList(context);
            cleaningTaskFullList.CalculateFullListOfTasks();

            if (cleaningTaskFullList.GetTaskIds() != null)
            {
                for (int i = 0; i < cleaningTaskFullList.GetTaskIds().Length; i++)
                {
                    DateTime dateTimeOfTask = cleaningTaskFullList.GetDateTimeOfTask()[i];

                    string dateString = context.GetString(Resource.String.today_text) + ": " +
                        DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() +
                        "/" + DateTime.Now.Day.ToString() + "\r\n" +
                    context.GetString(Resource.String.date_text) + ": " +
                        dateTimeOfTask.Year.ToString() + "/" + dateTimeOfTask.Month.ToString() +
                        "/" + dateTimeOfTask.Day.ToString();



                    bool switchOn = false;
                    if (cleaningTaskFullList.GetTasksCleanness()[i] == 1)
                    {
                        switchOn = true;
                    }

                    if (cleaningTaskFullList.GetIsTodayTask()[i])
                    {
                        IsTodayTask = true;

                        TaskCardSimpleList.Add(new TaskCardSimple(cleaningTaskFullList.GetTaskIds()[i],
                        cleaningTaskFullList.GetTaskTitles()[i], timeImageId,
                        cleaningTaskFullList.GetTasksTimeOfCleaning()[i].ToString() + " " + context.GetString(Resource.String.minute_text),
                        cleannessImageId,
                        cleaningTaskFullList.GetTasksCleannessInPercentages()[i].ToString() + "%",
                        cleaningTaskFullList.GetTasksCleanness()[i],
                        RoomDatabaseManagement.GetRoomTitle(context, cleaningTaskFullList.GetRoomIds()[i]),
                        switchOn, dateString,
                        ""));
                    }
                    else
                    {
                        IsTodayTask = false;

                        TaskCardSimpleList.Add(new TaskCardSimple(cleaningTaskFullList.GetTaskIds()[i],
                        cleaningTaskFullList.GetTaskTitles()[i], timeImageId,
                        cleaningTaskFullList.GetTasksTimeOfCleaning()[i].ToString() + " " + context.GetString(Resource.String.minute_text),
                        cleannessImageId,
                        cleaningTaskFullList.GetTasksCleannessInPercentages()[i].ToString() + "%",
                        cleaningTaskFullList.GetTasksCleanness()[i],
                        RoomDatabaseManagement.GetRoomTitle(context, cleaningTaskFullList.GetRoomIds()[i]),
                        switchOn, dateString,
                        context.GetString(Resource.String.transfer_task_to_today_task)));
                    }

                    

                }
            }
        }
    }
}