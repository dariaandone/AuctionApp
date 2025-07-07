using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Client_ADBD.Models
{
    public class Watch_:IProduct
    {
        public string Mechanism {  get; set; }
        public decimal Diameter {  get; set; }
         public string Manufacturer {  get; set; }
        public string Type {  get; set; }

        AuctionAppEntities _dbContext;
        public Watch_()
        {
            _dbContext = new AuctionAppEntities();
        }

        public Watch_(string mechanism, decimal diameter, string manufacturer,int productId,string name,decimal startPrice, decimal listPrice, string[]imagePaths,
                string description, DateTime invDate,string type): base(productId, name, description, invDate, startPrice, listPrice, imagePaths)
        {
            _dbContext = new AuctionAppEntities();
            Mechanism = mechanism;
            Diameter = diameter;
            Manufacturer = manufacturer;
            Type = type;
        }

        public void AddWatchPost(int auctionNumber, decimal startPrice, decimal listPrice, DateTime creationTime, string[] imagePath,
           string productName, string description, DateTime inventoryDate, string mechanism, string type, decimal diameter, string manufacturer)
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

                    int idMechanism = GetIdWatchMechanism(mechanism);
                    int idType = GetIdWatchType(type);

                    var newWatch = new Watch
                    {
                        id_product = productId,  // Asociem detaliile cu produsul prin ID
                        id_mechanism = idMechanism,
                        id_type = idType,
                        diameter = diameter,
                        manufacturer = manufacturer
                    };

                    _dbContext.Watches.Add(newWatch);
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

                Console.WriteLine($"Eroare la adăugarea ceasului: {ex.Message}");
            }
        }

        public int GetIdWatchMechanism(string mechanism)
        {
            var id = _dbContext.Watch_mechanism.Where(p => p.mechanism == mechanism).FirstOrDefault().id_mechanism;

            return id;
        }
  
        public int GetIdWatchType(string type)
        {
            var id = _dbContext.Watch_types.Where(p => p.type == type).FirstOrDefault().id_watch_type;

            return id;
        }

        public void UpdateWatchPostDetails(int productId, decimal diameter, string manufacturer, string type, string mechanism)
        {
            var watch = _dbContext.Watches.SingleOrDefault(w => w.id_product == productId);


            if (watch.diameter != diameter)
            {
                watch.diameter = diameter;
            }

            if (watch.manufacturer != manufacturer)
            {
                watch.manufacturer = manufacturer;
            }

            int idType = GetIdWatchType(type);

            if (watch.id_type != idType)
            {
                watch.id_type = idType;
            }

            int idMechanism = GetIdWatchMechanism(mechanism);

            if (watch.id_mechanism != idMechanism)
            {
                watch.id_mechanism = idMechanism;

            }

            _dbContext.SaveChanges();
        }
    }
}
