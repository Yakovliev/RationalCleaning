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
    public class CleaningTaskForToday
    {
        #region Fields and Properties
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

        private DateTime[] dateTimePlusPeriodicity;



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

        public int[] GetRoomIds()
        {
            return (int[])roomIds.Clone();
        }

        #region Tasks for Today
        private int[] taskIdsForToday;

        private int[] tasksTimeOfCleaningForToday;

        private int[] tasksCleannessForToday;

        private int[] tasksCleannessInPercentagesForToday;

        private string[] taskTitlesForToday;

        private int[] roomIdsForToday;

        private int[] yearForToday;

        private int[] monthForToday;

        private int[] dayOfMonthForToday;

        private int[] dateDefaultForToday;

        private int[] yearOfChangeForToday;

        private int[] monthOfChangeForToday;

        private int[] dayOfMonthOfChangeForToday;

        private int[] periodicityForToday;

        private int[] hourForToday;

        private int[] minuteForToday;

        private string[] dateOfTaskStringForToday;

        public int[] GetTaskIdsForToday()
        {
            if (taskIdsForToday != null)
            {
                return (int[])taskIdsForToday.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetTasksTimeOfCleaningForToday()
        {
            if (taskIdsForToday != null)
            {
                return (int[])tasksTimeOfCleaningForToday.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetTasksCleannessForToday()
        {
            if (taskIdsForToday != null)
            {
                return (int[])tasksCleannessForToday.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetTasksCleannessInPercentagesForToday()
        {
            if (taskIdsForToday != null)
            {
                return (int[])tasksCleannessInPercentagesForToday.Clone();
            }
            else
            {
                return null;
            }
        }

        public string[] GetTaskTitlesForToday()
        {
            if (taskIdsForToday != null)
            {
                return (string[])taskTitlesForToday.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetRoomIdsForToday()
        {
            if (taskIdsForToday != null)
            {
                return (int[])roomIdsForToday.Clone();
            }
            else
            {
                return null;
            }
        }


        private int countOfTodayTasks = 0;

        #endregion
        #endregion

        private DateTime dateTimeNow;

        public CleaningTaskForToday(Context context)
        {
            this.context = context;
        }


        public void CalculateTasksForToday()
        {
            GetAllTasksFromDatabase();

            OrderByRoomId();

            CalculateCountOfTodayTasks();

            InitializeArraysForToday();

        }


        private void GetAllTasksFromDatabase()
        {
            SQLiteOpenHelper rationalCleaningDatabaseHelper = new RationalCleaningDatabaseHelper(context);
            db = rationalCleaningDatabaseHelper.ReadableDatabase;
            taskCursor = db.Query("CLEANING_TASK_TABLE",
                new string[] { "_id", "TITLE", "ROOM_ID", "YEAR", "MONTH", "DAY_OF_MONTH", "DATE_DEFAULT", "HOUR", "MINUTE", "YEAR_OF_CHANGE", "MONTH_OF_CHANGE", "DAY_OF_MONTH_OF_CHANGE",
                "PERIODICITY", "TIME_OF_CLEANING", "CLEANNESS"},
                null, null, null, null, null);


            taskIds = new int[taskCursor.Count];
            tasksTimeOfCleaning = new int[taskCursor.Count];
            tasksCleanness = new int[taskCursor.Count];
            taskTitles = new string[taskCursor.Count];

            roomIds = new int[taskCursor.Count];
            year = new int[taskCursor.Count];
            month = new int[taskCursor.Count];
            dayOfMonth = new int[taskCursor.Count];
            dateDefault = new int[taskCursor.Count];
            yearOfChange = new int[taskCursor.Count];
            monthOfChange = new int[taskCursor.Count];
            dayOfMonthOfChange = new int[taskCursor.Count];
            periodicity = new int[taskCursor.Count];
            hour = new int[taskCursor.Count];
            minute = new int[taskCursor.Count];
            dateOfTaskString = new string[taskCursor.Count];   //Цю штуку ніде не опрацьовуємо
            dateTimePlusPeriodicity = new DateTime[taskCursor.Count];
            tasksCleannessInPercentages = new int[taskCursor.Count];

            if (taskCursor.MoveToFirst())
            {
                taskIds[0] = taskCursor.GetInt(0);
                taskTitles[0] = taskCursor.GetString(1);
                roomIds[0] = taskCursor.GetInt(2);
                year[0] = taskCursor.GetInt(3);
                month[0] = taskCursor.GetInt(4);
                dayOfMonth[0] = taskCursor.GetInt(5);
                dateDefault[0] = taskCursor.GetInt(6);
                hour[0] = taskCursor.GetInt(7);
                minute[0] = taskCursor.GetInt(8);
                yearOfChange[0] = taskCursor.GetInt(9);
                monthOfChange[0] = taskCursor.GetInt(10);
                dayOfMonthOfChange[0] = taskCursor.GetInt(11);
                periodicity[0] = taskCursor.GetInt(12);
                tasksTimeOfCleaning[0] = taskCursor.GetInt(13);
                tasksCleanness[0] = taskCursor.GetInt(14);
            }

            for (int i = 1; i < taskCursor.Count; i++)
            {
                if (taskCursor.MoveToNext())
                {
                    taskIds[i] = taskCursor.GetInt(0);
                    taskTitles[i] = taskCursor.GetString(1);
                    roomIds[i] = taskCursor.GetInt(2);
                    year[i] = taskCursor.GetInt(3);
                    month[i] = taskCursor.GetInt(4);
                    dayOfMonth[i] = taskCursor.GetInt(5);
                    dateDefault[i] = taskCursor.GetInt(6);
                    hour[i] = taskCursor.GetInt(7);
                    minute[i] = taskCursor.GetInt(8);
                    yearOfChange[i] = taskCursor.GetInt(9);
                    monthOfChange[i] = taskCursor.GetInt(10);
                    dayOfMonthOfChange[i] = taskCursor.GetInt(11);
                    periodicity[i] = taskCursor.GetInt(12);
                    tasksTimeOfCleaning[i] = taskCursor.GetInt(13);
                    tasksCleanness[i] = taskCursor.GetInt(14);
                }
            }

            taskCursor.Close();
            db.Close();

            InitializeTasksCleannessInPercentages();
        }

        /// <summary>
        /// Ініціалізуємо значення однойменного масиву. 0% - завдання не виконано, 100% - завдання виконано
        /// </summary>
        private void InitializeTasksCleannessInPercentages()
        {
            for (int i = 0; i < tasksCleannessInPercentages.Length; i++)
            {
                if (tasksCleanness[i] == 0)
                {
                    tasksCleannessInPercentages[i] = 0;
                }
                else if (tasksCleanness[i] == 1)
                {
                    tasksCleannessInPercentages[i] = 100;
                }
            }
        }

        private void CalculateCountOfTodayTasks()
        {
            dateTimeNow = DateTime.Now;

            CalculateDateTimePlusPeriodicity();
            
            for (int i = 0; i < taskIds.Length; i++)
            {
                DateTime dateTimeDB = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime dateTimeNowWithoutHoursAndMinutes = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
                DateTime dateTimeTomorrow = GetTomorrowDate();

                //Опис чому такі умови беруться див в RationalCleaningDatabaseHelper в коментарях
                if ((dateDefault[i] == 1 && dateTimePlusPeriodicity[i].Year == dateTimeNow.Year &&  
                    dateTimePlusPeriodicity[i].Month == dateTimeNow.Month && dateTimePlusPeriodicity[i].Day == dateTimeNow.Day) ||
                    (yearOfChange[i] == dateTimeNow.Year && monthOfChange[i] + 1 == dateTimeNow.Month && dayOfMonthOfChange[i] == dateTimeNow.Day) ||
                    (dateDefault[i] == 1 && tasksCleanness[i] == 0 &&
                        !(yearOfChange[i] == dateTimeTomorrow.Year && monthOfChange[i] + 1 == dateTimeTomorrow.Month && dayOfMonthOfChange[i] == dateTimeTomorrow.Day)) ||
                    (/*dateDefault[i] == 0 &&*/ year[i] == dateTimeNow.Year && month[i] + 1 == dateTimeNow.Month && dayOfMonth[i] == dateTimeNow.Day) ||
                    (/*tasksCleanness[i] == 0 &&*/ dateDefault[i] == 0 && dateTimeDB <= dateTimeNowWithoutHoursAndMinutes) ||
                    (dateDefault[i] == 1 && dateTimePlusPeriodicity[i] <= dateTimeNowWithoutHoursAndMinutes))
                {
                    countOfTodayTasks++;
                }
            }

        }

        /// <summary>
        /// return = DateTime from database + periosicity
        /// </summary>
        private void CalculateDateTimePlusPeriodicity()
        {
            for (int i = 0; i < dateTimePlusPeriodicity.Length; i++)
            {
                dateTimePlusPeriodicity[i] = AddDateTimePlusPeriodicity(year[i], month[i], dayOfMonth[i], periodicity[i]);
            }
        }

        /// <summary>
        /// return = DateTime from database + periosicity
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="dayOfMonth"></param>
        /// <param name="periodicity"></param>
        /// <returns>return = DateTime from database + periosicity</returns>
        private DateTime AddDateTimePlusPeriodicity(int year, int month, int dayOfMonth, int periodicity)
        {
            DateTime dateTime;

            DateTime someDateTime;

            someDateTime = new DateTime(year, month + 1, dayOfMonth);

            dateTime = someDateTime.AddDays(periodicity);

            return dateTime;

        }

        private void InitializeArraysForToday()
        {
            if (countOfTodayTasks > 0)
            {
                taskIdsForToday = new int[countOfTodayTasks];
                tasksTimeOfCleaningForToday = new int[countOfTodayTasks];
                tasksCleannessForToday = new int[countOfTodayTasks];
                tasksCleannessInPercentagesForToday = new int[countOfTodayTasks];
                taskTitlesForToday = new string[countOfTodayTasks];
                roomIdsForToday = new int[countOfTodayTasks];
                yearForToday = new int[countOfTodayTasks];
                monthForToday = new int[countOfTodayTasks];
                dayOfMonthForToday = new int[countOfTodayTasks];
                dateDefaultForToday = new int[countOfTodayTasks];
                yearOfChangeForToday = new int[countOfTodayTasks];
                monthOfChangeForToday = new int[countOfTodayTasks];
                dayOfMonthOfChangeForToday = new int[countOfTodayTasks];
                periodicityForToday = new int[countOfTodayTasks];
                hourForToday = new int[countOfTodayTasks];
                minuteForToday = new int[countOfTodayTasks];
                dateOfTaskStringForToday = new string[countOfTodayTasks];

                int counter = 0;

                for (int i = 0; i < taskIds.Length; i++)
                {
                    DateTime dateTimeDB = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                    DateTime dateTimeNowWithoutHoursAndMinutes = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
                    DateTime dateTimeTomorrow = GetTomorrowDate();

                    //Опис чому такі умови беруться див в RationalCleaningDatabaseHelper в коментарях
                    if ((dateDefault[i] == 1 && dateTimePlusPeriodicity[i].Year == dateTimeNow.Year &&
                        dateTimePlusPeriodicity[i].Month == dateTimeNow.Month && dateTimePlusPeriodicity[i].Day == dateTimeNow.Day) ||
                        (yearOfChange[i] == dateTimeNow.Year && monthOfChange[i] + 1 == dateTimeNow.Month && dayOfMonthOfChange[i] == dateTimeNow.Day) ||
                        (dateDefault[i] == 1 && tasksCleanness[i] == 0 &&
                        !(yearOfChange[i] == dateTimeTomorrow.Year && monthOfChange[i] + 1 == dateTimeTomorrow.Month && dayOfMonthOfChange[i] == dateTimeTomorrow.Day)) ||
                        (/*dateDefault[i] == 0 &&*/ year[i] == dateTimeNow.Year && month[i] + 1 == dateTimeNow.Month && dayOfMonth[i] == dateTimeNow.Day) ||
                        (/*tasksCleanness[i] == 0 && */ dateDefault[i] == 0 && dateTimeDB <= dateTimeNowWithoutHoursAndMinutes) ||
                        (dateDefault[i] == 1 && dateTimePlusPeriodicity[i] <= dateTimeNowWithoutHoursAndMinutes))
                    {
                        taskIdsForToday[counter] = taskIds[i];
                        tasksTimeOfCleaningForToday[counter] = tasksTimeOfCleaning[i];
                        tasksCleannessForToday[counter] = tasksCleanness[i];
                        tasksCleannessInPercentagesForToday[counter] = tasksCleannessInPercentages[i];
                        taskTitlesForToday[counter] = taskTitles[i];
                        roomIdsForToday[counter] = roomIds[i];
                        yearForToday[counter] = year[i];
                        monthForToday[counter] = month[i];
                        dayOfMonthForToday[counter] = dayOfMonth[i];
                        dateDefaultForToday[counter] = dateDefault[i];
                        yearOfChangeForToday[counter] = yearOfChange[i];
                        monthOfChangeForToday[counter] = monthOfChange[i];
                        dayOfMonthOfChangeForToday[counter] = dayOfMonthOfChange[i];
                        periodicityForToday[counter] = periodicity[i];
                        hourForToday[counter] = hour[i];
                        minuteForToday[counter] = minute[i];

                        counter++;
                    }
                }
            }          
        }


        private void OrderByRoomId()
        {
            RoomDatabaseManagement roomDatabaseManagement = new RoomDatabaseManagement();
            roomDatabaseManagement.FindRightOrderOfRoomTitles(context);

            //Упорядковані айдішніки кімнат
            int[] orderRoomIds = roomDatabaseManagement.GetRoomIds();

            int[] taskIdsSpare = new int[taskIds.Length];
            int[] tasksTimeOfCleaningSpare = new int[taskIds.Length];
            int[] tasksCleannessSpare = new int[taskIds.Length];
            string[] taskTitlesSpare = new string[taskIds.Length];

            int[] roomIdsSpare = new int[taskIds.Length];
            int[] yearSpare = new int[taskIds.Length];
            int[] monthSpare = new int[taskIds.Length];
            int[] dayOfMonthSpare = new int[taskIds.Length];
            int[] dateDefaultSpare = new int[taskIds.Length];
            int[] yearOfChangeSpare = new int[taskIds.Length];
            int[] monthOfChangeSpare = new int[taskIds.Length];
            int[] dayOfMonthOfChangeSpare = new int[taskIds.Length];
            int[] periodicitySpare = new int[taskIds.Length];
            int[] hourSpare = new int[taskIds.Length];
            int[] minuteSpare = new int[taskIds.Length];
            //string[] dateOfTaskStringSpare = new string[taskIds.Length];
            DateTime[] dateTimePlusPeriodicitySpare = new DateTime[taskIds.Length];
            int[] tasksCleannessInPercentagesSpare = new int[taskIds.Length];

            int counter = 0;

            for (int i = 0; i < orderRoomIds.Length; i++)
            {
                for (int j = 0; j < roomIdsSpare.Length; j++)
                {
                    if (roomIds[j] == orderRoomIds[i])
                    {
                        taskIdsSpare[counter] = taskIds[j];
                        tasksTimeOfCleaningSpare[counter] = tasksTimeOfCleaning[j];
                        tasksCleannessSpare[counter] = tasksCleanness[j];
                        taskTitlesSpare[counter] = taskTitles[j];
                        roomIdsSpare[counter] = roomIds[j];
                        yearSpare[counter] = year[j];
                        monthSpare[counter] = month[j];
                        dayOfMonthSpare[counter] = dayOfMonth[j];
                        dateDefaultSpare[counter] = dateDefault[j];
                        yearOfChangeSpare[counter] = yearOfChange[j];
                        monthOfChangeSpare[counter] = monthOfChange[j];
                        dayOfMonthOfChangeSpare[counter] = dayOfMonthOfChange[j];
                        periodicitySpare[counter] = periodicity[j];
                        hourSpare[counter] = hour[j];
                        minuteSpare[counter] = minute[j];
                        tasksCleannessInPercentagesSpare[counter] = tasksCleannessInPercentages[j];

                        counter++;
                    }
                }
            }


            for (int i = 0; i < roomIds.Length; i++)
            {
                taskIds[i] = taskIdsSpare[i];
                tasksTimeOfCleaning[i] = tasksTimeOfCleaningSpare[i];
                tasksCleanness[i] = tasksCleannessSpare[i];
                taskTitles[i] = taskTitlesSpare[i];
                roomIds[i] = roomIdsSpare[i];
                year[i] = yearSpare[i];
                month[i] = monthSpare[i];
                dayOfMonth[i] = dayOfMonthSpare[i];
                dateDefault[i] = dateDefaultSpare[i];
                yearOfChange[i] = yearOfChangeSpare[i];
                monthOfChange[i] = monthOfChangeSpare[i];
                dayOfMonthOfChange[i] = dayOfMonthOfChangeSpare[i];
                periodicity[i] = periodicitySpare[i];
                hour[i] = hourSpare[i];
                minute[i] = minuteSpare[i];
                tasksCleannessInPercentages[i] = tasksCleannessInPercentagesSpare[i];
            }

        }

        public void UpdateCleannessToZero()
        {
            if (tasksCleannessForToday != null)
            {
                for (int i = 0; i < tasksCleannessForToday.Length; i++)
                {
                    if (tasksCleannessForToday[i] == 1)
                    {
                        if (yearOfChangeForToday[i] == DateTime.Now.Year && monthOfChangeForToday[i] + 1 == DateTime.Now.Month &&
                            dayOfMonthOfChangeForToday[i] == DateTime.Now.Day)
                        {
                            //Нічого не робимо
                        }
                        else
                        {
                            tasksCleannessForToday[i] = 0;
                            tasksCleannessInPercentagesForToday[i] = 0;
                            new AsyncUpdateCleannessOfTask(context, taskIdsForToday[i], tasksCleannessForToday[i]).Execute(1);

                        }
                    }
                }
            }
            
        }


        private DateTime GetTomorrowDate()
        {
            DateTime dateTime = DateTime.Now;

            int year = dateTime.Year;
            int month = dateTime.Month - 1;
            int dayOfMonth = dateTime.Day;

            if (month == 0)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if ((year % 4) == 0 && month == 1)
            {
                if (dayOfMonth + 1 <= 29)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 29;
                }
            }
            else if (month == 1)
            {
                if (dayOfMonth + 1 <= 28)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 28;
                }
            }
            else if (month == 2)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 3)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 4)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 5)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 6)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 7)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 8)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 9)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 31;
                }
            }
            else if (month == 10)
            {
                if (dayOfMonth + 1 <= 30)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month++;
                    dayOfMonth = dayOfMonth + 1 - 30;
                }
            }
            else if (month == 11)
            {
                if (dayOfMonth + 1 <= 31)
                {
                    dayOfMonth += 1;
                }
                else
                {
                    month = 0;
                    dayOfMonth = dayOfMonth + 1 - 31;
                    year++;
                }
            }

            dateTime = new DateTime(year, month + 1, dayOfMonth);

            return dateTime;

        }
    }
}