using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
	public class ChainsMas
	{
		//массив всех цепочек ходов, только chain
		public System.Collections.ArrayList table_s_chains;
		public ChainsMas()
		{
			this.table_s_chains = new System.Collections.ArrayList();
		}

		public void add_move_and_auto_decide_where_should_add(Move m)
		{
			if (m.level == 0)
			{
				table_s_chains.Add(new Chain(m));
			}
			else
			{
				System.Collections.ArrayList chains_to_add = new System.Collections.ArrayList();
				foreach (Chain item in table_s_chains)
				{
					int i = item.get_number_of_chain_if_contain_cell(m.cell_from);
					if (i == item.chain_s_moves.Count - 1)
					{
						item.chain_s_moves.Add(m);
					}
					else if (i > -1)
					{
						Chain t = item.get_last_chain(i);
						t.chain_s_moves.Add(m);
						chains_to_add.Add(t);
					}
				}
				if (chains_to_add.Count > 0)
				{
					table_s_chains.AddRange(chains_to_add);
				}
			}
		}

		public System.Collections.ArrayList get_chains_where_cell_in_the_end(Cell c)
		{
			System.Collections.ArrayList temp = new System.Collections.ArrayList();
			foreach (Chain item in table_s_chains)
			{
				if (item.is_chain_contain_cell_in_the_end(c)) temp.Add(item);
			}
			return temp;
		}
		public System.Collections.ArrayList get_all_high_important_chains()
		{
			System.Collections.ArrayList temp = new System.Collections.ArrayList();

			//определяем максимум прыжков
			int max = -1;
			foreach (Chain item in table_s_chains)
			{
				int i = item.calc_jump_count();
				if (i > max) 
				{
					max = i;
				}
			}

			//выдираем максимум
			foreach (Chain item in table_s_chains)
			{
				int i = item.calc_jump_count();
				if (i == max)
				{
					temp.Add(item);
				}
			}
			return temp;
		}
		public bool is_last_chain_where_contain_cell_is_jump(Cell c)
		{
			System.Collections.ArrayList list = get_chains_where_cell_in_the_end(c);
			if (list.Count == 0) return false;
			foreach (Chain item in list)
			{
				if (item.chain_s_moves.Count == 0) break;
				if ((item.chain_s_moves[item.chain_s_moves.Count - 1] as Move).jump)
				{
					return true;
				}
			}
			return false;
		}
		public string get_first_color()
		{
			if (table_s_chains.Count == 0) return "";
			if ((table_s_chains[0] as Chain).chain_s_moves.Count == 0) return "";
			return ((table_s_chains[0] as Chain).chain_s_moves[0] as Move).cell_from.color;
		}
		public bool get_first_king()
		{
			if (table_s_chains.Count == 0) return false;
			if ((table_s_chains[0] as Chain).chain_s_moves.Count == 0) return false;
			return ((table_s_chains[0] as Chain).chain_s_moves[0] as Move).cell_from.king;
		}
		public override string ToString()
		{
			string s = "";
			foreach (Chain item in table_s_chains)
			{
				if (s != "") s += " | ";
				s += item.ToString();
			}
			return s;
		}
	}

	public class Chain
	{
		//массив всех ходов цепочки, только move, должен быть минимум 1
		public System.Collections.ArrayList chain_s_moves;
		public Chain(System.Collections.ArrayList moves_arr)
		{
			this.chain_s_moves = moves_arr;
		}
		public Chain()
		{
			this.chain_s_moves = new System.Collections.ArrayList();
		}
		public Chain(Move m)
		{
			this.chain_s_moves = new System.Collections.ArrayList();
			this.chain_s_moves.Add(m);
		}

		public int calc_jump_count() 
		{
			int i=0;
			foreach (Move m in chain_s_moves)
			{
				if (m.jump) 
				{
					i++;
				}
			}
			return i;
		}
		public bool is_chain_contain_cell_in_the_end(Cell c)
		{
			if (chain_s_moves == null || chain_s_moves.Count == 0) return false;
			if ((chain_s_moves[chain_s_moves.Count - 1] as Move).cell_to == c) return true;
			return false;
		}
		public int get_number_of_chain_if_contain_cell(Cell c)
		{
			if (chain_s_moves == null || chain_s_moves.Count == 0) return -2;
			for (int i = 0; i < chain_s_moves.Count; i++)
			{
				Move item = chain_s_moves[i] as Move;
				if (item.cell_to == c)
				{
					return i;
				}
			}
			
			return -1;
		}
		public Chain get_last_chain(int last_index)
		{
			Chain temp = new Chain();
			temp.chain_s_moves = chain_s_moves.Clone() as System.Collections.ArrayList;
			temp.chain_s_moves.RemoveRange(last_index + 1, temp.chain_s_moves.Count - last_index - 1);
			return temp;
		}
		public override string ToString()
		{
			string s = "";
			foreach (Move item in chain_s_moves)
			{
				if (s != "") s += "; ";
				s += item.ToString();
			}
			return s;
		}//печать
	}

	public class Move
	{
		public Cell cell_from;
		public Cell cell_to;
		public int level;

		public bool jump;
		public System.Collections.ArrayList eated_cells; //только Cell

		public bool perform_to_king;
		public Move(Cell c_from, Cell c_to, int l, bool j, 
			System.Collections.ArrayList eated_cells, 
			bool king = false)
		{
			this.cell_from = c_from;
			this.cell_to = c_to;
			this.level = l;
			this.jump = j;
			this.perform_to_king = king;
			this.eated_cells = eated_cells;
		}
		public override string ToString() 
		{
			string j = "";
			if (jump) j = "!";
			return cell_from.ToString() + ":" + cell_to.ToString() + j;
		}
	}

}


