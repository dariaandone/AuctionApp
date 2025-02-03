using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;

namespace Client_ADBD.Models
{
    public class Jewelry_:IProduct
    {
        public string Type {  get; set; }
        public string Brand {  get; set; }
        public decimal Weight {  get; set; }
        public int CreationYear {  get; set; }

        AuctionAppEntities _dbContext;
        public Jewelry_()
        {
            _dbContext = new AuctionAppEntities();
        }

        public Jewelry_(int productId, string name, string description, DateTime invDate,decimal startPrice,decimal listPrice, string[]imagePaths,
            string type, string brand, decimal weight, int creationYear) : base(productId, name, description, invDate, startPrice, listPrice, imagePaths)
        {
            _dbContext = new AuctionAppEntities();
            Type = type;
            Brand = brand;
            Weight = weight;
            CreationYear = creationYear;
           
        }

        public int GetJewelryIdType(string type)
        {
           
            var id = _dbContext.Jewelry_types.Where(p => p.type == type).FirstOrDefault().id_jewelry_type;

            return id;
        }


        public void AddJewelryPost(int auctionNumber, decimal startPrice, decimal listPrice, DateTime creationTime, string[] imagePath,
         string productName, string description, DateTime inventoryDate, string type, string brand, decimal weight, int year)
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
                    int idType = GetJewelryIdType(type);


                    var newJewelry = new Jewelry
                    {
                        id_product = productId,
                        id_type = idType,
                        brand = brand,
                        weight = weight,
                        creation_year = year
                    };

                    _dbContext.Jewelries.Add(newJewelry);
                    _dbContext.SaveChanges();

                    int idAuction = new Auction_().GetAuctionIdByNumber(auctionNumber);
                    int lotNumber = new Post_().GetNextLotNumber(idAuction);

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

        public void UpdateJewelryPostDetails(int productId, string brand, decimal weight, int creationYear, string type)
        {
            var jewelry = _dbContext.Jewelries.SingleOrDefault(j => j.id_product == productId);

            if (jewelry.brand != brand)
            {
                jewelry.brand = brand;
            }

            if (jewelry.weight != weight)
            {
                jewelry.weight = weight;
            }

            if (jewelry.creation_year != creationYear)
            {
                jewelry.creation_year = creationYear;
            }

            int idType = GetJewelryIdType(type);

            if (jewelry.id_type != idType)
            {
                jewelry.id_type = idType;
            }

            _dbContext.SaveChanges();
        }
    }
}
