using System.Collections.Generic;
using System;
using Nancy;
using Nancy.ViewEngines.Razor;
using LibraryApp.Objects;

namespace LibraryApp
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
        Get["/"] =_=> {
            return View["index.cshtml"];
        };

        Get["/wipe-database"] =_=> {
            return View["wipe-database.cshtml"];
        };

        Post["/wipe-database"] =_=> {
            Book.DeleteAll();
            Author.DeleteAll();
            Checkout.DeleteAll();
            Copy.DeleteAll();
            Patron.DeleteAll();
            return View["index.cshtml"];
        };

        Get["/books"] =_=> {
            Console.WriteLine("book page navigation");
            return View["book-management.cshtml", Book.GetAll()];
        };

        Post["/books"] =_=> {
            Console.WriteLine("adding new book");
            Book newBook = new Book(Request.Form["title"]);
            newBook.Save();
            string[] separator = new string[] {", "};
            string authorList = Request.Form["author"];
            string[] authors = authorList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            foreach(var author in authors)
            {
                if(Author.CheckIfAuthorExists(author))
                {
                    Author newAuthor = Author.FindByName(author);
                    newBook.AddAuthor(newAuthor);
                }
                else
                {
                    Author newAuthor = new Author(author);
                    newAuthor.Save();
                    newBook.AddAuthor(newAuthor);
                }
            }
            return View["book-management.cshtml", Book.GetAll()];
        };

        Get["/delete-single-book/{id}"] = parameter => {
            Book tempBook = Book.Find(parameter.id);
            return View["delete-single-book-confirm.cshtml", tempBook];
        };

        Post["/delete-book-confirmed"] = _ => {
            Book tempBook = Book.Find(Request.Form["id"]);
            tempBook.DeleteSingle();
            return View["book-management.cshtml", Book.GetAll()];
        };

        Get["/authors"] = _ =>
        {
            Console.WriteLine("author page navigation");
            return View["author-management.cshtml", Author.GetAll()];
        };

        Get["/single-author/{id}"]=parameter=> {
            Author tempAuthor = Author.Find(parameter.id);
            return View["single-author.cshtml", tempAuthor];
        };
    }
  }
}
