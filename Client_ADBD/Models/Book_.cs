using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Client_ADBD.Models
{
    public class Book_:IProduct
    {
        public string Condition {  get; set; }  
        public string Author {  get; set; }
        public int PublicationYear {  get; set; }
        public string PublishingHouse { get; set; }
        public int PageNumber {  get; set; }
        public string Language { get; set; }

        AuctionAppEntities _dbContext;
        public Book_()
        {
            _dbContext = new AuctionAppEntities();
        }

        public Book_(int  productId,string name,string description, DateTime invDate,decimal startPrice,decimal listPrice,string condition, string author, 
            int publicationYear, string publishingHouse, int pageNumber, string language, string[] imagePaths) :base(productId, name,description,invDate,startPrice,listPrice,imagePaths)
        {
            _dbContext = new AuctionAppEntities();
            Condition = condition;
            Author = author;
            PublicationYear = publicationYear;
            PublishingHouse = publishingHouse;
            PageNumber = pageNumber;
            Language = language;
        }

        public int GetBookConditionId(string condition)
        {
       
            var id = _dbContext.Book_conditions.Where(p => p.condition == condition).FirstOrDefault().id_book_condition;

            return id;
        }

       public void AddBookPost(int auctionNumber, decimal startPrice, decimal listPrice, DateTime creationTime, string[] imagePath,
       string productName, string description, DateTime inventoryDate, string author, string condition, int year, string ph, int pageNr, string language)
        {


            try
            {
                using (var transaction = new TransactionScope())
                {
                    var newProduct = new Product
                    {
                        name = productName,
                        description = description,
                        inventory_date = inventoryDate
                    };

                    _dbContext.Products.Add(newProduct);
                    _dbContext.SaveChanges();

                    int productId = newProduct.id_product;
                    int idCondition = GetBookConditionId(condition);



                    var newBook = new Book
                    {
                        id_product = productId,
                        id_condition = idCondition,
                        author = author,
                        publication_year = year,
                        publishing_house = ph,
                        page_number = pageNr,
                        book_language = language
                    };

                    _dbContext.Books.Add(newBook);
                    _dbContext.SaveChanges();

                    int idAuction = (new Auction_()).GetAuctionIdByNumber(auctionNumber);
                    int lotNumber = (new Post_()).GetNextLotNumber(idAuction);

                    var newPost = new Post
                    {
                        id_product = productId,
                        id_status = 2,
                        id_auction = idAuction,
                        start_price = startPrice,
                        list_price = listPrice,
                        created_at = creationTime,
                        lot = lotNumber,
                    };

                    _dbContext.Posts.Add(newPost);
                    _dbContext.SaveChanges();

                    var newImage = new Product_image
                    {
                        id_product = productId,
                        image_path = imagePath[0]
                    };

                    _dbContext.Product_images.Add(newImage);
                    _dbContext.SaveChanges();

                    if (!string.IsNullOrEmpty(imagePath[1]))
                    {
                        var newImage1 = new Product_image
                        {
                            id_product = productId,
                            image_path = imagePath[1]
                        };


                        _dbContext.Product_images.Add(newImage1);
                        _dbContext.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(imagePath[2]))
                    {
                        var newImage2 = new Product_image
                        {
                            id_product = productId,
                            image_path = imagePath[2]
                        };

                        _dbContext.Product_images.Add(newImage2);
                        _dbContext.SaveChanges();
                    }

                    transaction.Complete();



                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Eroare la adăugarea sculpturii: {ex.Message}");
            }
        }

        public void UpdateBookPostDetails(int productId, string author, int publicationYear, string publishingHouse, int pageNumber, string language, string condition)
        {
            var book = _dbContext.Books.SingleOrDefault(b => b.id_product == productId);


            if (book.author != author)
            {
                book.author = author;
            }

            if (book.publication_year != publicationYear)
            {
                book.publication_year = publicationYear;
            }

            if (book.publishing_house != publishingHouse)
            {
                book.publishing_house = publishingHouse;
            }

            if (book.page_number != pageNumber)
            {
                book.page_number = pageNumber;
            }

            if (book.book_language != language)
            {
                book.book_language = language;
            }

            int conditionId = GetBookConditionId(condition);

            if (conditionId != book.id_condition)
            {
                book.id_condition = conditionId;
            }

            _dbContext.SaveChanges();
        }

    }
}
