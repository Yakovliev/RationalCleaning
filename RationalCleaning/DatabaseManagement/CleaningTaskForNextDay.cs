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
    public class CleaningTaskForNextDay
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

        #endregion

        #region Tasks for Next Day Fields and Properties
        private int[] taskIdsForNextDay;

        private int[] tasksTimeOfCleaningForNextDay;

        private int[] tasksCleannessForNextDay;

        private int[] tasksCleannessInPercentagesForNextDay;

        private string[] taskTitlesForNextDay;

        private int[] roomIdsForNextDay;

        private int[] yearForNextDay;

        private int[] monthForNextDay;

        private int[] dayOfMonthForNextDay;

        private int[] dateDefaultForNextDay;

        private int[] yearOfChangeForNextDay;

        private int[] monthOfChangeForNextDay;

        private int[] dayOfMonthOfChangeForNextDay;

        private int[] periodicityForNextDay;

        private int[] hourForNextDay;

        private int[] minuteForNextDay;

        private string[] dateOfTaskStringForNextDay;

        public int[] GetTaskIdsForNextDay()
        {
            if (taskIdsForNextDay != null)
            {
                return (int[])taskIdsForNextDay.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetTasksTimeOfCleaningForNextDay()
        {
            if (taskIdsForNextDay != null)
            {
                return (int[])tasksTimeOfCleaningForNextDay.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetTasksCleannessForNextDay()
        {
            if (taskIdsForNextDay != null)
            {
                return (int[])tasksCleannessForNextDay.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetTasksCleannessInPercentagesForNextDay()
        {
            if (taskIdsForNextDay != null)
            {
                return (int[])tasksCleannessInPercentagesForNextDay.Clone();
            }
            else
            {
                return null;
            }
        }

        public string[] GetTaskTitlesForNextDay()
        {
            if (taskIdsForNextDay != null)
            {
                return (string[])taskTitlesForNextDay.Clone();
            }
            else
            {
                return null;
            }
        }

        public int[] GetRoomIdsForNextDay()
        {
            if (taskIdsForNextDay != null)
            {
                return (int[])roomIdsForNextDay.Clone();
            }
            else
            {
                return null;
            }
        }


        private int countOfTasksForNextDay = 0;

        public CleaningTaskForNextDay(Context context)
        {
            this.context = context;
        }
        #endregion

        /// <summary>
        /// Кількість днів до наступного дня після сьогоднішнього, на який заплановані задачі. Якщо нуль, то значить є таски на завтра.
        /// </summary>
        public int DaysToNextCleaningDay { get; private set; }

        private bool notEnough = true;

        /// <summary>
        /// Дата того дня, на який запланований хоч один таск і який найближчий до сьогоднішнього дня
        /// </summary>
        public DateTime DateTimeOfTasksForNextDay { get; private set; }

        private DateTime dateTimeNow;

        private DateTime dateTimeTomorrow;

        public void CalculateTasksForNextDay()
        {
            dateTimeNow = DateTime.Now;

            dateTimeTomorrow = dateTimeNow.AddDays(1);

            DaysToNextCleaningDay = 0;

            GetAllTasksFromDatabase();

            if (taskIds != null)
            {
                OrderByRoomId();

                CalculateCountOfTasksForNextDay();

                InitializeArraysForNextDay();

                DateTime dateTimeNow = DateTime.Now;            

                DateTimeOfTasksForNextDay = dateTimeNow.AddDays(DaysToNextCleaningDay + 1);
            }


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

        private void CalculateCountOfTasksForNextDay()
        {
            bool notEnough = true;

            for (int i = 0; i < taskIds.Length; i++)
            {
                //Для сьогоднішніх тасків. Сьогоднішні таски можуть перекриватися із тасками на наступний день у тому випадку, коли таск декілька днів не виконується
                DateTime dateTimeDB = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime dateTimeNowWithoutHoursAndMinutes = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
                ///////////////////////////////////////////////

                DateTime dateTimeAddSeveralDays = GetDateTimePlusFewDays(DateTime.Now, DaysToNextCleaningDay + 1);
                DateTime dateTime = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime datePlusPeriodicity = GetDateTimePlusFewDays(dateTime, periodicity[i]);
                DateTime dateTimeOfChange = new DateTime(yearOfChange[i], monthOfChange[i] + 1, dayOfMonthOfChange[i]);

                if ((dateDefault[i] == 0 && dateTime.Year == dateTimeAddSeveralDays.Year &&
                    dateTime.Month == dateTimeAddSeveralDays.Month &&
                    dateTime.Day == dateTimeAddSeveralDays.Day) ||
                    (dateDefault[i] == 1 && datePlusPeriodicity.Year == dateTimeAddSeveralDays.Year &&
                    datePlusPeriodicity.Month == dateTimeAddSeveralDays.Month &&
                    datePlusPeriodicity.Day == dateTimeAddSeveralDays.Day) ||
                    (dateTimeOfChange.Year == dateTimeAddSeveralDays.Year &&
                    dateTimeOfChange.Month == dateTimeAddSeveralDays.Month &&
                    dateTimeOfChange.Day == dateTimeAddSeveralDays.Day))
                {
                    if ((dateDefault[i] == 1 && dateTimePlusPeriodicity[i].Year == dateTimeNow.Year &&
                        dateTimePlusPeriodicity[i].Month == dateTimeNow.Month && dateTimePlusPeriodicity[i].Day == dateTimeNow.Day) ||
                        (yearOfChange[i] == dateTimeNow.Year && monthOfChange[i] + 1 == dateTimeNow.Month && dayOfMonthOfChange[i] == dateTimeNow.Day) ||
                        (dateDefault[i] == 1 && tasksCleanness[i] == 0 &&
                        !(yearOfChange[i] == dateTimeTomorrow.Year && monthOfChange[i] + 1 == dateTimeTomorrow.Month && dayOfMonthOfChange[i] == dateTimeTomorrow.Day)) ||
                        (/*dateDefault[i] == 0 &&*/ year[i] == dateTimeNow.Year && month[i] + 1 == dateTimeNow.Month && dayOfMonth[i] == dateTimeNow.Day) ||
                        (tasksCleanness[i] == 0 && dateDefault[i] == 0 && dateTimeDB <= dateTimeNowWithoutHoursAndMinutes))
                    {
                        //Якщо ця умова виконається - значить сьогоднішній таск перекривається із таксом на наступний день. У такому випадку його необхідно прибрати. Тому тут ми нічого й не робимо
                    }
                    else
                    {
                        notEnough = false;
                        break;
                    }
                        
                }
            }

            if (notEnough)
            {
                DaysToNextCleaningDay++;
            }

            if (notEnough)
            {
                int counter = 0;
                while (notEnough)
                {
                    for (int i = 0; i < taskIds.Length; i++)
                    {
                        //Для сьогоднішніх тасків. Сьогоднішні таски можуть перекриватися із тасками на наступний день у тому випадку, коли таск декілька днів не виконується
                        DateTime dateTimeDB = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                        DateTime dateTimeNowWithoutHoursAndMinutes = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
                        ///////////////////////////////////////////////

                        DateTime dateTimeAddSeveralDays = GetDateTimePlusFewDays(DateTime.Now, DaysToNextCleaningDay + 1);
                        DateTime dateTime = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                        DateTime datePlusPeriodicity = GetDateTimePlusFewDays(dateTime, periodicity[i]);

                        if ((dateDefault[i] == 0 && dateTime.Year == dateTimeAddSeveralDays.Year &&
                            dateTime.Month == dateTimeAddSeveralDays.Month &&
                            dateTime.Day == dateTimeAddSeveralDays.Day) ||
                            (dateDefault[i] == 1 && datePlusPeriodicity.Year == dateTimeAddSeveralDays.Year &&
                            datePlusPeriodicity.Month == dateTimeAddSeveralDays.Month &&
                            datePlusPeriodicity.Day == dateTimeAddSeveralDays.Day))
                        {
                            if ((dateDefault[i] == 1 && dateTimePlusPeriodicity[i].Year == dateTimeNow.Year &&
                                dateTimePlusPeriodicity[i].Month == dateTimeNow.Month && dateTimePlusPeriodicity[i].Day == dateTimeNow.Day) ||
                                (yearOfChange[i] == dateTimeNow.Year && monthOfChange[i] + 1 == dateTimeNow.Month && dayOfMonthOfChange[i] == dateTimeNow.Day) ||
                                (dateDefault[i] == 1 && tasksCleanness[i] == 0 &&
                                !(yearOfChange[i] == dateTimeTomorrow.Year && monthOfChange[i] + 1 == dateTimeTomorrow.Month && dayOfMonthOfChange[i] == dateTimeTomorrow.Day)) ||
                                (/*dateDefault[i] == 0 &&*/ year[i] == dateTimeNow.Year && month[i] + 1 == dateTimeNow.Month && dayOfMonth[i] == dateTimeNow.Day) ||
                                (tasksCleanness[i] == 0 && dateDefault[i] == 0 && dateTimeDB <= dateTimeNowWithoutHoursAndMinutes))
                            {
                                //Якщо ця умова виконається - значить сьогоднішній таск перекривається із таксом на наступний день. У такому випадку його необхідно прибрати. Тому тут ми нічого й не робимо
                            }
                            else
                            {
                                notEnough = false;
                                break;
                            }
                        }
                    }

                    if (notEnough)
                    {
                        DaysToNextCleaningDay++;
                    }

                    counter++;

                    //Про всяк випадок ставимо обов'язкову умову виходу з циклу, яка буде досягатися рано чи пізно. Щоб не виникло випадково ситуації зациклювання
                    if (counter > 1000)
                    {
                        break;
                    }
                }
            }

            for (int i = 0; i < taskIds.Length; i++)
            {
                //Для сьогоднішніх тасків. Сьогоднішні таски можуть перекриватися із тасками на наступний день у тому випадку, коли таск декілька днів не виконується
                DateTime dateTimeDB = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime dateTimeNowWithoutHoursAndMinutes = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
                ///////////////////////////////////////////////

                DateTime dateTimeAddSeveralDays = GetDateTimePlusFewDays(DateTime.Now, DaysToNextCleaningDay + 1);
                DateTime dateTime = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime datePlusPeriodicity = GetDateTimePlusFewDays(dateTime, periodicity[i]);
                DateTime dateTimeOfChange = new DateTime(yearOfChange[i], monthOfChange[i] + 1, dayOfMonthOfChange[i]);

                if ((dateDefault[i] == 0 && dateTime.Year == dateTimeAddSeveralDays.Year &&
                    dateTime.Month == dateTimeAddSeveralDays.Month &&
                    dateTime.Day == dateTimeAddSeveralDays.Day) ||
                    (dateDefault[i] == 1 && datePlusPeriodicity.Year == dateTimeAddSeveralDays.Year &&
                    datePlusPeriodicity.Month == dateTimeAddSeveralDays.Month &&
                    datePlusPeriodicity.Day == dateTimeAddSeveralDays.Day) ||
                    (dateTimeOfChange.Year == dateTimeAddSeveralDays.Year &&
                    dateTimeOfChange.Month == dateTimeAddSeveralDays.Month &&
                    dateTimeOfChange.Day == dateTimeAddSeveralDays.Day))
                {
                    if ((dateDefault[i] == 1 && dateTimePlusPeriodicity[i].Year == dateTimeNow.Year &&
                        dateTimePlusPeriodicity[i].Month == dateTimeNow.Month && dateTimePlusPeriodicity[i].Day == dateTimeNow.Day) ||
                        (yearOfChange[i] == dateTimeNow.Year && monthOfChange[i] + 1 == dateTimeNow.Month && dayOfMonthOfChange[i] == dateTimeNow.Day) ||
                        (dateDefault[i] == 1 && tasksCleanness[i] == 0 &&
                        !(yearOfChange[i] == dateTimeTomorrow.Year && monthOfChange[i] + 1 == dateTimeTomorrow.Month && dayOfMonthOfChange[i] == dateTimeTomorrow.Day)) ||
                        (/*dateDefault[i] == 0 &&*/ year[i] == dateTimeNow.Year && month[i] + 1 == dateTimeNow.Month && dayOfMonth[i] == dateTimeNow.Day) ||
                        (tasksCleanness[i] == 0 && dateDefault[i] == 0 && dateTimeDB <= dateTimeNowWithoutHoursAndMinutes))
                    {
                        //Якщо ця умова виконається - значить сьогоднішній таск перекривається із таксом на наступний день. У такому випадку його необхідно прибрати. Тому тут ми нічого й не робимо
                    }
                    else
                    {
                        countOfTasksForNextDay++;
                        continue;
                    }
                    
                }
            }   
        }

        private DateTime GetDateTimePlusFewDays(DateTime dateTime, int numberOfDays)
        {
            DateTime necessaryDateTime;

            necessaryDateTime = dateTime.AddDays(numberOfDays);

            return necessaryDateTime;
        }
        
        private void InitializeArraysForNextDay()
        {
            taskIdsForNextDay = new int[countOfTasksForNextDay];
            tasksTimeOfCleaningForNextDay = new int[countOfTasksForNextDay];
            tasksCleannessForNextDay = new int[countOfTasksForNextDay];
            tasksCleannessInPercentagesForNextDay = new int[countOfTasksForNextDay];
            taskTitlesForNextDay = new string[countOfTasksForNextDay];
            roomIdsForNextDay = new int[countOfTasksForNextDay];

            int counter = 0;

            for (int i = 0; i < taskIds.Length; i++)
            {
                //Для сьогоднішніх тасків. Сьогоднішні таски можуть перекриватися із тасками на наступний день у тому випадку, коли таск декілька днів не виконується
                DateTime dateTimeDB = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime dateTimeNowWithoutHoursAndMinutes = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day);
                ///////////////////////////////////////////////

                DateTime dateTimeAddSeveralDays = GetDateTimePlusFewDays(DateTime.Now, DaysToNextCleaningDay + 1);
                DateTime dateTime = new DateTime(year[i], month[i] + 1, dayOfMonth[i]);
                DateTime datePlusPeriodicity = GetDateTimePlusFewDays(dateTime, periodicity[i]);
                DateTime dateTimeOfChange = new DateTime(yearOfChange[i], monthOfChange[i] + 1, dayOfMonthOfChange[i]);

                if ((dateDefault[i] == 0 && dateTime.Year == dateTimeAddSeveralDays.Year &&
                    dateTime.Month == dateTimeAddSeveralDays.Month &&
                    dateTime.Day == dateTimeAddSeveralDays.Day) ||
                    (dateDefault[i] == 1 && datePlusPeriodicity.Year == dateTimeAddSeveralDays.Year &&
                    datePlusPeriodicity.Month == dateTimeAddSeveralDays.Month &&
                    datePlusPeriodicity.Day == dateTimeAddSeveralDays.Day) ||
                    (dateTimeOfChange.Year == dateTimeAddSeveralDays.Year &&
                    dateTimeOfChange.Month == dateTimeAddSeveralDays.Month &&
                    dateTimeOfChange.Day == dateTimeAddSeveralDays.Day))
                {
                    if ((dateDefault[i] == 1 && dateTimePlusPeriodicity[i].Year == dateTimeNow.Year &&
                        dateTimePlusPeriodicity[i].Month == dateTimeNow.Month && dateTimePlusPeriodicity[i].Day == dateTimeNow.Day) ||
                        (yearOfChange[i] == dateTimeNow.Year && monthOfChange[i] + 1 == dateTimeNow.Month && dayOfMonthOfChange[i] == dateTimeNow.Day) ||
                        (dateDefault[i] == 1 && tasksCleanness[i] == 0 &&
                        !(yearOfChange[i] == dateTimeTomorrow.Year && monthOfChange[i] + 1 == dateTimeTomorrow.Month && dayOfMonthOfChange[i] == dateTimeTomorrow.Day)) ||
                        (/*dateDefault[i] == 0 &&*/ year[i] == dateTimeNow.Year && month[i] + 1 == dateTimeNow.Month && dayOfMonth[i] == dateTimeNow.Day) ||
                        (tasksCleanness[i] == 0 && dateDefault[i] == 0 && dateTimeDB <= dateTimeNowWithoutHoursAndMinutes))
                    {
                        //Якщо ця умова виконається - значить сьогоднішній таск перекривається із таксом на наступний день. У такому випадку його необхідно прибрати. Тому тут ми нічого й не робимо
                    }
                    else
                    {
                        taskIdsForNextDay[counter] = taskIds[i];
                        tasksTimeOfCleaningForNextDay[counter] = tasksTimeOfCleaning[i];
                        tasksCleannessForNextDay[counter] = tasksCleanness[i];
                        tasksCleannessInPercentagesForNextDay[counter] = tasksCleannessInPercentages[i];
                        taskTitlesForNextDay[counter] = taskTitles[i];
                        roomIdsForNextDay[counter] = roomIds[i];

                        counter++;

                        continue;
                    }
             
                }

            }
        }
    }
}