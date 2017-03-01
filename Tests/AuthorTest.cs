using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using LibraryApp.Objects;

namespace LibraryApp
{
    public class AuthorTest : IDisposable
    {
        public AuthorTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void TEST_Save_CheckAuthorSaveToDb()
        {
            Author tempAuthor = new Author("Larry");
            List<Author> allAuthors = new List<Author>{tempAuthor};
            tempAuthor.Save();

            Assert.Equal(allAuthors, Author.GetAll());
        }

        [Fact]
       public void TEST_FindReturnsAuthor()
       {
           Author tempAuthor = new Author("Larry");
           tempAuthor.Save();

           Assert.Equal(tempAuthor, Author.Find(tempAuthor.GetId()));
       }

        // [Fact]
        // public void TEST_AddStudent_AddStudentToJoinTable()
        // {
        //     Book tempBook = new Book("Math", "30");
        //     tempBook.Save();
        //
        //     Student tempStudent = new Student("Melvin", "2010-12,30");
        //     tempStudent.Save();
        //
        //     List<Student> allStudents = new List<Student>{tempStudent};
        //     tempBook.AddStudent(tempStudent);
        //     Assert.Equal(allStudents, tempBook.GetStudents());
        // }

        public void Dispose()
        {
            Author.DeleteAll();
        }
    }
}
