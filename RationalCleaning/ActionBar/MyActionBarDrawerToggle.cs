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
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace RationalCleaning.ActionBar
{
    public class MyActionBarDrawerToggle : ActionBarDrawerToggle //Цей клас потрібен для керування Action Bar
                                                                 //Зокрема він може допомогти в тому, якщо нам захочеться 
                                                                 //змінювати вигляд Action Bar, коли висувний список розкривається
    {
        private Activity mActivity;
        private int mOpenedResource;
        private int mClosedResource;

        public MyActionBarDrawerToggle(Activity activity, DrawerLayout drawerLayout, int openedResource, int closedResource)
            : base(activity, drawerLayout, openedResource, closedResource)
        {
            mActivity = activity;
            mOpenedResource = openedResource;
            mClosedResource = closedResource;
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);
        }
    }
}