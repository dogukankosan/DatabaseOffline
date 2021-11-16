using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
namespace VeriTabanıOffline
{
    public partial class Anasayfa : DevExpress.XtraEditors.XtraForm
    {
        SqlConnection myconnect = new SqlConnection(System.IO.File.ReadAllText("Database.txt"));
        void DatabaseList(string command)
        {
            SqlDataAdapter da = new SqlDataAdapter(command, myconnect);
            DataSet ds = new DataSet();
            da.Fill(ds);
            gridControl1.DataSource = ds.Tables[0];
        }
        public Anasayfa()
        {
            InitializeComponent();
        }
        private void Anasayfa_Load(object sender, EventArgs e)
        {
            DatabaseList("select name as 'VERİ TABANI ADI',SIRKET.SIRKISAADI as 'SİRKET ADI',state_desc as 'DURUMU' from sys.databases inner join SIRKET on 'ETA_'+SIRKET.SIRKOD+'_'+Convert(varchar,SIRKET.SIRDONEM)=name");
        }
        private void Anasayfa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult cv = new DialogResult();
                cv = XtraMessageBox.Show("ÇIKIŞ YAPMAK İSTEDİĞİNİZDEN EMİN MİSİNİZ ?", "ÇIKIŞ İŞLEMİ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (cv == DialogResult.Yes)
                    Application.Exit();
            }
        }
        void AllStatusChanged(string status)
        {
            for (Int16 i = 0; i < gridView1.RowCount; i++)
            {
                try
                {
                    myconnect.Open();
                    SqlCommand command = new SqlCommand("ALTER DATABASE " + gridView1.GetRowCellDisplayText(i, "VERİ TABANI ADI") + " SET " + status + " WITH ROLLBACK IMMEDIATE", myconnect);
                    command.ExecuteNonQuery();
                    myconnect.Close();
                    DatabaseList("select name as 'VERİ TABANI ADI',SIRKET.SIRKISAADI as 'SİRKET ADI',state_desc as 'DURUMU' from sys.databases inner join SIRKET on 'ETA_'+SIRKET.SIRKOD+'_'+Convert(varchar,SIRKET.SIRDONEM)=name");
                }
                catch (Exception a)
                {
                    XtraMessageBox.Show(a.Message, "HATALI VERİ TABANI İŞLEMİ !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    myconnect.Close();
                }
            }
        }
        void StatusChanged(string databasename, string status)
        {
            try
            {
                myconnect.Open();
                if (status.ToString() == "ONLINE")
                {
                    SqlCommand command = new SqlCommand("ALTER DATABASE " + databasename.ToString() + " SET OFFLINE WITH ROLLBACK IMMEDIATE", myconnect);
                    command.ExecuteNonQuery();
                    myconnect.Close();
                }
                else
                {
                    SqlCommand command = new SqlCommand("ALTER DATABASE " + databasename.ToString() + "  SET ONLINE WITH ROLLBACK IMMEDIATE", myconnect);
                    command.ExecuteNonQuery();
                    myconnect.Close();
                }
                DatabaseList("select name as 'VERİ TABANI ADI',SIRKET.SIRKISAADI as 'SİRKET ADI',state_desc as 'DURUMU' from sys.databases inner join SIRKET on 'ETA_'+SIRKET.SIRKOD+'_'+Convert(varchar,SIRKET.SIRDONEM)=name");
            }
            catch (Exception a)
            {
                XtraMessageBox.Show(a.Message, "HATALI VERİ TABANI İŞLEMİ !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                myconnect.Close();
            }
        }
        private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            StatusChanged(gridView1.GetFocusedRowCellValue("VERİ TABANI ADI").ToString(), gridView1.GetFocusedRowCellValue("DURUMU").ToString());
        }
        private void tümünüSeçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllStatusChanged("ONLINE");
        }
        private void tümünüKapatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AllStatusChanged("OFFLINE");
        }
    }
}