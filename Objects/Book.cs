using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryApp.Objects
{
    public class Book
    {
        private int _id;
        private string _title;

        public Book(string title, int id = 0)
        {
            _id = id;
            _title = title;
        }

        public int GetId()
        {
            return _id;
        }

        public string GetTitle()
        {
            return _title;
        }

        public override bool Equals(System.Object otherBook)
        {
            if (!(otherBook is Book))
            {
                return false;
            }
            else
            {
                Book newBook = (Book) otherBook;
                bool idEquality = this.GetId() == newBook.GetId();
                bool titleEquality = this.GetTitle() == newBook.GetTitle();

                return (idEquality && titleEquality);
            }
        }

        public static List<Book> GetAll()
        {
            List<Book> BookList = new List<Book> {};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM books;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string title = rdr.GetString(1);
                Book newBook = new Book(title, id);
                BookList.Add(newBook);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return BookList;
        }

        public List<Author> GetAuthors()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT authors.* FROM books JOIN books_authors ON (books.id = books_authors.book_id) JOIN authors ON (books_authors.author_id = authors.id) WHERE books.id = @BookId;", conn);

            SqlParameter BookId = new SqlParameter("@BookId", this.GetId().ToString());

            cmd.Parameters.Add(BookId);
            SqlDataReader rdr = cmd.ExecuteReader();
            List<Author> authorList = new List<Author>{};

            while(rdr.Read())
            {
                int authorId = rdr.GetInt32(0);
                string authorName = rdr.GetString(1);

                Author tempAuthor = new Author(authorName, authorId);
                authorList.Add(tempAuthor);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }
            return authorList;
        }



        public void Save()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books (title) OUTPUT INSERTED.id VALUES (@Title);", conn);

            SqlParameter titleParameter = new SqlParameter("@Title", this.GetTitle());

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

        public static Book Find(int id)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);
            SqlParameter bookIdParameter = new SqlParameter();
            bookIdParameter.ParameterName = "@BookId";
            bookIdParameter.Value = id.ToString();
            cmd.Parameters.Add(bookIdParameter);
            SqlDataReader rdr = cmd.ExecuteReader();

            int foundBookId = 0;
            string foundBookName = null;

            while (rdr.Read())
            {
                foundBookId = rdr.GetInt32(0);
                foundBookName = rdr.GetString(1);
            }
            Book foundBook = new Book(foundBookName, foundBookId);

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

        public void AddAuthor(Author newAuthor)
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO books_authors (book_id, author_id) VALUES (@BookId, @AuthorsId);", conn);

            SqlParameter bookIdParameter = new SqlParameter("@BookId", this.GetId());
            SqlParameter authorIdParameter = new SqlParameter("@AuthorsId", newAuthor.GetId());

            cmd.Parameters.Add(bookIdParameter);
            cmd.Parameters.Add(authorIdParameter);

            cmd.ExecuteNonQuery();

            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM books;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public static void BA_DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM books_authors;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void DeleteSingle()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId; DELETE FROM books_authors WHERE book_id = @BookId", conn);

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
