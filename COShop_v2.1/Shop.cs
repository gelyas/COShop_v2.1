using MySql.Data.MySqlClient;
using System;
using System.Net.Http;
using System.Threading;

namespace COShop_v2._1
{
    class Shop
    {
        internal Shop(int store_id, string name, string dateOpen, string dateClose)
        {
            //Открытие с 7 до 8 и открытие с 8 до 9
            if (DateTime.Now.TimeOfDay > new TimeSpan(7, 0, 0)
                && DateTime.Now.TimeOfDay < new TimeSpan(7, 30, 0)
                && dateOpen == "07:00:00")
            {
                ShopOpen(store_id, name, dateOpen);
            }
            else if (DateTime.Now.TimeOfDay > new TimeSpan(8, 0, 0)
                && DateTime.Now.TimeOfDay < new TimeSpan(8, 30, 0)
                && dateOpen == "08:00:00")
            {
                ShopOpen(store_id, name, dateOpen);
            }
            else
            if (DateTime.Now.TimeOfDay > new TimeSpan(00, 0, 0) && DateTime.Now.TimeOfDay < new TimeSpan(03, 0, 0))
            {
                ShopClose(store_id, name, dateClose);
            }

        }

        public static void ShopOpen(int store_id, string store, string dateOpen)
        {
            string message1;
            string sqlSlataOpen = "";
            MySqlConnection dbConn = new MySqlConnection(Config.UkmConn);
            switch (dateOpen)
            {
                case "07:00:00":
                    sqlSlataOpen = $@"#Time('{dateOpen}') 
	store_id = {store_id}";
                    break;


                case "08:00:00":
                    sqlSlataOpen = $@" Time('{dateOpen}') 
	store_id = {store_id}";

                    break;
            }
            MySqlCommand cmd = dbConn.CreateCommand();
            cmd.CommandText = sqlSlataOpen;
            dbConn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();


            try
            {
                while (reader.Read())
                {
                    if (reader.GetString(3) == "1")
                    {
                        if (reader.GetString(2) != "0")
                        {
                            message1 = reader.GetString(0) + " слата открыта в " + reader.GetString(2) + ". Время открытия магазина " + dateOpen;
                            SendMessage(Config.idChat, message1);
                            Console.WriteLine(message1);

                            if (DateTime.Now < reader.GetDateTime(1))
                            {
                                string message2 = reader.GetString(0) + " неправильно время на одной из касс";
                                SendMessage(Config.idChat, message2);
                                Thread.Sleep(3000);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            dbConn.Close();
        }


        public static void ShopClose(int store_id, string store, string dateClose)
        {
            string message1;

            MySqlConnection dbConn = new MySqlConnection(Config.UkmConn);

            string sqlSlataClose = $@"Time('{dateClose}')
            store_id = {store_id}";

            MySqlCommand cmd = dbConn.CreateCommand();
            cmd.CommandText = sqlSlataClose;
            dbConn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.GetString(3) == "1")
                    {
                        if (reader.GetString(2) != "0")
                        {
                            message1 = reader.GetString(0) + " слата закрылась в " + reader.GetString(2) + ". Время закрытия магазина " + dateClose;
                            SendMessage(Config.idChat, message1);
                            Console.WriteLine(message1);

                            if (DateTime.Now < reader.GetDateTime(1))
                            {
                                string message2 = reader.GetString(0) + " неправильно время на одной из касс";
                                SendMessage(Config.idChat, message2);
                                Thread.Sleep(3000);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            dbConn.Close();
        }

        public static void SendMessage(string idChat, string message)
        {
            var client = new HttpClient();
            var content = client.GetStringAsync("http://smmon/slatatelegram/?token=ef6ccc86-8d7f-4cbd-8019-2a8c1bc1c0f2&chatid=" + idChat + "&message=" + message);
            Thread.Sleep(3000);
            Console.WriteLine(content);
        }

    }
}
