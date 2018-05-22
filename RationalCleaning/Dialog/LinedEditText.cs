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
using Android.Graphics;
using Android.Util;


namespace RationalCleaning.Dialog
{
    public class LinedEditText : EditText
    {
        private Rect mRect;
        private Paint mPaint;

        public LinedEditText(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            mRect = new Rect();
            mPaint = new Paint();
            mPaint.SetStyle(Android.Graphics.Paint.Style.FillAndStroke);
            mPaint.Color = Color.Black;
        }

        protected override void OnDraw(Canvas canvas)
        {
            int height = Height;
            int lineHeight = LineHeight;

            int count = height / lineHeight;
            if (LineCount > count)
                count = LineCount;

            Rect r = mRect;
            Paint paint = mPaint;
            int baseLine = GetLineBounds(0, r);

            for (int i = 0; i < count; i++)
            {
                canvas.DrawLine(r.Left, baseLine + 1, r.Right, baseLine + 1, paint);
                baseLine += LineHeight;
            }

            base.OnDraw(canvas);
        }

    }
}