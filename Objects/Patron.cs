using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryApp.Objects
{
    public class Patron
    {
        private int _id;
        private string _name;
        private string _duedate;

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

        public string GetDueDate()
        {
            return _duedate;
        }


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

        public void AddCopy(Copy tempCopy)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO partrons_copies(partron_id, copy_id, due_date) VALUES(@PatronId, @CopyId, @DueDate);", conn)

            SqlParameter patronid = new SqlParameter("@PatronId", this.GetId());
            SqlParameter copyid = new SqlParameter("@CopyId", tempCopy.GetId());
            SqlParameter duedate = new SqlParameter("@DueDate", this.GetDueDate());

            cmd.Parameters.Add(patronid);
            cmd.Parameters.Add(copyid);
            cmd.Parameters.Add(duedate);

            cmd.ExecuteNonQuery();

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
            SqlCommand cmd = new SqlCommand("DELETE FROM patrons_copies;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //delete single Patron from Patrons table based on ID
        public void DeleteSingle()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId; DELETE FROM patrons_copies WHERE book_id = @PatronId", conn);

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
