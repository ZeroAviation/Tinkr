using System;

using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace Skewworks.NETMF.Controls
{
   [Serializable]
   public class Treeview : ScrollableControl
   {

      #region Variables

      private TreeviewNode[] _nodes;
      private readonly Font _font;

      //private int _scrollY = 0;
      //private int _mY = 0;
      //private int _scrollX = 0;
      //private int _mX = 0;
      //private bool _bMoved = false;

      //private int _aH = 0;
      //private int _aW = 0;
      //private int _totalH = 0;
      //private int _totalW = 0;

      private TreeviewNode _selNode;

      private Color _color, _selColor;
      //private bool _customIcons;

      #endregion

      #region Events

      public event OnNodeTap NodeTapped;
      public event OnNodeCollapsed NodeCollapsed;
      public event OnNodeExpanded NodeExpanded;

      protected virtual void OnNodeCollapsed(object sender, TreeviewNode node)
      {
         if (NodeCollapsed != null)
            NodeCollapsed(sender, node);
      }

      protected virtual void OnNodeExpanded(object sender, TreeviewNode node)
      {
         if (NodeExpanded != null)
            NodeExpanded(sender, node);
      }

      protected virtual void OnNodeTap(TreeviewNode node, point e)
      {
         if (NodeTapped != null)
            NodeTapped(node, e);
      }

      #endregion

      #region Constructor

      public Treeview(string name, Font font, int x, int y, int width, int height)
      {
         Name = name;
         // ReSharper disable DoNotCallOverridableMethodsInConstructor
         X = x;
         Y = y;
         Width = width;
         Height = height;
         // ReSharper restore DoNotCallOverridableMethodsInConstructor
         _font = font;

         //_customIcons = false;
         DefaultColors();
      }

      public Treeview(string name, Font font, int x, int y, int width, int height, TreeviewNode[] nodes)
      {
         Name = name;
         // ReSharper disable DoNotCallOverridableMethodsInConstructor
         X = x;
         Y = y;
         Width = width;
         Height = height;
         // ReSharper restore DoNotCallOverridableMethodsInConstructor
         _font = font;
         _nodes = nodes;
         for (int i = 0; i < _nodes.Length; i++)
            _nodes[i].Container = this;
         //_customIcons = false;
         DefaultColors();
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets number of nodes
      /// </summary>
      public int Length
      {
         get
         {
            if (_nodes == null)
               return 0;
            return _nodes.Length;
         }
      }

      public TreeviewNode[] Nodes
      {
         get { return _nodes; }
         set
         {
            if (_nodes == value)
               return;
            _nodes = value;
            Invalidate();
         }
      }

      /// <summary>
      /// Gets/Sets selected node
      /// </summary>
      public TreeviewNode SelectedNode
      {
         get { return _selNode; }
         set
         {
            if (_selNode == value)
               return;
            if (_selNode != null)
               _selNode.Selected = false;
            _selNode = value;
            if (_selNode != null)
               _selNode.Selected = true;
            Invalidate();
         }
      }

      public Color SelectedTextColor
      {
         get { return _selColor; }
         set
         {
            if (_selColor == value)
               return;
            _selColor = value;
            Invalidate();
         }
      }

      public Color TextColor
      {
         get { return _color; }
         set
         {
            if (_color == value)
               return;
            _color = value;
            Invalidate();
         }
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Adds a node
      /// </summary>
      /// <param name="node"></param>
      public void AddNode(TreeviewNode node)
      {
         node.Container = this;

         if (_nodes == null)
         {
            _nodes = new[] { node };
         }
         else
         {
            var tmp = new TreeviewNode[_nodes.Length + 1];
            Array.Copy(_nodes, tmp, _nodes.Length);
            tmp[tmp.Length - 1] = node;
            _nodes = tmp;
         }

         Invalidate();
      }

      /// <summary>
      /// Removes all nodes
      /// </summary>
      public void ClearNodes()
      {
         _nodes = null;
         Invalidate();
      }

      /// <summary>
      /// Removes a specific node
      /// </summary>
      /// <param name="node"></param>
      public void RemoveNode(TreeviewNode node)
      {
         if (_nodes == null)
            return;

         for (int i = 0; i < _nodes.Length; i++)
         {
            if (_nodes[i] == node)
            {
               RemoveNodeAt(i);
               return;
            }
         }
      }

      /// <summary>
      /// Removes a node from a specific point in the array
      /// </summary>
      /// <param name="index"></param>
      public void RemoveNodeAt(int index)
      {
         if (_nodes == null)
            return;
         if (_nodes.Length == 1)
            _nodes = null;
         else
         {
            var tmp = new TreeviewNode[_nodes.Length];
            int c = 0;
            for (int i = 0; i < _nodes.Length; i++)
            {
               if (i != index)
                  tmp[c++] = _nodes[i];
            }
            _nodes = tmp;
         }

         Invalidate();
      }

      #endregion

      #region Touch

      protected override void TouchDownMessage(object sender, point point, ref bool handled)
      {
         if (_nodes != null)
         {
            TreeviewNode node = NodeFromPoint(point);
            if (node != null)
               node.PenDown = true;
         }
      }

      protected override void TouchUpMessage(object sender, point point, ref bool handled)
      {
         if (_nodes != null)
         {
            // Get Selected Node
            TreeviewNode node = NodeFromPoint(point);
            int icoSize = _font.Height;
            if (node != null)
            {
               var ecRect = new rect(node.Bounds.X, node.Bounds.Y, icoSize, icoSize);
               if (ecRect.Contains(point) && node.PenDown)
               {
                  /*node.Expanded = !node._expanded;
                  if (!node._expanded)
                     ResetChildrenBounds(node);*/
                  node.Expanded = !node.Expanded;
                  if (!node.Expanded)
                  {
                     ResetChildrenBounds(node);
                  }

                  Invalidate();
               }
               else
               {
                  // Reset States
                  if (!node.Selected && node.PenDown)
                  {
                     for (int i = 0; i < _nodes.Length; i++)
                     {
                        ResetNodesState(_nodes[i]);
                     }
                     node.Selected = true;
                     _selNode = node;
                     Invalidate();
                  }
                  OnNodeTap(node, point);
               }
            }

            // Reset Pen States
            for (int i = 0; i < _nodes.Length; i++)
            {
               SetChildrenPen(_nodes[i]);
            }
         }
         base.TouchUpMessage(sender, point, ref handled);
      }

      #endregion

      #region GUI

      private void DefaultColors()
      {
         _color = Core.SystemColors.FontColor;
         _selColor = Core.SystemColors.SelectionColor;
      }

      protected override void OnRender(int x, int y, int width, int height)
      {
         //x and y are Left and Top anyway
         //x = Left;
         //y = Top;
         int maxX = Width - 3;
         int icoSize = _font.Height + 8;

         // Draw the container
         if (Focused)
         {
            Core.Screen.DrawRectangle(Core.SystemColors.SelectionColor, 1, Left, Top, Width, Height, 0, 0,
               Core.SystemColors.WindowColor, 0, 0, Core.SystemColors.WindowColor, 0, 0, 256);
         }
         else
         {
            Core.Screen.DrawRectangle(Core.SystemColors.BorderColor, 1, Left, Top, Width, Height, 0, 0,
               Core.SystemColors.WindowColor, 0, 0, Core.SystemColors.WindowColor, 0, 0, 256);
         }

         int yy = y - ScrollY;
         int xx = x - ScrollX;

         // Draw Nodes
         if (_nodes != null)
         {
            Core.Screen.SetClippingRectangle(Left + 1, Top + 1, Width - 2, Height - 2);
            for (int i = 0; i < _nodes.Length; i++)
            {
               int tW;
               int tH;
               _font.ComputeExtent(_nodes[i].Text, out tW, out tH);
               _nodes[i].Bounds = new rect(xx, yy, Width + (xx - Left), _font.Height + 8);
               if (maxX < tW + icoSize + 12 + xx)
               {
                  maxX = tW + icoSize + 12 + xx;
               }

               if (yy + _font.Height + 4 > 0 && yy < Top + Height)
               {
                  // Draw Icon if node has children
                  if (_nodes[i].Length > 0)
                  {
                     Core.Screen.DrawRectangle(Colors.DarkGray, 1, xx + 2, yy + 2, icoSize - 4, icoSize - 4, 0, 0, Colors.Ghost, xx + 2, yy + 2, Colors.LightGray, xx + 2, yy + 6, 256);
                     Core.Screen.DrawTextInRect((_nodes[i].Expanded) ? "-" : "+", xx + 2, yy + 4, icoSize - 4, _font.Height, Bitmap.DT_AlignmentCenter, Colors.Charcoal, _font);
                  }

                  // Draw Node Text
                  Core.Screen.DrawTextInRect(_nodes[i].Text, xx + icoSize + 4, yy + 4, tW, _font.Height, Bitmap.DT_TrimmingCharacterEllipsis,
                      (_nodes[i].Selected) ? _selColor : _color, _font);
               }

               yy += icoSize + 4;

               if (_nodes[i].Expanded)
               {
                  point e = RenderSubNodes(_nodes[i], xx + icoSize, yy, icoSize);
                  yy = e.Y;
                  if (e.X > maxX)
                  {
                     maxX = e.X;
                  }
               }
            }
         }

         RequiredHeight = ScrollY + yy;
         RequiredWidth = ScrollX + maxX;

         base.OnRender(x, y, width, height);
      }

      private point RenderSubNodes(TreeviewNode node, int x, int y, int icoSize)
      {
         int maxX = x;

         for (int i = 0; i < node.Length; i++)
         {
            int tH;
            int tW;
            _font.ComputeExtent(node.Nodes[i].Text, out tW, out tH);
            node.Nodes[i].Bounds = new rect(x, y, Width + (x - Left), _font.Height + 8);

            if (maxX < tW + icoSize + 20 + x)
               maxX = tW + icoSize + 20 + x;

            if (y + _font.Height + 4 > 0 && y < Top + Height)
            {
               // Draw Icon if node has children
               if (node.Nodes[i].Length > 0)
               {
                  Core.Screen.DrawRectangle(Colors.DarkGray, 1, x + 2, y + 2, icoSize - 4, icoSize - 4, 0, 0, Colors.Ghost, x + 2, y + 2, Colors.LightGray, x + 2, y + 6, 256);
                  Core.Screen.DrawTextInRect((node.Nodes[i].Expanded) ? "-" : "+", x + 2, y + 4, icoSize - 4, _font.Height, Bitmap.DT_AlignmentCenter, Colors.Charcoal, _font);
               }

               // Draw Node Text
               Core.Screen.DrawTextInRect(node.Nodes[i].Text, x + icoSize + 4, y + 4, tW, _font.Height, Bitmap.DT_TrimmingCharacterEllipsis,
                   (node.Nodes[i].Selected) ? _selColor : _color, _font);
            }

            y += icoSize + 4;

            if (node.Nodes[i].Expanded)
            {
               point e = RenderSubNodes(node.Nodes[i], x + icoSize, y, icoSize);
               y = e.Y;
               if (e.X > maxX)
                  maxX = e.X;
            }
         }

         return new point(maxX, y);
      }

      #endregion

      #region Private Methods

      private TreeviewNode NodeFromPoint(point e)
      {
         for (int i = 0; i < _nodes.Length; i++)
         {
            if (_nodes[i].Bounds.Contains(e))
               return _nodes[i];
            if (_nodes[i].Length > 0)
            {
               TreeviewNode node = CheckNodePoint(_nodes[i], e);
               if (node != null)
                  return node;
            }
         }
         return null;
      }

      private TreeviewNode CheckNodePoint(TreeviewNode node, point e)
      {
         for (int i = 0; i < node.Length; i++)
         {
            if (node.Nodes[i].Bounds.Contains(e))
               return node.Nodes[i];
            if (node.Nodes[i].Length > 0)
            {
               TreeviewNode subnode = CheckNodePoint(node.Nodes[i], e);
               if (subnode != null)
                  return subnode;
            }
         }
         return null;
      }

      private void SetChildrenPen(TreeviewNode node)
      {
         node.PenDown = false;
         for (int i = 0; i < node.Length; i++)
         {
            node.Nodes[i].PenDown = false;
            if (node.Nodes[i].Length > 0)
               SetChildrenPen(node.Nodes[i]);
         }
      }

      private void SetChildrenSelected(TreeviewNode node)
      {
         _selNode = null;
         node.Selected = false;
         for (int i = 0; i < node.Length; i++)
         {
            node.Nodes[i].Selected = false;
            if (node.Nodes[i].Length > 0)
               SetChildrenSelected(node.Nodes[i]);
         }
      }

      private void ResetNodesState(TreeviewNode topNode)
      {
         _selNode = null;
         topNode.Selected = false;
         topNode.PenDown = false;

         for (int i = 0; i < topNode.Length; i++)
         {
            topNode.Nodes[i].Selected = false;
            topNode.Nodes[i].PenDown = false;
            if (topNode.Nodes[i].Length > 0)
               SetChildrenSelected(topNode.Nodes[i]);
         }
      }

      private void ResetChildrenBounds(TreeviewNode node)
      {
         for (int i = 0; i < node.Length; i++)
         {
            node.Nodes[i].Bounds = new rect(0, 0, 0, 0);
            if (node.Nodes[i].Length > 0)
               ResetChildrenBounds(node.Nodes[i]);
         }
      }

      #endregion

      #region Internal Methods

      protected internal void ChildCollapsed(TreeviewNode node)
      {
         OnNodeExpanded(this, node);
      }

      protected internal void ChildExpanded(TreeviewNode node)
      {
         OnNodeExpanded(this, node);
      }

      #endregion

   }
}
