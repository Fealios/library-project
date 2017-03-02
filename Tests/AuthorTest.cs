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

       [Fact]
       public void TEST_GetAllBooks()
       {
           Book testBook = new Book("harry potter");
           testBook.Save();
           Book testBook2 = new Book("twilight");
           testBook2.Save();
           Author testAuthor = new Author("Stud McMuffin");
           testAuthor.Save();
           testBook.AddAuthor(testAuthor);
           testBook2.AddAuthor(testAuthor);
           List<Book> bookList = testAuthor.GetBooks();
           List<Book> expected = new List<Book>{testBook, testBook2};
           Assert.Equal(expected, bookList);
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

        [Fact]
        public void TEST_CheckIfAuthorExist_ReturnFalse()
        {
            Author testAuthor = new Author("Doug");
            testAuthor.Save();

            Assert.Equal(false, Author.CheckIfAuthorExists("Melvin"));
        }

        [Fact]
        public void TEST_CheckIfAuthorExist_ReturnTrue()
        {
            Author testAuthor = new Author("Melvin");
            testAuthor.Save();

            Assert.Equal(true, Author.CheckIfAuthorExists(testAuthor.GetName()));

        }


        public void Dispose()
        {
            Book.DeleteAll();
            Author.DeleteAll();
        }
    }
}
