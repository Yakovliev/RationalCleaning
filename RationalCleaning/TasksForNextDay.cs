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
using RationalCleaning.CleaningTasks;

namespace RationalCleaning
{
    [Activity(Label = "@string/nav_tasks_for_next_day", Theme = "@style/AppTheme")]
    public class TasksForNextDay : AppCompatActivity
    {
        #region Висувний список та Action Bar
        private SupportToolbar mToolBar;
        private SupportActionBar mActionBar;
        private DrawerLayout mDrawerLayout;
        private NavigationView mNavigationView;

        private IMenuItem menuItem;
        #endregion

        private TaskCardSimple taskCardSimple;
        TaskAdapterExtended adapter;

        Android.Support.V7.Widget.RecyclerView tasksForNextDayRecyclerView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.tasks_for_next_day);

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

            tasksForNextDayRecyclerView = FindViewById<Android.Support.V7.Widget.RecyclerView>(Resource.Id.tasks_for_next_day_recycler_view);

            taskCardSimple = new TaskCardSimple();
            taskCardSimple.InitializeTasksForNextDay(this);

            adapter = new TaskAdapterExtended(taskCardSimple.TaskCardSimpleList);
            adapter.ItemClick += OnItemClick;
            adapter.ActionTextViewClick += Adapter_ActionTextViewClick;
            tasksForNextDayRecyclerView.SetAdapter(adapter);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            tasksForNextDayRecyclerView.SetLayoutManager(linearLayoutManager);
            tasksForNextDayRecyclerView.NestedScrollingEnabled = false;

        }

        private void Adapter_ActionTextViewClick(object sender, int position)
        {
            new AsyncUpdateCleannessOfTask(this, taskCardSimple.TaskCardSimpleList[position].TaskId, 0).Execute(1);
            taskCardSimple.TaskCardSimpleList.RemoveAt(position);
            adapter.NotifyDataSetChanged();
        }

        private void OnItemClick(object sender, int position)
        {
            IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, taskCardSimple.TaskCardSimpleList[position].TaskId,
                taskCardSimple.TaskCardSimpleList[position].GetRoomId(this));
        }


        #region Висувний список та Action Bar
        //Задаємо Action Bar

        private void SetUpDrawerContent(NavigationView navigationView)
        {
            navigationView.NavigationItemSelected += (object sender, NavigationView.NavigationItemSelectedEventArgs e) =>
            {
                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home:
                        IntentManagement.IntentManagement.CreateSimpleIntent(this, typeof(MainActivity));
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
                        mDrawerLayout.CloseDrawers();
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

            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            menuItem = menu.FindItem(Resource.Id.create_room);
            menuItem.SetVisible(false);
            this.InvalidateOptionsMenu();
            return true;
        }

        //Реагуємо на кліки по Action Bar та висувного списку
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mDrawerLayout.OpenDrawer((int)GravityFlags.Left);

                    mNavigationView.Menu.FindItem(Resource.Id.nav_tasks_for_next_day).SetCheckable(true);
                    mNavigationView.Menu.FindItem(Resource.Id.nav_tasks_for_next_day).SetChecked(true);

                    return true;
                case Resource.Id.create_cleaning_task:
                    IntentManagement.IntentManagement.CreateIntentToUpdateCleaningTask(this, -1, -1);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);

            }
        }

        #endregion

    }
}