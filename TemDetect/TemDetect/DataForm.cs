using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempDetect
{
    

    public partial class DataForm : Form
    {
        private string _Server;
        private string _DataBase;
        private string _Port;
        private string _UserName;
        private string _PassWord;
        private string _TableName;

 
         public DataForm()
        {
            InitializeComponent();          
        }

        public string Server
        {
            get
            {
                return _Server;
            }

            set
            {
                _Server = value;
            }
        }

        public string Port
        {
            get
            {
                return _Port;
            }

            set
            {
                _Port = value;
            }
        }

        public string DataBase
        {
            get
            {
                return _DataBase;
            }

            set
            {
                _DataBase = value;
            }
        }

        public string UserName
        {
            get
            {
                return _UserName;
            }

            set
            {
                _UserName = value;
            }
        }

        public string PassWord
        {
            get
            {
                return _PassWord;
            }

            set
            {
                _PassWord = value;
            }
        }

        public string TableName
        {
            get
            {
                return _TableName;
            }

            set
            {
                _TableName = value;
            }
        }

        public bool  Connect()
        {

        return    datatable.ConnectDatabase(Server,Port,UserName,PassWord,DataBase,TableName);
        }

        private void DataForm_Load(object sender, EventArgs e)
        {
        
          datatable.database = DataBase;
          datatable.server = Server;
          datatable.port = Port;
          datatable.user = UserName;
          datatable.password = PassWord;
          datatable.MainTableName = TableName;
          
        

        }
    }
}
