using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
	public partial class Form1 : Form
	{
		int mode = 0;
		Field main = new Field();
		Point begin_pos = new Point();
		ChainsMas mas;
		
		//конструкторы
		public Form1()
		{
			InitializeComponent();
			edit_menu_cell.picture = pictureBox2;
			//заполняем поле
			for (int i = 1; i <= 8; i++)
			{
				for (int j = 1; j <= 4; j++)
				{
					int _j = 0;
					if (i % 2 == 0)
					{
						_j = j * 2;
					}
					else
					{
						_j = j * 2 - 1;
					}

					string _color;
					if (i <= 4)
					{
						_color = "white";
					}
					else
					{
						_color = "black";
					}

					if (i < 4 || i > 5)
					{
						add_checker(_j, i, _color);
					}
					else
					{
						add_checker(_j, i, "empty");
					}
				}

			}
			double k = panel1.Height / 1800d;
			begin_pos = new Point((int)((panel1.Width - 1800d * k)/2d), 0);
			renew();
		}
		public void renew()
		{
			foreach (Cell item in main.field_state)
			{
				Point temp = calc_coord(item.x, item.y);
				temp.Offset(begin_pos);
				item.picture.Location = temp;
			}
		}
		public void add_checker(int _j, int _i, string _color)
		{
			//проверка на добавление дубля
			if (get_cell_for_coordinates(main, _j, _i) != null) return;
			//создаем временные объекты в пустоте, издеваемся над ними
			Cell c = new Cell(_j, _i, _color);
			PictureBox p = new PictureBox();
			double k = panel1.Height / 1800d;
			p.Size = new Size((int)(200 * k), (int)(200 * k));
			p.SizeMode = PictureBoxSizeMode.Zoom;
			p.BackColor = Color.Transparent;
			p.Click += new EventHandler(pictureBox_Click);
			p.Tag = main.field_state.Count;

			//добавляем в массив и отрисовываем
			panel1.Controls.Add(p);
			c.picture = p;
			c.fill_image();
			main.field_state.Add(c);
		}

		//просчет ходов, поведение по нажатию, и сами ходы
		Cell last_marked;

		public void calc_all_cell_moves(ChainsMas temp, Cell c, int level, Field f)
		{
			if (c == null || temp == null) return;

			bool is_additional_jump = temp.is_last_chain_where_contain_cell_is_jump(c);
			string mine_first_color;
			bool mine_first_king;
			if (!is_additional_jump)
			{
				mine_first_color = c.color;
				mine_first_king = c.king;
			}
			else
			{
				mine_first_color = temp.get_first_color();
				mine_first_king = temp.get_first_king();
			}
			int[,] dir = { 
						 { -1, 1 }, { 1, 1 }, 
						 { -1, -1 }, { 1, -1 }
						 };

			//множественные ходы и множественная проверка
			if (
				(c.king && русскиеToolStripMenuItem.Checked)
				)
			{
				for (int i = 0; i < dir.GetLength(0); i++)
				{
					bool dir_bool = true;
					Cell c_jumped = new Cell(); bool jump_begin = false;
					while (dir_bool)
					{
						if (c.x + dir[i, 0] > 0 && c.x + dir[i, 0] < 9 &&
						c.y + dir[i, 1] > 0 && c.y + dir[i, 1] < 9)
						{
							Cell c_ = get_cell_for_coordinates(f, c.x + dir[i, 0], c.y + dir[i, 1]);
							//ход
							if (c_.color == "empty" && !jump_begin)
							{
								temp.add_move_and_auto_decide_where_should_add(new Move(c, c_, level, false, new System.Collections.ArrayList()));
							}
							//битье
							if (jump_begin)
							{
								System.Collections.ArrayList jc = new System.Collections.ArrayList();
								jc.Add(c_jumped);
								temp.add_move_and_auto_decide_where_should_add(new Move(c, c_, level, true, jc));
								calc_all_cell_moves(temp, c_, level + 1, new Field(f, c_jumped, c_));
								dir_bool = false;
							}
							//первая пешка для битья
							if (!jump_begin && c_.color != mine_first_color && c_.color != "empty")
							{
								jump_begin = true;
								c_jumped = c_;
							}

							if (dir[i, 0] > 0) dir[i, 0]++; else dir[i, 0]--;
							if (dir[i, 1] > 0) dir[i, 1]++; else dir[i, 1]--; 
						}
						else
						{
							dir_bool = false;
						}
					}
				}
			}

			//одиночные ходы и одиночная проверка
			if (
				(английскиеToolStripMenuItem.Checked)
				||
				(!c.king && русскиеToolStripMenuItem.Checked)
				)
			{
				for (int i = 0; i < dir.GetLength(0); i++)
				{
					//условие для определения правильной стороны для пешек
					bool flag_right_direction = 
						((mine_first_color == "black" && dir[i, 1] < 0)
						||
						(mine_first_color == "white" && dir[i, 1] > 0)
						);

					if (c.x + dir[i, 0] > 0 && c.x + dir[i, 0] < 9 &&
						c.y + dir[i, 1] > 0 && c.y + dir[i, 1] < 9 &&
						((английскиеToolStripMenuItem.Checked && flag_right_direction && !c.king)
						|| (английскиеToolStripMenuItem.Checked && c.king)
						|| русскиеToolStripMenuItem.Checked))
					{
						Cell c_ = get_cell_for_coordinates(f, c.x + dir[i, 0], c.y + dir[i, 1]);
						dir[i, 0] += dir[i, 0];
						dir[i, 1] += dir[i, 1];
						//ход
						if (c_.color == "empty" && !is_additional_jump && flag_right_direction)
						{
							temp.add_move_and_auto_decide_where_should_add(new Move(c, c_, level, false, new System.Collections.ArrayList()));
						}
						//битье
						else if (c.x + dir[i, 0] > 0 && c.x + dir[i, 0] < 9 &&
								 c.y + dir[i, 1] > 0 && c.y + dir[i, 1] < 9)
						{
							Cell c_jump = get_cell_for_coordinates(f, c.x + dir[i, 0], c.y + dir[i, 1]);
							if (c_jump.color == "empty" && c_.color != mine_first_color && c_.color != "empty")
							{
								System.Collections.ArrayList jc = new System.Collections.ArrayList();
								jc.Add(c_);
								temp.add_move_and_auto_decide_where_should_add(new Move(c, c_jump, level, true, jc));
								//здесь идет рекурсивная проверка на ходы следующих уровней (двойное+ битье)
								calc_all_cell_moves(temp, c_jump, level + 1, new Field(f, c_, c_jump));
							}
						}
					}
				}
			}
		}
		public Cell get_cell_for_coordinates(Field f, int _j, int _i)
		{
			foreach (Cell item in f.field_state)
			{
				if (item.x == _j && item.y == _i)
				{
					return item;
				}
			}
			return null;
		}
		public Point calc_coord(int x, int y, Point delta = new Point()) {
			double k_x = panel1.Height / 1800d;
			double k_y = panel1.Height / 1798d;
			return new Point((int)((204 * x - 110 - x) * k_x), (int)((202 * (9 - y) - 110 - y) * k_y));
		}
		private void button1_Click(object sender, EventArgs e)
		{
			foreach (Cell item in main.field_state)
			{
				item.color = "empty";
				item.king = false;
				item.set_mark(false);
			}
		}
		private void pictureBox_Click(object sender, EventArgs e)
		{
			Cell temp = main.field_state[(int)(sender as PictureBox).Tag] as Cell;
			//режим редактирования
			if (edit_mode)
			{
				if (temp.color != edit_menu_cell.color || temp.king != edit_menu_cell.king)
				{
					temp.color = edit_menu_cell.color;
					temp.king = edit_menu_cell.king;
					temp.fill_image();
				}
			}
			else
			//игровой режим
			{
				//клик на шашке
				if (temp.color != "empty")
				{
					bool mark = temp.marked;
					//чистим все отметки
					clear_all_marks();

					//в зависимости от отметки, ставим\убираем ее
					temp.set_mark(!mark);
					if (temp.marked)
					{
						last_marked = temp;
						//рисуем доступные ходы
						mas = new ChainsMas();
						calc_all_cell_moves(mas, temp, 0, main);
						paint_all_moves(mas);
					}
					else
					{
						last_marked = null;
					}
				}
				else
				//клик по пустому месту
				{
					//совершение хода
					if (last_marked != null)
					{
						System.Collections.ArrayList chains = mas.get_all_high_important_chains();
						if (chains.Count > 0)
						{
							//поиск move
							foreach (Chain item in chains)
							{
								foreach (Move item_move in item.chain_s_moves)
								{
									//совершение move
									if (item_move.cell_to == temp)
									{
										if (!item_move.jump)
										{
											//простой ход
											clear_all_marks();
											item_move.cell_to.color = (item.chain_s_moves[0] as Move).cell_from.color;
											item_move.cell_to.king = item_move.cell_from.king;
											item_move.cell_to.fill_image();
											(item.chain_s_moves[0] as Move).cell_from.color = "empty";
											(item.chain_s_moves[0] as Move).cell_from.king = false;
											(item.chain_s_moves[0] as Move).cell_from.fill_image();
										}
										else
										{
											//прыжок со съеданием
											string first_color = (item.chain_s_moves[0] as Move).cell_from.color;
											foreach (Move internal_move in item.chain_s_moves)
											{
												clear_all_marks();
												
												internal_move.cell_to.color = first_color;
												internal_move.cell_to.king = internal_move.cell_from.king;
												internal_move.cell_to.set_mark(false);
												internal_move.cell_from.color = "empty";
												internal_move.cell_from.king = false;
												internal_move.cell_from.set_mark(false);
												
												foreach (Cell c in internal_move.eated_cells)
												{
													c.color = "empty";
													c.king = false;
													c.fill_image();
												}
												//рекурсивно, вызываем этот же метод, если ходы еще есть
												mas = new ChainsMas();
												calc_all_cell_moves(mas, temp, 0, main);
												System.Collections.ArrayList imp_chains = mas.get_all_high_important_chains();
												if (mas != null && imp_chains.Count > 0)
												{
													internal_move.cell_to.set_mark(true);
													pictureBox_Click(sender, new EventArgs());
												}
												if (internal_move == item_move) break;
											}
										}
										return;
									}
								}
							}
						}
					}
				}
			}

		}

		private void paint_all_moves(ChainsMas moves)
		{
			//заполнение всех ходов
			richTextBox1.Text = "";
			foreach (Chain item_chain in moves.table_s_chains) 
			{
				if (richTextBox1.Text != "") richTextBox1.Text += "|\n";
				richTextBox1.Text += item_chain.ToString();
			}
			//отрисовка только доступных
			foreach (Chain item in moves.get_all_high_important_chains())
			{
				foreach (Move item_move in item.chain_s_moves)
				{
					item_move.cell_to.set_mark(true);
				}
			}
		}

		public void clear_all_marks()
		{
			foreach (Cell item in main.field_state)
			{
				if (item.marked)
				{
					item.set_mark(false);
				}
			}
		}

		//меню редактирования
		Cell edit_menu_cell = new Cell(-1, -1, "white");
		bool edit_mode = false;
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			CheckBox c = sender as CheckBox;
			clear_all_marks();
			edit_mode = c.Checked;
			last_marked = null;
			if (c.Checked)
			{
				pictureBox2.Visible = true;
			}
			else
			{
				pictureBox2.Visible = false;
			}

		}
		private void pictureBox2_Click(object sender, EventArgs e)
		{
			string c = "";
			bool k = false;
			if (edit_menu_cell.king)
			{
				switch (edit_menu_cell.color)
				{
					case "white": { c = "black"; k = true; } break;
					case "black": { c = "empty"; k = true; } break;
					case "empty": { c = "white"; k = false; } break;
				}
			}
			else 
			{
				switch (edit_menu_cell.color)
				{
					case "white": { c = "black"; k = false; } break;
					case "black": { c = "empty"; k = false; } break;
					case "empty": { c = "white"; k = true; } break;
				}
			}
			edit_menu_cell.king = k;
			edit_menu_cell.color = c;
			edit_menu_cell.fill_image();
		}
		private void ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (ToolStripMenuItem item in правилаToolStripMenuItem.DropDownItems)
			{
				if (item == sender as ToolStripMenuItem)
				{
					item.Checked = true;
				}
				else
				{
					item.Checked = false;
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			button1_Click(button1, new EventArgs());
			Cell temp = get_cell_for_coordinates(main, 2, 2);
			temp.color = "black";
			if (mode == 0) temp.king = true;
			temp.fill_image();

			temp = get_cell_for_coordinates(main, 3, 3);
			temp.color = "white";
			temp.king = true;
			temp.fill_image();

			temp = get_cell_for_coordinates(main, 3, 5);
			temp.color = "white";
			temp.king = true;
			temp.fill_image();

			temp = get_cell_for_coordinates(main, 5, 5);
			temp.color = "white";
			temp.king = true;
			temp.fill_image();

			temp = get_cell_for_coordinates(main, 5, 7);
			temp.color = "white";
			temp.king = true;
			temp.fill_image();

			temp = get_cell_for_coordinates(main, 3, 7);
			temp.color = "white";
			temp.king = true;
			temp.fill_image();

			английскиеToolStripMenuItem.Checked = false;
			русскиеToolStripMenuItem.Checked = true;

			mode++;
			if (mode > 1) mode = 0;
		}
	}
}
