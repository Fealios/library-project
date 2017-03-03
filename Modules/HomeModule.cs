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
            for (int i = 0; i < Int32.Parse(Request.Form["copies"]); i++) {
                Copy newCopy = new Copy(newBook.GetTitle());
                newCopy.Save();
                newCopy.AddBookCopy(newBook);
            }
            return View["book-management.cshtml", Book.GetAll()];
        };

        Get["/confirm-checkout/{id}"]=parameter=> {
            Copy tempCopy = Copy.Find(parameter.id);
            Patron newPatron = new Patron(Request.Form["patron"]);
        }

        // Post["/confirm-checkout/{id}"] =parameter=> {
        //     Copy foundCopy = Copy.Find(parameter.id);
        //     if(Patron.CheckIfPatronExists(Request.Form["patron"]))
        //     {
        //         Patron foundPatron = Patron.FindByName(Request.Form["patron"]);
        //         foundPatron.CheckoutCopy(foundCopy);
        //
        //     }
        //     else
        //     {
        //         Patron newPatron = new Patron(Request.Form["patron"]);
        //         newPatron.Save();
        //         newPatron.CheckoutCopy(foundCopy);
        //     }
        //     Checkout foundCheckout = new Checkout()
        //     return View["checked-out.cshtml", ]
        // }

        Get["/delete-single-book/{id}"] = parameter => {
            Book tempBook = Book.Find(parameter.id);
            return View["delete-single-book-confirm.cshtml", tempBook];
        };

        Post["/delete-book-confirmed"] = _ => {
            Book tempBook = Book.Find(Request.Form["id"]);
            tempBook.DeleteSingle();
            return View["book-management.cshtml", Book.GetAll()];
        };

        Get["/checkout-book/{id}"]=parameter=> {
            Book tempBook = Book.Find(parameter.id);
            return View["checkout-book.cshtml", tempBook];
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

        Get["/search"] =_=> {
            return View["search.cshtml", Book.GetAll()];
        };

    }
  }
}
