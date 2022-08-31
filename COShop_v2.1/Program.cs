using System;
using System.Data.SqlClient;
using System.IO;

namespace COShop_v2._1
{
    class Program
    {
        public static string sqlSlataClose = @"";



        static void Main(string[] args)
        {
            try
            {
                SlataStart();
            }
            catch (Exception ex)
            {
                string Error = "Открытие закрытие Слат, ошибка: " + ex.Message;
                Shop.SendMessage(Error, Config.idChatEx);
            }

        }

        public static void SlataStart()
        {
            SqlConnection dbConn = new SqlConnection(Config.SqlConn);
            dbConn.Open();

            SqlCommand command = new SqlCommand(Config.sqlSlata, dbConn);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int store_id = reader.GetInt32(0);
                string name = reader.GetString(1);
                string dateOpen = reader.GetString(2);
                string dateClose = reader.GetString(3);
                if (name != "05_Карла_Маркса")
                    new Shop(store_id: store_id, name: name, dateOpen: dateOpen, dateClose: dateClose);

            }
            dbConn.Close();
            Console.WriteLine("Финиш");
        }

        public static void ReadCsv()
        {
            // указываем путь к файлу csv
            string pathCsvFile = @"D:\Downloads\Read_csv.csv";

            using (StreamReader sr = new StreamReader(pathCsvFile, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {

                    string[] strArray = line.Split(new char[1] { ';' }, 4);
                    if (strArray[0] != "" && strArray[2] != "Open")
                        Console.WriteLine(strArray[0] + " : " + strArray[2] + " : " + strArray[3]);

                    InsertDB(strArray[0], strArray[2], strArray[3]);
                }
            }
        }


        public static void InsertDB(string store_id, string open, string close)
        {
            SqlConnection dbConn = new SqlConnection("Data Source=;Initial Catalog=;User ID=;Password=");
            string sql = $@"update set dateOpen = '{open}', dateClose = '{close}' where store_id = {store_id}";
            dbConn.Open();

            SqlCommand command = new SqlCommand(sql, dbConn);
            command.ExecuteNonQuery();
            dbConn.Close();
        }
    }
}
