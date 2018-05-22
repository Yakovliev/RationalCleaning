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
using Android.Database.Sqlite;

namespace RationalCleaning.Database
{
    class RationalCleaningDatabaseHelper : SQLiteOpenHelper
    {
        private const string DB_NAME = "RATIONAL_CLEANING_DATABASE";
        private const int DB_VERSION = 1;

        private string anotherTasksString;
        private string tasksForWholeApartementString;

        public RationalCleaningDatabaseHelper(Context context) :
            base(context, DB_NAME, null, DB_VERSION)
        {
            anotherTasksString = context.GetString(Resource.String.another_tasks_text);
            tasksForWholeApartementString = context.GetString(Resource.String.tasks_for_whole_apartement_text);
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            UpdateMyDatabase(db, 0, DB_VERSION);
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            UpdateMyDatabase(db, oldVersion, newVersion);
        }

        private void UpdateMyDatabase(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            if (oldVersion < 1)
            {
                db.ExecSQL("CREATE TABLE ROOM_TABLE ("
                + "_id INTEGER PRIMARY KEY AUTOINCREMENT, "
                + "IS_ROOM INTEGER, "                             //Позначає, чи є заданий розділ із завданнями кімнатою. 0 - не є кімнатою, 1 - є кімнатою
                + "IMAGE_ID INTEGER, "
                + "TITLE TEXT);");

                db.ExecSQL("CREATE TABLE CLEANING_TASK_TABLE ("
                + "_id INTEGER PRIMARY KEY AUTOINCREMENT, "
                + "TITLE TEXT, "
                + "ROOM_ID INTEGER, "
                + "YEAR INT, "
                + "MONTH INT, "
                + "DAY_OF_MONTH INT, "
                + "DATE_DEFAULT INTEGER, "         //Визначає, чи збережена дата була задана користувачем чи збережена за замовчуванням. 0 - задана користувачем, 1 - за замовчуванням
                + "HOUR INT, "
                + "MINUTE INT, "
                + "YEAR_OF_CHANGE INT, "            //Ці поля необхідні для полегшення логіки роботи із задачами. Пояснення дано нижче великим коментарем
                + "MONTH_OF_CHANGE INT, "           //Ці поля необхідні для полегшення логіки роботи із задачами. Пояснення дано нижче великим коментарем
                + "DAY_OF_MONTH_OF_CHANGE INT, "    //Ці поля необхідні для полегшення логіки роботи із задачами. Пояснення дано нижче великим коментарем
                + "PERIODICITY INTEGER, "
                + "TIME_OF_CLEANING INTEGER, "
                + "CLEANNESS INTEGER);");


                /**
                 * Отже, питання в тому, навіщо потрібні додаткові поля YEAR_OF_CHANGE, MONTH_OF_CHANGE і DAY_OF_MONTH_OF_CHANGE (назвемо їх PARAMETERS в даному коментарі).
                 * 1. Користувач хоче подивитися на задачі, заплановані на сьогоднішній день.
                 * Система виводить список задач на сьогодні (якщо це відбувається вперше за сьогодні) на основі наступної логіки:
                 * Беремо дату YEAR, MONTH, DAY_OF_MONTH (далі в цьому коментары цей параметр будемо називати DATE1). Якщо DATE_DEFAULT рівний 1, то 
                 * якщо DATE1 + періодичність PERIODICITY дорівнює сьогоднішній даті, то завдання поміщається в список сьогоднішніх.
                 * При цьому ПЕРЕД поміщенням в список CLEANNESS (тобто виконання завдання) дорівнювало 1, але ПІСЛЯ першого поміщення воно повинно дорівнювати 0 (присвоюємо дане значення),
                 * що означає, що завдання зараз невиконане і його необхідно виконати.
                 * Тепер уявімо, що ми виконали декілька завдань і чекнули, що вони виконані, тобто CLEANNESS присвоюється значення 1.
                 * Якщо тепер другий раз викликати активність для відображення сьогоднішніх завдань, то буде неможливо без параметрів PARAMETERS зрозуміти, які таски були виконані сьогодні.
                 * Якщо це не можна зрозуміти, то система автоматично знову скине CLEANNESS усіх тасків на 0 і чекатиме, поки їх відмітять виконаними.
                 * Щоб уникнути подібної плутанити вводяться параметри PARAMETERS.
                 * При створенні таску їм присвоюється те ж значення, що й DATE1. Але коли користувач сьогодні відмічає, що даний таск виконаний, то їм присвоюється сьогоднішня дата.
                 * Таким чином, коли буде повторно відображатися список тасків на сьогодні, то коли PARAMETERS дорівнюють сьогоднішній даті, то брати CLEANNESS такий,
                 * як в базі даних, а не змінювати його на 0.
                 * 
                 * 2. Якщо таск відмічається виконаним, то потрібно змінити параметри DATE1, щоб таск уже був запрограмований для нагадування на наступний раз.
                 * Але це вплине на логіку відображення сьогоднішніх тасків. Але оскільки PARAMETERS присвоюється при цьому (при відміченні таска як виконаного) сьогоднішня дата, то це можна ввести
                 * в початкову логіку для фільтрації. 
                 * Якщо DATE1 + PERIODICITY дорівнює сьогоднішній даті або PARAMETERS дорівнює сьогоднішній даті, то виводити даний таск в списку сьогоднішніх.
                 * 
                 * Ще забув додати. Якщо CLEANNESS якогось таску дорівнює 0 (наприклад, невиконані таски із попередніх днів), то такий таск
                 * потрапляє до списку сьогоднішніх, якщо для нього DATE_DEFAULT рівний 1, або якщо DATE_DEFAULT рівний нулю, але при цьому, DATE1 є минулим відносно сьогоднішньої дати.
                 * І ще один варіант: якщо CLEANNESS = 0, а PARAMETERS дорівнює завтрішній даті, то в списку сьогоднішніх тасків даний таск не виводиться, а виводиться в списку завтрішніх тасків.
                 * Це можливо у тому випадку, коли користувач відклав даний таск на завтра.
                 * 
                 * Загалом із тасками, які мають кастомну дату, доведеться повозитись.
                 * 
                 * Підсумовуємо. Таски на сьогодні, якщо
                 * 1) DATE_DEFAULT == 1 && DATE1 + PERIODICITY == DateTime.Now
                 * 2) PARAMETERS == DateTime.Now
                 * 3) (DATE_DEFAULT == 0 не факт &&) DATE1 == DateTime.Now
                 * 4) DATE_DEFAULT == 1 && CLEANNESS == 0
                 * 5) DATE_DEFAULT == 0 && DATE1 менше або дорівнює DateTime.Now && CLEANNESS == 0
                 * При виконанні усіх цих умов виводиться список. Якщо користувач бажає перенести певні таски на завтра, то PARAMETERS присвоюється завтрішній даті.
                 * Таким чином провести ще внутрішню фільтрацію: 6) CLEANNESS == 0 && PARAMETERS == завтрішня дата, то такий таск не включаємо в список сьогоднішніх.
                 * З точки зору математики вищевказана умова еквівалентна наступній:
                 * Якщо CLEANNESS != 0 || PARAMETERS != завтрішня дата, то включаємо в список сьогоднішніх.
                 * 
                 * 
                 * 
                 * 3. Таски на завтра. Зауваження: тут йде мова за таски на завтра, але насправді ми даємо інформацію для тасків на наступний робочий день. Тобто це може бути і через, наприклад,
                 * два дні після сьогодні. Нижче цього не враховано, однак при розробці алгоритму даний момент враховувався.
                 * Якщо DATE1 + PERIODICITY у випадку DATE_DEFAULT == 1, або DATE1 у випадку DATE_DEFAULT == 0,
                 * або PARAMETERS (у випадку із PARAMETERS це можливо у тому випадку, коли користувач відклав даний таск із сьогодні на завтра) 
                 * дорівнює завтрішній даті, то виводимо даний таск у список завтрішніх тасків.
                 * Що далі із цими тасками робити? Для них у найпоширенішому випадку CLEANNESS дорівнює 1. Якщо користувач захоче це змінити і присвоїти значення 0, то тут можливі два варіанти:
                 * 1) DATE_DEFAULT == 1, тоді такий таск потрапить у список сьогоднішніх задач.
                 * 2) DATE_DEFAULT == 0, тоді такий таск залишиться в списку завтрішніх задач.
                 * Якщо користувач позмінював CLEANNESS деяких задач, то йому варто запропонувати зберегти зміни. Лише після його згоди зміни будуть записані в базу даних.
                 * При пропонуванні зберегти зміні буде зазначено, що деякі таски будуть переведені в стан "сьогоднішні"
                 * 
                 * 4. Загальний список тасків.
                 * Розташовуємо кімнати в потрібному порядку і виводимо усі таски. Можливо, буде допускатися якась фільтрація. Сьогоднішні таски підсвічуватимуться одним кольором, завтрішні іншим.
                 * Інші таски (не сьогоднішні та не завтрішні) будуть стандартними.
                 */

                db.ExecSQL("CREATE TABLE CLEANING_TIME_ON_WEEK ("
                + "_id INTEGER PRIMARY KEY AUTOINCREMENT, "
                + "DAY_OF_WEEK INTEGER, "
                + "HOUR INTEGER, "
                + "MINUTE INTEGER);");

                InsertTypeOfTasks(db, anotherTasksString, Resource.Drawable.roomImage1);
                InsertTypeOfTasks(db, tasksForWholeApartementString, Resource.Drawable.roomImage1);
            }

        }

        private static void InsertTypeOfTasks(SQLiteDatabase db, string title, int imageId)
        {
            ContentValues notRoomValues = new ContentValues();
            notRoomValues.Put("IS_ROOM", 0);
            notRoomValues.Put("TITLE", title);
            notRoomValues.Put("IMAGE_ID", imageId);
            db.Insert("ROOM_TABLE", null, notRoomValues);
        }
    }
}
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 