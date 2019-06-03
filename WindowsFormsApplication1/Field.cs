using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
	public class Field
	{
		public System.Collections.ArrayList field_state;

		public Field()
		{
			field_state = new System.Collections.ArrayList();
		}

		public Field(Field example, Cell without_cell, Cell without_cell2 = null)
		{
			field_state = new System.Collections.ArrayList();
			field_state.AddRange(example.field_state);
			foreach (Cell item in field_state)
			{
				if (item == without_cell)
				{
					item.color = "empty";
				}
				if (without_cell2 != null && without_cell2 == item)
				{
					item.color = "empty";
				}
			}
		}
	}
}
