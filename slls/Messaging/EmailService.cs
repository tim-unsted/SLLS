﻿using System;
using System.Data;
using System.Data.SqlClient;
using slls.Models;

namespace slls.Messaging
{
    public class EmailService
    {
        public static bool SendDbMail(string destination, string from = "", string cc = "", string bcc = "", string subject = "",
            string body = "")
        {
            var db = new DbEntities();

            try
            {
                using (var conn = new SqlConnection(db.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "EmailSendHTML";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@From", SqlDbType.VarChar, 100).Value = from;
                        cmd.Parameters.Add("@To", SqlDbType.VarChar, 100).Value = destination;
                        cmd.Parameters.Add("@Body", SqlDbType.NText).Value = body;
                        cmd.Parameters.Add("@Subject", SqlDbType.VarChar, 100).Value = subject;
                        cmd.Parameters.Add("@CC", SqlDbType.VarChar, 255).Value = cc;
                        cmd.Parameters.Add("@BCC", SqlDbType.VarChar, 255).Value = bcc;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                //throw;
                return false;
            }
            return true;
        }
    }
}