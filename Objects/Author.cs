using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace LibraryApp.Objects
{
    public class Author
    {
        private int _id;
        private string _name;

        public Author(string name, int id = 0)
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

        public override bool Equals(System.Object otherAuthor)
       {
           if (!(otherAuthor is Author))
           {
               return false;
           }
           else
           {
               Author newAuthor = (Author) otherAuthor;
               bool idEquality = this.GetId() == newAuthor.GetId();
               bool nameEquality = this.GetName() == newAuthor.GetName();

               return (idEquality && nameEquality);
           }
       }

       public static List<Author> GetAll()
        {
            List<Author> AuthorList = new List<Author> {};

            SqlConnection conn = DB.Connection();
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM authors;", conn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                int id = rdr.GetInt32(0);
                string name = rdr.GetString(1);
                Author newAuthor = new Author(name, id);
                AuthorList.Add(newAuthor);
            }

            if (rdr != null)
            {
                rdr.Close();
            }
            if (conn != null)
            {
                conn.Close();
            }

            return AuthorList;
        }

        public static Author Find(int id)
      {
          SqlConnection conn = DB.Connection();
          conn.Open();

          SqlCommand cmd = new SqlCommand("SELECT * FROM authors WHERE id = @AuthorId;", conn);
          SqlParameter authorIdParameter = new SqlParameter();
          authorIdParameter.ParameterName = "@AuthorId";
          authorIdParameter.Value = id.ToString();
          cmd.Parameters.Add(authorIdParameter);
          SqlDataReader rdr = cmd.ExecuteReader();

          int foundAuthorId = 0;
          string foundAuthorName = null;

          while (rdr.Read())
          {
              foundAuthorId = rdr.GetInt32(0);
              foundAuthorName = rdr.GetString(1);
          }
          Author foundAuthor = new Author(foundAuthorName, foundAuthorId);

          if (rdr != null)
          {
              rdr.Close();
          }
          if (conn != null)
          {
              conn.Close();
          }
          return foundAuthor;
      }

        public void Save()
       {
           SqlConnection conn = DB.Connection();
           conn.Open();

           SqlCommand cmd = new SqlCommand("INSERT INTO authors (name) OUTPUT INSERTED.id VALUES (@Name);", conn);

           SqlParameter nameParameter = new SqlParameter("@Name", this.GetName());

           cmd.Parameters.Add(nameParameter);

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

       public static void DeleteAll()
        {
            SqlConnection conn = DB.Connection();
            conn.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM authors;", conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
