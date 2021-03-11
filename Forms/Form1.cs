using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMS
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			DataSet dts = TextUtils.GetListDataFromSP("spGetAndonDetails", "AnDonDetails"
					, new string[1] { "@AndonID" }
					, new object[1] { 281 });
			DataTable dataTableCD1 = dts.Tables[0];
			DataTable dataTableCD2 = dts.Tables[1];
			DataTable dataTableCD3 = dts.Tables[2];
		}
	}
}
