using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryApp.Objects
{
    public class Checkout
    {
        private int _id;
        private int _patron_id;
        private int _copy_id;
        private string _due_date;

        public Checkout(int patronId, int copyId, string dueDate, int id = 0)
        {
            _id = id;
            _patron_id = patronId;
            _copy_id = copyId;
            _due_date = dueDate;
        }

        public int GetId()
        {
            return _id;
        }

        public int GetPatronId()
        {
            return _patron_id;
        }

        public int GetCopyId()
        {
            return _copy_id;
        }

        public string GetDueDate()
        {
            Console.WriteLine(_due_date);
            return _due_date;
        }

        public static Checkout FindCheckedOut(int)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT copies.* FROM copies JOIN checkouts ON (checkouts.copy_id = copies.id) WHERE copies.checkout = 1;", conn);
        }

        public static List<Checkout> GetOverDue()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM checkouts WHERE due_date <= GETDATE();", conn);
            List<Checkout> checkoutList = new List<Checkout>{};
            SqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                int patronId = rdr.GetInt32(1);
                int copyId = rdr.GetInt32(2);
                string dueDate = rdr.GetDateTime(3).ToString();
                Checkout foundCheckout = new Checkout(patronId, copyId, dueDate, id);
                checkoutList.Add(foundCheckout);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return checkoutList;
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM checkouts;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


    }
}
