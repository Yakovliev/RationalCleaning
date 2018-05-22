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

namespace RationalCleaning.CleaningTasks
{
    public class TaskCardFull : TaskCardSimple
    {
        public TaskCardFull(int taskId, string taskTitle, int timeImageId, string timeOfCleaning,
            int cleannessImageId, string cleanness, int cleannessInteger, string roomTitle, bool switchOn,
            string actionTextViewText, string dateText) 
            : base(taskId, taskTitle, timeImageId, timeOfCleaning, cleannessImageId, cleanness, cleannessInteger, roomTitle,
                  switchOn, actionTextViewText)
        {

        }
    }
}