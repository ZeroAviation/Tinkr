using System;
using Microsoft.SPOT.Presentation.Media;
using Skewworks.NETMF;
using Skewworks.NETMF.Controls;

namespace Skewworks.Tinkr.Controls
{
   [Serializable]
   public class SlidePanel : Container
   {

      #region Variables

      private Color _bkg;
      private string _title;
      //private rect _Bounds;

      protected internal bool BeingDisplayed { get; set; }
      protected internal rect DispRect { get; set; }

      // Auto Scroll
      private int _maxX;
      private int _maxY;
      private int _minX;
      private int _minY;
      private bool _moving;
      private bool _touch;

      int _w, _h;
      int _x, _y;

      #endregion

      #region Constructors

      public SlidePanel(string name, string title)
      {
         Name = name;
         _title = title;
         _bkg = Colors.White;
      }

      // ReSharper disable once UnusedParameter.Local
      public SlidePanel(string name, string title, Color background)
      {
         Name = name;
         _title = title;
         _bkg = Colors.White;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets/Sets background color
      /// </summary>
      public Color BackColor
      {
         get { return _bkg; }
         set
         {
            if (value == _bkg)
               return;

            _bkg = value;
            Invalidate();
         }
      }

      public string Title
      {
         get { return _title; }
         set
         {
            if (_title == value)
               return;
            _title = value;
            Invalidate();
         }
      }

      public override int Height
      {
         get { return _h; }
         set { _h = value; }
      }

      public override int Width
      {
         get { return _w; }
         set { _w = value; }
      }

      public override int X
      {
         get { return _x; }
         set
         {
            if (_x == value)
               return;
            _x = value;
            if (Children != null)
            {
               for (int i = 0; i < Children.Length; i++)
                  Children[i].UpdateOffsets();
            }
         }
      }

      public override int Y
      {
         get { return _y; }
         set
         {
            if (_y == value)
               return;
            _y = value;
            if (Children != null)
            {
               for (int i = 0; i < Children.Length; i++)
                  Children[i].UpdateOffsets();
            }
         }
      }

      #endregion

      #region Button Methods

      protected override void ButtonPressedMessage(int buttonId, ref bool handled)
      {
         if (Children != null)
         {
            if (ActiveChild != null)
               ActiveChild.SendButtonEvent(buttonId, true);
            else
            {
               for (int i = 0; i < Children.Length; i++)
               {
                  if (Children[i].ScreenBounds.Contains(Core.MousePosition))
                  {
                     handled = true;
                     Children[i].SendButtonEvent(buttonId, true);
                     break;
                  }
               }
            }
         }
      }

      protected override void ButtonReleasedMessage(int buttonId, ref bool handled)
      {
         if (Children != null)
         {
            if (ActiveChild != null)
               ActiveChild.SendButtonEvent(buttonId, false);
            else
            {
               for (int i = 0; i < Children.Length; i++)
               {
                  if (Children[i].ScreenBounds.Contains(Core.MousePosition))
                  {
                     handled = true;
                     Children[i].SendButtonEvent(buttonId, false);
                     break;
                  }
               }
            }
         }
      }

      #endregion

      #region Keyboard Methods

      protected override void KeyboardAltKeyMessage(int key, bool pressed, ref bool handled)
      {
         if (ActiveChild != null)
         {
            handled = true;
            ActiveChild.SendKeyboardAltKeyEvent(key, pressed);
         }
         base.KeyboardAltKeyMessage(key, pressed, ref handled);
      }

      protected override void KeyboardKeyMessage(char key, bool pressed, ref bool handled)
      {
         if (ActiveChild != null)
         {
            handled = true;
            ActiveChild.SendKeyboardKeyEvent(key, pressed);
         }
         base.KeyboardKeyMessage(key, pressed, ref handled);
      }

      #endregion

      #region Touch Methods

      protected override void TouchDownMessage(object sender, point point, ref bool handled)
      {
         // Check Controls
         if (Children != null)
         {
            lock (Children)
            {
               for (int i = Children.Length - 1; i >= 0; i--)
               {
                  if (Children[i].Visible && Children[i].HitTest(point))
                  {
                     ActiveChild = Children[i];
                     Children[i].SendTouchDown(this, point);
                     handled = true;
                     return;
                  }
               }
            }
         }

         if (ActiveChild != null)
         {
            ActiveChild.Blur();
            Render(ActiveChild.ScreenBounds, true);
            ActiveChild = null;
         }

         _touch = true;
      }

      protected override void TouchGestureMessage(object sender, TouchType type, float force, ref bool handled)
      {
         if (ActiveChild != null)
         {
            handled = true;
            ActiveChild.SendTouchGesture(sender, type, force);
         }
      }

      protected override void TouchMoveMessage(object sender, point point, ref bool handled)
      {
         if (_touch)
         {
            bool bUpdated = false;

            int diffY = 0;
            int diffX = 0;

            // Scroll Y
            if (_maxY > Height)
            {
               diffY = point.Y - LastTouch.Y;

               if (diffY > 0 && _minY < Top)
               {
                  diffY = Math.Min(diffY, _minY + Top);
                  bUpdated = true;
               }
               else if (diffY < 0 && _maxY > Height)
               {
                  diffY = Math.Min(-diffY, _maxY - _minY - Height);
                  bUpdated = true;
               }
               else
               {
                  diffY = 0;
               }

               _moving = true;
            }

            // Scroll X
            if (_maxX > Width)
            {
               diffX = point.X - LastTouch.X;

               if (diffX > 0 && _minX < Left)
               {
                  diffX = Math.Min(diffX, _minX + Left);
                  bUpdated = true;
               }
               else if (diffX < 0 && _maxX > Width)
               {
                  diffX = Math.Min(-diffX, _maxX - _minX - Width);
                  bUpdated = true;
               }
               else
               {
                  diffX = 0;
               }

               _moving = true;
               handled = true;
            }

            LastTouch = point;
            if (bUpdated)
            {
               var ptOff = new point(diffX, diffY);
               for (int i = 0; i < Children.Length; i++)
               {
                  Children[i].UpdateOffsets(ptOff);
               }
               Render(true);
               handled = true;
            }
            else if (_moving)
            {
               Render(true);
            }
         }
         else
         {
            // Check Controls
            if (ActiveChild != null && ActiveChild.Touching)
            {
               ActiveChild.SendTouchMove(this, point);
               handled = true;
            }
            else if (Children != null)
            {
               for (int i = Children.Length - 1; i >= 0; i--)
               {
                  if (Children[i].Touching || Children[i].HitTest(point))
                     Children[i].SendTouchMove(this, point);
               }
            }
         }
         base.TouchMoveMessage(sender, point, ref handled);
      }

      protected override void TouchUpMessage(object sender, point point, ref bool handled)
      {
         _touch = false;

         if (_moving)
         {
            _moving = false;
            Render(true);
         }
         else
         {
            // Check Controls
            if (Children != null)
            {
               for (int i = Children.Length - 1; i >= 0; i--)
               {
                  try
                  {
                     if (Children[i].HitTest(point) && !handled)
                     {
                        handled = true;
                        Children[i].SendTouchUp(this, point);
                     }
                     else if (Children[i].Touching)
                        Children[i].SendTouchUp(this, point);
                  }
                     // ReSharper disable once EmptyGeneralCatchClause
                  catch
                  {
                     // This can happen if the user clears the Form during a tap
                  }
               }
            }
         }
         base.TouchUpMessage(sender, point, ref handled);
      }

      #endregion

      #region GUI

      protected override void OnRender(int x, int y, int width, int height)
      {
         var area = new rect(x, y, width, height);

         _maxX = Width;
         _maxY = Height;

         Core.Screen.DrawRectangle(0, 0, Left, Top, Width, Height, 0, 0, _bkg, 0, 0, _bkg, 0, 0, 256);

         // Render controls
         if (Children != null)
         {
            for (int i = 0; i < Children.Length; i++)
            {
               if (Children[i] != null)
               {
                  if (Children[i].ScreenBounds.Intersects(area))
                     Children[i].Render();

                  if (Children[i].Y < _minY)
                     _minY = Children[i].Y;
                  else if (Children[i].Y + Children[i].Height > _maxY)
                     _maxY = Children[i].Y + Children[i].Height;

                  if (Children[i].X < _minX)
                     _minX = Children[i].X;
                  else if (Children[i].X + Children[i].Width > _maxX)
                     _maxX = Children[i].X + Children[i].Width;

               }
            }
         }
      }

      #endregion

   }
}
