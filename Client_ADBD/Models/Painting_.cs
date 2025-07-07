using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Client_ADBD.Models
{
    public class Painting_:IProduct
    {
        public string Type { get; set; }
        public string Artist { get; set; }
        public int CreationYear { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }

        AuctionAppEntities _dbContext;

        public Painting_()
        {
            _dbContext = new AuctionAppEntities();
        }
        public Painting_(int productId, string name, string description, DateTime invDate, decimal startPrice, decimal listPrice, string type, string artist,
            int creationYear, decimal length,decimal width, string[] imagePaths) : base(productId, name, description, invDate, startPrice, listPrice, imagePaths)
        {
            _dbContext = new AuctionAppEntities();
            Type = type;
            Artist = artist;
            CreationYear = creationYear;
            Length = length;
            Width = width;
        }
        public void AddPaintingPost(int auctionNumber, decimal startPrice, decimal listPrice, DateTime creationTime, string[] imagePath,
      string productName, string description, DateTime inventoryDate, string type, string artist, int year, decimal length, decimal width)
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
                    int idType = GetPaintingIdType(type);


                    var newPainting = new Painting
                    {
                        id_produs = productId,
                        id_type = idType,
                        artist = artist,
                        creation_year = year,
                        length = length,
                        width = width
                    };

                    _dbContext.Paintings.Add(newPainting);
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

        public int GetPaintingIdType(string type)
        {
            var id = _dbContext.Painting_types.Where(p => p.type == type).FirstOrDefault().id_painting_type;

            return id;
        }


    }
}
