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

namespace RationalCleaning
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        #region Висувний список та Action Bar

        private SupportToolbar mToolBar;
        private SupportActionBar mActionBar;
        private DrawerLayout mDrawerLayout;
        private NavigationView mNavigationView;

        #endregion

        //private int currentPosition = 0;     // За допомогою цього поля ми виділяємо пункт у висувному списку, який відповідає даній активності

        //private Android.Support.V7.Widget.Toolbar mToolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            #region Висувний список та Action Bar
            mToolBar = FindViewById<SupportToolbar>(Resource.Id.toolBar);
            SetSupportActionBar(mToolBar);

            mActionBar = SupportActionBar;
            mActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
            mActionBar.SetHomeButtonEnabled(true);
            mActionBar.SetDisplayHomeAsUpEnabled(true);

            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            mNavigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            if (mNavigationView != null)
            {
                SetUpDrawerContent(mNavigationView);
            }
            #endregion

        }


        #region Висувний список та Action Bar

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        mDrawerLayout.CloseDrawers();
                        break;
                    case Resource.Id.nav_management:
                        IntentManagement.IntentManagement.RoomManagementIntent(this, -1);
                        break;
                    case Resource.Id.nav_list_of_rooms:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(ListOfRooms));
                        break;
                    case Resource.Id.nav_tasks_for_today:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(TasksForToday));
                        break;
                    case Resource.Id.nav_tasks_for_next_day:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(TasksForNextDay));
                        break;
                    case Resource.Id.nav_list_of_tasks:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(FullListOfTasks));
                        break;

                    case Resource.Id.nav_add_room:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(CreateRoom));
                        break;
                    case Resource.Id.create_cleaning_task:
                        IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, -1);
                        break;
                    default:
                        mDrawerLayout.CloseDrawers();
                        break;
                }

                mNavigationView.Menu.GetItem(0).SetCheckable(true);
                mNavigationView.Menu.GetItem(0).SetChecked(true);


            };

            mNavigationView.SetCheckedItem(Resource.Id.nav_home);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);

                    mNavigationView.Menu.GetItem(0).SetCheckable(true);
                    mNavigationView.Menu.GetItem(0).SetChecked(true);

                    return true;

                case Resource.Id.create_room:
                    IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(CreateRoom));
                    return true;
                case Resource.Id.create_cleaning_task:
                    IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, -1);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;

            //return base.OnCreateOptionsMenu(menu);
        }



        #endregion


        protected override void OnDestroy()
        {

            base.OnDestroy();
        }
    }
}

