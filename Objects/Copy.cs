using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryApp.Objects
{
    public class Copy
    {
        private int _id;
        private string _title;
        private bool _checkout;

        public Copy(string title, int id = 0, bool checkout = false)
        {
            _id = id;
            _title = title;
            _checkout = checkout;
        }

        public int GetId()
        {
            return _id;
        }

        public string GetTitle()
        {
            return _title;
        }

        public bool GetCheckout()
        {
            return _checkout;
        }

        public void SetCheckout(bool status)
        {
            _checkout = status;
        }



        public override bool Equals(System.Object otherCopy)
        {
            if (!(otherCopy is Copy))
            {
                return false;
            }
            else
            {
                Copy newCopy = (Copy) otherCopy;
                bool idEquality = this.GetId() == newCopy.GetId();
                bool titleEquality = this.GetTitle() == newCopy.GetTitle();

                return (idEquality && titleEquality);
            }
        }

        //get a list of all Copies in DB
        public static List<Copy> GetAll()
        {
            List<Copy> CopyList = new List<Copy> {};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM copies;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string title = rdr.GetString(1);
                bool checkout = false;
                if(rdr.GetByte(2) == 1)
                {
                    checkout = true;
                }
                Copy newCopy = new Copy(title, id, checkout);
                CopyList.Add(newCopy);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return CopyList;
        }

        //get single book based off Copy Id
        public Book GetBook()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT books.* FROM copies JOIN books_copies ON (books_copies.copy_id = copies.id) JOIN books ON (books.id = books_copies.book_id) WHERE copies.id = @CopyId;", conn);
            SqlParameter CopyId = new SqlParameter("@CopyId", this.GetId().ToString());

            cmd.Parameters.Add(CopyId);
            SqlDataReader rdr = cmd.ExecuteReader();
            int bookId = 0;
            string bookTitle = null;

            while(rdr.Read())
            {
                bookId = rdr.GetInt32(0);
                bookTitle = rdr.GetString(1);


            }

            Book foundBook = new Book(bookTitle, bookId);

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
            return foundBook;
        }


        //save Copy to DB
        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO copies (book_name, checkout) OUTPUT INSERTED.id VALUES (@Title, @Checkout);", conn);

            SqlParameter titleParameter = new SqlParameter("@Title", this.GetTitle());
            SqlParameter checkoutParameter = new SqlParameter("@Checkout", this.GetCheckout());

            cmd.Parameters.Add(titleParameter);
            cmd.Parameters.Add(checkoutParameter);

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

        //Find copy in DB based off copy ID
        public static Copy Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM copies WHERE id = @CopyId;", conn);
            SqlParameter copyIdParameter = new SqlParameter();
            copyIdParameter.ParameterName = "@CopyId";
            copyIdParameter.Value = id.ToString();
            cmd.Parameters.Add(copyIdParameter);
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundCopyId = 0;
            string foundCopyName = null;
            bool foundCopyCheckoutStatus = false;

            while (rdr.Read())
            {
                foundCopyId = rdr.GetInt32(0);
                foundCopyName = rdr.GetString(1);
                if(rdr.GetByte(2) == 1)
                {
                    foundCopyCheckoutStatus = true;
                }
            }
            Copy foundCopy = new Copy(foundCopyName, foundCopyId, foundCopyCheckoutStatus);

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
            return foundCopy;
        }

        //add CopyBook relationship to join table
        public void AddBookCopy(Book tempBook)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books_copies (book_id, copy_id) VALUES (@BookId, @CopyId);", conn);

            SqlParameter bookIdParameter = new SqlParameter("@BookId", tempBook.GetId());
            SqlParameter copyIdParameter = new SqlParameter("@CopyId", this.GetId());

            cmd.Parameters.Add(bookIdParameter);
            cmd.Parameters.Add(copyIdParameter);

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        // public bool CheckAvailabe()
        // {
        //     SqlConnection conn = DB.Connection();
        //     conn.Open();
        //
        //     SqlCommand cmd = new SqlCommand("SELECT * FROM copies WHERE id = @CopyId");
        //     // List<Copy> instockCopies = new List<Copy> {};
        //
        //     SqlParameter copyId = new SqlParameter("@CopyId", this.GetId());
        //
        //     cmd.Parameters.Add(copyId);
        //     SqlDataReader rdr = cmd.ExecuteReader();
        //
        //     while(rdr.Read())
        //     {
        //         if(rdr.GetByte(2) == 0)
        //         {
        //             return true;
        //         }
        //         else
        //         {
        //             return false;
        //         }
        //     }
        // }

        //delete all copies from copies table
        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM copies;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //delete relationship from BC join table
        public static void BC_DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM books_copies;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //delete single copy from copies table based on ID
        public void DeleteSingle()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId; DELETE FROM books_copies WHERE book_id = @BookId", conn);

            SqlParameter nameParameter = new SqlParameter("@BookId", this.GetId());
            cmd.Parameters.Add(nameParameter);

            cmd.ExecuteNonQuery();

            if(conn != null)
            {
                conn.Close();
            }
        }
    }
}
