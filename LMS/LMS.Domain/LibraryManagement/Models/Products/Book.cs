﻿using LMS.Domain.LibraryManagement.Models.Authors;
using LMS.Domain.LibraryManagement.Models.Relations;

namespace LMS.Domain.LibraryManagement.Models.Products
{
    public class Book : Product
    {
        //Foreign Key: AuthorId ==> one(Author)-to-many(Author) relationship
        public Guid AuthorId { get; set; }
        

        public required string ISBN { get; set; }
        public int Pages { get; set; }
        public decimal RentalCost { get; set; }
        public int PublishedYear { get; set; }


        //Navigation Property:
        public ICollection<PublisherBook> PublisherBooks { get; set; }
        public ICollection<GenreBook> GenreBooks { get; set; }
        public Author Author { get; set; }


        public Book()
        {
            Author = null!;
            PublisherBooks = [];
            GenreBooks = [];
        }
    }
}