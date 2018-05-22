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

namespace RationalCleaning.Dialog
{
    public class EventArgsForTextDialogFragment : EventArgs
    {
        public string Name { get; private set; }
        public EventArgsForTextDialogFragment(string name)
        {
            Name = name;
        }
    }
}