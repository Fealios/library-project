using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryApp.Objects
{
    public class Patron
    {
        private int _id;
        private string _name;
        // private string _duedate;

        public Patron(string name, int id = 0)
        {
            _id = id;
            _name = name;
        }

        public int GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        // public string GetDueDate()
        // {
        //     return _duedate;
        // }


        public override bool Equals(System.Object otherPatron)
        {
            if (!(otherPatron is Patron))
            {
                return false;
            }
            else
            {
                Patron newPatron = (Patron) otherPatron;
                bool idEquality = this.GetId() == newPatron.GetId();
                bool nameEquality = this.GetName() == newPatron.GetName();

                return (idEquality && nameEquality);
            }
        }

        public List<Checkout> GetCheckouts()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM checkouts WHERE patron_id = @PatronId", conn);

            SqlParameter patronIdParameter = new SqlParameter("@PatronId", this.GetId());
            cmd.Parameters.Add(patronIdParameter);
            SqlDataReader rdr = cmd.ExecuteReader();

            List<Checkout> checkoutList = new List<Checkout>{};

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

        //get list of all Patrons
        public static List<Patron> GetAll()
        {
            List<Patron> PatronList = new List<Patron> {};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM patrons;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                Patron newPatron = new Patron(name, id);
                PatronList.Add(newPatron);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return PatronList;
        }

        //save Patron to DB
        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO patrons (name) OUTPUT INSERTED.id VALUES (@Name);", conn);

            SqlParameter titleParameter = new SqlParameter("@Name", this.GetName());

            cmd.Parameters.Add(titleParameter);

            SqlDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                this._id = rdr.GetInt32(0);
            }
            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
        }

        public void CheckoutCopy(Copy tempCopy)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO checkouts(patron_id, copy_id) VALUES(@PatronId, @CopyId);", conn);
            SqlCommand incrementTime = new SqlCommand("UPDATE checkouts SET due_date = DATEADD(day,14,due_date) WHERE patron_id = @Patron;",conn);

            SqlParameter PatronId = new SqlParameter("@Patron", this.GetId());
            SqlParameter patronid = new SqlParameter("@PatronId", this.GetId());
            SqlParameter copyid = new SqlParameter("@CopyId", tempCopy.GetId());


            // SqlParameter duedate = new SqlParameter("@DueDate", this.GetDueDate());

            cmd.Parameters.Add(patronid);
            cmd.Parameters.Add(copyid);
            incrementTime.Parameters.Add(PatronId);
            // cmd.Parameters.Add(duedate);

            cmd.ExecuteNonQuery();
            incrementTime.ExecuteNonQuery();
            if(conn != null)
            {
                conn.Close();
            }
        }

        //======= various delete functions =====================

        //delete all copies from Patrons table
        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM patrons;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //delete relationship from PC join table
        public static void PC_DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM checkouts;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //delete single Patron from Patrons table based on ID
        public void DeleteSingle()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId; DELETE FROM checkouts WHERE book_id = @PatronId", conn);

            SqlParameter idParameter = new SqlParameter("@PatronId", this.GetId());
            cmd.Parameters.Add(idParameter);

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }
    }
}
