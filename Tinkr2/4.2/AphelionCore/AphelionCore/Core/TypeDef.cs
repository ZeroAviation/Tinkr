using System;

namespace Skewworks.NETMF
{

   /// <summary>
   /// Integer point coordinates
   /// </summary>
   [Serializable]
   // ReSharper disable once InconsistentNaming
   public struct point
   {
      /// <summary>
      /// X coordinate
      /// </summary>
      public int X;

      /// <summary>
      /// Y coordinate
      /// </summary>
      public int Y;

      /// <summary>
      /// Create a new point
      /// </summary>
      /// <param name="x">X coordinate</param>
      /// <param name="y">Y coordinate</param>
      public point(int x, int y)
      {
         X = x;
         Y = y;
      }

      /// <summary>
      /// Returns a string that represents the current object.
      /// </summary>
      /// <returns>
      /// A string that represents the current object.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return X + ", " + Y;
      }
   }

   /// <summary>
   /// Floating-point point coordinates
   /// </summary>
   [Serializable]
   // ReSharper disable once InconsistentNaming
   public struct precisionpoint
   {
      /// <summary>
      /// X coordinate
      /// </summary>
      public float X;

      /// <summary>
      /// Y coordinate
      /// </summary>
      public float Y;

      /// <summary>
      /// Create a new precision point
      /// </summary>
      /// <param name="x">X coordinate</param>
      /// <param name="y">Y coordinate</param>
      public precisionpoint(float x, float y)
      {
         X = x;
         Y = y;
      }

      /// <summary>
      /// Returns a string that represents the current object.
      /// </summary>
      /// <returns>
      /// A string that represents the current object.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return X + ", " + Y;
      }
   }

   /// <summary>
   /// Rectangle (rect) Structure
   /// </summary>
   [Serializable]
   // ReSharper disable once InconsistentNaming
   public struct rect
   {
      #region Variables

      private int _x;
      private int _y;
      private int _w;
      private int _h;

      #endregion

      #region Constructor

      /// <summary>
      /// Creates a new rect
      /// </summary>
      /// <param name="x">x location of rect</param>
      /// <param name="y">y location of rect</param>
      /// <param name="width">width of rect</param>
      /// <param name="height">height of rect</param>
      public rect(int x, int y, int width, int height)
      {
         _x = x;
         _y = y;
         _w = width;
         _h = height;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets/Sets x location of rect
      /// </summary>
      public int X
      {
         get { return _x; }
         set { _x = value; }
      }

      /// <summary>
      /// Gets/Sets y location of rect
      /// </summary>
      public int Y
      {
         get { return _y; }
         set { _y = value; }
      }

      /// <summary>
      /// Gets/Sets width of rect
      /// </summary>
      public int Width
      {
         get { return _w; }
         set
         {
            if (value < 0)
            {
               throw new IndexOutOfRangeException("width cannot be less than 0");
            }
            _w = value;
         }
      }

      /// <summary>
      /// Gets/Sets height of rect
      /// </summary>
      public int Height
      {
         get { return _h; }
         set
         {
            if (value < 0)
            {
               throw new IndexOutOfRangeException("height cannot be less than 0");
            }
            _h = value;
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Adds the area of a second rect to current rect
      /// </summary>
      /// <param name="newRect">rect to add</param>
      public void Combine(rect newRect)
      {
         if (_w == 0)
         {
            _x = newRect.X;
            _y = newRect.Y;
            _w = newRect.Width;
            _h = newRect.Height;
            return;
         }

         int x1 = (_x < newRect.X) ? _x : newRect.X;
         int y1 = (_y < newRect.Y) ? _y : newRect.Y;
         int x2 = (_x + Width > newRect.X + newRect.Width) ? _x + _w : newRect.X + newRect.Width;
         int y2 = (_y + Height > newRect.Y + newRect.Height) ? _y + _h : newRect.Y + newRect.Height;
         _x = x1;
         _y = y1;
         _w = x2 - x1;
         _h = y2 - y1;
      }

      /// <summary>
      /// Returns the combination of two rects
      /// </summary>
      /// <param name="region1">rect 1</param>
      /// <param name="region2">rect 2</param>
      /// <returns>Combined rect</returns>
      public rect Combine(rect region1, rect region2)
      {
         if (region1.Width == 0)
         {
            return region2;
         }
         if (region2.Width == 0)
         {
            return region1;
         }

         int x1 = (region1.X < region2.X) ? region1.X : region2.X;
         int y1 = (region1.Y < region2.Y) ? region1.Y : region2.Y;
         int x2 = (region1.X + region1.Width > region2.X + region2.Width) ? region1.X + region1.Width : region2.X + region2.Width;
         int y2 = (region1.Y + region1.Height > region2.Y + region2.Height) ? region1.Y + region1.Height : region2.Y + region2.Height;

         return new rect(x1, y1, x2 - x1, y2 - y1);
      }

      /// <summary>
      /// Checks if a point is inside the rect
      /// </summary>
      /// <param name="x">x location</param>
      /// <param name="y">y location</param>
      /// <returns>true if point is inside rect; else false</returns>
      public bool Contains(int x, int y)
      {
         return (x >= _x && x <= _x + _w && y >= _y && y <= _y + _h);
      }

      /// <summary>
      /// Checks if a point is inside the rect
      /// </summary>
      /// <param name="pointe">point to check</param>
      /// <returns>true if point is inside rect; else false</returns>
      public bool Contains(point pointe)
      {
         return (pointe.X >= _x && pointe.X <= _x + _w && pointe.Y >= _y && pointe.Y <= _y + _h);
      }

      /// <summary>
      /// Checks to see if two rects interset
      /// </summary>
      /// <param name="area">rect to check</param>
      /// <returns>true if rects intersect; else false</returns>
      public bool Intersects(rect area)
      {
         return !(area.X >= (_x + _w)
                 || (area.X + area.Width) <= _x
                 || area.Y >= (_y + _h)
                 || (area.Y + area.Height) <= _y
                 );
      }

      /// <summary>
      /// Returns the intersection of two rects
      /// </summary>
      /// <param name="region1">rect 1</param>
      /// <param name="region2">rect 2</param>
      /// <returns>Intersected rect</returns>
      public static rect Intersect(rect region1, rect region2)
      {
         if (!region1.Intersects(region2))
         {
            return new rect(0, 0, 0, 0);
         }

         var rct = new rect
         {
            X = (region1.X > region2.X) ? region1.X : region2.X,
            Y = (region1.Y > region2.Y) ? region1.Y : region2.Y
         };

         // For X1 & Y1 we'll want the highest value

         // For X2 & Y2 we'll want the lowest value
         int r1V2 = region1.X + region1.Width;
         int r2V2 = region2.X + region2.Width;
         rct.Width = (r1V2 < r2V2) ? r1V2 - rct.X : r2V2 - rct.X;
         r1V2 = region1.Y + region1.Height;
         r2V2 = region2.Y + region2.Height;
         rct.Height = (r1V2 < r2V2) ? r1V2 - rct.Y : r2V2 - rct.Y;

         return rct;
      }

      /// <summary>
      /// Returns a string that represents the current object.
      /// </summary>
      /// <returns>
      /// A string that represents the current object.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return "{" + X + ", " + Y + ", " + Width + ", " + Height + "}";
      }

      #endregion
   }


   /// <summary>
   /// Structure containing object height &amp; width
   /// </summary>
   [Serializable]
   // ReSharper disable once InconsistentNaming
   public struct size
   {
      #region Variables

      /// <summary>
      /// width of the object
      /// </summary>
      public int Width;

      /// <summary>
      /// height of the object
      /// </summary>
      public int Height;

      #endregion

      #region Constructor

      /// <summary>
      /// Creates a new Size
      /// </summary>
      /// <param name="width">Width</param>
      /// <param name="height">Height</param>
      public size(int width, int height)
      {
         if (width < 0)
         {
            throw new ArgumentOutOfRangeException("width", @"Must be > 0");
         }
         if (height < 0)
         {
            throw new ArgumentOutOfRangeException("height", @"Must be > 0");
         }
         Width = width;
         Height = height;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Adds height &amp; width to existing size
      /// </summary>
      /// <param name="addWidth">Additional width</param>
      /// <param name="addHeight">Additional height</param>
      public void Grow(int addWidth, int addHeight)
      {
         Width += addWidth;
         Height += addHeight;
      }

      /// <summary>
      /// Subtracts height &amp; width from existing size
      /// </summary>
      /// <param name="subtractWidth">Width to reduce</param>
      /// <param name="subtractHeight">Height to reduce</param>
      public void Shrink(int subtractWidth, int subtractHeight)
      {
         Width += subtractWidth;
         if (Width < 0)
         {
            Width = 0;
         }
         Height += subtractHeight;
         if (Height < 0)
         {
            Height = 0;
         }
      }

      /// <summary>
      /// Returns a string that represents the current object.
      /// </summary>
      /// <returns>
      /// A string that represents the current object.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return "{" + Width + ", " + Height + "}";
      }

      #endregion
   }


   /// <summary>
   /// Event argument for touch events
   /// </summary>
   [Serializable]
   public struct TouchEventArgs
   {
      // ReSharper disable once InconsistentNaming
      /// <summary>
      /// Location (point) of the touch event
      /// </summary>
      public point location;

      // ReSharper disable once InconsistentNaming
      /// <summary>
      /// Type of the touch event.
      /// </summary>
      public int type;

      /// <summary>
      /// Create new touch event arguments
      /// </summary>
      /// <param name="point">Point of the touch event</param>
      /// <param name="type">Type of the touch event</param>
      public TouchEventArgs(point point, int type)
      {
         location = point;
         this.type = type;
      }

      /// <summary>
      /// Returns a string that represents the current object.
      /// </summary>
      /// <returns>
      /// A string that represents the current object.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      public override string ToString()
      {
         return location + "; " + type;
      }
   }
}
