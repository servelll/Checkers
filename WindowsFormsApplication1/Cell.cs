using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public class Cell
	{
		//координаты поля
		public int x;
		public int y;

		public string color;
		public bool king;

		public PictureBox picture;
		public bool marked;

		public bool jump;

		public Cell()
		{

		}

		public Cell(int x, int y, string color, bool mark = false, bool king = false)
		{
			this.x = x;
			this.y = y;
			this.color = color;
			this.marked = mark;
			this.king = king;
		}

		public void fill_image() 
		{
			if (!marked)
			{
				if (color == "white" && !king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\1.png");
				}
				if (color == "white" && king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\2.png");
				}
				if (color == "black" && !king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\3.png");
				}
				if (color == "black" && king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\4.png");
				}
				if (color == "empty" && !jump)
				{
					picture.Image = null;
				}

			}
			else
			{
				if (color == "white" && !king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\1h.png");
				}
				if (color == "white" && king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\2h.png");
				}
				if (color == "black" && !king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\3h.png");
				}
				if (color == "black" && king)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\4h.png");
				}
				if (color == "empty" && !king && !jump)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\5_null_cell.png");
				}
				if (color == "empty" && !king && jump)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\5_null_cell_jump.png");
				}
				if (color == "empty" && king && !jump)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\6_null_king.png");
				}
				if (color == "empty" && king && jump)
				{
					picture.Image = System.Drawing.Image.FromFile(Environment.CurrentDirectory + "\\6_null_king_jump.png");
				}
			}
		}

		public void set_mark(bool par)
		{
			marked = par;
			fill_image();
		}
		public override string ToString()
		{
			string _x = "";
			switch (x)
			{
				case 1: _x = "a"; break;
				case 2: _x = "b"; break;
				case 3: _x = "c"; break;
				case 4: _x = "d"; break;
				case 5: _x = "e"; break;
				case 6: _x = "f"; break;
				case 7: _x = "g"; break;
				case 8: _x = "h"; break;
			}
			return _x + y;
		}
	}
}
